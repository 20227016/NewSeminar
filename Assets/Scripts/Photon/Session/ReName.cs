using Fusion;
using UnityEngine;

/// <summary>
/// �Q���҂̖��O��ύX����
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
        Debug.Log("���[���͂���");
        int index = _roomInfo.ReName(newName, participant.name);
        INameMemory iNameMemory = participant.GetComponent<INameMemory>();
        if (iNameMemory == null)
        {

            Debug.LogError($"���O��ݒ肷�邽�߂̃C���^�[�t�F�[�X��������܂���");

        }
        iNameMemory.Name = newName;
        iNameMemory.RPC_NameUpdate();
        // �e�L�X�g�ݒ�
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().Character = $"{newName}";
        // �e�L�X�g�X�V
        GameObject.Find($"Participant_{index}").GetComponent<TextMemory>().RPC_TextUpdate();

    }
}
