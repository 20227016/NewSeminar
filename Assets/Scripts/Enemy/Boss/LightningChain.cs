using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChain : BaseEnemy
{

    [SerializeField, Header("消える時間")]
    private float _time = default;

    private float _timer = default;

    protected override void OnDeath()
    {

        EnemyDespawn();

    }

    // Start is called before the first frame update
    void Start()
    {

      

    }

    // Update is called once per frame
    void Update()
    {

        _timer += Time.deltaTime;
        if (_time <= _timer)
        {

            _timer = 0;
            EnemyDespawn();

        }

    }

    /// <summary>
    /// ダメージを与える処理
    /// </summary>
    /// <param name="other"></param>
    public override void OnTriggerEnter(Collider hitCollider)
    {
       
    }

    /// <summary>
    /// ダメージを与える処理
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerStay(Collider hitCollider)
    {

        if (!hitCollider.CompareTag("Player"))
        {
            return;
        }

        IReceiveDamage receiveDamage = hitCollider.GetComponent<IReceiveDamage>();
        if (receiveDamage == null)
        {
            return;
        }
        // 攻撃力に攻撃倍率を渡して渡す
        receiveDamage.ReceiveDamage((int)(_enemyStatusStruct._attackPower * _currentAttackMultiplier));

    }

}
