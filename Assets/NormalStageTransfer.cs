using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NormalStageTransfer : NetworkBehaviour
{

    // �e���|�[�g���̃v���C���[���Ǘ����郊�X�g
    private List<GameObject> _playersInPortal = new List<GameObject>();

    private EnemySpawner _enemySpawner = default; // �G�Ǘ��N���X

    [Networked]
    private bool _clearNormalStage { get; set; } = false;

    // �K�v�ȃv���C���[��
    [Networked, SerializeField,Tooltip("�m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��")]
    private int _normalStageRequiredPlayers { get; set; } = 2; // �K�v�ȃv���C���[��

    private GameObject _normalTeleportPosition = default;
    private GameObject _bossTeleportPosition = default;

    [Tooltip("�m�[�}���X�e�[�W�̃e���|�[�g���W")]
    private Transform _normalStageteleportPos = default;

    [Tooltip("�{�X�X�e�[�W�̃e���|�[�g���W")]
    private Transform _bossStageteleportPos = default;

    public override void Spawned()
    {

        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _normalTeleportPosition = GameObject.Find("NormalTeleportPosition");
        _bossTeleportPosition = GameObject.Find("BossTeleportPosition");

        _normalStageteleportPos = _normalTeleportPosition.transform;
        _bossStageteleportPos = _bossTeleportPosition.transform;

        _enemySpawner.OnAllEnemiesDefeatedObservable.Subscribe(_ =>
        {
            // �G�l�~�[�S�Ŏ��̏������L�q
            Debug.Log("���̃X�N���v�g�ŃG�l�~�[�S�ŃC�x���g���󂯎��܂����I");
            HandleAllEnemiesDefeated();
        }).AddTo(this);
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
            print("�����̃e���|�[�g");
            NormalTeleportAllPlayers();
        }
        else if ((_playersInPortal.Count >= _normalStageRequiredPlayers) && (_clearNormalStage))
        {
            print("�{�X�e���|�[�g�A����");
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

            player.transform.position = _bossStageteleportPos.position;
            print($"{player.name} ���{�X�X�e�[�W�ɓr���Q���Ƃ��ăe���|�[�g���܂���");

        }
    }


    private void HandleAllEnemiesDefeated()
    {
        RPC_AllEnemiesDefeated();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_AllEnemiesDefeated()
    {
        _clearNormalStage = true;
        print("�G�S�ł̒ʒm���󂯎��܂���" + _clearNormalStage);
    }
}
