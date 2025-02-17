using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.Playables;
using System.Collections;

public class GameOverManagement : NetworkBehaviour
{

    // 現在、生き残っているプレイヤーの数
    private int _currentTruePlayer = default;

    private CharacterBase _characterBase = default;

    private PlayableDirector _movieGameOver = default;

    [SerializeField]
    private Material _gameOverMaterial = default;

    private bool _gameOverStart = false;

    public override void Spawned()
    {
        print("とりあえずゲームオーバーマネージャーは爆誕したお");
        CharacterBase[] characters = FindObjectsOfType<CharacterBase>();


        _movieGameOver = FindObjectOfType<PlayableDirector>();

        ChangeAlpha(_gameOverMaterial, 0f);

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
        _movieGameOver.Play();
        if(!_gameOverStart)
        {
            StartCoroutine(FadeIn(_gameOverMaterial, 5.0f));
            _gameOverStart = true;
        }
    }

    /// <summary>
    /// マテリアル関数
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="alpha"></param>
    private void ChangeAlpha(Material mat, float alpha)
    {
        if (mat.HasProperty("_Color"))
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
        else
        {
            Debug.LogError("このマテリアルには _Color プロパティがありません！");
        }
    }

    /// <summary>
    /// 徐々に暗くする
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator FadeIn(Material mat, float duration)
    {
        float startAlpha = mat.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration); // 0 → 1 に増やす
            ChangeAlpha(mat, alpha);
            yield return null;
        }

        ChangeAlpha(mat, 1f); // 完全に黒くする
    }
}
