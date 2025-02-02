using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 45f; // 回転速度（度/秒）

    private void Update()
    {
        // X軸方向に回転させる
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}