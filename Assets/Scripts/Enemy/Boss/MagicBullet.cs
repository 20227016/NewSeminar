using UnityEngine;

/// <summary>
/// MagicBullet.cs
/// ���@�w����]������
/// �쐬��: 11/17
/// �쐬��: �Έ䒼�l 
/// </summary>
public class MagicBullet : MonoBehaviour
{
    [Tooltip("���e�̃_���[�W")]
    [SerializeField] private float _damage = 30f;

    private Vector3 _targetScale = new Vector3(5f, 5f, 5f); // �ŏI�I�ȑ傫��
    private float _scaleSpeed = 2f; // �傫���Ȃ鑬�x
    private float _moveSpeed = 5f; // ��ԑ��x

    private float _currentTimer = 0f; // ���ݎ���
    private float _displayTime = 5f; // �\������

    private bool isMoving = false; // �ړ���ԃt���O

    private void Update()
    {
        // �܂��ڕW�T�C�Y�ɒB���Ă��Ȃ��ꍇ�A���X�ɑ傫������
        if (!isMoving)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, _targetScale, _scaleSpeed * Time.deltaTime);

            // �ڕW�T�C�Y�ɒB������ړ����J�n
            if (transform.localScale == _targetScale)
            {
                isMoving = true;
            }
        }
        else
        {
            // �����Ă�������Ɉړ�
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;

            // �\�����Ԃ��v��
            _currentTimer += Time.deltaTime;

            // ��莞�Ԍ�ɔ�\��
            if (_currentTimer >= _displayTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g�ƏՓ˂����ۂ̏����B
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    private void OnTriggerEnter(Collider other)
    {
        // �_���[�W��^���鏈���i��: �v���C���[�ȂǓ���̃��C���[�̏ꍇ�j
        if (other.CompareTag("Player")) // �v���C���[�ɑ΂��ă_���[�W��^����
        {
            // �v���C���[�̃_���[�W�������Ăяo���i���̗�j
            Debug.Log($"MagicBullet Hit {other.gameObject.name}, dealt {_damage} damage.");
        }
    }
}
