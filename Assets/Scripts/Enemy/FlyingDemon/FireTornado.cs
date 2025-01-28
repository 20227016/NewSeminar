
using UnityEngine;
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
        gameObject.SetActive(false); // �I�u�W�F�N�g���A�N�e�B�u��
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }
}
