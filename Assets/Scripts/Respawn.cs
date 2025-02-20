using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�����X�|�[�������邽�߂̃X�N���v�g
/// </summary>
public class Respawn : MonoBehaviour
{
    [SerializeField, Tooltip("���X�|�[��Pos�I�u�W�F�N�g")]
    private Transform _playerRespawnPos = default;

    [SerializeField, Tooltip("���X�|�[��Pos�I�u�W�F�N�g")]
    private Transform _enemyRespawnPos = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("���X�|�[�����܂�");
            other.transform.position = _playerRespawnPos.position;
            other.transform.rotation = _playerRespawnPos.rotation;
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("���X�|�[�����܂�");
            other.transform.position = _enemyRespawnPos.position;
            other.transform.rotation = _enemyRespawnPos.rotation;
        }

    }

}
