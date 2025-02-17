using Cysharp.Threading.Tasks;
using Fusion;
using System;
using System.Collections;
using System.Threading;
using UniRx;
using UnityEngine;

/// <summary>
/// CharacterBase.cs
/// クラス説明
/// キャラクターの基底クラス
///
/// 作成日: 9/2
/// 作成者: 山田智哉
/// </summary>
public abstract class CharacterBase : NetworkBehaviour, IReceiveDamage, IReceiveHeal
{
    #region プロパティ

    public IReadOnlyReactiveProperty<float> CurrentHP => _currentHP;

    public IReadOnlyReactiveProperty<float> CurrentStamina => _currentStamina;

    public IReadOnlyReactiveProperty<float> CurrentSkillPoint => _currentSkillPoint;

    public PlayerUIPresenter PlayerUIPresenter { get => _playerUIPresenter; set => _playerUIPresenter = value; }

    protected NetworkObject[] Effects { get; set; } = default;

    [Networked(OnChanged = nameof(OnNetworkedHPChanged))]
    protected float NetworkedHP { get; set; } = 100f;

    public CharacterStateEnum CurrentState { get; set; } = default;

    public Subject<float> SkillCoolTimeSubject { get => skillCoolTimeSubject; set => skillCoolTimeSubject = value; }
    public Subject<Unit> DeathSubject { get => _deathSubject; set => _deathSubject = value; }
    public Subject<Unit> ResurrectionSubject { get => _resurrectionSubject; set => _resurrectionSubject = value; }
    #endregion

    #region 定数

    // スタミナの更新頻度定数
    const float STAMINA_UPDATE_INTERVAL = 0.1f;

    // 攻撃の受付時間定数
    const float ATTACK_ACCEPTED_TIME = 0.7f;

    // コンボのリセット時間定数
    const float COMBO_RESET_TIME = 1.0f;

    #endregion

    #region 変数

    // 構造体 ------------------------------------------------------------------------------------

    // ステータス設定
    public CharacterStatusStruct _characterStatusStruct = default;
    // アニメーション設定
    public CharacterAnimationStruct _characterAnimationStruct = default;
    // エフェクト設定
    public CharacterEffectStruct _characterEffectStruct = default;
    // サウンド
    public CharacterSoundStruct _characterSoundStruct = default;

    // Enum ------------------------------------------------------------------------------------

    // ステート
    protected CharacterStateEnum _characterStateEnum = default;

    // 物理系 ------------------------------------------------------------------------------------

    // リジッドボディ
    protected Rigidbody _rigidbody = default;
    // 自身のトランスフォーム
    protected Transform _playerTransform = default;

    // アニメーター ------------------------------------------------------------------------------------

    protected Animator _animator = default;

    // クラス ------------------------------------------------------------------------------------

    // カメラコントローラー
    protected CameraDirection _cameraDirection = default;
    protected PlayerUIPresenter _playerUIPresenter = default;

    // キャンセルトークン ------------------------------------------------------------------------------------

    // ステートリセット用トークンソース
    protected CancellationTokenSource _resetStateTokenSource = new();
    protected CancellationTokenSource invincibleCts;

    // 各種インターフェース ------------------------------------------------------------------------------------

    protected IMoveProvider _moveProvider = new PlayerMoveProvider();
    protected IMove _move = default;
    protected IAvoidance _avoidance = new PlayerAvoidance();
    protected IAttackProvider _attackProvider = new PlayerAttackProvider();
    protected IAttackLight _playerAttackLight = default;
    protected IAttackStrong _playerAttackStrong = default;
    protected ITargetting _target = default;
    protected ISkill _skill = default;
    protected IPassive _passive = default;
    protected IResurrection _resurrection = new PlayerResurrection();
    protected IAnimation _animation = new PlayerAnima();
    protected IPlayEffect _playEffect = new PlayerPlayEffect();
    protected ISound _sound = new SoundManager();
    // 弱攻撃コンボ段階リセット用
    protected IDisposable _attackLightComboResetDisposable = default;

    // Vector ------------------------------------------------------------------------------------

    // 移動方向
    protected Vector2 _moveDirection = default;

    // Float ------------------------------------------------------------------------------------

