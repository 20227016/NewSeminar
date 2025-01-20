
using UnityEngine;
using System.Collections;

/// <summary>
/// FlyingDemon.cs
/// �f�[�����̍s�����W�b�N���Ǘ�����N���X�B
/// �ړ���U���ȂǁA���܂��܂ȏ�Ԃɉ����������𐧌䂷��B
/// �쐬��: /
/// �쐬��: �k�\�V��
/// </summary>
public class FlyingDemon : BaseEnemy
{
    // ������Enem���쐬�B
    // �Ȃ�Enem��2���邩�Ƃ����ƁA����Ȃ���U�����邽�߁i���ɂ����邯��...�j
    // ����ƁA�ړ����Ȃ��牽�������邱�Ƃ��ł��Ȃ�����A�����Ă܂��B�������肷�����߂�
    // ��Ԃ�ǉ��������ꍇ�A�p�u���b�N��Enem��ݒ肵�Ă���̂ŁAEnem�p�u���b�N�N���X(���������X�N���v�g������)�ɒǉ�����Ύg���܂��B����
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField]
    private float _stateSwitchInterval = 3.0f; // ��Ԑ؂�ւ��Ԋu

    private float _stateTimer = 0.0f; // ��ԊǗ��p�^�C�}�[

    [SerializeField]
    private float _attackStateSwitchInterval = 2.0f; // ��Ԑ؂�ւ��Ԋu

    private float _attackStateTimer = 0.0f; // ��ԊǗ��p�^�C�}�[

    private float _searchHeight = default;

    [SerializeField, Header("�ǂ����������I�u�W�F�N�g�̃g�����X�t�H�[��")]
    private Transform _targetTrans = default;

    [Tooltip("�����͈͂̔��a���w�肵�܂�")]
    [SerializeField] private float _searchRadius = 30f; // �����͈́i���a�j

    [SerializeField, Tooltip("�U���͈�")]
    private float _attackRange = 5.0f;

    [SerializeField, Tooltip("�����X�s�[�h")]
    private float _warkRange = 3.0f;

    [SerializeField, Tooltip("�_���[�W���󂯂鎞��")]
    private float _damageRange = 1.0f;

    [Header("�����Ώۂ̐ݒ�")]
    [Tooltip("�����ΏۂƂȂ郌�C���[�ԍ����w�肵�܂�")]
    [SerializeField] private int _targetLayer = 6; // �Ώۂ̃��C���[�ԍ�

    private Vector3 _randomTargetPos; // �����_���ړ��̖ڕW�ʒu

    [SerializeField]
    private bool _attackAction = true;

    [SerializeField]
    private bool _attackEnd = true;

    // �A�j���[�^�[�ϐ�
    // TransitionNo.0 Idle
    // TransitionNo.1 Walk
    // TransitionNo.2 Running
    // TransitionNo.3 Frying
    // TransitionNo.4 Attack01
    // TransitionNo.5 Attack02
    // TransitionNo.6 Fire
    // TransitionNo.7 Downed
    // TransitionNo.8 Stunned 
    // TransitionNo.9 Die
    Animator _animator;

    BoxCollider _boxCollider;

    private float speed = 5f;     // �ړ����x

    public GameObject fireballPrefab; // ���̋���Prefab
    public Transform firePoint; // �ˏo�ʒu
    public float fireballSpeed = 10f; // ���̋��̑��x

    private void Awake()
    {
        // Raycast���������߂̊�{�ݒ�����Ă����֐�
        BasicRaycast();

        _animator = GetComponent<Animator>();
        _boxCollider = GetComponentInChildren<BoxCollider>();

        _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    protected void Update()
    {
        /* 
         * ���C�L���X�g�̒��S�_�������ōX�V���Ă����
         * ����ŁA���C�L���X�g����Ɏ����̑O����ł��Ă����
         * �܂�A�����Ȃ��ꍇ�i�����ʒu��ς��������j�����Ȃ���OK
         * �ς����������畷���Ă��ꂽ�炻�̓s�x��������
         * ����ȊO�ɂ��T�C�Y�A��������ς��郁�\�b�h������
        */
        SetPostion();
        SetDirection();
        CheckAttackRange();

        if (Input.GetKeyDown(KeyCode.S))
        {
            _movementState = EnemyMovementState.STUNNED;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _movementState = EnemyMovementState.DIE;
        }

        // ��ԊǗ��^�C�}�[�̍X�V
        _stateTimer += Time.deltaTime;

        // ��Ԃ�؂�ւ���^�C�~���O�ɂȂ�����؂�ւ���
        if (_stateTimer >= _stateSwitchInterval && _movementState != EnemyMovementState.DIE)
        {
            SwitchMovementState();
            _stateTimer = 0.0f; // �^�C�}�[�����Z�b�g
        }

        switch (_movementState)
        {
            // �ҋ@
            case EnemyMovementState.IDLE:

                ideling();

                break;

            // �����i����j
            case EnemyMovementState.WALKING:

                warking();

                break;

            // �ǐ�
            case EnemyMovementState.RUNNING:

                running();

                break;

            // �~���i�~���j
            case EnemyMovementState.FALLING:

                break;

            //��ԁ@�i�㏸�j
            case EnemyMovementState.FRYING:


                break;

            //�_���[�W(�󂯂��Ƃ�)
            case EnemyMovementState.STUNNED:

                enemyDamage();

                return;


            // �_�E��(�u���C�N���)
            case EnemyMovementState.DOWNED:

                break;

            // ���S
            case EnemyMovementState.DIE:

                // Y���W��0.7������������~
                if (transform.position.y > 0.7f)
                {
                    transform.position -= speed * Time.deltaTime * transform.up * 2;
                }

                StartCoroutine(EnemyDie(3f));

                return;
        }

        switch (_actionState)
        {

            // �T�[�`
            case EnemyActionState.SEARCHING:

                _boxCollider.enabled = false;

                PlayerSearch();
                PlayerLook();

                break;

            // �U��
            case EnemyActionState.ATTACKING:

                _boxCollider.enabled = true;
                playerAttack();

                break;
        }
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
        _boxCastStruct._distance = _searchRadius;
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
        if (_targetTrans != null)
        {
            // �v���C���[��Transform���擾
            Transform playerTrans = _targetTrans;

            // �v���C���[�̈ʒu���擾
            Vector3 playerPosition = playerTrans.position;

            // �v���C���[��Y���𖳎������^�[�Q�b�g�̈ʒu���v�Z
            Vector3 lookPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);

            // �v���C���[�̕����Ɍ���
            transform.LookAt(lookPosition);
        }
    }


    /// </summary>
    //�v���C���[����������ǐՊJ�n
    //�v���C���[����Ȃ��ꍇnull
    //�ǐՂ������ɓ���Ė��ߔ����̏��Ԃ�ς��Ă���

    /// <summary>
    /// �����𒆐S�Ƃ����~���`�̈��͈͓��ŁA�w��̃��C���[�ɑ�����I�u�W�F�N�g���������A
    /// �ł��߂��I�u�W�F�N�g����肵�܂��B
    /// </summary>
    private void PlayerSearch()
    {
        // �~���͈͂��J�v�Z���ŋߎ�
        Vector3 capsuleBottom = transform.position - Vector3.up * (_searchHeight / 5f);
        Vector3 capsuleTop = transform.position + Vector3.up * (_searchHeight / 5f);

        Collider[] hits = Physics.OverlapCapsule(
            capsuleBottom,
            capsuleTop,
            _searchRadius
        );

        float closestDistance = Mathf.Infinity;
        Transform closestObject = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == _targetLayer)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.transform;
                }
            }
        }

        _targetTrans = closestObject;

        float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

        if (distanceToTarget <= 7)
        {
            _movementState = EnemyMovementState.RUNNING;

            // Y���W��0������������~
            if (transform.position.y < 0.0f)
            {
                return;
            }
            // ��苗�������ƍ~������
            transform.position -= speed * Time.deltaTime * transform.up;
        }
        else
        {
            if (transform.position.y > 5.0f)
            {
                return;
            }
            // ��苗�������Ə㏸����
            transform.position += speed * Time.deltaTime * transform.up;
        }
    }

    /*
    /// <summary>
    /// �����͈͂��V�[���r���[�ɕ\�����܂��i�~���`�j�B
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // �����͈͂̉~���`��\��
        Vector3 bottomCenter = transform.position - Vector3.up * (_searchHeight / 2f);
        Vector3 topCenter = transform.position + Vector3.up * (_searchHeight / 2f);

        // �J�v�Z���̉��Ə����Ō���
        Gizmos.DrawWireSphere(bottomCenter, _searchRadius);
        Gizmos.DrawWireSphere(topCenter, _searchRadius);
        Gizmos.DrawLine(bottomCenter + Vector3.forward * _searchRadius, topCenter + Vector3.forward * _searchRadius);
        Gizmos.DrawLine(bottomCenter - Vector3.forward * _searchRadius, topCenter - Vector3.forward * _searchRadius);
        Gizmos.DrawLine(bottomCenter + Vector3.right * _searchRadius, topCenter + Vector3.right * _searchRadius);
        Gizmos.DrawLine(bottomCenter - Vector3.right * _searchRadius, topCenter - Vector3.right * _searchRadius);

        if (_targetTrans != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _targetTrans.position);
        }
    }
    */

    /// <summary>
    /// �U���͈͂��`�F�b�N���A�͈͓��Ƀv���C���[���������ꍇ�U����Ԃɐ؂�ւ���
    /// </summary>
    private void CheckAttackRange()
    {
        if (_targetTrans != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

            if (distanceToTarget <= _attackRange && transform.position.y <= 0.0f)
            {
                _actionState = EnemyActionState.ATTACKING;
            }
            else
            {
                if (!_attackEnd)
                {
                    return;
                }

                _actionState = EnemyActionState.SEARCHING;
            }
        }
    }

    /// <summary>
    /// �ړ���Ԃ�؂�ւ���
    /// </summary>
    private void SwitchMovementState()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }

        if (_movementState == EnemyMovementState.IDLE)
        {
            _movementState = EnemyMovementState.WALKING;
        }
        else if (_movementState == EnemyMovementState.WALKING)
        {
            _movementState = EnemyMovementState.IDLE;
            _animator.SetInteger("TransitionNo", 6);
        }
    }

    private void ideling()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }

        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02") || IsAnimationFinished("Fire"))
        {
            _animator.SetInteger("TransitionNo", 0);
        }
    }

    private void running()
    {
        if (_targetTrans == null)
        {
            return;
        }

        _attackAction = true;
        _animator.SetInteger("TransitionNo", 2);

        //Debug.Log("�ǐՒ�");

        // �O�i���x
        _warkRange = 5.0f;

        // ���݂̍������ێ�����
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _targetTrans.position;

        targetPosition.y = currentPosition.y; // �S�[�����̍������Œ�
        // �v���C���[�̍Ō�̈ʒu�܂ł̋������v�Z
        float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

        // �v���C���[�̍Ō�̈ʒu�Ɍ������Ĉړ�
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            _warkRange * Time.deltaTime
        );
    }

    private void playerAttack()
    {
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _attackEnd = true;
            _actionState = EnemyActionState.SEARCHING;
        }

        _movementState = EnemyMovementState.IDLE;

        if (_attackStateTimer >= _attackStateSwitchInterval)
        {
            _attackAction = true;
            _attackStateTimer = 0f;
        }

        _attackStateTimer += Time.deltaTime;

        if (!_attackAction)
        {
            return;
        }

        int randomAttack = Random.Range(0, 2);
        switch (randomAttack)
        {
            case 0:
                _animator.SetInteger("TransitionNo", 4);

                break;

            case 1:
                _animator.SetInteger("TransitionNo", 5);

                break;
        }
        _attackAction = false;
        _attackEnd = false;
    }

    private void warking()
    {
        _actionState = EnemyActionState.SEARCHING;

        _animator.SetInteger("TransitionNo", 1);
        //Debug.Log("����");

        _warkRange = 2.5f;

        // ���݂̈ʒu
        Vector3 currentPosition = transform.position;

        // �^�[�Q�b�g�ʒu (_randomTargetPos��y���W�����݂�y���W�ɌŒ�)
        Vector3 targetPosition = new Vector3(_randomTargetPos.x, currentPosition.y, _randomTargetPos.z);

        // �ړ�
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _warkRange * Time.deltaTime);

        Vector3 direction = (_randomTargetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // �ڕW�n�_���B
        if (Vector2.Distance(
        new Vector2(transform.position.x, transform.position.z),
        new Vector2(_randomTargetPos.x, _randomTargetPos.z)
    ) < 0.1f)
        {
            _movementState = EnemyMovementState.IDLE;
            _randomTargetPos = GenerateRandomPosition(); // �����_���Ȉʒu�𐶐�
        }
    }

    /// <summary>
    /// �����_���Ȉʒu�𐶐�����
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        //float range = 7.5f; // �����_���ړ��͈�
        Vector3 randomOffset = new Vector3(
            Random.Range(-5, 16),
            0f,
            Random.Range(-5, 16)
        );
        return randomOffset; // ���݈ʒu����Ƀ����_���Ȉʒu�𐶐�
    }

    /*
    /// <summary>
    /// �����_���ړ��̖ڕW�ʒu�ւ̐���\��
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_randomTargetPos != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _randomTargetPos); // ���݈ʒu����ڕW�ʒu�ւ̐���\��
            Gizmos.DrawSphere(_randomTargetPos, 0.2f); // �ڕW�ʒu�����ŕ\��
        }
    }*/

    private void enemyDamage()
    {
        if (IsAnimationFinished("Stunned"))
        {
            _movementState = EnemyMovementState.IDLE;

            _animator.SetInteger("TransitionNo", 0);
            return;
        }

        _animator.SetInteger("TransitionNo", 7);
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

    private void FirebulletInstantiate()
    {
        // �^�[�Q�b�g�������v�Z
        Vector3 directionToTarget = (_targetTrans.position - firePoint.position).normalized;

        // �^�[�Q�b�g�����ɉ�]��ݒ�
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        // ���̋��𐶐�
        Instantiate(fireballPrefab, firePoint.position, rotationToTarget);
    }

    /// <summary>
    /// �|���
    /// </summary>
    private IEnumerator EnemyDie(float fadeDuration)
    {
        // �g���K�[���Z�b�g
        _animator.SetInteger("TransitionNo", 9);

        // �b��
        yield return new WaitForSeconds(fadeDuration);

        // ���S�ɓ����ɂ�����A�I�u�W�F�N�g���A�N�e�B�u��
        gameObject.SetActive(false);
    }
}