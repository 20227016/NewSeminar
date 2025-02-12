
using UnityEngine;
using System.Collections;
using Fusion;

/// <summary>
/// EvilMage.cs
/// 邪悪な魔術師の行動ロジックを管理するクラス。
/// 移動、探索、方向転換、攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: 12/9
/// 作成者: 石井直人 
/// </summary>
public class EvilMage : BaseEnemy
{
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [Header("検索対象の設定")]
    [Tooltip("検索対象となるレイヤー番号を指定します")]
    private int _targetLayer = 6; // 対象のレイヤー番号

    [Tooltip("検索範囲の半径を指定します")]
    private float _searchRadius = 50f; // 検索範囲（半径）

    [Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    private bool isAttackInterval = default; // 連続攻撃をしない

    private float _downedTimer = 5f; // ダウンタイマー
    private float _stunnedTimer = default; // のけぞりタイマー

    private float _turnSpeed = 60f; // 回転速度 (度/秒)

    // アニメーター変数
    // TransitionNo.0 Idle
    // TransitionNo.1 Attack
    // TransitionNo.2 Downed
    // TransitionNo.3 Stunned
    // TransitionNo.4 Die
    private Animator _animator;

    private GameObject _magicCharge = default; // エフェクト本体を取得
    private ParticleSystem[] _attackEffects = default; // 子オブジェクトのParticleSystemを取得

    [Header("魔法攻撃設定")]
    [Tooltip("発射する魔法弾のPrefab")]
    [SerializeField] private GameObject _magicProjectilePrefab;

    [Tooltip("魔法攻撃の溜め時間")]
    private float _chargeTime = 1.5f;

    private bool isCharging = default; // 溜め動作中かどうか

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _chargeSE = default;

    public override void Spawned()
    {
        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        _animator = GetComponent<Animator>();

        _magicCharge = GameObject.Find("MagicCharge");
        _attackEffects = GetComponentsInChildren<ParticleSystem>();
    }

    /// <summary>
    /// フレームごとの更新処理。
    /// 状態ごとに処理を分けて実行。
    /// </summary>
    protected void Update()
    {
        /* 
         * レイキャストの中心点を自動で更新してくれsる
         * これで、レイキャストを常に自分の前から打ってくれる
         * つまり、何かない場合（発生位置を変えたい等）かえなくてOK
         * 変えたかったら聞いてくれたらその都度説明する
         * これ以外にもサイズ、方向等を変えるメソッドもある
        */
        SetPostion(); // レイキャストの中心位置を設定
        SetDirection(); // レイキャストの方向を更新

        // のけぞるまでの時間
        _stunnedTimer -= Time.deltaTime;

        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                EnemyIdle();

                break;

            // ダウン(ブレイク状態)
            case EnemyMovementState.DOWNED:

                EnemyDowned();

                return;

            // のけぞり中
            case EnemyMovementState.STUNNED:

                EnemyStunned();

                return;

            // 死亡
            case EnemyMovementState.DIE:

                StartCoroutine(EnemyDie(3f));

                return;
        }

        switch (_actionState)
        {
            // サーチ
            case EnemyActionState.SEARCHING:

                PlayerSearch();
                SmoothTurn();

                break;

            // 攻撃
            case EnemyActionState.ATTACKING:

                EnemyAttacking();

                break;
        }
    }

    /// <summary>
    /// ここにアイドル状態のときの処理を書く
    /// </summary>
    private void EnemyIdle()
    {
        // アニメーションが終了したらサーチ状態に遷移
        if (IsAnimationFinished("Attack"))
        {
            _actionState = EnemyActionState.SEARCHING;
            isAttackInterval = false;
            isCharging = false;
            _chargeTime = 1.3f;

            // トリガーをセット
            _animator.SetInteger("TransitionNo", 0);
        }
    }

