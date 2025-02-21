using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChain : BaseEnemy
{

    [SerializeField, Header("Á‚¦‚éŠÔ")]
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
    /// ƒ_ƒ[ƒW‚ğ—^‚¦‚éˆ—
    /// </summary>
    /// <param name="other"></param>
    public override void OnTriggerEnter(Collider hitCollider)
    {
       
    }

    /// <summary>
    /// ƒ_ƒ[ƒW‚ğ—^‚¦‚éˆ—
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
        // UŒ‚—Í‚ÉUŒ‚”{—¦‚ğ“n‚µ‚Ä“n‚·
        receiveDamage.ReceiveDamage((int)(_enemyStatusStruct._attackPower * _currentAttackMultiplier));

    }

}
