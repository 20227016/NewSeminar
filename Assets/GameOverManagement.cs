﻿using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UniRx;

public class GameOverManagement : NetworkBehaviour
{

    // 現在、生き残っているプレイヤーの数
    private int _currentTruePlayer = default;

    private CharacterBase _characterBase = default;

    public override void Spawned()
    {
        print("とりあえずゲームオーバーマネージャーは爆誕したお");
        CharacterBase[] characters = FindObjectsOfType<CharacterBase>();

        if (characters.Length > 0)
        {
            _characterBase = characters[0]; // ひとまず最初のキャラを取得
            print($"取得完了！ {_characterBase.name}");

        }
        else
        {
            print("プレイヤーキャラクターが見つからない！やべえ！");
        }

        // 死亡イベントの購読
        _characterBase.DeathSubject.Subscribe(_ =>
        {
            Debug.Log("プレイヤーが死亡しました！");
            HandlePlayerDeath();
        }).AddTo(this);

        // 復活イベントの購読
        _characterBase.ResurrectionSubject.Subscribe(_ =>
        {
            Debug.Log("プレイヤーが復活しました！");
            HandlePlayerResurrection();
        }).AddTo(this);
    }

    /// <summary>
    /// プレイヤー死亡イベント
    /// </summary>
    private void HandlePlayerDeath()
    {
        _currentTruePlayer++;
        if (_currentTruePlayer >= Runner.SessionInfo.PlayerCount)
        {
            print("プレイヤーが全滅しました");
            RPC_GameOver();
        }
    }


    /// <summary>
    /// プレイヤー蘇生イベント
    /// </summary>
    private void HandlePlayerResurrection()
    {
        _currentTruePlayer--;
    }

   [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_GameOver()
    {
        print("はい、おつかれ～");
        SceneManager.LoadScene("GameOver");
    }
}
