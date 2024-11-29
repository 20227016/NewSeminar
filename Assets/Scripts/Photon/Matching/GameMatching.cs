
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.UI;

/// <summary>
/// GameMatching.cs
/// クラス説明
///　マッチングシステム
///
/// 作成日: 10/4
/// 作成者: 高橋光栄
/// </summary>
public class GameMatching : MonoBehaviour
{
    public InputField roomNameInputField;
    private NetworkRunner _networkRunner;

    private void Start()
    {
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            StartHost();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            string roomName = roomNameInputField.text;
            JoinRoom(roomName);
        }
    }

    public async void StartHost()
    {
        // ホストとして部屋を作成
        var result = await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = "MyRoom", // 部屋の名前
            Scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex,
            PlayerCount = 4 // 最大4人
        });

        if (result.Ok)
        {
            Debug.Log("ホストを作成しました！");
        }
        else
        {
            Debug.LogError($"ホスト作成に失敗しました: {result.ShutdownReason}");
        }
    }

    public async void JoinRoom(string roomName)
    {
        // 指定された部屋に参加
        var result = await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName
        });

        if (result.Ok)
        {
            Debug.Log($"{roomName}に参加しました！");
        }
        else
        {
            Debug.LogError($"参加に失敗しました: {result.ShutdownReason}");
        }
    }
}