using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChain : BaseEnemy
{

    [SerializeField,Header("レーザーの攻撃範囲の見た目")]
    private GameObject _laserRangeView = default;

    [SerializeField, Header("レーザーの見た目（エフェクト）")]
    private GameObject _laserView = default;

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
        


    }
}
