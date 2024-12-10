using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System;

public class CharacterSelectionManager : NetworkBehaviour
{

    // 現在選択しているキャラクター（共有する変数）
    public static int CurrentSelectionCharacter { get; private set; } = 0;

    // 決定したキャラクター(共有するBool)
    public static bool _characterDecision { get; private set; } = false;

    // 確定選択しているキャラクター
    private int _confirmedSelectionCharacter = default;

    [SerializeField,Tooltip("キャラクターモデルを格納")]
    private List<GameObject> _characterModel = new List<GameObject>();

    private void Update()
    {
        print(_characterDecision);
    }

    //キャラクター1のボタンにつける
    public void OnClick1()
    {

        if((CurrentSelectionCharacter != 1) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 1;

            // リストの1番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[0];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }

    }

    //キャラクター2のボタンにつける
    public void OnClick2()
    {

        if ((CurrentSelectionCharacter != 2) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 2;

            // リストの2番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[1];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    //キャラクター3のボタンにつける
    public void OnClick3()
    {
        if ((CurrentSelectionCharacter != 3) && (!_characterDecision))
        {

            DeleteCharacter();
            CurrentSelectionCharacter = 3;

            // リストの3番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[2];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    //キャラクター4のボタンにつける
    public void OnClick4()
    {

        if ((CurrentSelectionCharacter != 4) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 4;

            // リストの4番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[3];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    //キャラクター5のボタンにつける
    public void OnClick5()
    {

        if ((CurrentSelectionCharacter != 5) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 5;

            // リストの5番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[4];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    //キャラクター6のボタンにつける
    public void OnClick6()
    {

        if ((CurrentSelectionCharacter != 6) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 6;

            // リストの6番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[5];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    //キャラクター7のボタンにつける
    public void OnClick7()
    {

        if ((CurrentSelectionCharacter != 7) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 7;

            // リストの7番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[6];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    //キャラクター8のボタンにつける
    public void OnClick8()
    {

        if ((CurrentSelectionCharacter != 8) && (!_characterDecision))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 8;

            // リストの8番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[7];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    // 選択しているキャラクターを確定する
    public void ConfirmedOnClick()
    {
        _characterDecision = true;
        _confirmedSelectionCharacter = CurrentSelectionCharacter;
        print("決定されたキャラクター "+_confirmedSelectionCharacter);
    }

    /// <summary>
    /// すべてのオブジェクトを非表示にする
    /// </summary>
    private void DeleteCharacter()
    {

        foreach (GameObject obj in _characterModel)
        {
            obj.SetActive(false);
        }
    }

}
