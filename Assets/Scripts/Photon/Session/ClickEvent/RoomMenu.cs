using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenu : MonoBehaviour
{

    /// <summary>
    /// ���O�̓��͗�
    /// </summary>
    private InputField _nameInputField = default;

    /// <summary>
    /// �l�b�g���[�N�����i�[
    /// </summary>
    private NetworkRunner _networkRunner = default;

    /// <summary>
    /// �X�V�O����
    /// </summary>
    private void Start()
    {

        _nameInputField = GameObject.Find("NameInputField").GetComponent<InputField>();
        if (_nameInputField == null)
        {

            Debug.LogError("���O�̓��͗���������܂���");

        }
        _networkRunner = GetRunner.GetRunnerMethod();
        if (_networkRunner == null)
        {

            Debug.LogError($"�����ɂ��Ă���l�b�g���[�N�����i�[��������܂���");

        }

    }

    /// <summary>
    /// ���O�̕ύX
    /// </summary>
    public void ReName()
    {

        string newName = _nameInputField.text;
        // �����̃Z�b�V�����̃v���C���[���t
        PlayerRef playerRef = _networkRunner.LocalPlayer;
        // �����̃Z�b�V�����̎Q���҃I�u�W�F�N�g
        NetworkObject participantsObj = _networkRunner.GetPlayerObject(playerRef);
        Debug.LogError($"�v���C���[���t�Ƃ̊֘A�I�u�W�F{participantsObj}");
        IRoomController _iRoomController = participantsObj.GetComponent<IRoomController>();
        if (_iRoomController == null)
        {

            Debug.LogError("�z�X�g�I�u�W�F�N�g�Ƀ��[������n���C���^�[�t�F�[�X��������܂���");

        }
        _iRoomController.RPC_ParticipantReName(newName, participantsObj);

    }

}
