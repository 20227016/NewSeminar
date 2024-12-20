using Fusion;
using UnityEngine;

/// <summary>
/// 参加者の名前を変更する
/// </summary>
public class ReName : BaseRoom,IReName
{

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public async void RPC_ParticipantReName(string newName, NetworkObject participant)
    {

        if (!_networkRunner.IsServer || newName == null)
        {

            return;

        }
        await GetRoomAwait();
        Debug.Log("ルームはある");
        int index = _roomInfo.ReName(newName, participant.name);
        INameMemory iNameMemory = participant.GetComponent<INameMemory>();
        if (iNameMemory == null)
        {

            Debug.LogError($"名前を設定するためのインターフェースが見つかりません");

        }
        iNameMemory.Name = newName;
        iNameMemory.RPC_NameUpdate();
        // テキスト設定
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Character = $"{newName}";
        // テキスト更新
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();

    }
}
