using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�����X�|�[�������邽�߂̃X�N���v�g
/// </summary>
public class Respawn : MonoBehaviour
{
    [SerializeField, Tooltip("���X�|�[��Pos�I�u�W�F�N�g")]
    private Transform _respawnPos = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("���X�|�[�����܂�");
            other.transform.position = _respawnPos.position;
            other.transform.rotation = _respawnPos.rotation;
        }
    }

}
