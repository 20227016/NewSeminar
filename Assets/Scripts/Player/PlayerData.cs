using Fusion;
using UnityEngine;
using TMPro;

public class PlayerData : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] _playerAvatar = default;

    [SerializeField]
    private TextMeshProUGUI[] _playerNameTexts = default;

    [Networked]
    private int _avatarNumber { get; set; } = default;

    [Networked, HideInInspector]
    public string _avatarName { get; set; } = default;


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            CharacterSelectionManager characterSelectionManager = canvas.GetComponentInChildren<CharacterSelectionManager>();

            // アクティブ
            characterSelectionManager.ActiveUI();

            // プレイヤーセット
            characterSelectionManager.SetPlayer(this);
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ActiveAvatar()
    {
        if(_avatarNumber <= 0)
        {
            return;
        }

        _playerAvatar[_avatarNumber - 1].SetActive(true);

        _playerNameTexts[_avatarNumber - 1].text = _avatarName;

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetAvatarInfo(int avatarNumber, string avatarName)
    {

        _avatarNumber = avatarNumber;


        _avatarName = avatarName;
    }

}
