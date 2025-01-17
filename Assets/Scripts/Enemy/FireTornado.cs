
using UnityEngine;
using System.Collections;

/// <summary>
/// FireTornado.cs
/// �N���X����
/// ���̗�������
/// 
/// �쐬��: 1/17
/// �쐬��: �Έ䒼�l
/// </summary>

public class FireTornado : MonoBehaviour
{
    [Tooltip("���@�e�̐�������")]
    [SerializeField] private float lifeTime = 5f;

    [Tooltip("���@�e�̃_���[�W")]
    [SerializeField] private float damage = 10f;

    private float _elapsedTime = 0f; // �o�ߎ���
    private bool _isActive = true;  // �A�N�e�B�u��Ԃ��Ǘ�

    /// <summary>
    /// ���̗����̐�������
    /// </summary>
    private void Update()
    {
        if (!_isActive) return;

        // �����𒴂����ꍇ�A��A�N�e�B�u��
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= lifeTime)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// ���̗������A�N�e�B�u������B
    /// </summary>
    private void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false); // �I�u�W�F�N�g���A�N�e�B�u��
        _elapsedTime = 0f;           // �o�ߎ��Ԃ����Z�b�g
    }
}
