

using UnityEngine;
using System.Collections;


/// <summary>
/// Sample.cs
/// サンプルコード。原型がなくなるぐらい改造していいよ
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class WraithBoss : BaseEnemy
{

    // ここでEnemを作成。
    // なぜEnemが2つあるかというと、走りながら攻撃するため（他にもあるけど...）
    // 一つだと、移動しながら何かをすることができないから、二つ作ってます。説明下手すぎごめん
    // 状態を追加したい場合、パブリックでEnemを設定しているので、Enemパブリッククラス(そういうスクリプトがある)に追加すれば使えます。多分
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField]
    private float _stateSwitchInterval = 3.0f; // 状態切り替え間隔

    private float _stateTimer = 0.0f; // 状態管理用タイマー

    [SerializeField]
    private float _attackStateSwitchInterval = 2.0f; // 状態切り替え間隔

    private float _attackStateTimer = 0.0f; // 状態管理用タイマー

   

    [SerializeField, Tooltip("探索範囲(前方距離)")]
    private float _searchRange = default;


    [SerializeField, Tooltip("ダメージを受ける時間")]
    private float _damageRange = 1.0f;

    private Vector3 _randomTargetPos; // ランダム移動の目標位置

    [SerializeField]
    private bool _attackAction = true;

    [SerializeField]
    private bool _attackEnd = true;

    //アニメーターを呼び出す変数
    Animator _animator;

    BoxCollider _boxCollider;

    private void Awake()
    {
        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        _animator = GetComponent<Animator>();
        _boxCollider = GetComponentInChildren<BoxCollider>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected void Update()
    {
        /* 
         * レイキャストの中心点を自動で更新してくれる
         * これで、レイキャストを常に自分の前から打ってくれる
         * つまり、何かない場合（発生位置を変えたい等）かえなくてOK
         * 変えたかったら聞いてくれたらその都度説明する
         * これ以外にもサイズ、方向等を変えるメソッドもある
        */
        SetPostion();
        SetDirection();

        if (Input.GetKeyDown(KeyCode.D))
        {
            _movementState = EnemyMovementState.STUNNED;
        }

        // 状態管理タイマーの更新
        _stateTimer += Time.deltaTime;

        // 状態を切り替えるタイミングになったら切り替える
        if (_stateTimer >= _stateSwitchInterval)
        {
            SwitchMovementState();
            _stateTimer = 0.0f; // タイマーをリセット
        }

        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                ideling();

                break;

            // 歩く（巡回）
            case EnemyMovementState.WALKING:

                break;

            // 移動
            case EnemyMovementState.RUNNING:

                break;

            // ダメージ
            case EnemyMovementState.STUNNED:

                break;


            // ダウン(ブレイク状態)
            case EnemyMovementState.DOWNED:

                break;

            // 死亡
            case EnemyMovementState.DIE:

                break;
        }

        switch (_actionState)
        {

            // サーチ
            case EnemyActionState.SEARCHING:

                break;

            // 攻撃
            case EnemyActionState.ATTACKING:

                _boxCollider.enabled = true;
                playerAttack();

                break;
        }
    }


    /// <summary>
    /// レイキャスト設定
    /// </summary>
    private void RayCastSetting()
    {
        //例キャスト初期設定
        BasicRaycast();

        // 中心点を取得
        _boxCastStruct._originPos = this.transform.position;

        // 自分のスケール(x)を取得
        float squareSize = transform.localScale.x;

        // BoxCastを正方形のサイズにする
        _boxCastStruct._size = new Vector2(squareSize, squareSize);
    }

    /// <summary>
    /// レイキャストの距離(探索範囲)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = _searchRange;
    }

    /// <summary>
    /// Lookat設定
    /// ここでは、
    ///    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    ///    private Transform _objTrans = default;
    /// の中に入れたオブジェクトをずっと見つめ続ける処理が書いてあるよ
    /// これで方向は取得できるので、あとは前進するだけで、上記で格納したオブジェクトを追うようになります(Playerとか)
    /// </summary>
  

    /// <summary>
    /// 移動状態を切り替える
    /// </summary>
    private void SwitchMovementState()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }
        else 
        {
            _movementState = EnemyMovementState.IDLE;

                return;
        }

    }

  

    private void ideling()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }
        _animator.ResetTrigger("Attack1");
        _animator.ResetTrigger("Attack2");
        _animator.SetTrigger("IdleNormal");
    }

    private void playerAttack()
    {
        _movementState = EnemyMovementState.IDLE;

        _animator.ResetTrigger("Fishman_IdleNormal");

        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _animator.ResetTrigger("Attack1");
            _animator.ResetTrigger("Attack2");
            _animator.SetTrigger("IdleNormal");
            _attackEnd = true;
        }

        if (_attackStateTimer >= _attackStateSwitchInterval)
        {
            _attackAction = true;
            _attackStateTimer = 0f;
        }

        _attackStateTimer += Time.deltaTime;

        if (!_attackAction)
        {
            return;
        }
        int randomAttack = Random.Range(0, 3);
        switch (randomAttack)
        {
            case 0:
                _animator.SetTrigger("Spin Slash Attack");

                break;

            case 1:
                _animator.SetTrigger("Attack2");

                break;

            case 2:
                _animator.SetTrigger("Attack3");

                break;
        }
        _attackAction = false;
        _attackEnd = false;
    }

    private void enemyDamage()
    {
        if (IsAnimationFinished("Fishman_Damage"))
        {
            _movementState = EnemyMovementState.IDLE;

            return;
        }
    }

    /// <summary>
    /// アニメーションが終了しているかを確認する。
    /// </summary>
    /// <param name="animationName">確認するアニメーションの名前</param>
    /// <returns>アニメーションが終了しているか</returns>
    private bool IsAnimationFinished(string animationName)
    {
        // 現在のアニメーション状態を取得
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // アニメーションが指定した名前かつ終了しているかを確認
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f;
    }
}
