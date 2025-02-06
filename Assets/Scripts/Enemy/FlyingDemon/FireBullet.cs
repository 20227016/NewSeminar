
using UnityEngine;
using System.Collections;

/// <summary>
/// FireBullet.cs
/// �N���X����
/// �΋�����
/// 
/// �쐬��: 1/17
/// �쐬��: �Έ䒼�l
/// </summary>
public class FireBullet : BaseEnemy
{
    [Tooltip("�΋��̑��x")]
    [SerializeField] private float _speed = 7.5f;

    [Tooltip("�΋��̐�������")]
    [SerializeField] private float _lifeTime = 5f;

    private float _elapsedTime = 0f; // �o�ߎ���

    [SerializeField] private GameObject _fireTornadoPrefab = default;

    //AudioSource�^�̕ϐ���錾
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip�^�̕ϐ���錾
    [SerializeField] private AudioClip _sE = default;

    /// <summary>
    /// ���ʉ�����
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_sE);
    }

    /// <summary>
    /// ���t���[���A�΋����ړ�������B
    /// </summary>
    private void Update()
    {
        // �O���Ɉړ�������
        transform.position += transform.forward * _speed * Time.deltaTime;

        // �����𒴂����ꍇ�A��A�N�e�B�u��
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _lifeTime)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g�ƏՓ˂����ۂ̏����B
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.layer != 6 && other.gameObject.layer != 7)
        {
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

            // �Փˌ�ɔ�A�N�e�B�u��
            Deactivate();
        }
    }

    /// <summary>
    /// �΋����A�N�e�B�u������B
    /// </summary>
    private void Deactivate()
    {
        Destroy(gameObject);
        // gameObject.SetActive(false); // �I�u�W�F�N�g���A�N�e�B�u��
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }

    public override void RPC_ReceiveDamage(int damegeValue)
    {

        Debug.Log("�I�[�o�[���C�h");
        Destroy(gameObject);

    }

}