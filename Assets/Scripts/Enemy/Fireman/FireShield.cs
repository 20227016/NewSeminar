using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShield : BaseEnemy
{
    private Vector3 _targetScale = new Vector3(2.5f, 2.5f, 2.5f); // 最終的な大きさ
    private float _scaleSpeed = 0.75f; // 大きくなる速度

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _explosionSE = default;

    /// <summary>
    /// 効果音発生
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_explosionSE);
    }

    private void Update()
    {
        // 徐々に大きくする
        transform.localScale = Vector3.MoveTowards(transform.localScale, _targetScale, _scaleSpeed * Time.deltaTime);

        // 目標サイズに達したら削除
        if (transform.localScale == _targetScale)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {

    }

    public override void RPC_ReceiveDamage(int damegeValue)
    {

    }
}
