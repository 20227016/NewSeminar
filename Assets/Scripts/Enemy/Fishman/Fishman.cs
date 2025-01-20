
using UnityEngine;
using System.Collections;

/// <summary>
/// Fishman.cs
/// �t�B�b�V���}���̍s�����W�b�N���Ǘ�����N���X�B
/// �ړ��A�T���A�����]���A�U���ȂǁA���܂��܂ȏ�Ԃɉ����������𐧌䂷��B
/// �쐬��: 
/// �쐬��: 
/// </summary>
public class Fishman : BaseEnemy
{
    // ������Enem���쐬�B
    // �Ȃ�Enem��2���邩�Ƃ����ƁA����Ȃ���U�����邽�߁i���ɂ����邯��...�j
    // ����ƁA�ړ����Ȃ��牽�������邱�Ƃ��ł��Ȃ�����A�����Ă܂��B�������肷�����߂�
    // ��Ԃ�ǉ��������ꍇ�A�p�u���b�N��Enem��ݒ肵�Ă���̂ŁAEnem�p�u���b�N�N���X(���������X�N���v�g������)�ɒǉ�����Ύg���܂��B����
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField, Header("�ǂ����������I�u�W�F�N�g�̃g�����X�t�H�[��")]
    private Transform _targetTrans = default;

    [SerializeField, Tooltip("�T���͈�(�O������)")]
    protected float _searchRange = 20f;

    [SerializeField, Tooltip("�ړ����x")]
    private float moveSpeed = default; // �S�[�����̈ړ����x

    [SerializeField, Tooltip("��~���鋗��")]
    private float stopDistance = 2.0f; // �v���C���[��O�Œ�~���鋗��

    private Vector3 _playerLastKnownPosition; // �v���C���[�̍Ō�̈ʒu

    [SerializeField]
    private Transform _attackingPlayer; // �U�����Ă����v���C���[��Transform

    private float detectionDistance = 3.0f; // �ǂ����o���鋗��

    private Vector3 _randomTargetPos; // �����_���ړ��̖ڕW�ʒu

    private bool isAttackInterval = default; // �A���U�������Ȃ�

    private float _downedTimer = 5f; // �_�E���^�C�}�[
    private float _stunnedTimer = default; // �̂�����^�C�}�[

    /// <summary>
    /// �S�[�����̎��͂����n����Ԃ�\���񋓌^�B
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

    private GameObject _harpoon = default;
    private BoxCollider _boxCollider = default;

    // �A�j���[�^�[�ϐ�
    // TransitionNo.0 Idle
    // TransitionNo.1 Walk
    // TransitionNo.2 Running
    // TransitionNo.3 Attack01
    // TransitionNo.4 Attack02
    // TransitionNo.5 Downed
    // TransitionNo.6 Stunned
    // TransitionNo.7 Die
    private Animator _animator;

    private void Awake()
    {
        _searchRange = 20f;

        // Raycast���������߂̊�{�ݒ�����Ă����֐�
        BasicRaycast();

        _harpoon = GameObject.Find("Harpoon");
        _boxCollider = _harpoon.GetComponent<BoxCollider>();
        _boxCollider.enabled = false;

        _animator = GetComponent<Animator>();

        _randomTargetPos = GenerateRandomPosition();
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

        // �̂�����܂ł̎���
        _stunnedTimer -= Time.deltaTime;

        // �_�E��
        if (Input.GetKeyDown(KeyCode.B))
        {
            _movementState = EnemyMovementState.DOWNED;
        }
        // �̂�����
        else if (Input.GetKeyDown(KeyCode.S)
            && _stunnedTimer <= 0)
        {
            _movementState = EnemyMovementState.STUNNED;
        }
        // �|���
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _movementState = EnemyMovementState.DIE;
        }

        switch (_movementState)
        {
            // �ҋ@
            case EnemyMovementState.IDLE:

                EnemyIdle();

                break;

            // �ړ�
            case EnemyMovementState.WALKING:

                EnemyWalking();

                break;

            // �ǐ�
            case EnemyMovementState.RUNNING:

                EnemyRunning();

                break;

            // �_�E��(�u���C�N���)
            case EnemyMovementState.DOWNED:

                EnemyDowned();

                return;

            // �̂����蒆
            case EnemyMovementState.STUNNED:

                EnemyStunned();

                return;

            // ���S
            case EnemyMovementState.DIE:

                StartCoroutine(EnemyDie(3f));

                return;
        }

        switch (_actionState)
        {
            // �T�[�`
            case EnemyActionState.SEARCHING:

                PlayerSearch();

                break;

            // �U��
            case EnemyActionState.ATTACKING:

                EnemyAttacking();

                break;
        }
    }

    /// <summary>
    /// �����ɃA�C�h����Ԃ̂Ƃ��̏���������
    /// </summary>
    private void EnemyIdle()
    {
        // �A�j���[�V�������I�������玟�̏�ԂɑJ��
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�
            _actionState = EnemyActionState.SEARCHING;
            _boxCollider.enabled = false;
            isAttackInterval = false;

            // �g���K�[���Z�b�g
            _animator.SetInteger("TransitionNo", 0);
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
    /// �S�[�����̑O���ɍL���͈͂ŏ�Q�������邩�𔻒肷��
    /// </summary>
    /// <param name="direction">�ړ�����</param>
    /// <param name="distance">���o�͈͂̋���</param>
    /// <returns>��Q��������� true</returns>
    private bool IsPathBlocked(Vector3 direction, float distance)
    {
        // BoxCast�̐ݒ�
        Vector3 boxSize = new Vector3(3.0f, 1.0f, 0.5f); // ���A�����A���s��
        Vector3 origin = transform.position + Vector3.up * 0.5f; // �S�[�����̏����ォ�甭��
        Quaternion rotation = Quaternion.LookRotation(direction); // �S�[�����̌����ɍ��킹��

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
    private void EnemyWalking()
    {
        _animator.SetInteger("TransitionNo", 1);

        moveSpeed = 2.5f;
        transform.position = Vector3.MoveTowards(transform.position, _randomTargetPos, moveSpeed * Time.deltaTime);

        Vector3 direction = (_randomTargetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // �ǂ����m
        if (IsPathBlocked(direction, detectionDistance))
        {
            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�
            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.TURNING;
            return;
        }

        // �ڕW�n�_���B
        if (Vector3.Distance(transform.position, _randomTargetPos) < 0.1f)
        {
            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�
            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.LOOKING_AROUND;
            _lookAroundTimer = 3.0f; // ���n�����Ԃ��Z�b�g
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
        _animator.SetInteger("TransitionNo", 0);

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
        _animator.SetInteger("TransitionNo", 0);

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
    private void EnemyRunning()
    {
        if (_actionState == EnemyActionState.SEARCHING)
        {
            _animator.SetInteger("TransitionNo", 2);
        }

        // �O�i���x
        moveSpeed = 5.0f;

        // ���݂̍������ێ�����
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _playerLastKnownPosition;

        if (_attackingPlayer != null)
        {
            targetPosition = _attackingPlayer.position; // �v���C���[�̈ʒu���X�V
        }

        targetPosition.y = currentPosition.y; // �S�[�����̍������Œ�

        // �v���C���[�̍Ō�̈ʒu�܂ł̋������v�Z
        float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

        // ��~�����ȓ��Ȃ�ړ����~
        if (distanceToTarget <= stopDistance)
        {
            _movementState = EnemyMovementState.IDLE;  // �ҋ@��Ԃɖ߂�
            _actionState = EnemyActionState.ATTACKING;
            _boxCollider.enabled = true;
            _attackingPlayer = null;
            return;
        }

        // �v���C���[�̍Ō�̈ʒu�Ɍ������Ĉړ�
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // �S�[�����̕������v���C���[�Ɍ����� (Y����]�̂�)
        Vector3 direction = (targetPosition - currentPosition).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Y���̂݉�]
        }
    }

    /// <summary>
    /// �U���������������B
    /// �U���I����A�ҋ@��Ԃɖ߂�B
    /// </summary>
    private void EnemyAttacking()
    {
        // �U���A�j���[�V�������I��������ҋ@�ɖ߂�
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _animator.SetInteger("TransitionNo", 0);
            isAttackInterval = false;
        }

        if (isAttackInterval)
        {
            return;
        }

        isAttackInterval = true;

        // �U���A�j���[�V�����������_���ɑI��
        int randomAttack = Random.Range(0, 2); // 0 �܂��� 1 �𐶐�
        if (randomAttack == 0)
        {
            _animator.SetInteger("TransitionNo", 3);
        }
        else
        {
            _animator.SetInteger("TransitionNo", 4);
        }

        // �v���C���[��������U�����J��Ԃ�
        PlayerSearch();
        if (_targetTrans != null && _targetTrans.gameObject.layer == 6)
        {
            _targetTrans = null;
            return;
        }

        //_movementState = EnemyMovementState.IDLE;  // �ҋ@��Ԃɖ߂�
    }

    /// <summary>
    /// �v���C���[�ɍU�����ꂽ�Ƃ��ɌĂяo��
    /// </summary>
    /// <param name="playerTransform">�U�������v���C���[��Transform</param>
    public void OnPlayerAttack(Transform playerTransform)
    {
        _attackingPlayer = playerTransform; // �U�����Ă����v���C���[���L�^
        _movementState = EnemyMovementState.RUNNING; // �ǐՏ�ԂɕύX
        _playerLastKnownPosition = playerTransform.position; // �v���C���[�̌��݈ʒu���L�^
    }

    /// <summary>
    /// �_�E�����
    /// </summary>
    private void EnemyDowned()
    {
        // �_�E�����I�������ꍇ�A��Ԃ�߂�
        if (_downedTimer <= 0)
        {
            _animator.SetInteger("TransitionNo", 0);

            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.SEARCHING;
            _lookAroundState = EnemyLookAroundState.NONE;
            isAttackInterval = false;

            _downedTimer = 5f;
            return;
        }

        // �g���K�[���Z�b�g
        _animator.SetInteger("TransitionNo", 5);

        _downedTimer -= Time.deltaTime;
    }

    private void EnemyStunned()
    {
        // �̂�����I��������ԑJ��
        if (IsAnimationFinished("Stunned"))
        {
            _animator.SetInteger("TransitionNo", 0);

            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�

            _movementState = EnemyMovementState.RUNNING;
            _actionState = EnemyActionState.SEARCHING;
            _lookAroundState = EnemyLookAroundState.NONE;
            isAttackInterval = false;

            return;
        }

        // �g���K�[���Z�b�g
        _animator.SetInteger("TransitionNo", 6);

        // ���ɂ̂�����܂ł̎��Ԃ��Z�b�g
        _stunnedTimer = 3f;
    }

    /// <summary>
    /// �|���
    /// </summary>
    private IEnumerator EnemyDie(float fadeDuration)
    {
        // �g���K�[���Z�b�g
        _animator.SetInteger("TransitionNo", 7);

        // �b��
        yield return new WaitForSeconds(fadeDuration);

        // ���S�ɓ����ɂ�����A�I�u�W�F�N�g���A�N�e�B�u��
        gameObject.SetActive(false);
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
    /// ���C�L���X�g�ݒ�
    /// </summary>
    private void RayCastSetting()
    {
        //��L���X�g�����ݒ�
        BasicRaycast();

        // ���S�_���擾
        _boxCastStruct._originPos = this.transform.position;

        // �����̃X�P�[��(x)���擾
        float squareSize = transform.localScale.x;

        // BoxCast�𐳕��`�̃T�C�Y�ɂ���
        _boxCastStruct._size = new Vector2(squareSize, squareSize);
    }

    /// <summary>
    /// ���C�L���X�g�̋���(�T���͈�)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = _searchRange;
    }

    /// <summary>
    /// Lookat�ݒ�
    /// �����ł́A
    ///    [SerializeField, Header("�ǂ����������I�u�W�F�N�g�̃g�����X�t�H�[��")]
    ///    private Transform _objTrans = default;
    /// �̒��ɓ��ꂽ�I�u�W�F�N�g�������ƌ��ߑ����鏈���������Ă����
    /// ����ŕ����͎擾�ł���̂ŁA���Ƃ͑O�i���邾���ŁA��L�Ŋi�[�����I�u�W�F�N�g��ǂ��悤�ɂȂ�܂�(Player�Ƃ�)
    /// </summary>
    private void PlayerLook()
    {
        // �v���C���[��Transform���擾
        Transform playerTrans = _targetTrans;

        if (playerTrans != null)
        {
            // �v���C���[�̈ʒu���擾
            Vector3 playerPosition = playerTrans.position;

            // �v���C���[��Y���𖳎������^�[�Q�b�g�̈ʒu���v�Z
            Vector3 lookPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);

            // �v���C���[�̕����Ɍ���
            transform.LookAt(lookPosition);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g��T��(�T���v��)
    /// �T�[�`�̗�B�D���Ȃ悤�ɉ��ǂ��ĂˁB
    /// ���̍�������\�b�h���������ƁA
    /// �v���C���[�̕����ɏ�Ƀ��C�L���X�g��L�΂��A�I�u�W�F�N�g�ȂǂɎז����ꂸ�A���ڃv���C���[�Ƀ��C�L���X�g���G�ꂽ�Ƃ���
    /// �A�N�V����Enum���T�[�`����U���ɐ؂�ւ��鏈���������Ă��B
    /// �ŁA�������C�L���X�g���I�u�W�F�N�g�ȂǂɎז�����āA�v���C���[�ɓ͂��Ȃ������ꍇ�́A�A�N�V����Enum�̓T�[�`�̂܂�
    /// MoveEnum�͈ړ��ɂ��A�T�[�`���Ȃ���ړ��i�ڂ̑O�ɂ����Q����������邽�߁j���鏈���������Ă����B
    /// </summary>
    private void PlayerSearch()
    {
        // �{�b�N�X�L���X�g�̐ݒ�
        Vector3 center = transform.position; // �L���X�g�J�n�ʒu
        Vector3 halfExtents = new Vector3(1f, 1f, 1f); // �{�b�N�X�̔��a
        Vector3 direction = transform.forward; // �L���X�g�̕���
        float maxDistance = 10f; // �L���X�g�̍ő勗��
        Quaternion orientation = Quaternion.identity; // �{�b�N�X�̉�]�i��]�Ȃ��j
        int layerMask = (1 << 6) | (1 << 8); // ���C���[�}�X�N�i���C���[6��8�j

        // �{�b�N�X�L���X�g�̎��s
        if (Physics.BoxCast(center, halfExtents, direction, out RaycastHit hit, orientation, maxDistance, layerMask))
        {
            // Debug.Log("�q�b�g�����I�u�W�F�N�g: " + hit.collider.gameObject.name);

            // �v���C���[�i���C���[6�j�̏ꍇ�̏���
            if (hit.collider.gameObject.layer == 6)
            {
                _targetTrans = hit.collider.gameObject.transform;
                _playerLastKnownPosition = _targetTrans.position; // �v���C���[�̈ʒu���L�^
                _movementState = EnemyMovementState.RUNNING;
            }
            else
            {
                _targetTrans = null; // �v���C���[�ȊO�Ȃ�^�[�Q�b�g������
            }
        }
        else
        {
            // �q�b�g���Ȃ������ꍇ
            _targetTrans = null;
            // Debug.Log("�v���C���[���X�e�[�W�����o����܂���ł����B");
        }
    }
}