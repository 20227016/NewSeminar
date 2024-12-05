using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class CharacterSelectionManager : NetworkBehaviour
{
    // 現在選択しているキャラクター
    private int _currentSelectionCharacter = 0;

    // 確定選択しているキャラクター
    private int _confirmedSelectionCharacter = default;


    //キャラクター1のボタンにつける
    public void OnClick1()
    {
        _currentSelectionCharacter = 1;
    }

    //キャラクター2のボタンにつける
    public void OnClick2()
    {
        _currentSelectionCharacter = 2;
    }

    //キャラクター3のボタンにつける
    public void OnClick3()
    {
        _currentSelectionCharacter = 3;
    }

    //キャラクター4のボタンにつける
    public void OnClick4()
    {
        _currentSelectionCharacter = 4;
    }

    //キャラクター5のボタンにつける
    public void OnClick5()
    {
        _currentSelectionCharacter = 5;
    }

    //キャラクター6のボタンにつける
    public void OnClick6()
    {
        _currentSelectionCharacter = 6;
    }

    //キャラクター7のボタンにつける
    public void OnClick7()
    {
        _currentSelectionCharacter = 7;
    }

    //キャラクター8のボタンにつける
    public void OnClick8()
    {
        _currentSelectionCharacter = 8;
    }

    // 選択しているキャラクターを確定する
    public void ConfirmedOnClick()
    {
        _confirmedSelectionCharacter = _currentSelectionCharacter;
    }

}
