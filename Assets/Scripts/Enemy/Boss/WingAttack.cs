using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAttack : MonoBehaviour
{
    [Tooltip("���̃_���[�W")]
    [SerializeField] private float _damage = 10f;

    /// <summary>
    /// ���̃I�u�W�F�N�g�ƏՓ˂����ۂ̏����B
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    private void OnTriggerEnter(Collider other)
    {
        // �_���[�W��^���鏈���i��: �v���C���[�ȂǓ���̃��C���[�̏ꍇ�j
        if (other.CompareTag("Player")) // �v���C���[�ɑ΂��ă_���[�W��^����
        {
            // �v���C���[�̃_���[�W�������Ăяo���i���̗�j
            Debug.Log($"Wing Hit {other.gameObject.name}, dealt {_damage} damage.");
        }
    }
}