    protected ReactiveProperty<float> _currentHP = new ReactiveProperty<float>(100f);
    // スタミナ
    protected ReactiveProperty<float> _currentStamina = new ReactiveProperty<float>();
    // スキルゲージ
    protected ReactiveProperty<float> _currentSkillPoint = new ReactiveProperty<float>();
    // 移動速度
    protected float _moveSpeed = default;

    // int ------------------------------------------------------------------------------------

    // 現在の弱攻撃コンボ段階
    protected int _attackLightComboCount = default;

    // Bool ------------------------------------------------------------------------------------

    // 走るフラグ
    protected bool _isRun = default;
    // スタミナ切れフラグ
    protected bool _isOutOfStamina = default;
    // スキルクールタイム中フラグ
    protected bool _isSkillCoolTime = default;
    /// <summary>
    /// 攻撃受付不可状態
    /// </summary>
    protected bool _notAttackAccepted = default;
    protected bool _isInvincible = false;

    protected Subject<float> skillCoolTimeSubject = new();

    private Subject<Unit> _deathSubject = new();

    private Subject<Unit> _resurrectionSubject = new();

    #endregion

    private static void OnNetworkedHPChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentHP.Value = changed.Behaviour.NetworkedHP;
    }

    /// <summary>
    /// 生成時処理
    /// </summary>
    public override void Spawned()
    {
        // 値の初期化
        InitialValues();

        // キャッシュ
        CacheComponents();

        // 同期の設定
        Setup();
    }


    /// <summary>
    /// ネットワーク同期アップデート
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerNetworkInput data))
        {
            // 入力情報収集
            ProcessInput(data);
        }
    }

    /// <summary>
    /// プレイヤーごとに設定するもの
    /// </summary>
    protected virtual void Setup()
    {
        if (Object.HasInputAuthority)
        {
            // カメラをリンク
            Camera mainCamera = Camera.main;
            _cameraDirection = new CameraDirection(mainCamera.transform);
            _target.InitializeSetting(mainCamera);

            // UIをリンク
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            PlayerUIPresenter = canvas.GetComponentInChildren<PlayerUIPresenter>();
            LockOnCursorPresenter lockOnCursorPresenter = canvas.GetComponentInChildren<LockOnCursorPresenter>();
            PlayerUIPresenter.SetMyModel(this.gameObject);
            lockOnCursorPresenter.SetModel(this.gameObject);


        }
        RPC_SetAllyHPBar(this);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAllyHPBar(CharacterBase character)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }

        int modelCount = PlayerUIPresenter.GetAllyModelCount() + 1;

        // シーン内の全てのPlayerを取得
        CharacterBase[] allCharacters = FindObjectsOfType<CharacterBase>();

        if (allCharacters == null || allCharacters.Length == 0)
        {
            return;
        }

        foreach (CharacterBase characterBase in allCharacters)
        {
            if (characterBase == this || PlayerUIPresenter.IsAllyModelSet(characterBase))
            {
                // 自分自身または既に登録されているモデルはスキップ
                continue;
            }

            // PlayerUIPresenterにモデルを設定
            character.PlayerUIPresenter.SetAllyModel(characterBase, modelCount);

            modelCount++;
        }
    }

    /// <summary>
    /// コンポーネントのキャッシュ
    /// </summary>
    protected virtual void CacheComponents()
    {
        _target = GetComponent<PlayerTargetting>();
        _skill = GetComponent<ISkill>();
        _passive = GetComponent<IPassive>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponentInParent<Rigidbody>();
        _move = _moveProvider.GetWalk();
        _playerAttackLight = _attackProvider.GetAttackLight();
        _playerAttackStrong = _attackProvider.GetAttackStrong();

    }

    protected async virtual void InitialValues()
    {

        // 初期化
        _playerTransform = this.transform;
        _moveSpeed = _characterStatusStruct._walkSpeed;
        _characterStatusStruct._playerStatus = new WrapperPlayerStatus();

        CurrentState = CharacterStateEnum.IDLE;

        // エフェクトを配列に格納
        Effects = new NetworkObject[]
        {
        _characterEffectStruct._attackLight1Effect,
        _characterEffectStruct._attackLight2Effect,
        _characterEffectStruct._attackLight3Effect,
        _characterEffectStruct._attackStrongEffect,
        _characterEffectStruct._skillEffect
        };

        // エフェクトを設定
        InstanceEffect();

        // スタミナ管理
        StaminaManagement();

        // 1フレーム待つ
        await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

        // ネットワーク同期用変数とリアクティブプロパティを初期化
        _currentHP.Value = _characterStatusStruct._playerStatus.MaxHp;
        NetworkedHP = _currentHP.Value;

        _currentStamina.Value = _characterStatusStruct._playerStatus.MaxStamina;

        _currentSkillPoint.Value = _characterStatusStruct._skillPointUpperLimit;
        _currentSkillPoint.Value = 0f;
    }


    public void InstanceEffect()
    {
        // エフェクトを配列に格納
        Effects = new NetworkObject[]
        {
            _characterEffectStruct._attackLight1Effect,
            _characterEffectStruct._attackLight2Effect,
            _characterEffectStruct._attackLight3Effect,
            _characterEffectStruct._attackStrongEffect,
            _characterEffectStruct._skillEffect
        };

        for (int i = 0; i < Effects.Length; i++)
        {
            if (Effects[i] != null)
            {
                // プレハブの位置（ローカル座標）を取得
                Vector3 prefabLocalPosition = Effects[i].transform.localPosition;

                // プレハブの位置に基づいてインスタンス化する
                Effects[i] = Instantiate(Effects[i], transform.position + prefabLocalPosition, Effects[i].transform.rotation, transform);

                // インスタンスの名前を設定（任意）
                Effects[i].name = Effects[i].name;

                // インスタンスを非アクティブにする
                Effects[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// スタミナ管理
    /// </summary>
    protected virtual void StaminaManagement()
    {
        // 走った時のスタミナ消費処理
        HandleRunStaminaConsumption();

        // スタミナ自動回復処理
        HandleStaminaRecovery();

        // スタミナ切れフラグの管理
        HandleOutOfStamina();

        // スタミナの最小、最大値を制限
        _currentStamina.Value = Mathf.Clamp(_currentStamina.Value, 0, _characterStatusStruct._playerStatus.MaxStamina);
    }

    /// <summary>
    /// 走った時のスタミナ消費
    /// </summary>
    protected virtual void HandleRunStaminaConsumption()
    {
        Observable.Interval(TimeSpan.FromSeconds(STAMINA_UPDATE_INTERVAL))
            // 走り状態時
            .Where(_ => CurrentState == CharacterStateEnum.RUN)
            // スタミナが0以上の時
            .Where(_ => _currentStamina.Value > 0)
            .Subscribe(_ =>
            {
                _currentStamina.Value -= _characterStatusStruct._runStamina * STAMINA_UPDATE_INTERVAL;
            })
            .AddTo(this);
    }

    /// <summary>
    /// スタミナ自動回復
    /// </summary>
    protected virtual void HandleStaminaRecovery()
    {
        Observable.Interval(TimeSpan.FromSeconds(STAMINA_UPDATE_INTERVAL))
            // 回避状態ではない
            .Where(_ => CurrentState != CharacterStateEnum.AVOIDANCE)
            // 走っていない or スタミナ切れ or 移動していない
            .Where(_ => !_isRun || _isOutOfStamina || _moveDirection == Vector2.zero)
            // スタミナが最大値以下
            .Where(_ => _currentStamina.Value < _characterStatusStruct._playerStatus.MaxStamina)
            .Subscribe(_ =>
            {
                // スタミナ切れ時は回復速度が半減
                float recoveryRate = _isOutOfStamina ? 2.0f : 1.0f;
                _currentStamina.Value += _characterStatusStruct._recoveryStamina * STAMINA_UPDATE_INTERVAL / recoveryRate;
            })
            .AddTo(this);
    }

    /// <summary>
    /// スタミナ切れフラグの管理
    /// </summary>
    protected virtual void HandleOutOfStamina()
    {
        // スタミナが0を下回ったらスタミナ切れフラグをtrueに
        _currentStamina
            .Where(stamina => stamina <= 0)
            .Subscribe(_ => _isOutOfStamina = true)
            .AddTo(this);

        // スタミナが最大値の半分まで回復したらスタミナ切れフラグをfalseに
        _currentStamina
            .Where(stamina => stamina >= _characterStatusStruct._playerStatus.MaxStamina / 2)
            .Subscribe(_ => _isOutOfStamina = false)
            .AddTo(this);
    }

    /// <summary>
    /// 入力によるアクション処理
    /// </summary>
    /// <param name="input">入力情報</param>
    protected virtual void ProcessInput(PlayerNetworkInput input)
    {

        // Run状態を切り替える
        if (input.IsRunning)
        {
            _isRun = !_isRun;
        }

        // 状態が特定のものなら入力を無視
        if ((_notAttackAccepted ||
            CurrentState == CharacterStateEnum.SKILL ||
            CurrentState == CharacterStateEnum.DAMAGE_REACTION) &&
            !input.IsAvoidance ||
            CurrentState == CharacterStateEnum.DEATH)
        {
            return;
        }

        if (CurrentState != CharacterStateEnum.AVOIDANCE && input.IsAvoidance)
        {
            Debug.Log($"<color=#AFDFE4>回避ステート以外のステータス中に回避入力</color>");
        }

        // 回避中に回避をできないようにする
        if (CurrentState == CharacterStateEnum.AVOIDANCE)
        {
            return;
        }

        // 色コード#AFDFE4
        if (CurrentState != CharacterStateEnum.ATTACK)
        {
            // 移動処理
            MoveManagement(input);
        }

        // 入力の処理
        switch (input)
        {
            case { IsAttackLight: true }:
                AttackLight(this, _characterStatusStruct._attackPower, _characterStatusStruct._attackLightMultiplier);
                break;

            case { IsAttackStrong: true }:
                AttackStrong(this, _characterStatusStruct._attackPower, _characterStatusStruct._attackStrongMultiplier);
                break;

            case { IsAvoidance: true }:
                Avoidance(_playerTransform, input);
                break;

            case { IsTargetting: true }:
                Targetting();
                break;

            case { IsSkill: true } when _currentSkillPoint.Value >= _characterStatusStruct._skillPointUpperLimit:
                Skill(this, _characterStatusStruct._skillDuration, _characterStatusStruct._skillCoolTime);
                break;

            case { IsResurrection: true }:
                Resurrection(this.transform, _characterStatusStruct._ressurectionTime);
                return;

            default:
                break;
        }
    }

    /// <summary>
    /// 移動入力を管理
    /// </summary>
    /// <param name="input">収集した入力情報</param>
    protected virtual void MoveManagement(PlayerNetworkInput input)
    {

        // 入力情報を移動方向に格納
        _moveDirection = input.MoveDirection;

        // 移動値がない場合は待機状態に
        if (_moveDirection == Vector2.zero)
        {
            CurrentState = CharacterStateEnum.IDLE;
            RPC_BoolAnimation(_characterAnimationStruct._walkAnimation.name, false);
            RPC_BoolAnimation(_characterAnimationStruct._runAnimation.name, false);
            ResetState(0);
            return;
        }

        // スタミナ切れの場合は常に歩き
        bool isWalking = !_isRun || _isOutOfStamina;

        // 状態に応じて移動設定を変更
        _move = isWalking ? _moveProvider.GetWalk() : _moveProvider.GetRun();
        _moveSpeed = isWalking ? _characterStatusStruct._walkSpeed : _characterStatusStruct._runSpeed;
        CurrentState = isWalking ? CharacterStateEnum.WALK : CharacterStateEnum.RUN;

        if (isWalking)
        {
            Debug.Log("歩き");
            RPC_BoolAnimation(_characterAnimationStruct._runAnimation.name, false);
            RPC_BoolAnimation(_characterAnimationStruct._walkAnimation.name, true);
        }
        else
        {
            Debug.Log("走り");
            RPC_BoolAnimation(_characterAnimationStruct._walkAnimation.name, false);
            RPC_BoolAnimation(_characterAnimationStruct._runAnimation.name, true);
        }

        // 移動を実行
        Move(_playerTransform, _moveDirection, _moveSpeed, _rigidbody, CurrentState);
    }

    protected virtual void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody, CharacterStateEnum characterState)
    {
        CurrentState = characterState;

        _move.Move(transform, moveDirection, moveSpeed, rigidbody);
    }

    public virtual void AttackLight(CharacterBase characterBase, float attackPower, float attackMultiplier)
    {
        CurrentState = CharacterStateEnum.ATTACK;

        if (Object.HasInputAuthority)
        {
            _playEffect.RPC_PlayEffect(Effects[_attackLightComboCount], Effects[_attackLightComboCount].transform.position);
        }

        // 攻撃速度を適用
        _animator.speed = _characterStatusStruct._attackSpeed;

        // 弱攻撃の段階に応じたパラメータを取得
        (AnimationClip animation, float delay, float range, AudioSource audioSource, AudioClip audioClip, float playBackSpeed, float audioVolume, float audioDelay) = GetAttackParameters(_attackLightComboCount);

        // 攻撃処理
        _playerAttackLight.AttackLight(characterBase, attackPower, attackMultiplier, delay / _animator.speed, range);

        float animationDuration = _animation.GetAnimationLength(_animator, animation.name) / _characterStatusStruct._attackSpeed;

        // アニメーション再生
        RPC_PlayAnimation(animation.name);

        // 効果音
        _sound.ProduceSE(audioSource, audioClip, playBackSpeed, audioVolume, audioDelay);

        // 連続攻撃リセットタイマー
        _attackLightComboResetDisposable?.Dispose();

        _attackLightComboResetDisposable = Observable.Timer(TimeSpan.FromSeconds(animationDuration + COMBO_RESET_TIME))
            .Subscribe(_ => _attackLightComboCount = 0)
            .AddTo(this);

        // 攻撃受付不可状態
        _notAttackAccepted = true;

        Observable.Timer(TimeSpan.FromSeconds(animationDuration * ATTACK_ACCEPTED_TIME))
           .Subscribe(_ => _notAttackAccepted = false)
           .AddTo(this);

        ResetState(animationDuration);

        // 次の攻撃段階へ
        _attackLightComboCount = (_attackLightComboCount + 1) % 3;
    }

    /// <summary>
    /// 連続攻撃
    /// </summary>
    /// <param name="attackLightComboCount">攻撃段階</param>
    /// <returns></returns>
    private (AnimationClip, float, float, AudioSource, AudioClip, float, float, float) GetAttackParameters(int attackLightComboCount)
    {
        return attackLightComboCount switch
        {
            0 => (_characterAnimationStruct._attackLightAnimation1, _characterStatusStruct._attackLight1HitboxDelay, _characterStatusStruct._attackLight1HitboxRange
                  , _characterSoundStruct._audioSource, _characterSoundStruct._attack_1, _characterSoundStruct._playBackSpeed_1, _characterSoundStruct._audioVolume_1, _characterSoundStruct._delay_1),
            1 => (_characterAnimationStruct._attackLightAnimation2, _characterStatusStruct._attackLight2HitboxDelay, _characterStatusStruct._attackLight2HitboxRange
                  , _characterSoundStruct._audioSource, _characterSoundStruct._attack_2, _characterSoundStruct._playBackSpeed_2, _characterSoundStruct._audioVolume_2, _characterSoundStruct._delay_2),
            2 => (_characterAnimationStruct._attackLightAnimation3, _characterStatusStruct._attackLight3HitboxDelay, _characterStatusStruct._attackLight3HitboxRange
                  , _characterSoundStruct._audioSource, _characterSoundStruct._attack_3, _characterSoundStruct._playBackSpeed_3, _characterSoundStruct._audioVolume_3, _characterSoundStruct._delay_3),
            _ => (null, 0f, 0f, null, null, 0f, 0f, 0f),
        };
    }

    public virtual void AttackStrong(CharacterBase characterBase, float attackPower, float attackMultipiler)
    {
        _notAttackAccepted = true;

        CurrentState = CharacterStateEnum.ATTACK;

        _playerAttackStrong.AttackStrong(
            characterBase,
            attackPower,
            attackMultipiler,
            _characterStatusStruct._attackStrongHitboxDelay / _animator.speed,
            _characterStatusStruct._attackStrongHitboxRange);

        _animator.speed = _characterStatusStruct._attackSpeed;

        float animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._attackStrongAnimation.name) / _characterStatusStruct._attackSpeed;

        RPC_PlayAnimation(_characterAnimationStruct._attackStrongAnimation.name);

        // 効果音
        _sound.ProduceSE(_characterSoundStruct._audioSource, _characterSoundStruct._attack_Strong, _characterSoundStruct._playBackSpeed_Strong, _characterSoundStruct._audioVolume_Strong, _characterSoundStruct._delay_Strong);

        if (Object.HasInputAuthority)
        {
            _playEffect.RPC_PlayEffect(Effects[3], Effects[3].transform.position);
        }

        ResetState(animationDuration, () => _notAttackAccepted = false);
    }

    protected virtual void Targetting()
    {
        RPC_ReceiveDamage(100);
        _target.Targetting();
    }

    protected virtual void Avoidance(Transform transform, PlayerNetworkInput input)
    {
        // 移動値0 or スタミナ切れ状態の時はリターン
        if (_isOutOfStamina) return;

        // 入力情報を移動方向に格納
        Vector2 moveDirection = input.MoveDirection;

        if (moveDirection == Vector2.zero)
        {
            moveDirection = new Vector2(transform.forward.x, transform.forward.z);
        }

        CurrentState = CharacterStateEnum.AVOIDANCE;

        _currentStamina.Value -= _characterStatusStruct._avoidanceStamina;

        float animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._avoidanceActionAnimation.name);

        RPC_PlayAnimation(_characterAnimationStruct._avoidanceActionAnimation.name);

        _avoidance.Avoidance(transform, _rigidbody, moveDirection, _characterStatusStruct._avoidanceDistance, animationDuration);

        // 効果音
        _sound.ProduceSE(_characterSoundStruct._audioSource, _characterSoundStruct._dodge, _characterSoundStruct._playBackSpeed_Dodge, _characterSoundStruct._audioVolume_Dodge, _characterSoundStruct._delay_Dodge);
        Invincible(animationDuration);
        ResetState(animationDuration);
    }

    protected virtual void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        // クールタイム中ならリターン
        if (_isSkillCoolTime) return;

        CurrentState = CharacterStateEnum.SKILL;

        // クールタイム管理
        _isSkillCoolTime = true;
        Observable.Timer(TimeSpan.FromSeconds(skillCoolTime))
            .Subscribe(_ => _isSkillCoolTime = false);

        SkillCoolTimeSubject.OnNext(skillCoolTime);

        // 発動後スキルポイントを０に
        _currentSkillPoint.Value = 0f;

        _skill.Skill(this, skillTime);

        float animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._skillAnimation.name);

        RPC_TriggerAnimation(_characterAnimationStruct._skillAnimation.name);

        // 効果音
        _sound.ProduceSE(_characterSoundStruct._audioSource, _characterSoundStruct._attack_Special, _characterSoundStruct._playBackSpeed_Special, _characterSoundStruct._audioVolume_Special, _characterSoundStruct._delay_Special);

        if (Object.HasInputAuthority)
        {
            _playEffect.RPC_LoopEffect(Effects[4], Effects[4].transform.position, _characterStatusStruct._skillDuration);
        }

        Invincible(animationDuration);
        ResetState(animationDuration);
    }

    protected virtual void Resurrection(Transform transform, float resurrectionTime)
    {
        Debug.Log("蘇生入力検知");
        CurrentState = CharacterStateEnum.RESURRECTION;

        _resurrection.Resurrection(transform, resurrectionTime);

    }


    public virtual void RPC_ReceiveDamage(int damageValue)
    {
        // 無敵中はリターン
        if (_isInvincible) return;

        CurrentState = CharacterStateEnum.DAMAGE_REACTION;

        // ダメージ量に防御力を適応して最終ダメージを算出
        float damage = (damageValue - (damageValue * _characterStatusStruct._defensePower * 0.01f));

        // 現在HPから最終ダメージを引く
        NetworkedHP = NetworkedHP - damage;

        if (NetworkedHP <= 0)
        {
            NetworkedHP = 0f;
            Death();

            return;
        }

        // 被弾時のリアクション
        float animationDuration;
        if (damageValue <= 30)
        {
            animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._damageReactionLightAnimation.name);
            // 怯み
            RPC_PlayAnimation(_characterAnimationStruct._damageReactionLightAnimation.name);
        }
        else
        {
            animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._damageReactionHeavyAnimation.name);
            // 吹っ飛び
            RPC_PlayAnimation(_characterAnimationStruct._damageReactionHeavyAnimation.name);

            // ノックバック
            _avoidance.Avoidance(transform, _rigidbody, new Vector2(-transform.forward.x, -transform.forward.z), _characterStatusStruct._avoidanceDistance, animationDuration / 5);
        }

        // 効果音
        _sound.ProduceSE(_characterSoundStruct._audioSource, _characterSoundStruct._getHit, _characterSoundStruct._playBackSpeed_GetHit, _characterSoundStruct._audioVolume_GetHit, _characterSoundStruct._delay_GetHit);
        Invincible(animationDuration * 2f);
        ResetState(animationDuration);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public virtual void RPC_ReceiveHeal(int healValue)
    {
        // 死亡状態から回復処理をした場合は蘇生
        if (CurrentState == CharacterStateEnum.DEATH)
        {
            float animationDuration = _animation.GetAnimationLength(_animator, _characterAnimationStruct._reviveAnimation.name);
            RPC_PlayAnimation(_characterAnimationStruct._reviveAnimation.name);
            _resurrectionSubject.OnNext(Unit.Default);
            ResetState(animationDuration);
        }
        NetworkedHP = Mathf.Clamp(NetworkedHP + healValue, 0, _characterStatusStruct._playerStatus.MaxHp);
    }

    /// <summary>
    /// 自分の攻撃がヒットしたときの処理
    /// </summary>
    public virtual void AttackHit(int damage)
    {
        // スキルポイントを与ダメージを参照してチャージする
        float chargeSkillPoint = damage;

        _currentSkillPoint.Value = Mathf.Clamp(_currentSkillPoint.Value + chargeSkillPoint, 0, _characterStatusStruct._skillPointUpperLimit);
        // ヒットストップを実行
        StartCoroutine(HitStopCoroutine(0.1f)); // 50msのヒットストップ
    }

    /// <summary>
    /// ヒットストップを実装するコルーチン
    /// </summary>
    private IEnumerator HitStopCoroutine(float stopDuration)
    {
        _animator.speed = 0; // 一時的に時間を停止
        yield return new WaitForSecondsRealtime(stopDuration); // 経過時間をリアルタイムで待つ
        _animator.speed = 1;
    }

    protected async virtual void ResetState(float resetTime, Action onResetComplete = null)
    {
        // 既存のトークンソースがあればキャンセル
        _resetStateTokenSource?.Cancel();
        _resetStateTokenSource = new CancellationTokenSource();

        var token = _resetStateTokenSource.Token;

        try
        {
            // ミリ秒に変換
            resetTime *= 1000;

            // キャンセル可能な遅延処理
            await UniTask.Delay((int)resetTime, cancellationToken: token);

            // GameObjectがDestroyされていたら処理を中断
            if (this == null || _animator == null)
            {
                return;
            }

            if (CurrentState == CharacterStateEnum.DEATH)
            {
                return;
            }


            // アニメーション速度を元に戻す
            _animator.speed = 1.0f;

            // 待機状態に
            CurrentState = CharacterStateEnum.IDLE;

            _notAttackAccepted = false;

            // リセット完了を通知
            onResetComplete?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた場合は処理しない
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    protected virtual void Death()
    {
        print("Deathステート。死亡アニメーションを再生します。現在のHPは : " + NetworkedHP);
        CurrentState = CharacterStateEnum.DEATH;
        RPC_PlayAnimation(_characterAnimationStruct._deathAnimation.name);
        _deathSubject.OnNext(Unit.Default);
    }

    protected async virtual void Invincible(float resetTime)
    {
        // 以前の処理があればキャンセル
        invincibleCts?.Cancel();
        invincibleCts = new CancellationTokenSource();

        _isInvincible = true;

        try
        {
            await UniTask.Delay(Mathf.RoundToInt(resetTime * 1000), cancellationToken: invincibleCts.Token);
            // 無敵解除の処理
            _isInvincible = false;
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected void RPC_PlayAnimation(string animationClipName)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animation.PlayAnimation(_animator, animationClipName);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected void RPC_TriggerAnimation(string animationClipName)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        _animation.TriggerAnimation(_animator, animationClipName);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected void RPC_BoolAnimation(string animationClipName, bool isPlay)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        _animation.BoolAnimation(_animator, animationClipName, isPlay);
    }
}