    /// <summary>
    /// 滑らかに方向転換する
    /// </summary>
    private void SmoothTurn()
    {
        if (_targetTrans == null)
        {
            return;
        }

        // ターゲットの位置を加工して高さYを無視
        Vector3 targetPosition = new Vector3(_targetTrans.position.x, transform.position.y, _targetTrans.position.z);

        // 自分とターゲットの方向を計算
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 回転目標を計算（高さYを無視した方向）
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 現在の回転を取得し、X軸の回転を固定
        Quaternion currentRotation = transform.rotation;
        targetRotation = Quaternion.Euler(
            currentRotation.eulerAngles.x, // 現在のX回転を維持
            targetRotation.eulerAngles.y, // ターゲット方向のY回転
            currentRotation.eulerAngles.z // 現在のZ回転を維持
        );

        // 滑らかに回転
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

        // 回転が目標角度にほぼ一致した場合、次の状態へ
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f) // 角度差が1度未満の場合
        {
            _actionState = EnemyActionState.ATTACKING;
        }
    }

    /// <summary>
    /// 攻撃動作を処理する。
    /// 魔法弾を溜めた後、発射する。
    /// </summary>
    private void EnemyAttacking()
    {
        _targetTrans = null;
        _chargeTime -= Time.deltaTime;

        // 溜め時間の待機
        if (_chargeTime <= 0 && isAttackInterval == false)
        {
            isAttackInterval = true;

            if (_magicProjectilePrefab == null)
            {
                Debug.LogWarning("魔法弾のPrefabまたは発射位置が設定されていません。");
                return;
            }

            // 魔法弾を生成して発射
            Instantiate(_magicProjectilePrefab, transform.position + transform.forward * 2.5f + Vector3.up * 0.75f, transform.rotation);
        }

        // アニメーションを設定
        if (!isCharging)
        {
            isCharging = true;

            // 溜め動作を開始
            Charge();
        }
    }

    /// <summary>
    /// 魔法弾を溜めるアニメーション・エフェクトを再生
    /// </summary>
    private void Charge()
    {
        // 溜め&発射アニメーションを再生
        _animator.SetInteger("TransitionNo", 1);

        // 溜め中のエフェクトを再能
        foreach (var effect in _attackEffects)
        {
            effect.Play();
        }

        _audioSource.PlayOneShot(_chargeSE);
    }

    /// <summary>
    /// ダウン状態
    /// </summary>
    private void EnemyDowned()
    {
        // ダウンが終了した場合、状態を戻す
        if (_downedTimer <= 0)
        {
            _animator.SetInteger("TransitionNo", 0);

            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.SEARCHING;

            isAttackInterval = false;
            _chargeTime = 1.4f;

            _magicCharge.SetActive(true);

            _downedTimer = 5f;
            return;
        }

        // トリガーをセット
        _animator.SetInteger("TransitionNo", 2);

        // 攻撃エフェクト停止
        _magicCharge.SetActive(false);

        _downedTimer -= Time.deltaTime;
    }

    private void EnemyStunned()
    {
        // のけぞり終わったら状態遷移
        if (IsAnimationFinished("Stunned"))
        {
            _animator.SetInteger("TransitionNo", 0);

            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.SEARCHING;

            isAttackInterval = false;
            _chargeTime = 1.4f;

            _magicCharge.SetActive(true);

            return;
        }

        // トリガーをセット
        _animator.SetInteger("TransitionNo", 3);

        // 攻撃エフェクト停止
        _magicCharge.SetActive(false);

        // 次にのけぞるまでの時間をセット
        _stunnedTimer = 3f;
    }

    /// <summary>
    /// 倒れる
    /// </summary>
    private IEnumerator EnemyDie(float fadeDuration)
    {

        // トリガーをセット
        _animator.SetInteger("TransitionNo", 4);

        // 攻撃エフェクトが存在する場合のみ停止
        if (_magicCharge != null)
        {
            _magicCharge.SetActive(false);
        }
        // 秒後
        yield return new WaitForSeconds(fadeDuration);

        EnemyDespawn();
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
    /// 自分を中心とした一定範囲内で、指定のレイヤーに属するオブジェクトを検索し、
    /// 最も近いオブジェクトを特定します。
    /// </summary>
    private void PlayerSearch()
    {
        if (_targetTrans != null)
        {
            return;
        }

        // Physics.OverlapSphereで指定範囲内のすべてのコライダーを取得
        Collider[] hits = Physics.OverlapSphere(transform.position, _searchRadius);

        // 最も近いオブジェクトを探すための変数を初期化
        float closestDistance = Mathf.Infinity; // 無限大の距離で初期化
        Transform closestObject = null; // 最も近いオブジェクトのTransform

        // 検索範囲内のすべてのオブジェクトをループ処理
        foreach (var hit in hits)
        {
            // オブジェクトが指定されたレイヤーに属しているかを確認
            if (hit.gameObject.layer == _targetLayer)
            {
                // 自分との距離を計算
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                // これまでの最小距離と比較し、より近ければ更新
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.transform; // 最も近いオブジェクトを更新
                }
            }
        }

        // 最も近いオブジェクトをプロパティに保存
        _targetTrans = closestObject;
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {
        _movementState = EnemyMovementState.DIE;
    }
}