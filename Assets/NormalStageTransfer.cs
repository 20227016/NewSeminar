using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NormalStageTransfer : NetworkBehaviour
{
    private NetworkRunner _runner = default;

    // �e���|�[�g���̃v���C���[���Ǘ����郊�X�g
    private List<GameObject> _playersInPortal = new List<GameObject>();

    private StageEnemyManagement _enemyManagement = default; // �G�Ǘ��N���X

    [Networked]
    private bool _clearNormalStage { get; set; } = false;

    // �K�v�ȃv���C���[��
    [Networked,Tooltip("�m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��")]
    private int _normalStageRequiredPlayers { get; set; }= 4; // �K�v�ȃv���C���[��

    private GameObject _normalTeleportPosition = default;
    private GameObject _bossTeleportPosition = default;

    [Tooltip("�m�[�}���X�e�[�W�̃e���|�[�g���W")]
    private Transform _normalStageteleportPos = default;

    [Tooltip("�{�X�X�e�[�W�̃e���|�[�g���W")]
    private Transform _bossStageteleportPos = default;

    public override void Spawned()
    {

        _enemyManagement = FindObjectOfType<StageEnemyManagement>();
        _normalTeleportPosition = GameObject.Find("NormalTeleportPosition");
        _bossTeleportPosition = GameObject.Find("BossTeleportPosition");

        _normalStageteleportPos = _normalTeleportPosition.transform;
        _bossStageteleportPos = _bossTeleportPosition.transform;

        // �G�S�ł�҂�
        Observable.CombineLatest(
            _enemyManagement.AllEnemiesDefeated
        )
        .Subscribe(_ => HandleAllEnemiesDefeated())
        .AddTo(this);
    }

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
        if ((_playersInPortal.Count >= _normalStageRequiredPlayers) && (!_clearNormalStage))
        {
            NormalTeleportAllPlayers();
        }
        else if ((_playersInPortal.Count >= _normalStageRequiredPlayers) && (_clearNormalStage))
        {
            BossTeleportAllPlayers();
        }
    }

    /// <summary>
    /// �]���|�[�^������v���C���[���������Ƃ��Ƀ��X�g����폜����
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player") && _playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Remove(collider.gameObject);
            print($"�v���C���[���|�[�^���𗣂�܂����B���݂̐l���� {_playersInPortal.Count} �ł�");
        }
    }

    /// <summary>
    /// �S�Ẵv���C���[���e���|�[�g������
    /// </summary>
    private void NormalTeleportAllPlayers()
    {
        // �m�[�}���X�e�[�W�Ƀe���|�[�g
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _normalStageteleportPos.position;
            print($"{player.name} ���m�[�}���X�e�[�W�Ƀe���|�[�g���܂���");
        }
        // ��x�m�[�}���X�e�[�W�Ƀe���|�[�g������m�[�}���X�e�[�W�ɍs�����߂̃e���|�[�g�l����1�l�ɂ���(�Đڑ��p)
        _normalStageRequiredPlayers = 1;
    }

    private void BossTeleportAllPlayers()
    {
        // �{�X�X�e�[�W�Ƀe���|�[�g
        foreach (GameObject player in _playersInPortal)
        {
            if (_normalStageRequiredPlayers != 1)
            {
                player.transform.position = _bossStageteleportPos.position;
                print($"{player.name} ���{�X�X�e�[�W�ɓr���Q���Ƃ��ăe���|�[�g���܂���");
            }
        }
    }


    private void HandleAllEnemiesDefeated()
    {
        _clearNormalStage = true;
        print("�G�S�ł̒ʒm���󂯎��܂���" + _clearNormalStage);
    }
}
