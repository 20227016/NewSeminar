using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class PlayerData : NetworkBehaviour
{
    [Networked]
    private int _playerNumber { get; set; } = default;

    [Networked]
    private bool _characterDecision { get; set; } = default;

    public CharacterSelectionManager _characterSelectionManager;

    public Subject<Unit> _onCharacterDecision = new();

    public NetworkPrefabRef[] _playerAvatar = default;

    private PlayerRef _playerRef = default;

    private NetworkRunner _networkRunner = default;

    private GameLauncher gameLauncher;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }

        _networkRunner = FindObjectOfType<NetworkRunner>();

        // プレイヤーリストを取得して最後尾のPlayerRefを取得
        var playerList = _networkRunner.ActivePlayers.ToList();

        if (playerList.Count > 0)
        {
            _playerRef = playerList[playerList.Count - 1];
        }
        //Debug.Log(playerList.Count);
        //Debug.Log(_playerRef + "あああああ");
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

        gameLauncher = canvas.GetComponent<GameLauncher>();

        _characterSelectionManager = Instantiate(_characterSelectionManager, canvas.transform);

        Debug.Log(_characterSelectionManager._characterDecision);
        _characterSelectionManager
           ._characterDecision
           .Subscribe(_ => SetCharacterDecision(_))
           .AddTo(this);

        _playerNumber = 0;
        _characterDecision = false;
    }

    private void SetCharacterDecision(bool isSelected)
    {
        if (!Object.HasInputAuthority)
        {   
            return;
        }
        _playerNumber = _characterSelectionManager._currentSelectionCharacter;

        //Create();

        _characterDecision = isSelected;

        //_onCharacterDecision.OnNext(Unit.Default);
        
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
        _playerNumber = number;
    }

    public int GetPlayerNumber()
    {
        Debug.Log(_playerNumber);
        return _playerNumber;
    }

    public void Create()
    {
        if (!Object.HasInputAuthority)
        {
            return;
        }

        if (_networkRunner == null)
        {
            Debug.LogError("NetworkRunner is null.");
            return;
        }

        if (_playerAvatar == null || _playerAvatar.Length <= _playerNumber - 1)
        {
            Debug.LogError("PlayerAvatar array is invalid.");
            return;
        }

        if (!_playerRef.IsValid)
        {
            Debug.LogError("Invalid PlayerRef.");
            return;
        }

        Debug.Log("Spawning avatar...");

        //gameLauncher.AvatarSpawn(_networkRunner, _playerRef, _playerNumber);
        //var avatarObject = _networkRunner.Spawn(_playerAvatar[_playerNumber - 1], transform.position, Quaternion.identity, _playerRef);
    }
}
