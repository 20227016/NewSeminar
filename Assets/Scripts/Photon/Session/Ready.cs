using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class Ready : BaseRoom, IReady
{

    /// <summary>
    /// 読み込みを完了した人数
    /// </summary>
    [Networked]
    public int LoadCompletCount { get; set; } = 0;

    /// <summary>
    /// ロードを終えたかのフラグ
    /// </summary>
    [Networked]
    public bool _isLoadComplete { get; set; } = false;

    /// <summary>
    /// Readyオブジェクトのテキストメモリー
    /// </summary>
    private TextMemory _textMemory = default;

    /// <summary>
    /// ルームの参加者人数が増えるまで待つ時間
    /// </summary>
    private const int _awaitTime = 1000;

    /// <summary>
    /// ロード状態が入る
    /// </summary>
    private AsyncOperation _asyncLoad = default;



    public override async void Spawned()
    {

        base.Spawned();
        await Task.Delay(_awaitTime);
        _textMemory = GameObject.Find("Ready").transform.Find("Text").GetComponent<TextMemory>();
        ChangeText();

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {


        if (_networkRunner.IsServer)
        {

            ChangeText();

        }

    }

    /// <summary>
    /// 呼び出したサーバーがホストかクライアントかを判断
    /// </summary>
    /// <param name="participant"></param>
    public async void ParticipantReady(NetworkObject participant)
    {

        Debug.Log($"{_roomInfo}準備");
        await GetRoomAwait();
        bool isHost = false;
        if (_networkRunner.IsServer)
        {

            Debug.Log("ホスト");
            isHost = true;

        }
        else
        {

            Debug.Log("クライアント");
            isHost = false;

        }
        RPC_ParticipantReady(isHost, participant);

    }

    /// <summary>
    /// ホストが準備完了・取り消しを担当
    /// </summary>
    /// <param name="isHost"></param>
    /// <param name="participant"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ParticipantReady(bool isHost, NetworkObject participant)
    {

        if (isHost)
        {

            Debug.Log("ホスト");
            _roomInfo.ChangeReady(participant.name);
            ChangeText();
            //全員が準備を完了していないとき　
            if (!_roomInfo.GoCheck())
            {

                _roomInfo.ChangeReady(participant.name);
                ChangeText();
                return;
            }
            // シーン移動同期
            RPC_LoadScene();

        }
        else
        {

            Debug.Log("クライアント");
            _roomInfo.ChangeReady(participant.name);
            ChangeText();

        }

    }

    /// <summary>
    /// 次のシーン読み込み
    /// </summary>
    /// <returns></returns>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_LoadScene()
    {

        // 非同期でシーンをロード
        _asyncLoad = SceneManager2.LoadSceneAsync("CharacterSelection");
        // 自動でシーンがアクティブになるのを防ぐ
        _asyncLoad.allowSceneActivation = false;
        LoadAwait();

    }

    /// <summary>
    /// シーンが読み込まれるまで待つ
    /// </summary>
    /// <param name="asyncLoad"></param>
    private async void LoadAwait()
    {

        // 完了を待つ
        while (true)
        {

            await Task.Delay(1);
            Debug.Log($"ロード進捗：{_asyncLoad.progress}");
            // 読み込み進捗が90%になったときに読み込めたと変数に通知
            if (_asyncLoad.progress >= 0.9f)
            {

                // カウント通知
                RPC_LoadCompletNotice();
                // ラグ考慮のループ
                while(!_isLoadComplete)
                {

                    await Task.Delay(1000);

                }
                RPC_LoadCheck();
                break;

            }

        }

    }

    /// <summary>
    /// ロードが完了したとき変数にカウント通知
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_LoadCompletNotice()
    {

        LoadCompletCount++;
        _isLoadComplete = true;
        Debug.LogWarning($"{LoadCompletCount}ロード完了報告");

    }

    /// <summary>
    /// 全員がロードを終えたか確認する
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LoadCheck()
    {

        if (_networkRunner.IsServer)
        {

            _isLoadComplete = false;

        }
        Debug.LogWarning($"全員がロード完了したかの確認 現在:ロード完了{LoadCompletCount}　参加人数{_roomInfo.CurrentParticipantCount} ");
        if (LoadCompletCount >= _roomInfo.CurrentParticipantCount)
        {
            Debug.LogWarning("ロード完了");
            // シーンをアクティブにする
            _asyncLoad.allowSceneActivation = true;

        }

    }

    /// <summary>
    /// 準備完了人数を知らせるテキストを変える
    /// </summary>
    private void ChangeText()
    {

        if (!_networkRunner.IsRunning)
        {

            return;

        }
        // 準備完了人数
        int num = _roomInfo.ReadyGoCount();
        _textMemory.Character = $"準備完了({num}/{_roomInfo.CurrentParticipantCount})";
        _textMemory.RPC_TextUpdate();

    }
}
