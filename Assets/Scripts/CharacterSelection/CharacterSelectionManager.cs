using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public static int CurrentSelectionCharacter { get; private set; } = default;

    // ���肵���L�����N�^�[(���L����Bool)
    public static bool _characterDecision { get; private set; } = false;

    // �m��I�����Ă���L�����N�^�[
    private int _confirmedSelectionCharacter { get; set; } = default;

    [SerializeField,Tooltip("�L�����N�^�[���f�����i�[")]
    private List<GameObject> _characterModel = new List<GameObject>();

    public bool _tankChoice { get; set; } = false;
    public bool _knightChoice { get; set; } = false;
    public bool _healerChoice { get; set; } = false;
    public bool _fighterChoice { get; set; } = false;

    [SerializeField]
    private PlayerData _playerData = default;


    //�L�����N�^�[1�̃{�^���ɂ���
    public void OnClick1()
    {

        if((CurrentSelectionCharacter != 1) && (!_characterDecision) && (!_tankChoice))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 1;

            // ���X�g��1�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[0];
            secondObject.SetActive(true);
            print("�^���N");
        }

    }

    //�L�����N�^�[2�̃{�^���ɂ���
    public void OnClick2()
    {

        if ((CurrentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 2;

            // ���X�g��2�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[1];
            secondObject.SetActive(true);
            print("�R�m");
        }
    }

    //�L�����N�^�[3�̃{�^���ɂ���
    public void OnClick3()
    {
        if ((CurrentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
        {

            DeleteCharacter();
            CurrentSelectionCharacter = 3;

            // ���X�g��3�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[2];
            secondObject.SetActive(true);
            print("�q�[���[");
        }
    }

    //�L�����N�^�[4�̃{�^���ɂ���
    public void OnClick4()
    {

        if ((CurrentSelectionCharacter != 4) && (!_characterDecision) && (!_fighterChoice))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 4;

            // ���X�g��4�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
            GameObject secondObject = _characterModel[3];
            secondObject.SetActive(true);
            print("�t�@�C�^�[");
        }
    }

    // �I�����Ă���L�����N�^�[���m�肷��
    public void ConfirmedOnClick()
    {
        _characterDecision = true;
        _confirmedSelectionCharacter = CurrentSelectionCharacter;
        switch(_confirmedSelectionCharacter)
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
        print("���肳�ꂽ�L�����N�^�[ "+_confirmedSelectionCharacter);

        SetPlayerData();

        SceneManager.LoadScene("Main");
    }

    private void SetPlayerData()
    {
        var playerData = FindObjectOfType<PlayerData>();
        if (playerData == null)
        {
            playerData = Instantiate(_playerData);
        }
        playerData.SetPlayerNumber(_confirmedSelectionCharacter);
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
