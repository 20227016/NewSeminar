using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    [SerializeField, Tooltip("選択用キャラクターモデル")]
    private List<GameObject> _animeCharacterModel = new List<GameObject>();

    [SerializeField, Tooltip("名前入力フィールド")]
    private TMP_InputField _nameInputField = default;

    [SerializeField, Tooltip("警告文")]
    private TextMeshProUGUI _warningText = default;

    [SerializeField, Tooltip("ロール名")]
    private TextMeshProUGUI _roolName = default;

    // 現在選択しているキャラクター（共有する変数）
    private int _currentSelectionCharacter = default;

    // 決定したキャラクター(共有するBool)
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
        Debug.Log("上");
        if (_currentSelectionCharacter < 1)
        {
            _currentSelectionCharacter = _animeCharacterModel.Count; // 最後のキャラクターにループ
        }
        UpdateCharacterSelection();
    }

    public void DownSelect(InputAction.CallbackContext context)
    {
        if (!context.performed || _characterDecision) return;
        Debug.Log("下");
        _currentSelectionCharacter++;
        if (_currentSelectionCharacter > _animeCharacterModel.Count)
        {
            _currentSelectionCharacter = 1; // 最初のキャラクターにループ
        }
        UpdateCharacterSelection();
    }

    public void Decision(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("決定");
        Decision();
    }

    /// <summary>
    /// キャラクター選択を更新
    /// </summary>
    private void UpdateCharacterSelection()
    {
        DeleteCharacter();

        switch (_currentSelectionCharacter)
        {
            case 1:
                if (!_tankChoice) _roolName.text = "ノーマル";
                break;
            case 2:
                if (!_knightChoice) _roolName.text = "ファイター";
                break;
            case 3:
                if (!_healerChoice) _roolName.text = "ヒーラー";
                break;
            case 4:
                if (!_fighterChoice) _roolName.text = "タンク";
                break;
            default:
                return;
        }

        _animeCharacterModel[_currentSelectionCharacter - 1].SetActive(true);
    }

    //キャラクター1のボタンにつける
    public void OnClick1()
    {
        if ((_currentSelectionCharacter != 1) && (!_characterDecision) && (!_tankChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 1;
            _roolName.text = "ノーマル";
            _animeCharacterModel[0].SetActive(true);
        }
    }

    //キャラクター2のボタンにつける
    public void OnClick2()
    {
        if ((_currentSelectionCharacter != 2) && (!_characterDecision) && (!_knightChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 2;
            _roolName.text = "ファイター";
            _animeCharacterModel[1].SetActive(true);
        }
    }

    //キャラクター3のボタンにつける
    public void OnClick3()
    {
        if ((_currentSelectionCharacter != 3) && (!_characterDecision) && (!_healerChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 3;
            _roolName.text = "ヒーラー";
            _animeCharacterModel[2].SetActive(true);
        }
    }

    //キャラクター4のボタンにつける
    public void OnClick4()
    {
        if ((_currentSelectionCharacter != 4) && (!_characterDecision) && (!_fighterChoice))
        {
            DeleteCharacter();
            _currentSelectionCharacter = 4;
            _roolName.text = "タンク";
            _animeCharacterModel[3].SetActive(true);
        }
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
    }

    // 選択しているキャラクターを確定する
    public void Decision()
    {
        // 名前が入力されていない場合はリターン
        if (string.IsNullOrWhiteSpace(_nameInputField.text))
        {
            _warningText.text = "名前を入力してください";
            _warningText.gameObject.SetActive(true);
            return;
        }

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
            // 選択されてないとき
            default:
                _warningText.text = "キャラクターを選択してください";
                _warningText.gameObject.SetActive(true);
                return;
        }
        _characterDecision = true;

        // アバター情報をセットし、他プレイヤーに通知
        _playerData.RPC_SetAvatarInfo(_currentSelectionCharacter, _nameInputField.text);
        _playerData.RPC_ActiveAvatar();

        // 表示しているキャラクターを非表示
        DeleteCharacter();

        // 選択画面を非表示
        this.gameObject.SetActive(false);


        // シーン内の全てのPlayerを取得
        PlayerData[] allPlayerData = FindObjectsOfType<PlayerData>();

        if (allPlayerData == null)
        {
            return;
        }

        // 全プレイヤーに入室を通知
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
    /// 操作プレイヤーをセット
    /// </summary>
    /// <param name="playerData">操作プレイヤーの情報</param>
    public void SetPlayer(PlayerData playerData)
    {
        _playerData = playerData;
    }

    /// <summary>
    /// すべてのオブジェクトを非表示にする
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
