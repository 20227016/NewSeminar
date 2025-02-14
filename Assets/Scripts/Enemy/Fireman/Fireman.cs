
using Fusion;
using System.Collections;
using UnityEngine;

/// <summary>
/// Golem.cs
/// �t�@�C�A�[�}���̍s�����W�b�N���Ǘ�����N���X�B
/// �ړ��A�T���A�����]���A�U���ȂǁA���܂��܂ȏ�Ԃɉ����������𐧌䂷��B
/// �쐬��: 2/13
/// �쐬��: �Έ䒼�l 
/// </summary>
public class Fireman : BaseEnemy
{
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField, Tooltip("�T���͈�")]
    protected float _searchRange = default;

    [SerializeField, Tooltip("�ړ����x(����)")]
    private float _workMoveSpeed = default; // �t�@�C�A�[�}���̈ړ����x

    [SerializeField, Tooltip("�ړ����x(����)")]
    private float _runMoveSpeed = default; // �t�@�C�A�[�}���̈ړ����x

    [SerializeField, Tooltip("��~���鋗��")]
    private float _stopDistance = default; // �v���C���[��O�Œ�~���鋗��

    private Vector3 _playerLastKnownPosition; // �v���C���[�̍Ō�̈ʒu

    [SerializeField] private float _detectionDistance = default; // �ǂ����o���鋗��

    [Networked] private Vector3 _randomTargetPos { get; set; } // �����_���ړ��̖ڕW�ʒu

    [Tooltip("������Prefab")]
    [SerializeField] private GameObject _fireShieldPrefab;

    private bool isAttack = default;

    /// <summary>
    /// �t�@�C�A�[�}���̎��͂����n����Ԃ�\���񋓌^�B
    /// </summary>
    private enum EnemyLookAroundState
    {
        NONE, // �������Ă��Ȃ���ԁB
        TURNING, // �����]�����Ă����ԁB
        LOOKING_AROUND // ���͂����n���Ă����ԁB
    }

    // ���n����Ԃ�ێ�����t�B�[���h
    [SerializeField]
    private EnemyLookAroundState _lookAroundState = EnemyLookAroundState.NONE;

    private float _currentAngle = default; // ���݂̉�]�p�x
    private float _lookAroundTimer = 0f; // ���͂����n�����߂̃^�C�}�[
    private float _turnSpeed = 60f; // ��]���x (�x/�b)

    // �A�j���[�^�[�ϐ�
    // TransitionNo.0 Idle
    // TransitionNo.1 Running
    // TransitionNo.2 Attack01
    // TransitionNo.3 Attack02
    // TransitionNo.4 Downed
    // TransitionNo.5 Stunned
    // TransitionNo.6 Die
    private Animator _animator;

    // �q�I�u�W�F�N�g��ParticleSystem���擾
    private ParticleSystem[] _attackEffects = default;

    //AudioSource�^�̕ϐ���錾
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip�^�̕ϐ���錾
    [SerializeField] private AudioClip _chargeSE = default;

    public override void Spawned()
    {
        _searchRange = 20f;

        // Raycast���������߂̊�{�ݒ�����Ă����֐�
        BasicRaycast();

        // HPUI�̏�����
        RPC_UpdateHPBar();

        _animator = GetComponent<Animator>();

        // �q�̃I�u�W�F�N�g��
        Transform effectObj = FindChild(transform, "FireImpactParent");

        _attackEffects = effectObj.GetComponentsInChildren<ParticleSystem>();

        _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�
    }

    /// <summary>
    /// �ċA�I�Ɏq�I�u�W�F�N�g��T��
    /// </summary>
    private Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child; // ���������炻�̃I�u�W�F�N�g��Ԃ�
            }

            // �ċA�I�ɂ���ɐ[���q�K�w��T��
            Transform found = FindChild(child, childName);
            if (found != null)
            {
                return found;
            }
        }

        return null; // ������Ȃ������ꍇ�� null
    }

    /// <summary>
    /// �t���[�����Ƃ̍X�V�����B
    /// ��Ԃ��Ƃɏ����𕪂��Ď��s�B
    /// </summary>
    protected void Update()
    {
        /* 
         * ���C�L���X�g�̒��S�_�������ōX�V���Ă���s��
         * ����ŁA���C�L���X�g����Ɏ����̑O����ł��Ă����
         * �܂�A�����Ȃ��ꍇ�i�����ʒu��ς��������j�����Ȃ���OK
         * �ς����������畷���Ă��ꂽ�炻�̓s�x��������
         * ����ȊO�ɂ��T�C�Y�A��������ς��郁�\�b�h������
        */
        SetPostion(); // ���C�L���X�g�̒��S�ʒu��ݒ�
        SetDirection(); // ���C�L���X�g�̕������X�V

        switch (_movementState)
        {
            // �ҋ@
            case EnemyMovementState.IDLE:

                if (Runner.IsServer)
                {
                    RPC_EnemyIdle();
                }
                else
                {
                    return;
                }

                break;

            // �ړ�
            case EnemyMovementState.WALKING:

                if (Runner.IsServer)
                {
                    RPC_EnemyWalking();
                }
                else
                {
                    return;
                }

                break;

            // �ǐ�
            case EnemyMovementState.RUNNING:

                if (Runner.IsServer)
                {
                    RPC_EnemyRunning();
                }
                else
                {
                    return;
                }

                break;

            // ���S
            case EnemyMovementState.DIE:

                EnemyDie();

                return;
        }

        switch (_actionState)
        {
            // �T�[�`
            case EnemyActionState.SEARCHING:

                if (Runner.IsServer)
                {
                    RPC_PlayerSearch();
                }
                else
                {
                    return;
                }

                break;

            // �U��
            case EnemyActionState.ATTACKING:

                if (Runner.IsServer)
                {
                    RPC_EnemyAttacking();
                }
                else
                {
                    return;
                }

                break;
        }
    }

    /// <summary>
    /// �����ɃA�C�h����Ԃ̂Ƃ��̏���������
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyIdle()
    {
        // �A�j���[�V�������I�������玟�̏�ԂɑJ��
        if (IsAnimationFinished("Attack"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _movementState = EnemyMovementState.DIE;
            Instantiate(_fireShieldPrefab, transform.position, Quaternion.identity);
        }

        if (_actionState != EnemyActionState.SEARCHING)
        {
            return;
        }

        if (_lookAroundState != EnemyLookAroundState.NONE)
        {
            switch (_lookAroundState)
            {
                case EnemyLookAroundState.TURNING:
                    SmoothTurn();
                    break;

                case EnemyLookAroundState.LOOKING_AROUND:
                    LookAround();
                    break;
            }
        }
        else
        {
            // ���n���I����A�ړ���ԂɑJ��
            _movementState = EnemyMovementState.WALKING;
            _lookAroundState = EnemyLookAroundState.TURNING; // ����̕����]��������
        }
    }

    /// <summary>
    /// �t�@�C�A�[�}���̑O���ɍL���͈͂ŏ�Q�������邩�𔻒肷��
    /// </summary>
    /// <param name="direction">�ړ�����</param>
    /// <param name="distance">���o�͈͂̋���</param>
    /// <returns>��Q��������� true</returns>
    private bool IsPathBlocked(Vector3 direction, float distance)
    {
        // BoxCast�̐ݒ�
        Vector3 boxSize = new Vector3(3.0f, 1.0f, 0.5f); // ���A�����A���s��
        Vector3 origin = transform.position + Vector3.up * 0.5f; // �t�@�C�A�[�}���̏����ォ�甭��
        Quaternion rotation = Quaternion.LookRotation(direction); // �t�@�C�A�[�}���̌����ɍ��킹��

        // BoxCast�ŏ�Q�������o
        RaycastHit hit;
        if (Physics.BoxCast(origin, boxSize / 2, direction, out hit, rotation, distance))
        {
            // �ǂ̃��C���[�����o
            if (hit.collider.gameObject.layer == 8)
            {
                return true; // �ǂɏՓ�
            }
        }

        return false; // ��Q���Ȃ�
    }

    /// <summary>
    /// ���s��ԁF�ړ���A���n����Ԃֈڍs
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyWalking()
    {
        // ���݂̈ʒu
        Vector3 currentPosition = transform.position;

        // �^�[�Q�b�g�ʒu (_randomTargetPos��y���W�����݂�y���W�ɌŒ�)
        Vector3 targetPosition = new Vector3(_randomTargetPos.x, currentPosition.y, _randomTargetPos.z);

        // �ړ�
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _workMoveSpeed * Time.deltaTime);

        // �����x�N�g�����v�Z���AY����]�݂̂�K�p
        Vector3 direction = _randomTargetPos - transform.position;
        direction.y = 0f; // X��]�𖳎����邽�߂�Y��������0�ɂ���
        direction.Normalize(); // ���K��

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // �ǂ����m
        if (IsPathBlocked(direction, _detectionDistance))
        {
            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�

            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.TURNING;
            return;
        }

        // �ڕW�n�_���B
        if (Vector2.Distance(
        new Vector2(transform.position.x, transform.position.z),
        new Vector2(_randomTargetPos.x, _randomTargetPos.z)) < 0.1f)
        {
            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�

            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.LOOKING_AROUND;
            //_lookAroundTimer = 3.0f; // ���n�����Ԃ��Z�b�g
            _currentAngle = transform.rotation.eulerAngles.y; // ���݂̉�]�p�x���擾�iY����]�j
        }
    }

    /// <summary>
    /// �����_���Ȉʒu�𐶐�����
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        float range = 10.0f; // �����_���ړ��͈�
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            0f,
            Random.Range(-range, range)
        );
        return transform.position + randomOffset; // ���݈ʒu����Ƀ����_���Ȉʒu�𐶐�
    }

    /// <summary>
    /// ���n������
    /// </summary>
    private void LookAround()
    {
        // ���n�����I�������ꍇ�A�����]�����J�n
        if (_lookAroundTimer <= 0)
        {
            _lookAroundState = EnemyLookAroundState.TURNING; // �����]����Ԃ֑J��
            return;
        }

        // ���n���̓������쐬
        float angle = Mathf.PingPong(Time.time * 60f, 90f) - 45f;

        // ���݂̊p�x�Ɍ��n���̊p�x�����Z
        transform.rotation = Quaternion.Euler(0f, _currentAngle + angle, 0f);

        _lookAroundTimer -= Time.deltaTime;
    }

    /// <summary>
    /// ���炩�ɕ����]������
    /// </summary>
    private void SmoothTurn()
    {
        Vector3 direction = (_randomTargetPos - transform.position).normalized;

        // ��]�ڕW���v�Z
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // ���݂̉�]�ƖڕW��]�̊p�x�����擾
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        // ���炩�ɉ�]
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

        // ��]���ڕW�p�x�ɂقڈ�v�����ꍇ�A���̏�Ԃ�
        if (angleDifference < 1f) // �p�x����1�x�����̏ꍇ
        {
            _lookAroundState = EnemyLookAroundState.NONE;
        }
    }

    /// <summary>
    /// �v���C���[�̍Ō�̏ꏊ�܂ňړ�����
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyRunning()
    {
        if (_actionState == EnemyActionState.SEARCHING)
        {
            _animator.SetInteger("TransitionNo", 1);
        }

        // ���݂̍������ێ�����
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _playerLastKnownPosition;

        targetPosition.y = currentPosition.y; // �t�@�C�A�[�}���̍������Œ�

        // �v���C���[�̍Ō�̈ʒu�܂ł̋������v�Z
        float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

        // �t�@�C�A�[�}���̕������v���C���[�Ɍ����� (Y����]�̂�)
        Vector3 direction = (targetPosition - currentPosition).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Y���̂݉�]
        }

        // ��~�����ȓ��Ȃ�ړ����~
        if (distanceToTarget <= _stopDistance)
        {
            _movementState = EnemyMovementState.IDLE;  // �ҋ@��Ԃɖ߂�
            _actionState = EnemyActionState.ATTACKING;
            return;
        }

        // �v���C���[�̍Ō�̈ʒu�Ɍ������Ĉړ�
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            _runMoveSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// �U���������������B
    /// �U���I����A�ҋ@��Ԃɖ߂�B
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyAttacking()
    {
        if (isAttack) return;

        isAttack = true;

        _animator.SetInteger("TransitionNo", 2);

        foreach (var effect in _attackEffects)
        {
            effect.Play();
        }

        _audioSource.PlayOneShot(_chargeSE);
    }

    /// <summary>
    /// �|���
    /// </summary>
    private void EnemyDie()
    {
        EnemyDespawn();
    }

    /// <summary>
    /// �A�j���[�V�������I�����Ă��邩���m�F����B
    /// </summary>
    /// <param name="animationName">�m�F����A�j���[�V�����̖��O</param>
    /// <returns>�A�j���[�V�������I�����Ă��邩</returns>
    private bool IsAnimationFinished(string animationName)
    {
        // ���݂̃A�j���[�V������Ԃ��擾
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // �A�j���[�V�������w�肵�����O���I�����Ă��邩���m�F
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f;
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
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerSearch()
    {
        if (TargetTrans != null) return;

        int layerMask = (1 << 6) | (1 << 8); // ���C���[�}�X�N�i���C���[6��8�j

        Collider[] hits = Physics.OverlapSphere(_boxCastStruct._originPos,_searchRange, layerMask);

        // �{�b�N�X�L���X�g�̎��s
        if (hits.Length > 0)
        {
            Collider playerCollider = default;
            foreach(Collider hit in hits)
            {
                if (hit.gameObject.layer == 6)
                {
                    playerCollider = hit;
                }
            }
            // �v���C���[�i���C���[6�j�̏ꍇ�̏���
            if (playerCollider != null)
            {
                TargetTrans = playerCollider.gameObject.transform;
                _playerLastKnownPosition = TargetTrans.position; // �v���C���[�̈ʒu���L�^
                _movementState = EnemyMovementState.RUNNING;
            }
            else
            {
                TargetTrans = null; // �v���C���[�ȊO�Ȃ�^�[�Q�b�g������
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
        _movementState = EnemyMovementState.DIE;
    }
}