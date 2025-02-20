using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーをリスポーンさせるためのスクリプト
/// </summary>
public class Respawn : MonoBehaviour
{
    [SerializeField, Tooltip("リスポーンPosオブジェクト")]
    private Transform _playerRespawnPos = default;

    [SerializeField, Tooltip("リスポーンPosオブジェクト")]
    private Transform _enemyRespawnPos = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("リスポーンします");
            other.transform.position = _playerRespawnPos.position;
            other.transform.rotation = _playerRespawnPos.rotation;
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("リスポーンします");
            other.transform.position = _enemyRespawnPos.position;
            other.transform.rotation = _enemyRespawnPos.rotation;
        }

    }

}
