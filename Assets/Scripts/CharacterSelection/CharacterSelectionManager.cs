using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System;

public class CharacterSelectionManager : NetworkBehaviour
{

    // ���ݑI�����Ă���L�����N�^�[�i���L����ϐ��j
    public static int CurrentSelectionCharacter { get; private set; } = 0;

    // �m��I�����Ă���L�����N�^�[
    private int _confirmedSelectionCharacter = default;

    [SerializeField,Tooltip("�L�����N�^�[���f�����i�[")]
    private List<GameObject> _characterModel = new List<GameObject>();

    private void Update()
    {
       
    }

    //�L�����N�^�[1�̃{�^���ɂ���
    public void OnClick1()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 1;

        // ���X�g��1�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[0];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[2�̃{�^���ɂ���
    public void OnClick2()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 2;

        // ���X�g��2�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[1];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[3�̃{�^���ɂ���
    public void OnClick3()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 3;

        // ���X�g��3�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[2];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[4�̃{�^���ɂ���
    public void OnClick4()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 4;

        // ���X�g��4�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[3];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[5�̃{�^���ɂ���
    public void OnClick5()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 5;

        // ���X�g��5�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[4];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[6�̃{�^���ɂ���
    public void OnClick6()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 6;

        // ���X�g��6�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[5];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[7�̃{�^���ɂ���
    public void OnClick7()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 7;

        // ���X�g��7�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[6];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    //�L�����N�^�[8�̃{�^���ɂ���
    public void OnClick8()
    {
        DeleteCharacter();
        CurrentSelectionCharacter = 8;

        // ���X�g��8�Ԗڂ̃I�u�W�F�N�g���擾���Đ���
        GameObject secondObject = _characterModel[7];
        secondObject.SetActive(true);
        print(CurrentSelectionCharacter);
    }

    // �I�����Ă���L�����N�^�[���m�肷��
    public void ConfirmedOnClick()
    {
        _confirmedSelectionCharacter = CurrentSelectionCharacter;
        print(_confirmedSelectionCharacter);
        print(CurrentSelectionCharacter);
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
