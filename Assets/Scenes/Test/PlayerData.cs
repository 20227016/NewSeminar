using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private int _playerNumber = default;

    private void Start()
    {
        var count = FindObjectsOfType<PlayerData>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
            return;
        };

        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerNumber(int number)
    {
        _playerNumber = number;
    }

    public int GetPlayerNumber()
    {
        return _playerNumber;
    }
}
