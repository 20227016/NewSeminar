using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField, Tooltip("�L�����N�^�[���f��")]
    private List<GameObject> _characterModel = new List<GameObject>();

    [SerializeField, Tooltip("�I��p�L�����N�^�[���f��")]
    private List<GameObject> _animeCharacterModel = new List<GameObject>();

    [SerializeField, Tooltip("���O���̓t�B�[���h")]
    private TMP_InputField _nameInputField = default;

    [SerializeField, Tooltip("�x����")]
    private TextMeshProUGUI _warningText = default;

    // ���ݑI�����Ă���L�����N�^�[�i���L����ϐ��j
    private int _currentSelectionCharacter = default;

    // ���肵���L�����N�^�[(���L����Bool)
    private bool _characterDecision = new();

    private PlayerData _playerData = default;

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
            _animeCharacterModel[0].SetActive(true);
            _currentSelectionCharacter = 1;

            // ���X�g��1�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[0];
            secondObject.SetActive(true);
        }
    }

    //�L�����N�^�[2�̃{�^���ɂ���
    public void OnClick2()
    {
        if ((_currentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 2;

            _animeCharacterModel[1].SetActive(true);
            // ���X�g��2�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[1];
            secondObject.SetActive(true);
        }
    }

    //�L�����N�^�[3�̃{�^���ɂ���
    public void OnClick3()
    {
        if ((_currentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 3;

            _animeCharacterModel[2].SetActive(true);
            // ���X�g��3�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[2];
            secondObject.SetActive(true);
        }
    }

    //�L�����N�^�[4�̃{�^���ɂ���
    public void OnClick4()
    {
        if ((_currentSelectionCharacter != 4) && (!_characterDecision) && (!_fighterChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 4;

            _animeCharacterModel[3].SetActive(true);
            // ���X�g��4�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[3];
            secondObject.SetActive(true);
        }
    }

    // �I�����Ă���L�����N�^�[���m�肷��
    public void Decision()
    {
        // ���O�����͂���Ă��Ȃ��ꍇ�̓��^�[��
        if (string.IsNullOrWhiteSpace(_nameInputField.text))
        {
            _warningText.text = "���O����͂��Ă�������";
            _warningText.gameObject.SetActive(true);
            return;
        }

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
            // �I������ĂȂ��Ƃ�
            default:
                _warningText.text = "�L�����N�^�[��I�����Ă�������";
                _warningText.gameObject.SetActive(true);
                return;
        }
        _characterDecision = true;

        // �A�o�^�[�����Z�b�g���A���v���C���[�ɒʒm
        _playerData.RPC_SetAvatarInfo(_currentSelectionCharacter, _nameInputField.text);
        _playerData.RPC_ActiveAvatar();

        // �\�����Ă���L�����N�^�[���\��
        DeleteCharacter();

        // �I����ʂ��\��
        this.gameObject.SetActive(false);


        // �V�[�����̑S�Ă�Player���擾
        PlayerData[] allPlayerData = FindObjectsOfType<PlayerData>();

        if (allPlayerData == null)
        {
            return;
        }

        // �S�v���C���[�ɓ�����ʒm
        foreach (PlayerData playerData in allPlayerData)
        {
            playerData.RPC_ActiveAvatar();
        }

    }

    /// <summary>
    /// ����v���C���[���Z�b�g
    /// </summary>
    /// <param name="playerData">����v���C���[�̏��</param>
    public void SetPlayer(PlayerData playerData)
    {
        _playerData = playerData;
    }

    /// <summary>
    /// ���ׂẴI�u�W�F�N�g���\���ɂ���
    /// </summary>
    private void DeleteCharacter()
    {
        foreach (GameObject obj in _animeCharacterModel)
        {
            obj.SetActive(false);
        }
    }

    public void ActiveUI()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            _warningText.gameObject.SetActive(false);
        }
    }
}
