
using UnityEngine;
using System.Collections;

/// <summary>
/// FireMagic.cs
/// �N���X����
/// ���@�e����
/// 
/// �쐬��: 12/11
/// �쐬��: �Έ䒼�l
/// </summary>
public class FireBullet : MonoBehaviour
{
    [Tooltip("���@�e�̑��x")]
    [SerializeField] private float speed = 10f;

    [Tooltip("���@�e�̐�������")]
    [SerializeField] private float lifeTime = 5f;

    [Tooltip("���@�e�̃_���[�W")]
    [SerializeField] private float damage = 10f;

    private float _elapsedTime = 0f; // �o�ߎ���
    private bool _isActive = true;  // �A�N�e�B�u��Ԃ��Ǘ�

    [SerializeField] private GameObject _fireTornadoPrefab = default;

    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// �X�V�O����
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// ���t���[���A���@�e���ړ�������B
    /// </summary>
    private void Update()
    {
        if (!_isActive) return;

        // �O���Ɉړ�������
        transform.position += transform.forward * speed * Time.deltaTime;

        // �����𒴂����ꍇ�A��A�N�e�B�u��
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= lifeTime)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g�ƏՓ˂����ۂ̏����B
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;

        // �X�e�[�W�ɓ���������
        else if (other.gameObject.layer == 8)
        {
            // �_���[�W��^���鏈���i��: �v���C���[�ȂǓ���̃��C���[�̏ꍇ�j
            if (other.CompareTag("Player")) // �v���C���[�ɑ΂��ă_���[�W��^����
            {
                // �v���C���[�̃_���[�W�������Ăяo���i���̗�j
                Debug.Log($"Hit {other.gameObject.name}, dealt {damage} damage.");
            }

            // �Փ˓_���擾
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                // ���̗����𐶐�
                Instantiate(
                    _fireTornadoPrefab,
                    hit.point,              // �Փ˂����\�ʂ̈ʒu
                    Quaternion.Euler(0, 0, 0) // ��]�� (0, 0, 0) �ɌŒ�
                );
            }
        }
        else
        {
            return;
        }

        // �Փˌ�ɔ�A�N�e�B�u��
        Deactivate();
    }

    /// <summary>
    /// ���@�e���A�N�e�B�u������B
    /// </summary>
    private void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false); // �I�u�W�F�N�g���A�N�e�B�u��
        _elapsedTime = 0f;           // �o�ߎ��Ԃ����Z�b�g
    }

    /// <summary>
    /// ���@�e���ė��p����ۂ̏����������B
    /// </summary>
    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        _elapsedTime = 0f;
        _isActive = true;
        gameObject.SetActive(true); // �I�u�W�F�N�g���A�N�e�B�u��
    }
}