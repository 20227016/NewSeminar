using System.Collections.Generic;
using UnityEngine;

public class BossStageTransfer : MonoBehaviour
{
    // �e���|�[�g���̃v���C���[���Ǘ����郊�X�g
    private List<GameObject> _playersInPortal = new List<GameObject>();

    [SerializeField]
    private StageEnemyManagement _enemyManagement = default; // �G�Ǘ��N���X

    // �K�v�ȃv���C���[��
    [SerializeField, Tooltip("�m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��")]
    private int _bossStageRequiredPlayers = 1; // �K�v�ȃv���C���[��

    [SerializeField, Tooltip("�{�X�X�e�[�W�̃e���|�[�g���W")]
    private Transform _bossTeleportPos = default;


    /// <summary>
    /// �]���|�[�^���Ƀv���C���[���������Ƃ��Ƀ��X�g�ɒǉ�����
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !_playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Add(collider.gameObject);
            print($"�v���C���[�����m�B���݂̐l���� {_playersInPortal.Count} �ł�");
        }

        // �K�v�l������������S�����e���|�[�g
        if (_playersInPortal.Count >= _bossStageRequiredPlayers)
        {
            BossTeleportAllPlayers();
        }
    }

    private void BossTeleportAllPlayers()
    {
        // �{�X�X�e�[�W�Ƀe���|�[�g
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _bossTeleportPos.position;
            print($"{player.name} ���{�X�����Ƀe���|�[�g���܂���");
        }
    }
}
