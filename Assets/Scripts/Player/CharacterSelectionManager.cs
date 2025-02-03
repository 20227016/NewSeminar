using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    [SerializeField, Tooltip("�I��p�L�����N�^�[���f��")]
    private List<GameObject> _animeCharacterModel = new List<GameObject>();

    [SerializeField, Tooltip("���O���̓t�B�[���h")]
    private TMP_InputField _nameInputField = default;

    [SerializeField, Tooltip("�x����")]
    private TextMeshProUGUI _warningText = default;

    [SerializeField, Tooltip("���[����")]
    private TextMeshProUGUI _roolName = default;

    // ���ݑI�����Ă���L�����N�^�[�i���L����ϐ��j
    private int _currentSelectionCharacter = default;

    // ���肵���L�����N�^�[(���L����Bool)
    private bool _characterDecision = new();

    private PlayerData _playerData = default;

    public bool _tankChoice { get; set; } = false;
    public bool _knightChoice { get; set; } = false;
    public bool _healerChoice { get; set; } = false;
    public bool _fighterChoice { get; set; } = false;

    public void UpSelect(InputAction.CallbackContext context)
    {
        if (!context.performed || _characterDecision) return;

        _currentSelectionCharacter--;
        Debug.Log("��");
        if (_currentSelectionCharacter < 1)
        {
            _currentSelectionCharacter = _animeCharacterModel.Count; // �Ō�̃L�����N�^�[�Ƀ��[�v
        }
        UpdateCharacterSelection();
    }

    public void DownSelect(InputAction.CallbackContext context)
    {
        if (!context.performed || _characterDecision) return;
        Debug.Log("��");
        _currentSelectionCharacter++;
        if (_currentSelectionCharacter > _animeCharacterModel.Count)
        {
            _currentSelectionCharacter = 1; // �ŏ��̃L�����N�^�[�Ƀ��[�v
        }
        UpdateCharacterSelection();
    }

    public void Decision(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("����");
        Decision();
    }

    /// <summary>
    /// �L�����N�^�[�I�����X�V
    /// </summary>
    private void UpdateCharacterSelection()
    {
        DeleteCharacter();

        switch (_currentSelectionCharacter)
        {
            case 1:
                if (!_tankChoice) _roolName.text = "�m�[�}��";
                break;
            case 2:
                if (!_knightChoice) _roolName.text = "�t�@�C�^�[";
                break;
            case 3:
                if (!_healerChoice) _roolName.text = "�q�[���[";
                break;
            case 4:
                if (!_fighterChoice) _roolName.text = "�^���N";
                break;
            default:
                return;
        }

        _animeCharacterModel[_currentSelectionCharacter - 1].SetActive(true);
    }

    //�L�����N�^�[1�̃{�^���ɂ���
    public void OnClick1()
    {
        if ((_currentSelectionCharacter != 1) && (!_characterDecision) && (!_tankChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 1;
            _roolName.text = "�m�[�}��";
            _animeCharacterModel[0].SetActive(true);
        }
    }

    //�L�����N�^�[2�̃{�^���ɂ���
    public void OnClick2()
    {
        if ((_currentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 2;
            _roolName.text = "�t�@�C�^�[";
            _animeCharacterModel[1].SetActive(true);
        }
    }

    //�L�����N�^�[3�̃{�^���ɂ���
    public void OnClick3()
    {
        if ((_currentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 3;
            _roolName.text = "�q�[���[";
            _animeCharacterModel[2].SetActive(true);
        }
    }

    //�L�����N�^�[4�̃{�^���ɂ���
    public void OnClick4()
    {
        if ((_currentSelectionCharacter != 4) && (!_characterDecision) && (!_fighterChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 4;
            _roolName.text = "�^���N";
            _animeCharacterModel[3].SetActive(true);
        }
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
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

            CharacterBase characterBase = playerData.GetComponentInChildren<CharacterBase>();

            if(characterBase == null)
            {
                continue;
            }

            characterBase.RPC_SetAllyHPBar(characterBase);
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
