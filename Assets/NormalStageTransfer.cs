using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NormalStageTransfer : NetworkBehaviour
{

    /// <summary>
    ///  �e���|�[�g���̃v���C���[���Ǘ����郊�X�g
    /// </summary>
    private List<GameObject> _playersInPortal = new List<GameObject>();

    /// <summary>
    /// �m�[�}���X�e�[�W�̃|�[�^���ɓ���X�e�[�W�ړ�������̈ʒu
    /// </summary>
    private GameObject _normalTeleportPosition = default;
    /// <summary>
    /// �{�X�X�e�[�W�̃|�[�^���ɓ���X�e�[�W�ړ�������̈ʒu
    /// </summary>
    private GameObject _bossTeleportPosition = default;

    [Tooltip("�m�[�}���X�e�[�W�̃e���|�[�g���W")]
    private Transform _normalStageteleportPos = default;

    [Tooltip("�{�X�X�e�[�W�̃e���|�[�g���W")]
    private Transform _bossStageteleportPos = default;

    /// <summary>
    /// �G�Ǘ��N���X
    /// </summary>
    private EnemySpawner _enemySpawner = default;

    /// <summary>
    /// �m�[�}���X�e�[�W���N���A�������̃t���O
    /// </summary>
    [Networked]
    public bool ClearNormalStage { get; set; } = false;

    /// <summary>
    /// �m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��
    /// </summary>
    [Networked, Tooltip("�m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��")]
    public int StageRequiredPlayers { get; set; }


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
            print($"�v���C���[�����m�B���݂̐l���� {_playersInPortal.Count}/{StageRequiredPlayers} �ł�");
        }
        // �K�v�l������������S�����e���|�[�g
        if ((_playersInPortal.Count >= StageRequiredPlayers) && (!ClearNormalStage))
        {
            Debug.Log($"<color=red>�|�[�^���l���F{_playersInPortal.Count}</color>");
            Debug.Log($"<color=red>�Q���l���F{StageRequiredPlayers}</color>");
            print("�����̃e���|�[�g");
            NormalTeleportAllPlayers();
        }
        else if ((_playersInPortal.Count >= StageRequiredPlayers) && (ClearNormalStage))
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
            print($"<color=red>�v���C���[���|�[�^���𗣂�܂����B���݂̐l���� {_playersInPortal.Count} �ł�</color>");
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
        StageRequiredPlayers = 1;
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
        ClearNormalStage = true;
        print("�G�S�ł̒ʒm���󂯎��܂���" + ClearNormalStage);
    }
}
