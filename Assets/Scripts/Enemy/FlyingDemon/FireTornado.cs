
using UnityEngine;
using Fusion;
using System.Collections;

/// <summary>
/// FireTornado.cs
/// �N���X����
/// ���̗�������
/// 
/// �쐬��: 1/17
/// �쐬��: �Έ䒼�l
/// </summary>

public class FireTornado : BaseEnemy
{
    [Tooltip("���̗����̐�������")]
    [SerializeField] private float _lifeTime = 5f;

    private float _elapsedTime = 0f; // �o�ߎ���

    //AudioSource�^�̕ϐ���錾
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip�^�̕ϐ���錾
    [SerializeField] private AudioClip _sE = default;

    /// <summary>
    /// ���ʉ�����
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_sE);
    }

    /// <summary>
    /// ���̗����̐�������
    /// </summary>
    private void Update()
    {
        // �����𒴂����ꍇ�A��A�N�e�B�u��
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _lifeTime)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g�ƏՓ˂����ۂ̏����B
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    /// <summary>
    /// ���̗������A�N�e�B�u������B
    /// </summary>
    private void Deactivate()
    {
        Destroy(gameObject);
        // gameObject.SetActive(false); // �I�u�W�F�N�g���A�N�e�B�u��
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }

    /// <summary>
    /// �_���[�W����
    /// </summary>
    /// <param name="damegeValue">�_���[�W</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public override void RPC_ReceiveDamage(int damegeValue)
    {

    }

}
