using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�����X�|�[�������邽�߂̃X�N���v�g
/// </summary>
public class Respawn : MonoBehaviour
{
    [SerializeField, Tooltip("���X�|�[��Pos�I�u�W�F�N�g")]
    private GameObject _respawnPos = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("���X�|�[�����܂���");
            _respawnPos.transform.position = other.gameObject.transform.position;
        }
    }

}
