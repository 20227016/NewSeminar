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
public class BossDemo : BaseEnemy
{

    // �{�X�̍s���p�^�[���p.�ϐ�(�ŏ��̓A�C�h����Ԃ���X�^�[�g)
    // 1.�A�C�h��
    // 2.�U��
    // 3.�_�E��
    // 4.���S
    private int _actionState = 2; // �f�o�b�N��2�ɂ��Ă邾���B�{����1����X�^�[�g�B������t����2�ɂ��A�s���p�^�[���𐧌䂵��(��.�A�C�h����5�b�ԌJ��Ԃ���2�ɂȂ�)

    // �{�X�̍U���p�^�[���p.�ϐ�(���I���A�s���p�^�[�������߂�)
    // 1.�H�̓ガ�����U��
    // 2.���e
    // 3.���[�U�[
    private int _currentAttack = default;

    // �s���p�^�[���𒊑I���A���̌��ʂ�z��Ɋi�[����
    private int[] _confirmedAttackState = new int[3];

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

                // �U���p�^�[���𒊑I����
                AttackState();

                // ���I�����U���p�^�[����ϐ��Ɋi�[�B���Ɏ��s����
                for (int i = 0; i < _confirmedAttackState.Length; i++)
                {
                    _currentAttack = _confirmedAttackState[i];
                    switch(_currentAttack)
                    {
                        case 1:

                            // �H�̓ガ�����U��
                            MowingDownAttack();

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
                }

                // ���ׂĂ̍U���p�^�[�������s��A�A�C�h���ɖ߂�(�������Ă邾���A�ς��Ă�����)
                _actionState = 1;

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
        print("�A�C�h���Ȃ�");
        // �A�C�h����Ԃ̏���������
    }

�@�@/// <summary>
 �@ /// �A�^�b�N���
    /// �{�X�̍U���p�^�[���𒊑I����
    /// </summary>
    private void AttackState()
    {

        Random _random = new Random();
        for (int i = 0; i < _confirmedAttackState.Length; i++)
        {
            _confirmedAttackState[i] = i + 1;
        }

        Debug.Log("�z��̏������: " + string.Join(", ", _confirmedAttackState));

        // �V���b�t��
        for (int i = _confirmedAttackState.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            int tmp = _confirmedAttackState[i];
            _confirmedAttackState[i] = _confirmedAttackState[j];
            _confirmedAttackState[j] = tmp;
        }

        Debug.Log("�V���b�t����̔z��: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// �H�̓ガ�����U��
    /// </summary>
    private void MowingDownAttack()
    {
        print("�H�̓ガ�����U�������s���܂���");
    }

    /// <summary>
    /// ���e�U��
    /// </summary>
    private void MagicBulletAttack()
    {
        print("���e�U�������s���܂���");
    }

    /// <summary>
    /// ���[�U�[�U��
    /// </summary>
    private void LaserAttack()
    {
        print("���[�U�[�U�������s���܂���");
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
}
