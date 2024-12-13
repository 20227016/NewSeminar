
using UnityEngine;
using System.Collections;

/// <summary>
/// Golem.cs
/// クラス説明
/// エネミー（ゴーレム）処理
///
/// 作成日: 11/1
/// 作成者: 石井直人 
/// </summary>
public class Golem2 : BaseEnemy
{
    // ここでEnemを作成。
    // なぜEnemが2つあるかというと、走りながら攻撃するため（他にもあるけど...）
    // 一つだと、移動しながら何かをすることができないから、二つ作ってます。説明下手すぎごめん
    // 状態を追加したい場合、パブリックでEnemを設定しているので、Enemパブリッククラス(そういうスクリプトがある)に追加すれば使えます。多分
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    [SerializeField, Tooltip("探索範囲(前方距離)")]
    private float _searchRange = default;

    private void Awake()
    {
        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();
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
        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                Samplemethod();

                break;

            // 移動
            case EnemyMovementState.RUNNING:

                EnemyRunning();

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

                PlayerLook();

                break;

            // 攻撃
            case EnemyActionState.ATTACKING:

                break;
        }
    }

    /// <summary>
    /// サンプルメソッド
    /// ここにアイドル状態のときの処理を書く()
    /// </summary>
    private void Samplemethod()
    {
        // 動けなくするとか
    }

    private void EnemyRunning()
    {
        // 前進する処理を書く
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
    private void PlayerLook()
    {
        // プレイヤーのTransformを取得
        Transform playerTrans = _targetTrans;

        if (playerTrans != null)
        {
            // プレイヤーの位置を取得
            Vector3 playerPosition = playerTrans.position;

            // プレイヤーのY軸を無視したターゲットの位置を計算
            Vector3 lookPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);

            // プレイヤーの方向に向く
            transform.LookAt(lookPosition);
        }
    }

    /// <summary>
    /// オブジェクトを探す(サンプル)
    /// サーチの例。好きなように改良してね。
    /// 私の作ったメソッドを解説すると、
    /// プレイヤーの方向に常にレイキャストを伸ばし、オブジェクトなどに邪魔されず、直接プレイヤーにレイキャストが触れたときに
    /// アクションEnumをサーチから攻撃に切り替える処理を書いてるよ。
    /// で、もしレイキャストがオブジェクトなどに邪魔されて、プレイヤーに届かなかった場合は、アクションEnumはサーチのまま
    /// MoveEnumは移動にし、サーチしながら移動（目の前にある障害物を回避するため）する処理を書いているよ。
    /// </summary>
    private void PlayerSearch()
    {
        RaycastHit hit = Search.BoxCast(_boxCastStruct);
        if (hit.collider.gameObject.layer == 6)
        {
            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.ATTACKING;
        }
        else
        {
            _movementState = EnemyMovementState.RUNNING;
        }

        if (!hit.collider)
        {
            return;
        }
    }
}