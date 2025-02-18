using Fusion;
using UnityEngine;

public class EnemyTest : BaseEnemy
{
    [Networked]
    public int Health { get; private set; } // ���������̗�
    private EnemyNetworkManager _networkManager;

    //[SerializeField]
    //private int maxHealth = 100; // �ő�̗�

    [SerializeField]
    private GameObject deathEffectPrefab; // ���S�G�t�F�N�g

    private void Awake()
    {
        // �l�b�g���[�N�Ǘ��N���X���擾
        _networkManager = GetComponent<EnemyNetworkManager>();
    }

    /*
    /// <summary>
    /// ����������
    /// </summary>
    public override void OnNetworkSpawn()
    { 
        if (Object.HasStateAuthority) // �T�[�o�[����������ꍇ
        {
            Health = maxHealth; // �T�[�o�[���ŏ�����
        }    
    }
    */

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    /// <param name="damage">�󂯂��_���[�W��</param>
    public void TakeDamage(int damage)
    {
        // if (!Object.HasStateAuthority) return; // �T�[�o�[�݂̂�����

        if (Health <= 0) return; // ���Ɏ��S���Ă���ꍇ�͖���

        Health -= damage;
        Health = Mathf.Max(Health, 0); // �ŏ��l��0�ɐ���

        Debug.Log($"{gameObject.name} took {damage} damage, remaining health: {Health}");

        if (Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// �G�����S�����Ƃ��̏���
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        if (deathEffectPrefab != null)
        {
            // ���S�G�t�F�N�g�𐶐�
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Runner.Despawn(Object); // �l�b�g���[�N�I�u�W�F�N�g���폜
    }

    /// <summary>
    /// �N���C�A���g�����̓����pUI���\�b�h
    /// </summary>
    public int GetHealth()
    {
        return Health;
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {
    
    }
}
