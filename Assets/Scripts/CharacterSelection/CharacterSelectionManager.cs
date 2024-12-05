using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class CharacterSelectionManager : NetworkBehaviour
{
    // ���ݑI�����Ă���L�����N�^�[
    private int _currentSelectionCharacter = 0;

    // �m��I�����Ă���L�����N�^�[
    private int _confirmedSelectionCharacter = default;


    //�L�����N�^�[1�̃{�^���ɂ���
    public void OnClick1()
    {
        _currentSelectionCharacter = 1;
    }

    //�L�����N�^�[2�̃{�^���ɂ���
    public void OnClick2()
    {
        _currentSelectionCharacter = 2;
    }

    //�L�����N�^�[3�̃{�^���ɂ���
    public void OnClick3()
    {
        _currentSelectionCharacter = 3;
    }

    //�L�����N�^�[4�̃{�^���ɂ���
    public void OnClick4()
    {
        _currentSelectionCharacter = 4;
    }

    //�L�����N�^�[5�̃{�^���ɂ���
    public void OnClick5()
    {
        _currentSelectionCharacter = 5;
    }

    //�L�����N�^�[6�̃{�^���ɂ���
    public void OnClick6()
    {
        _currentSelectionCharacter = 6;
    }

    //�L�����N�^�[7�̃{�^���ɂ���
    public void OnClick7()
    {
        _currentSelectionCharacter = 7;
    }

    //�L�����N�^�[8�̃{�^���ɂ���
    public void OnClick8()
    {
        _currentSelectionCharacter = 8;
    }

    // �I�����Ă���L�����N�^�[���m�肷��
    public void ConfirmedOnClick()
    {
        _confirmedSelectionCharacter = _currentSelectionCharacter;
    }

}
