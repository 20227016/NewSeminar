using System.Collections;
using UnityEngine;

public class EFManager : MonoBehaviour
{
    [SerializeField, Tooltip("�v���C���[")]
    private GameObject _player = default;

    [SerializeField, Tooltip("�G�t�F�N�g")]
    private GameObject _ef = default;

    // �v���C���[�̏�����Ԃ�ۑ�
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _initialScale;

    private void Start()
    {
        // ������Ԃ�ۑ�
        _initialPosition = _player.transform.position;
        _initialRotation = _player.transform.rotation;
        _initialScale = _player.transform.localScale;
    }

    private void OnDisable()
    {
        ResetPlayerState();
        ResetEffect();
    }

    /// <summary>
    /// �v���C���[�̈ʒu�A��]�A�傫����������Ԃɖ߂�
    /// </summary>
    private void ResetPlayerState()
    {
        _player.transform.position = _initialPosition;
        _player.transform.rotation = _initialRotation;
        _player.transform.localScale = _initialScale;
    }

    /// <summary>
    /// �G�t�F�N�g���A�N�e�B�u�����ăX�P�[�������Z�b�g����
    /// </summary>
    private void ResetEffect()
    {
        if (_ef != null)
        {
            _ef.SetActive(false);
            _ef.transform.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("�G�t�F�N�g���ݒ肳��Ă��܂���I");
        }
    }
}
