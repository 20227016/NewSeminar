
using Cysharp.Threading.Tasks;
using Fusion;
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

    // 現在のHP量
    protected ReactiveProperty<float> _currentHP = new ReactiveProperty<float>();

    // 現在のスタミナ量
    protected ReactiveProperty<float> _currentStamina = new ReactiveProperty<float>();

    // 現在のスキルポイント
    protected ReactiveProperty<float> _currentSkillPoint = new ReactiveProperty<float>();

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

    // 前回のIsRunning状態を記録するフィールド
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
        Initialize();
        SetupCamera();
    }


    /// <summary>
    /// ネットワーク同期アップデート
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerNetworkInput data))
        {
            ProcessInput(data);
        }
    }


    /// <summary>
    /// プレイヤー別にカメラを設定する処理
    /// </summary>
    private void SetupCamera()
    {
        if (Object.HasInputAuthority)
        {
            Camera mainCamera = Camera.main;
            _cameraDirection = new CameraDirection(mainCamera.transform);
            _target.InitializeSetting(mainCamera);
        }
    }


    /// <summary>
    /// 初期化処理
    /// </summary>
    protected virtual void Initialize()
    {
        CacheComponents();
        SetupInitialValues();
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
    /// 初期値設定
    /// </summary>
    private void SetupInitialValues()
    {
        _currentState = CharacterStateEnum.IDLE;
        _playerTransform = this.transform;
        _moveSpeed = _characterStatusStruct._walkSpeed;
        _characterStatusStruct._playerStatus = new WrapperPlayerStatus();
        _currentHP.Value = _characterStatusStruct._playerStatus.MaxHp;
        _currentStamina.Value = _characterStatusStruct._playerStatus.MaxStamina;
        _currentSkillPoint.Value = 0f;

        // 死亡オブザーバー
        _currentHP.Where(_ => _ <= 0f)
                  .Subscribe(_ => Death())
                  .AddTo(this);
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

        // 移動処理
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
            _animation.BoolAnimation(_animator, "Walk", false);
            _animation.BoolAnimation(_animator, "Run", false);
        }
        else
        {
            if (!_isRun)
            {
                _move = _moveProvider.GetWalk();
                _animation.BoolAnimation(_animator, "Walk", true);
                _animation.BoolAnimation(_animator, "Run", false);
                _moveSpeed = _characterStatusStruct._walkSpeed;
            }
            else
            {
                _move = _moveProvider.GetRun();
                _animation.BoolAnimation(_animator, "Walk", false);
                _animation.BoolAnimation(_animator, "Run", true);
                _moveSpeed = _characterStatusStruct._runSpeed;
            }
            Move(_playerTransform, _moveDirection, _moveSpeed, _rigidbody);
        }

        switch (input)
        {
            case { IsAttackLight: true }:
                AttackLight();
                break;

            case { IsAttackStrong: true }:
                AttackStrong();
                break;

            case { IsAvoidance: true }:
                Avoidance();
                break;

            case { IsTargetting: true }:
                Targetting();
                break;

            case { IsSkill: true } when _currentSkillPoint.Value >= _characterStatusStruct._skillPointUpperLimit:
                Skill(this, _characterStatusStruct._skillTime, _characterStatusStruct._skillCoolTime);
                break;

            case { IsResurrection: true }:
                Resurrection();
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


    public virtual async void AttackLight()
    {
        _currentState = CharacterStateEnum.ATTACK;

        _playerAttackLight.AttackLight(this.transform, _characterStatusStruct._attackMultipiler);

        _animation.TriggerAnimation(_animator, "AttackLight");

        ReceiveDamage(10);
        Debug.Log(_currentHP.Value);
        // ミリ秒に変換して待機
        await UniTask.Delay((int)(800)); 

        _currentState = CharacterStateEnum.IDLE;

    }


    public virtual async void AttackStrong()
    {

        _currentState = CharacterStateEnum.ATTACK;

        _playerAttackStrong.AttackStrong(this.transform, _characterStatusStruct._attackMultipiler);

        _animation.TriggerAnimation(_animator, "AttackStrong");

        await UniTask.Delay((int)(800));

        _currentState = CharacterStateEnum.IDLE;

    }


    public virtual void Targetting()
    {
        _target.Targetting();
    }


    public virtual async void Avoidance()
    {
        _currentState = CharacterStateEnum.AVOIDANCE;

        _avoidance.Avoidance(_playerTransform, _moveDirection, _characterStatusStruct._avoidanceDistance, _characterStatusStruct._avoidanceDuration);

        await UniTask.Delay((int)(500));

        _currentState = CharacterStateEnum.IDLE;
    }


    public abstract void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime);


    public virtual void Resurrection()
    {
        _resurrection.Resurrection(_characterStatusStruct._ressurectionTime, this.transform);
    }


    public virtual void ReceiveDamage(int damageValue)
    {
        _currentHP.Value = Mathf.Clamp(_currentHP.Value - (damageValue - _characterStatusStruct._defensePower), 
            // 最小値 , 最大値
            0, _characterStatusStruct._playerStatus.MaxHp);
    }


    public virtual void Death()
    {
        _currentState = CharacterStateEnum.DEATH;
        _animation.PlayAnimation(_animator, "Death");
        //this.gameObject.SetActive(false);
    }
}