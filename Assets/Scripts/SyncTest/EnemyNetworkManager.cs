using Fusion;
using UnityEngine;

public class EnemyNetworkManager : NetworkBehaviour
{
    [Networked] // �N���C�A���g�Ԃœ��������ϐ�
    public int Health { get; private set; } // �̗͂𓯊�

    [SerializeField]
    private int maxHealth = 100;

    /// <summary>
    /// ������
    /// </summary>
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Health = maxHealth; // �T�[�o�[���ŏ�����
        }
    }

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    /// <param name="damage">�󂯂�_���[�W��</param>
    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority) return; // �T�[�o�[�݂̂�����

        if (Health <= 0) return;

        Health -= damage;
        Health = Mathf.Max(Health, 0); // �ŏ��l��0�ɐ���

        Debug.Log($"Enemy {gameObject.name} took {damage} damage, remaining health: {Health}");

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
        Debug.Log($"Enemy {gameObject.name} has died.");
        Runner.Despawn(Object); // �T�[�o�[���ō폜
    }
}
