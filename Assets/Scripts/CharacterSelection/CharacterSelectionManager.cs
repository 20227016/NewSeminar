using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

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
    private ReactiveProperty<bool> _characterDecision = new();

    [SerializeField, Tooltip("キャラクターモデルを格納")]
    private List<GameObject> _characterModel = new List<GameObject>();

    private PlayerData _player = default;

    public bool _tankChoice { get; set; } = false;
    public bool _knightChoice { get; set; } = false;
    public bool _healerChoice { get; set; } = false;
    public bool _fighterChoice { get; set; } = false;

    public void SetPlayer(PlayerData playerData)
    {
        _player = playerData;
    }

    public void Start()
    {
        _currentSelectionCharacter = 0;
        _characterDecision.Value = false;

    }

    //キャラクター1のボタンにつける
    public void OnClick1()
    {

        if ((_currentSelectionCharacter != 1) && (!_characterDecision.Value) && (!_tankChoice))
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

        if ((_currentSelectionCharacter != 2) && (!_characterDecision.Value) && (!_knightChoice))
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
        if ((_currentSelectionCharacter != 3) && (!_characterDecision.Value) && (!_healerChoice))
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

        if ((_currentSelectionCharacter != 4) && (!_characterDecision.Value) && (!_fighterChoice))
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
        _characterDecision.Value = true;
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

        // シーン内の全てのPlayerDataコンポーネントを取得
        var allPlayerData = FindObjectsOfType<PlayerData>();

        if (allPlayerData == null)
        {
            return;
        }

        foreach (var playerData in allPlayerData)
        {
            Debug.Log("入った");
            playerData.RPC_ActiveAvatar();
        }

        this.gameObject.SetActive(false);
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
