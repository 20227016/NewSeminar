using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;

/// <summary>
/// �{�X�G�l�~�[�̊��
/// �X�N���v�g����
/// �E�A�N�V�����X�e�[�g�ƍU���X�e�[�g������
/// �E�A�N�V�����X�e�[�g�̓{�X�{�̂̍s���p�^�[���𐧌䂷�邽�߂Ɋ��p(��.�A�C�h��.�U��)
/// �E�U���X�e�[�g�̓A�N�V�����X�e�[�g���U���̂Ƃ��ɍs���U���𐧌䂷�邽�߂Ɋ��p(�U���������_���ōs������)
/// �E���܂������ɃA�C�h���ƍU�����J��Ԃ��A�{�X�̏󋵂ɂ���āA�_�E���⎀�S���s����OK
/// �E�U���̓����������쐬���Ăق���(����print�Ńf�o�b�N���Ă邾��)
/// </summary>
public class StageBoss : BaseEnemy
{

    // �{�X�̍s���p�^�[���p.�ϐ�(�ŏ��̓A�C�h����Ԃ���X�^�[�g)
    // 1.�A�C�h��
    // 2.�U��
    // 3.�_�E��
    // 4.���S
    private int _actionState = 1; // �f�o�b�N��2�ɂ��Ă邾���B�{����1����X�^�[�g�B������t����2�ɂ��A�s���p�^�[���𐧌䂵��(��.�A�C�h����5�b�ԌJ��Ԃ���2�ɂȂ�)

    [SerializeField]
    private float _currentTimer = 5f; // ���ݎ���

    //�A�j���[�^�[���Ăяo���ϐ�
    Animator _animator;

    BoxCollider _boxCollider;

    // �{�X�̍U���p�^�[���p.�ϐ�(���I���A�s���p�^�[�������߂�)
    // 1.�H�̓ガ�����U��
    // 2.���e
    // 3.���[�U�[
    private int _currentAttack = default;
    private int _currentLottery = default; // ���݂̒��I��

    // �s���p�^�[���𒊑I���A���̌��ʂ�z��Ɋi�[����
    private int[] _confirmedAttackState = new int[3];

    private int _faintingState = 1;

    [SerializeField]
    private float _samonTimer = 3.0f;

    private GameObject _evilMage = default;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponentInChildren<BoxCollider>();
        _evilMage = GameObject.Find("EvilMagePADefault");
        _evilMage.SetActive(false);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

        // �{�X�̍s���p�^�[���X�e�[�g
        switch(_actionState)
        {

            // �A�C�h��
            case 1:

                IdleState();

                break;


            // �U��
            case 2:

                // ���I���ĂȂ����
                if (_currentLottery == 0)
                {
                    // �U���p�^�[���𒊑I����
                    AttackState();
                }

                // ���I�����U���p�^�[����ϐ��Ɋi�[�B���Ɏ��s����
                _currentAttack = _confirmedAttackState[_currentLottery];

                switch (_currentAttack)
                {
                    case 1:

                        // �H�̓ガ�����U��
                        WingAttack();

                        break;

                    case 2:

                        // ���e�U��
                        MagicBulletAttack();

                        break;

                    case 3:

                        // ���[�U�[�U��
                        LaserAttack();
                        
                        break;
                        
                    default:
                        print("�{�X�̍U���p�^�[���ŗ�O�����m�B�����G�ɍU���p�^�[��1��I�����܂��B");
                        _currentAttack = 1;
                        break;
                }

                _currentLottery++; // ���̍U����

                // �S�Ă̍U���������璊�I���Z�b�g
                if (_currentLottery == _confirmedAttackState.Length)
                {
                    _currentLottery = 0;
                }

                break;


�@�@�@�@�@�@// �_�E��
            case 3:

                // ��(HP�������؂����炷�ׂẴX�e�[�g�������I�����A�_�E����ԂɈڍs����)
                FaintingState();

                break;


            // ���S
            case 4:

                // �Ƃ肠������\���ɂ����OK
                DeathState();

                break;

            default:
                print("�{�X�̍s���p�^�[���ŗ�O�����m�B�����I�ɃA�C�h���ɖ߂��܂�");
                _actionState = 1;
                break;
        }
    }

    /// <summary>
    /// �A�C�h�����
    /// </summary>
    private void IdleState()
    {
        //print("�A�C�h���Ȃ�");

        if (IsAnimationFinished("Slash_Attack")
            || IsAnimationFinished("Magic_Attack")
            || IsAnimationFinished("beam_Attack")
            || IsAnimationFinished("revive"))
        {
            _animator.SetInteger("Action_State", 0);
        }
            
        _currentTimer -= Time.deltaTime;
        if (_currentTimer <= 0)
        {
            _actionState = 2;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _actionState = 3;
        }
    }

�@�@/// <summary>
 �@ /// �A�^�b�N���
    /// �{�X�̍U���p�^�[���𒊑I����
    /// </summary>
    private void AttackState()
    {

        Random _random = new Random();
        for (_currentLottery = 0; _currentLottery < _confirmedAttackState.Length; _currentLottery++)
        {
            _confirmedAttackState[_currentLottery] = _currentLottery + 1;
        }

        Debug.Log("�z��̏������: " + string.Join(", ", _confirmedAttackState));

        // �V���b�t��
        for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
        {
            int j = _random.Next(_currentLottery + 1);
            int tmp = _confirmedAttackState[_currentLottery];
            _confirmedAttackState[_currentLottery] = _confirmedAttackState[j];
            _confirmedAttackState[j] = tmp;
        }

        Debug.Log("�V���b�t����̔z��: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// �H�̓ガ�����U��
    /// </summary>
    private void WingAttack()
    {
        print("�H�̓ガ�����U�������s���܂���");
        _animator.SetInteger("Action_State",1);
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// ���e�U��
    /// </summary>
    private void MagicBulletAttack()
    {
        _animator.SetInteger("Action_State", 2);
        print("���e�U�������s���܂���");
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// ���[�U�[�U��
    /// </summary>
    private void LaserAttack()
    {
        _animator.SetInteger("Action_State", 3);
        print("���[�U�[�U�������s���܂���");
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// �_�E�����
    /// </summary>
    private void FaintingState()
    {
        // �_�E����Ԃ̏���������
        print("�_�E��");

        _samonTimer -= Time.deltaTime;

        switch (_faintingState)
        {
            case 1:
                _animator.SetInteger("Action_State", 4);

                if (IsAnimationFinished("Samon"))
                {
                    _evilMage.SetActive(true);
                }

                if (_samonTimer <= 0f)
                {
                    _faintingState = 2;
                    _samonTimer = 5.0f;
                }
                
                break;

            case 2:
                _animator.SetInteger("Action_State", 5);
                if (_samonTimer <= 0f)
                {
                    _faintingState = 3;
                }
                break;

            case 3:
                _animator.SetInteger("Action_State", 6);
                _actionState = 1;
                break;
        }
    }

    /// <summary>
    /// ���S���
    /// </summary>
    private void DeathState()
    {
        // ���S��Ԃ̏���������
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
}
