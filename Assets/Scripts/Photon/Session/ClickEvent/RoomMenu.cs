using Fusion;
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
    /// ���[�J���ɂ�����Q���҃I�u�W�F�N�g
    /// </summary>
    private NetworkObject _myParticipantObj = default;

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

        // �Z�b�V�����ɕR�Â��Ă���v���C���[���t
        PlayerRef playerRef = _networkRunner.LocalPlayer;
        // �Q���҃I�u�W�F�N�g���擾
        _myParticipantObj = _networkRunner.GetPlayerObject(playerRef);
        string newName = _nameInputField.text;
        Debug.LogError($"�v���C���[���t�Ƃ̊֘A�Â����Ă���I�u�W�F�N�g{_myParticipantObj}");
        IReName iRename = _myParticipantObj.GetComponent<IReName>();
        if (iRename == null)
        {

            Debug.LogError("�z�X�g�I�u�W�F�N�g�Ƀ��[������n���C���^�[�t�F�[�X��������܂���");

        }
        iRename.RPC_ParticipantReName(newName, _myParticipantObj);

    }

    /// <summary>
    /// ���������܂��z�X�g�̎��܂��͏o��
    /// </summary>
    public void Ready()
    {

        // �Z�b�V�����ɕR�Â��Ă���v���C���[���t
        PlayerRef playerRef = _networkRunner.LocalPlayer;
        // �Q���҃I�u�W�F�N�g���擾
        _myParticipantObj = _networkRunner.GetPlayerObject(playerRef);
        Debug.Log($"{_myParticipantObj.name}�Ăяo�����I�u�W�F�N�g");
        IReady iReady = _myParticipantObj.GetComponent<IReady>();
        if (iReady == null)
        {

            Debug.LogError("�z�X�g�I�u�W�F�N�g�Ƀ��[������n���C���^�[�t�F�[�X��������܂���");

        }
        iReady.ParticipantReady(_myParticipantObj);

    }

}
