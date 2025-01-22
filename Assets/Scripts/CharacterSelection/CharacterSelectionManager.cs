using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター選択画面制御用スクリプト
/// 〇情報記述
/// 1 タンク 
/// 2 騎士
/// 3 ヒーラー
/// 4 ファイター
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{

    // 現在選択しているキャラクター（共有する変数）
    private int _currentSelectionCharacter = default;

    // 決定したキャラクター(共有するBool)
    private bool _characterDecision = new();

    [SerializeField, Tooltip("キャラクターモデルを格納")]
    private List<GameObject> _characterModel = new List<GameObject>();

    private PlayerData _player = default;

    public bool _tankChoice { get; set; } = false;
    public bool _knightChoice { get; set; } = false;
    public bool _healerChoice { get; set; } = false;
    public bool _fighterChoice { get; set; } = false;

    //キャラクター1のボタンにつける
    public void OnClick1()
    {

        if ((_currentSelectionCharacter != 1) && (!_characterDecision) && (!_tankChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 1;

            // リストの1番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[0];
            secondObject.SetActive(true);
            print("タンク");
        }

    }

    //キャラクター2のボタンにつける
    public void OnClick2()
    {

        if ((_currentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 2;

            // リストの2番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[1];
            secondObject.SetActive(true);
            print("騎士");
        }
    }

    //キャラクター3のボタンにつける
    public void OnClick3()
    {
        if ((_currentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
        {

            DeleteCharacter();
            _currentSelectionCharacter = 3;

            // リストの3番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[2];
            secondObject.SetActive(true);
            print("ヒーラー");
        }
    }

    //キャラクター4のボタンにつける
    public void OnClick4()
    {

        if ((_currentSelectionCharacter != 4) && (!_characterDecision) && (!_fighterChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 4;

            // リストの4番目のオブジェクトを取得して制御
            GameObject secondObject = _characterModel[3];
            secondObject.SetActive(true);
            print("ファイター");
        }
    }

    // 選択しているキャラクターを確定する
    public void Decision()
    {
        _characterDecision = true;

        switch (_currentSelectionCharacter)
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

        _player.RPC_SetAvatarNumber(_currentSelectionCharacter);
        _player.RPC_ActiveAvatar();
        // シーン内の全てのPlayerを取得
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
