using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーをリスポーンさせるためのスクリプト
/// </summary>
public class Respawn : MonoBehaviour
{
    [SerializeField, Tooltip("リスポーンPosオブジェクト")]
    private GameObject _respawnPos = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("リスポーンしました");
            _respawnPos.transform.position = other.gameObject.transform.position;
        }
    }

}
