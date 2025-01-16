using Fusion;
using UnityEngine;

public class StageTransfer : NetworkBehaviour
{
    private int _playersInPortal = 0;       // �R���C�_�[���̃v���C���[��
    private const int _requiredPlayers = 4; // �K�v�ȃv���C���[��

    private void Start()
    {
        if (Runner == null)
        {
            Debug.LogError("�V�[���ǂݍ��݂����m�A�X�^�[�g���݂̂̌����");
            Debug.LogError("Runner���ݒ肳��Ă��܂���B���̃X�N���v�g�͐��������삵�Ȃ��\��������܂��B");
        }
    }


    /// <summary>
    /// �]���|�[�^���Ƀv���C���[���������Ƃ��ɐl�������Z����
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            // �R���C�_�[�ɓ������v���C���[���J�E���g
            if (Object.HasStateAuthority)
            {
                _playersInPortal++;
                CheckTeleportCondition();
                print("�v���C���[�����m�B���݂̐l����" + _playersInPortal + "�ł�");
            }
        }
    }

    /// <summary>
    /// �]���|�[�^������v���C���[���������Ƃ��ɐl��������������
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            // �R���C�_�[����o���v���C���[���J�E���g
            if (Object.HasStateAuthority)
            {
                _playersInPortal--;
                print("���݂̐l��.�ύX�����m�B���݂̐l����" + _playersInPortal + "�ł�");
            }
        }
    }

    /// <summary>
    /// �K�v�l�����]���|�[�^���ɓ������Ƃ��A�S�����w��|�C���g�܂Ńe���|�[�g������
    /// </summary>
    private void CheckTeleportCondition()
    {
        // �K�v�l������������e���|�[�g�����s����
        if (_playersInPortal >= _requiredPlayers)
        {
            TeleportAllPlayers();
        }
    }

    /// <summary>
    /// �e���|�[�g����
    /// </summary>
    private void TeleportAllPlayers()
    {

        print("�w��l�������m�B�e���|�[�g���J�n���܂�");

        // �S�v���C���[���w��|�C���g�܂ňړ�������
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject networkObject))
            {

                // �e���|�[�g��̈ʒu��ݒ�
                Vector3 teleportPosition = new Vector3(0, 0, 0); // �ڂ������W�͂��ƂŐݒ肷��
                networkObject.transform.position = teleportPosition;
            }
            print("���ׂẴv���C���[�̃e���|�[�g�ɐ������܂���");
        }
    }
}
