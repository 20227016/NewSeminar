using UnityEngine;
using System.Collections.Generic;
using Fusion;

/// <summary>
/// MagicBullet.cs
/// ���@�w����]������
/// �쐬��: 11/17
/// �쐬��: �Έ䒼�l 
/// </summary>
public class MagicBullet : BaseEnemy
{

    private List<Collider> _playerColliders = new List<Collider>();

    private Vector3 _targetScale = new Vector3(5f, 5f, 5f); // �ŏI�I�ȑ傫��
    private float _scaleSpeed = 2f; // �傫���Ȃ鑬�x
    private float _moveSpeed = 5f; // ��ԑ��x

    private float _currentTimer = 0f; // ���ݎ���
    private float _displayTime = 5f; // �\������

    private bool isMoving = false; // �ړ���ԃt���O

    [SerializeField, Header("�T���͈�")]
    protected float _searchRange = default;

    [SerializeField, Header("�z�����݂̑���")]
    protected float _suctionSpeed = default;

    //AudioSource�^�̕ϐ���錾
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip�^�̕ϐ���錾
    [SerializeField] private AudioClip _magicBulletSE = default;

    /// <summary>
    /// ���ʉ�����
    /// </summary>
    private void Start()
    {
        _audioSource.PlayOneShot(_magicBulletSE);
    }

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
                Destroy(gameObject);
            }
        }
        SetPostion();
        PlayerSearch();
        if(_playerColliders.Count <= 0)
        {

            return;

        }
        foreach (Collider collider in _playerColliders)
        {

            Debug.Log("�_�C�\����");

            Vector3 direction = this.transform.position - collider.transform.position;

            direction.y = 0;
            Debug.Log($"�����x�N�g���F{direction}");
            collider.transform.position += direction * _suctionSpeed * Time.deltaTime;
            Debug.Log($"��{direction * _suctionSpeed * Time.deltaTime}");

        }

    }

    /// <summary>
    /// �L���X�g�̈ʒu
    /// </summary>
    protected override void SetPostion()
    {
        // �����̖ڂ̑O����
        // ���S�_
        _boxCastStruct._originPos = this.transform.position;
    }

    /// <summary>
    /// �L���X�g�̔��a
    /// </summary>
    protected override void SetSiz()
    {
        // ���a�i���a�ł͂Ȃ��j
        _boxCastStruct._size = Vector3.one * _searchRange;
    }

    /// <summary>
    /// ���C�L���X�g�̋���(�T���͈�)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = 0;
    }

    /// <summary>
    /// �v���C���[��T��
    /// </summary>
    private void PlayerSearch()
    {
        if (TargetTrans != null) return;

        int layerMask = (1 << 6) | (1 << 8); // ���C���[�}�X�N�i���C���[6��8�j

        Collider[] hits = Physics.OverlapSphere(_boxCastStruct._originPos, _searchRange, layerMask);

        // �{�b�N�X�L���X�g�̎��s
        if (hits.Length > 0)
        {

            foreach (Collider hit in hits)
            {
                if (hit.gameObject.layer == 6)
                {
                    Debug.Log($"{hit.name}");
                    _playerColliders.Add(hit);
                }
            }
        }
        else
        {
            // �q�b�g���Ȃ������ꍇ
            TargetTrans = null;
        }
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }

    public override void RPC_ReceiveDamage(int damegeValue)
    {

    }
}
