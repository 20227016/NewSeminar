using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShield : BaseEnemy
{
    private Vector3 _targetScale = new Vector3(2.5f, 2.5f, 2.5f); // �ŏI�I�ȑ傫��
    private float _scaleSpeed = 0.75f; // �傫���Ȃ鑬�x

    //AudioSource�^�̕ϐ���錾
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip�^�̕ϐ���錾
    [SerializeField] private AudioClip _explosionSE = default;

    /// <summary>
    /// ���ʉ�����
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_explosionSE);
    }

    private void Update()
    {
        // ���X�ɑ傫������
        transform.localScale = Vector3.MoveTowards(transform.localScale, _targetScale, _scaleSpeed * Time.deltaTime);

        // �ڕW�T�C�Y�ɒB������폜
        if (transform.localScale == _targetScale)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }

    public override void RPC_ReceiveDamage(int damegeValue)
    {

    }
}
