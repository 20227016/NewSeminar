using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;
using System.Collections;
using System.Net.NetworkInformation;

/// <summary>
/// �{�X�G�l�~�[�̊��
/// �X�N���v�g����
/// �E�A�N�V�����X�e�[�g�ƍU���X�e�[�g������
/// �E�A�N�V�����X�e�[�g�̓{�X�{�̂̍s���p�^�[���𐧌䂷�邽�߂Ɋ��p(��.�A�C�h��.�U��)
/// �E�U���X�e�[�g�̓A�N�V�����X�e�[�g���U���̂Ƃ��ɍs���U���𐧌䂷�邽�߂Ɋ��p(�U���������_���ōs������)
/// �E���܂������ɃA�C�h���ƍU�����J��Ԃ��A�{�X�̏󋵂ɂ���āA�_�E���⎀�S���s����OK
/// �E�U���̓����������쐬���Ăق���(����print�Ńf�o�b�N���Ă邾��)
/// </summary>
public class BossDemo : BaseEnemy
{
    [Header("���e�U���ݒ�")]
    [Tooltip("���˂��閂�e��Prefab")]
    [SerializeField] private GameObject _magicBullet;

    [Tooltip("���e�U���̗��ߎ���")]
    [SerializeField] private float _bulletChargeTime = 1.3f;

    [Tooltip("���e�̐�������")]
    [SerializeField] private bool isBulletGeneration = true;

    [SerializeField]
    private float _currentTimer = 5f; // ���ݎ���

    private Animator _animator; // �A�j���[�^�[

    Transform _LaserBeam = default; // ���[�U�[�r�[��

    // �{�X�̍s���p�^�[���p.�ϐ�(�ŏ��̓A�C�h����Ԃ���X�^�[�g)
    // 1.�A�C�h��
    // 2.�U��
    // 3.�_�E��
    // 4.���S
    [SerializeField]
    private int _actionState = 1;

    // �{�X�̍U���p�^�[���p.�ϐ�(���I���A�s���p�^�[�������߂�)
    // 1.�H�̓ガ�����U��
    // 2.���e
    // 3.���[�U�[
    [SerializeField]
    private int _currentAttack = default;
    private int _currentLottery = default; // ���݂̒��I��

    // �s���p�^�[���𒊑I���A���̌��ʂ�z��Ɋi�[����
    private int[] _confirmedAttackState = new int[3];

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _LaserBeam = transform.Find("LaserBeam");
        _LaserBeam.gameObject.SetActive(false);
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

                switch(_currentAttack)
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
        if (IsAnimationFinished("Wing Slash Attack")
            || IsAnimationFinished("MagicBullet")
            || IsAnimationFinished("LaserBeam"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _LaserBeam.gameObject.SetActive(false); // ��A�N�e�B�u��
            isBulletGeneration = true;
        }

        _currentTimer -= Time.deltaTime;
        if (_currentTimer <= 0)
        {
            _actionState = 2;
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

        // Debug.Log("�z��̏������: " + string.Join(", ", _confirmedAttackState));

        // �V���b�t��
        for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
        {
            int j = _random.Next(_currentLottery + 1);
            int tmp = _confirmedAttackState[_currentLottery];
            _confirmedAttackState[_currentLottery] = _confirmedAttackState[j];
            _confirmedAttackState[j] = tmp;
        }

        // Debug.Log("�V���b�t����̔z��: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// �H�̓ガ�����U��
    /// </summary>
    private void WingAttack()
    {
        _animator.SetInteger("TransitionNo", 1);
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// ���e�U��
    /// </summary>
    private void MagicBulletAttack()
    {
        _animator.SetInteger("TransitionNo", 2);
        if (isBulletGeneration)
        {
            // ���@�e�𐶐�
            Vector3 position = new Vector3(6f, 6f, 15f); // �ʒu (x6, y6, z15)
            Quaternion rotation = Quaternion.Euler(0f, -180f, 0f); // ���� (Y���� -180�� ��])
            Instantiate(_magicBullet, position, rotation);
            isBulletGeneration = false;
        }

        _actionState = 1;
        _currentTimer = 10f;
    }

    /// <summary>
    /// ���[�U�[�U��
    /// </summary>
    private void LaserAttack()
    {
        _animator.SetInteger("TransitionNo", 3);
        _LaserBeam.gameObject.SetActive(true);
        _actionState = 1;
        _currentTimer = 15f;
    }

    /// <summary>
    /// �_�E�����
    /// </summary>
    private void FaintingState()
    {
        // �_�E����Ԃ̏���������
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
