using Cysharp.Threading.Tasks;
using Fusion;
using System;
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

    // ステータス
    public CharacterStatusStruct _characterStatusStruct = default;

    // 再生するアニメーション設定
    public CharacterAnimationStruct _characterAnimationStruct = default;

    // ステート
    protected CharacterStateEnum _characterStateEnum = default;

    // 現在のステート
    [HideInInspector]
    public CharacterStateEnum _currentState = default;

    // HP ---------------------------------------------------------------------------------
    protected ReactiveProperty<float> _currentHP = new ReactiveProperty<float>();

    [Networked(OnChanged = nameof(OnNetworkedHPChanged))]
    protected float _networkedHP { get; set; }

    private static void OnNetworkedHPChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentHP.Value = changed.Behaviour._networkedHP;
    }
    // ------------------------------------------------------------------------------------


    // スタミナ ---------------------------------------------------------------------------
    protected ReactiveProperty<float> _currentStamina = new ReactiveProperty<float>();

    [Networked(OnChanged = nameof(OnNetworkedStaminaChanged))]
    protected float _networkedStamina { get; set; }

    private static void OnNetworkedStaminaChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentStamina.Value = changed.Behaviour._networkedStamina;
    }
    // ------------------------------------------------------------------------------------


    // スキルゲージ -----------------------------------------------------------------------
    protected ReactiveProperty<float> _currentSkillPoint = new ReactiveProperty<float>();

    [Networked(OnChanged = nameof(OnNetworkedSkillPointChanged))]
    protected float _networkedSkillPoint { get; set; }

    private static void OnNetworkedSkillPointChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentSkillPoint.Value = changed.Behaviour._networkedSkillPoint;
    }
    // ------------------------------------------------------------------------------------

    // カメラコントローラー
    protected CameraDirection _cameraDirection = default;

    // アニメーター
    protected Animator _animator = default;

    // リジッドボディ
    private Rigidbody _rigidbody = default;

    // 移動方向
    protected Vector2 _moveDirection = default;

    // 走るフラグ
    protected bool _isRun = default;

    // スタミナ切れフラグ
    protected bool _isOutOfStamina = default;

    // スキルクールタイム中フラグ
    protected bool _isSkillCoolTime = default;

    // 現在の弱攻撃コンボ段階
    protected int _comboCount = 1;

    // 攻撃受付不可状態
    protected bool _notAttackAccepted = default;

    // ステートリセット用トークンソース
    private CancellationTokenSource _resetStateTokenSource;

    // コンボ段階リセット用
    private IDisposable _comboResetDisposable;

    // 移動速度
    protected float _moveSpeed = default;

    // 自身のトランスフォーム
    protected Transform _playerTransform = default;

    // 各種インターフェース
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

    #endregion


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
            // カメラを設定
            Camera mainCamera = Camera.main;
            _cameraDirection = new CameraDirection(mainCamera.transform);
            _target.InitializeSetting(mainCamera);

            // UIを設定
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            PlayerUIPresenter playerUIPresenter = canvas.GetComponentInChildren<PlayerUIPresenter>();
            LockOnCursorPresenter lockOnCursorPresenter = canvas.GetComponentInChildren<LockOnCursorPresenter>();
            playerUIPresenter.SetModel(this.gameObject);
            lockOnCursorPresenter.SetModel(this.gameObject);
        }
    }


    /// <summary>
    /// コンポーネントのキャッシュ
    /// </summary>
    protected virtual void CacheComponents()
    {
        _move = _moveProvider.GetWalk();
        _playerAttackLight = _attackProvider.GetAttackLight();
        _playerAttackStrong = _attackProvider.GetAttackStrong();
        _target = GetComponent<PlayerTargetting>();
        _skill = GetComponent<ISkill>();
        _passive = GetComponent<IPassive>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }


    /// <summary>
    /// 数値等の初期設定
    /// </summary>
    protected virtual void InitialValues()
    {
        // 初期化
        _currentState = CharacterStateEnum.IDLE;
        _playerTransform = this.transform;
        _moveSpeed = _characterStatusStruct._walkSpeed;
        _characterStatusStruct._playerStatus = new WrapperPlayerStatus();

        // ネットワーク同期用変数とリアクティブプロパティを初期化
        _currentHP.Value = _characterStatusStruct._playerStatus.MaxHp;
        _networkedHP = _currentHP.Value;
        _currentStamina.Value = _characterStatusStruct._playerStatus.MaxStamina;
        _networkedStamina = _currentStamina.Value;
        _currentSkillPoint.Value = 0f;
        _networkedSkillPoint = _currentSkillPoint.Value;

        ManageStamina();
    }


    /// <summary>
    /// スタミナ管理
    /// </summary>
    protected virtual void ManageStamina()
    {
        // 走った時のスタミナ消費処理
        HandleRunStaminaConsumption();

        // スタミナ自動回復処理
        HandleStaminaRecovery();

        // スタミナ切れフラグの管理
        HandleOutOfStaminaFlag();

        _networkedStamina = Mathf.Clamp(_networkedStamina, 0, _characterStatusStruct._playerStatus.MaxStamina);
    }


    /// <summary>
    /// 走った時のスタミナ消費
    /// </summary>
    protected virtual void HandleRunStaminaConsumption()
    {
        Observable.Interval(TimeSpan.FromSeconds(STAMINA_UPDATE_INTERVAL))
            // 走り状態時
            .Where(_ => _currentState == CharacterStateEnum.RUN)
            // スタミナが0以上の時
            .Where(_ => _networkedStamina > 0)
            .Subscribe(_ =>
            {
                _networkedStamina -= _characterStatusStruct._runStamina * STAMINA_UPDATE_INTERVAL;
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
            .Where(_ => _currentState != CharacterStateEnum.AVOIDANCE)
            // 走っていない or スタミナ切れ or 移動していない
            .Where(_ => !_isRun || _isOutOfStamina || _moveDirection == Vector2.zero)
            // スタミナが最大値以下
            .Where(_ => _networkedStamina < _characterStatusStruct._playerStatus.MaxStamina)
            .Subscribe(_ =>
            {
                // スタミナ切れ時は回復速度が半減
                float recoveryRate = _isOutOfStamina ? 2.0f : 1.0f;
                _networkedStamina += _characterStatusStruct._recoveryStamina * STAMINA_UPDATE_INTERVAL / recoveryRate ;
            })
            .AddTo(this);
    }


    /// <summary>
    /// スタミナ切れフラグの管理
    /// </summary>
    protected virtual void HandleOutOfStaminaFlag()
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
        if (_notAttackAccepted ||
            _currentState == CharacterStateEnum.AVOIDANCE ||
            //_currentState == CharacterStateEnum.SKILL ||
            _currentState == CharacterStateEnum.DAMAGE_REACTION ||
            _currentState == CharacterStateEnum.DEATH)
        {
            return;
        }

        if(_currentState != CharacterStateEnum.ATTACK)
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
                Avoidance(_playerTransform);
                break;

            case { IsTargetting: true }:
                Targetting();
                break;

            case { IsSkill: true } when _currentSkillPoint.Value >= _characterStatusStruct._skillPointUpperLimit:
                Skill(this, _characterStatusStruct._skillDuration, _characterStatusStruct._skillCoolTime);
                break;

            case { IsResurrection: true }:
                Resurrection(this.transform, _characterStatusStruct._ressurectionTime);
                break;

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
            _currentState = CharacterStateEnum.IDLE;
            _animation.BoolAnimation(_animator, _characterAnimationStruct._walkAnimation, false);
            _animation.BoolAnimation(_animator, _characterAnimationStruct._runAnimation, false);
            return;
        }

        // スタミナ切れの場合は常に歩き
        bool isWalking = !_isRun || _isOutOfStamina;

        // 状態に応じて移動設定を変更
        _move = isWalking ? _moveProvider.GetWalk() : _moveProvider.GetRun();
        _moveSpeed = isWalking ? _characterStatusStruct._walkSpeed : _characterStatusStruct._runSpeed;
        _currentState = isWalking ? CharacterStateEnum.WALK : CharacterStateEnum.RUN;

        if (isWalking)
        {
            _animation.BoolAnimation(_animator, _characterAnimationStruct._walkAnimation, true);
        }
        else
        {
            _animation.BoolAnimation(_animator, _characterAnimationStruct._runAnimation, true);
        }

        // 移動を実行
        Move(_playerTransform, _moveDirection, _moveSpeed, _rigidbody, _currentState);
    }


    protected virtual void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody, CharacterStateEnum characterState)
    {
        _currentState = characterState;

        _move.Move(transform, moveDirection, moveSpeed, rigidbody);
    }


    protected virtual void AttackLight(CharacterBase characterBase, float attackPower, float attackMultiplier)
    {
        _currentState = CharacterStateEnum.ATTACK;

        _playerAttackLight.AttackLight(characterBase, attackPower, attackMultiplier);

        // 攻撃速度を適用
        _animator.speed = _characterStatusStruct._attackSpeed;

        // アニメーション配列とコンボロジック
        AnimationClip[] attackAnimations = new[]
        {
            _characterAnimationStruct._attackLightAnimation1,
            _characterAnimationStruct._attackLightAnimation2,
            _characterAnimationStruct._attackLightAnimation3
        };

        AnimationClip attackAnimation = attackAnimations[(_comboCount - 1) % attackAnimations.Length];
        _comboCount = (_comboCount % attackAnimations.Length) + 1;

        float animationDuration = _animation.PlayAnimation(_animator, attackAnimation) / _characterStatusStruct._attackSpeed;

        // 前のリセット処理を解除
        _comboResetDisposable?.Dispose();

        _notAttackAccepted = true;

        // 一定時間経過で攻撃受付不可状態を解除
        _comboResetDisposable = Observable.Timer(TimeSpan.FromSeconds(animationDuration * ATTACK_ACCEPTED_TIME))
        .Subscribe(_ => _notAttackAccepted = false)
        .AddTo(this);

        // 一定時間経過でコンボがリセット
        _comboResetDisposable = Observable.Timer(TimeSpan.FromSeconds(animationDuration + COMBO_RESET_TIME))
            .Subscribe(_ => _comboCount = 1)
            .AddTo(this);

        ResetState(animationDuration);
    }


    protected virtual void AttackStrong(CharacterBase characterBase, float attackPower, float attackMultipiler)
    {
        _notAttackAccepted = true;

        _currentState = CharacterStateEnum.ATTACK;

        _animator.speed = _characterStatusStruct._attackSpeed;

        _networkedSkillPoint = 100f;
        _playerAttackStrong.AttackStrong(characterBase, attackPower, attackMultipiler);

        float animationDuration = _animation.TriggerAnimation(_animator, _characterAnimationStruct._attackStrongAnimation) / _characterStatusStruct._attackSpeed;

        ResetState(animationDuration, () => _notAttackAccepted = false);
    }


    protected virtual void Targetting()
    {
        _target.Targetting();
    }


    protected virtual void Avoidance(Transform transform)
    {
        if (_moveDirection == Vector2.zero || _isOutOfStamina) return;

        _currentState = CharacterStateEnum.AVOIDANCE;

        _networkedStamina -= _characterStatusStruct._avoidanceStamina;

        float animationDuration = _animation.TriggerAnimation(_animator, _characterAnimationStruct._avoidanceActionAnimation);

        _avoidance.Avoidance(transform, _moveDirection, _characterStatusStruct._avoidanceDistance, animationDuration);

        ResetState(animationDuration);
    }


    protected virtual void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime) 
    {
        //// クールタイム中ならリターン
        //if (_isSkillCoolTime) return;

        //_currentState = CharacterStateEnum.SKILL;

        //// クールタイム管理
        //_isSkillCoolTime = true;

        //Observable.Timer(TimeSpan.FromSeconds(skillCoolTime))
        //    .Subscribe(_ =>
        //    {
        //        //Debug.Log("スキルクールタイム終了");
        //        _isSkillCoolTime = false;
        //    });

        //// 発動後スキルポイントを０に
        //_networkedSkillPoint = 0f;

        //_skill.Skill(this, skillTime);

        //float animationDuration = _animation.TriggerAnimation(_animator, _characterAnimationStruct._skillAnimation);

        //ResetState(animationDuration);

        ReceiveDamage(70);
    }


    protected virtual void Resurrection(Transform transform, float ressurectionTime)
    {
        _resurrection.Resurrection(transform, ressurectionTime);
    }


    public virtual void ReceiveDamage(int damageValue)
    {
        if (!Object.HasStateAuthority) return;

        // 被弾中は無敵
        if (_currentState == CharacterStateEnum.DAMAGE_REACTION) return;

        _currentState = CharacterStateEnum.DAMAGE_REACTION;

        // ダメージ量に防御力を適応して最終ダメージを算出
        float damage = (damageValue - _characterStatusStruct._defensePower);

        // 現在HPから最終ダメージを引く
        _networkedHP = Mathf.Clamp(_networkedHP - damageValue, 0, _characterStatusStruct._playerStatus.MaxHp);

        if(_networkedHP <= 0)
        {
            Death();
            return;
        }

        float animationDuration;

        if (damage <= _characterStatusStruct._playerStatus.MaxHp / 2)
        {
            animationDuration = _animation.PlayAnimation(_animator, _characterAnimationStruct._damageReactionLightAnimation);
        }
        else
        {
            animationDuration = _animation.PlayAnimation(_animator, _characterAnimationStruct._damageReactionHeavyAnimation);
        }

        ResetState(animationDuration);
    }


    public virtual void ReceiveHeal(int healValue)
    {
        if (!Object.HasStateAuthority) return;
        _networkedHP = Mathf.Clamp(_networkedHP + healValue, 0, _characterStatusStruct._playerStatus.MaxHp);
    }


    /// <summary>
    /// 自分の攻撃がヒットしたときの処理
    /// </summary>
    public virtual void AttackHit(int damage) 
    {
        // スキルポイントを与ダメージを参照してチャージする
        float chargeSkillPoint = damage / 2;
        _networkedSkillPoint = Mathf.Clamp(_networkedSkillPoint+ chargeSkillPoint, 0, _characterStatusStruct._skillPointUpperLimit);
    }


    protected async virtual void ResetState(float resetTime, Action onResetComplete = null)
    {
        // 既存のトークンソースがあればキャンセル
        _resetStateTokenSource?.Cancel();
        _resetStateTokenSource = new CancellationTokenSource();

        try
        {
            // ミリ秒に変換
            resetTime *= 1000;

            // キャンセル可能な遅延処理
            await UniTask.Delay((int)resetTime, cancellationToken: _resetStateTokenSource.Token);

            // アニメーション速度を元に戻す
            _animator.speed = 1.0f;

            // 待機状態に
            _currentState = CharacterStateEnum.IDLE;

            // リセット完了を通知
            onResetComplete?.Invoke();
        }
        catch (OperationCanceledException)
        {
            //Debug.Log("ResetStateがキャンセルされました");
        }
    }


    /// <summary>
    /// 死亡処理
    /// </summary>
    protected virtual void Death()
    {
        _currentState = CharacterStateEnum.DEATH;
        _animation.PlayAnimation(_animator, _characterAnimationStruct._deathAnimation);
    }
}