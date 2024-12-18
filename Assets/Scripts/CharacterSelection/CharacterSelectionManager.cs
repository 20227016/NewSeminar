using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System;

/// <summary>
/// キャラクター選択画面制御用スクリプト
/// 〇情報記述
/// 1 タンク 
/// 2 騎士
/// 3 ヒーラー
/// 4 ファイター
/// </summary>
public class CharacterSelectionManager : NetworkBehaviour
{

    // 現在選択しているキャラクター（共有する変数）
    public static int CurrentSelectionCharacter { get; private set; } = 0;

    // 決定したキャラクター(共有するBool)
    public static bool _characterDecision { get; private set; } = false;

    // 確定選択しているキャラクター
    private int _confirmedSelectionCharacter { get; set; } = default;

    [SerializeField,Tooltip("キャラクターモデルを格納")]
    private List<GameObject> _characterModel = new List<GameObject>();

    [Networked]
    private NetworkBool _tankChoice { get; set; } = false;
    [Networked]
    private NetworkBool _knightChoice { get; set; } = false;
    [Networked]
    private NetworkBool _healerChoice { get; set; } = false;
    [Networked]
    private NetworkBool _fighterChoice { get; set; } = false;

    //キャラクター1のボタンにつける
    public void OnClick1()
    {

        if((CurrentSelectionCharacter != 1) && (!_characterDecision) && (!_tankChoice))
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

        if ((CurrentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
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
        if ((CurrentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
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

        if ((CurrentSelectionCharacter != 4) && (!_characterDecision) && (_fighterChoice))
        {
            DeleteCharacter();
            CurrentSelectionCharacter = 4;

            // リストの4番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[3];
            secondObject.SetActive(true);
            print(CurrentSelectionCharacter);
        }
    }

    // 選択しているキャラクターを確定する
    public void ConfirmedOnClick()
    {
        _characterDecision = true;
        _confirmedSelectionCharacter = CurrentSelectionCharacter;
        switch(_confirmedSelectionCharacter)
        {
            // タンク
            case 1:
                _tankChoice = true;
                break;
            // 騎士
            case 2:
                _knightChoice = true;
                break;
            // ヒーラー
            case 3:
                _healerChoice = true;
                break;
            // ファイター
            case 4:
                _fighterChoice = true;
                break;
        }
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
