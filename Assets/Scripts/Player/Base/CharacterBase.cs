using Cysharp.Threading.Tasks;
using Fusion;
using System;
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
public abstract class CharacterBase : NetworkBehaviour, IReceiveDamage
{
    // ステータス
    [SerializeField, Tooltip("ステータス値")]
    public CharacterStatusStruct _characterStatusStruct = default;

    // ステート
    protected CharacterStateEnum _characterStateEnum = default;

    // 現在のステート
    [HideInInspector]
    public CharacterStateEnum _currentState = default;


    // HP -----------------------------------------------------------------------------------
    protected ReactiveProperty<float> _currentHP = new ReactiveProperty<float>();

    [Networked(OnChanged = nameof(OnNetworkedHPChanged))]
    private float _networkedHP { get; set; }

    private static void OnNetworkedHPChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentHP.Value = changed.Behaviour._networkedHP;
    }
    // -------------------------------------------------------------------------------------


    // スタミナ ----------------------------------------------------------------------------
    protected ReactiveProperty<float> _currentStamina = new ReactiveProperty<float>();

    [Networked(OnChanged = nameof(OnNetworkedStaminaChanged))]
    private float _networkedStamina { get; set; }

    private static void OnNetworkedStaminaChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentStamina.Value = changed.Behaviour._networkedStamina;
    }
    // -------------------------------------------------------------------------------------


    // スキルゲージ ------------------------------------------------------------------------
    protected ReactiveProperty<float> _currentSkillPoint = new ReactiveProperty<float>();

    [Networked(OnChanged = nameof(OnNetworkedSkillPointChanged))]
    private float _networkedSkillPoint { get; set; }

    private static void OnNetworkedSkillPointChanged(Changed<CharacterBase> changed)
    {
        changed.Behaviour._currentSkillPoint.Value = changed.Behaviour._networkedSkillPoint;
    }
    // -------------------------------------------------------------------------------------

    // カメラコントローラー
    protected CameraDirection _cameraDirection = default;

    // アニメーター
    private Animator _animator = default;

    // リジッドボディ
    private Rigidbody _rigidbody = default;

    // 移動方向
    protected Vector2 _moveDirection = default;

    // 走るフラグ
    protected bool _isRun = default;

    // 前回のダッシュ状態を記録するフィールド
    private bool _wasRunningPressed = false;

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

    #region プロパティ

    public IReadOnlyReactiveProperty<float> CurrentHP => _currentHP;

    public IReadOnlyReactiveProperty<float> CurrentStamina => _currentStamina;

    public IReadOnlyReactiveProperty<float> CurrentSkillPoint => _currentSkillPoint;

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
    private void Setup()
    {
        if (Object.HasInputAuthority)
        {
            // カメラを設定
            Camera mainCamera = Camera.main;
            _cameraDirection = new CameraDirection(mainCamera.transform);
            _target.InitializeSetting(mainCamera);

            // UIを設定
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            PlayerUIPresenter playerUIPresenter = canvas.GetComponent<PlayerUIPresenter>();
            LockOnCursorPresenter lockOnCursorPresenter = canvas.GetComponent<LockOnCursorPresenter>();
            playerUIPresenter.SetModel(this.gameObject);
            lockOnCursorPresenter.SetModel(this.gameObject);
        }
    }

    /// <summary>
    /// コンポーネントのキャッシュ
    /// </summary>
    private void CacheComponents()
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
    private void InitialValues()
    {
        // 初期化
        _currentState = CharacterStateEnum.IDLE;
        _playerTransform = this.transform;
        _moveSpeed = _characterStatusStruct._walkSpeed;
        _characterStatusStruct._playerStatus = new WrapperPlayerStatus();

        // HP、スタミナ、スキルポイント変数をネットワークに同期する
        _currentHP.Value = _characterStatusStruct._playerStatus.MaxHp;
        _networkedHP = _currentHP.Value;
        _currentStamina.Value = _characterStatusStruct._playerStatus.MaxStamina;
        _networkedStamina = _currentStamina.Value;
        _currentSkillPoint.Value = 0f;
        _networkedSkillPoint = _currentSkillPoint.Value;

        // 死亡オブザーバー
        _currentHP.Where(value => value <= 0f)
                  .Subscribe(_ => Death())
                  .AddTo(this);

        // 走った時のスタミナ消費オブザーバー
        Observable.Interval(TimeSpan.FromSeconds(0.1f))
           .Where(_ => _isRun && _networkedStamina > 0)
           .Subscribe(_ =>
           {
               _networkedStamina -= _characterStatusStruct._runStamina;
           })
           .AddTo(this);

        // スタミナ自動回復オブザーバー
        Observable.Interval(TimeSpan.FromSeconds(0.1f))
            .Where(_ => _networkedStamina < _characterStatusStruct._playerStatus.MaxStamina)
            .Where(_ => _currentState == CharacterStateEnum.IDLE)
            .Subscribe(_ =>
            {
                _networkedStamina += _characterStatusStruct._recoveryStamina;
            })
            .AddTo(this);

        _networkedStamina = Mathf.Clamp(_networkedStamina, 0, _characterStatusStruct._playerStatus.MaxStamina);
    }


    /// <summary>
    /// 入力によるアクション処理
    /// </summary>
    /// <param name="input">入力情報</param>
    public void ProcessInput(PlayerNetworkInput input)
    {

        if (_currentState == CharacterStateEnum.ATTACK || _currentState == CharacterStateEnum.AVOIDANCE ||
            _currentState == CharacterStateEnum.SKILL || _currentState == CharacterStateEnum.DEATH)
        {
            return;
        }

        // 入力情報を移動方向に格納
        _moveDirection = input.MoveDirection;

        // Run状態の切り替え
        if (input.IsRunning && !_wasRunningPressed)
        {
            _isRun = !_isRun;
        }
        _wasRunningPressed = input.IsRunning;

        if (_moveDirection == Vector2.zero)
        {
            _currentState = CharacterStateEnum.IDLE;
            //_animation.BoolAnimation(_animator, "Walk", false);
            //_animation.BoolAnimation(_animator, "Run", false);
        }
        else
        {
            if (!_isRun)
            {
                _move = _moveProvider.GetWalk();
                //_animation.BoolAnimation(_animator, "Walk", true);
                //_animation.BoolAnimation(_animator, "Run", false);
                _moveSpeed = _characterStatusStruct._walkSpeed;
            }
            else
            {
                _move = _moveProvider.GetRun();
                //_animation.BoolAnimation(_animator, "Walk", false);
                //_animation.BoolAnimation(_animator, "Run", true);
                _moveSpeed = _characterStatusStruct._runSpeed;
            }

            Move(_playerTransform, _moveDirection, _moveSpeed, _rigidbody);
        }

        switch (input)
        {
            case { IsAttackLight: true }:
                AttackLight(_playerTransform, _characterStatusStruct._attackMultipiler);
                break;

            case { IsAttackStrong: true }:
                AttackStrong(_playerTransform, _characterStatusStruct._attackMultipiler);
                break;

            case { IsAvoidance: true }:
                Avoidance(_playerTransform, _moveDirection, _characterStatusStruct._avoidanceDistance, _characterStatusStruct._avoidanceDuration);
                break;

            case { IsTargetting: true }:
                Targetting();
                break;

            case { IsSkill: true } when _currentSkillPoint.Value >= _characterStatusStruct._skillPointUpperLimit:
                Skill(this, _characterStatusStruct._skillTime, _characterStatusStruct._skillCoolTime);
                break;

            case { IsResurrection: true }:
                Resurrection(this.transform, _characterStatusStruct._ressurectionTime);
                break;

            default:
                break;
        }
    }


    public virtual void Move(Transform transform, Vector2 moveDirection, float moveSpeed, Rigidbody rigidbody)
    {
        _currentState = CharacterStateEnum.MOVE;
        _move.Move(transform, moveDirection, moveSpeed, rigidbody);
    }


    public virtual async void AttackLight(Transform transform, float attackMultipiler)
    {
        _currentState = CharacterStateEnum.ATTACK;

        _playerAttackLight.AttackLight(transform, attackMultipiler);

        //_animation.TriggerAnimation(_animator, "AttackLight");

        ReceiveDamage(10);

        // ミリ秒に変換して待機
        await UniTask.Delay((int)(800));

        _currentState = CharacterStateEnum.IDLE;

    }


    public virtual async void AttackStrong(Transform transform, float attackMultipiler)
    {

        _currentState = CharacterStateEnum.ATTACK;

        _playerAttackStrong.AttackStrong(transform, attackMultipiler);

        //_animation.TriggerAnimation(_animator, "AttackStrong");

        await UniTask.Delay((int)(800));

        _currentState = CharacterStateEnum.IDLE;

    }


    public virtual void Targetting()
    {
        _target.Targetting();
    }


    public virtual async void Avoidance(Transform transform, Vector2 moveDirection, float avoidanceDistance, float avoidanceDuration)
    {
        _currentState = CharacterStateEnum.AVOIDANCE;

        _networkedStamina -= _characterStatusStruct._avoidanceStamina;

        _avoidance.Avoidance(transform, moveDirection, avoidanceDistance, avoidanceDuration);

        await UniTask.Delay((int)(500));

        _currentState = CharacterStateEnum.IDLE;
    }


    public abstract void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime);


    public virtual void Resurrection(Transform transform, float ressurectionTime)
    {
        _resurrection.Resurrection(transform, ressurectionTime);
    }


    public virtual void ReceiveDamage(int damageValue)
    {
        if (!Object.HasStateAuthority) return;

        _networkedHP = Mathf.Clamp(
            _networkedHP - (damageValue - _characterStatusStruct._defensePower),
            0,
            _characterStatusStruct._playerStatus.MaxHp
        );
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public virtual void Death()
    {
        _currentState = CharacterStateEnum.DEATH;
        //_animation.PlayAnimation(_animator, "Death");
        this.gameObject.SetActive(false);
    }
}