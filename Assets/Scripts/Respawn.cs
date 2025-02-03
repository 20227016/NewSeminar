using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーをリスポーンさせるためのスクリプト
/// </summary>
public class Respawn : MonoBehaviour
{
    [SerializeField, Tooltip("リスポーンPosオブジェクト")]
    private Transform _respawnPos = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("リスポーンします");
            other.transform.position = _respawnPos.position;
            other.transform.rotation = _respawnPos.rotation;
        }
    }

}
