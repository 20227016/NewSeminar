using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 45f; // ��]���x�i�x/�b�j

    private void Update()
    {
        // X�������ɉ�]������
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}