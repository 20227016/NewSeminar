using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �L�����N�^�[�I����ʐ���p�X�N���v�g
/// �Z���L�q
/// 1 �^���N 
/// 2 �R�m
/// 3 �q�[���[
/// 4 �t�@�C�^�[
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{

    // ���ݑI�����Ă���L�����N�^�[�i���L����ϐ��j
    private int _currentSelectionCharacter = default;

    // ���肵���L�����N�^�[(���L����Bool)
    private bool _characterDecision = new();

    [SerializeField, Tooltip("�L�����N�^�[���f�����i�[")]
    private List<GameObject> _characterModel = new List<GameObject>();

    private PlayerData _player = default;

    public bool _tankChoice { get; set; } = false;
    public bool _knightChoice { get; set; } = false;
    public bool _healerChoice { get; set; } = false;
    public bool _fighterChoice { get; set; } = false;

    //�L�����N�^�[1�̃{�^���ɂ���
    public void OnClick1()
    {

        if ((_currentSelectionCharacter != 1) && (!_characterDecision) && (!_tankChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 1;

            // ���X�g��1�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[0];
            secondObject.SetActive(true);
            print("�^���N");
        }

    }

    //�L�����N�^�[2�̃{�^���ɂ���
    public void OnClick2()
    {

        if ((_currentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 2;

            // ���X�g��2�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[1];
            secondObject.SetActive(true);
            print("�R�m");
        }
    }

    //�L�����N�^�[3�̃{�^���ɂ���
    public void OnClick3()
    {
        if ((_currentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
        {

            DeleteCharacter();
            _currentSelectionCharacter = 3;

            // ���X�g��3�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[2];
            secondObject.SetActive(true);
            print("�q�[���[");
        }
    }

    //�L�����N�^�[4�̃{�^���ɂ���
    public void OnClick4()
    {

        if ((_currentSelectionCharacter != 4) && (!_characterDecision) && (!_fighterChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 4;

            // ���X�g��4�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[3];
            secondObject.SetActive(true);
            print("�t�@�C�^�[");
        }
    }

    // �I�����Ă���L�����N�^�[���m�肷��
    public void Decision()
    {
        _characterDecision = true;

        switch (_currentSelectionCharacter)
        {
            // �^���N
            case 1:
                _tankChoice = true;
                break;
            // �R�m
            case 2:
                _knightChoice = true;
                break;
            // �q�[���[
            case 3:
                _healerChoice = true;
                break;
            // �t�@�C�^�[
            case 4:
                _fighterChoice = true;
                break;
        }

        _player.RPC_SetAvatarNumber(_currentSelectionCharacter);
        _player.RPC_ActiveAvatar();
        // �V�[�����̑S�Ă�Player���擾
        PlayerData[] allPlayerData = FindObjectsOfType<PlayerData>();

        if (allPlayerData == null)
        {
            return;
        }

        foreach (PlayerData playerData in allPlayerData)
        {
            playerData.RPC_ActiveAvatar();
        }

        this.gameObject.SetActive(false);
    }

    public void SetPlayer(PlayerData playerData)
    {
        _player = playerData;
    }

    /// <summary>
    /// ���ׂẴI�u�W�F�N�g���\���ɂ���
    /// </summary>
    private void DeleteCharacter()
    {

        foreach (GameObject obj in _characterModel)
        {
            obj.SetActive(false);
        }
    }

}
