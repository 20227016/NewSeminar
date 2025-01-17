using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerData : NetworkBehaviour
{
    [Networked]
    private int _playerNumber { get; set; } = default;

    [Networked]
    private bool _characterDecision { get; set; } = default;

    public CharacterSelectionManager _characterSelectionManager;

    public NetworkPrefabRef[] _playerAvatar = default;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

        _characterSelectionManager = Instantiate(_characterSelectionManager, canvas.transform);

        Debug.Log(_characterSelectionManager._characterDecision);
        _characterSelectionManager
           ._characterDecision
           .Subscribe(_ => SetCharacterDecision(_))
           .AddTo(this);

        _playerNumber = 0;
        _characterDecision = false;

        Debug.Log(_playerNumber + "”Ô†");
        Debug.Log(_characterDecision + "Œˆ’è");
    }


    private void SetCharacterDecision(bool isSelected)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        Debug.Log(isSelected + "‘I‘ğ");
        _characterDecision = isSelected;
    }

    public bool GetCharacterDecision()
    {
        Debug.Log(_characterDecision);
        return _characterDecision;
    }

    public void SetPlayerNumber(int number)
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }
        Debug.Log(number + "‘I‘ğ”Ô†");
        _playerNumber = number;
    }

    public int GetPlayerNumber()
    {
        Debug.Log(_playerNumber);
        return _playerNumber;
    }

    public void Create(NetworkRunner runner, int avatarNumber, PlayerRef playerRef)
    {
        var avatarObject = runner.Spawn(_playerAvatar[avatarNumber - 1], transform.position, Quaternion.identity, playerRef);
        avatarObject.transform.SetParent(transform);
    }
}
