using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChain : BaseEnemy
{

    [SerializeField,Header("���[�U�[�̍U���͈͂̌�����")]
    private GameObject _laserRangeView = default;

    [SerializeField, Header("���[�U�[�̌����ځi�G�t�F�N�g�j")]
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
