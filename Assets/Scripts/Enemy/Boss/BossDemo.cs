using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;
using System.Collections;
using System.Net.NetworkInformation;
using Unity.VisualScripting;

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
    [Tooltip("���̃_���[�W")]
    [SerializeField] private float _damage = 10f;

    [Header("���e�U���ݒ�")]
    [Tooltip("���˂��閂�e��Prefab")]
    [SerializeField] private GameObject _magicBullet;

    [Tooltip("���e�U���̗��ߎ���")]
    [SerializeField] private float _bulletChargeTime = 1.3f;

    [Tooltip("���e�̐�������")]
    [SerializeField] private bool isBulletGeneration = true;

    [SerializeField]
    private float _currentTimer = 5f; // ���ݎ���

    // �A�j���[�^�[�ϐ�
    // TransitionNo.-2 Appearance
    // TransitionNo.-1 Roar
    // TransitionNo.0  Idle
    // TransitionNo.1  WingAttack
    // TransitionNo.2  MagicBullet
    // TransitionNo.3  LaserBeam
    // TransitionNo.4  Summon
    // TransitionNo.5  Fainting
    // TransitionNo.6  Heel
    // TransitionNo.7  Die
    private Animator _animator; // �A�j���[�^�[

    Transform _LaserBeam = default; // ���[�U�[�r�[��

    private bool isStartAction = default;

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
    private int _lastValue = default;

    private int _hp = 100;
    private float _summonTimer = default;
    private bool isFaintg = false;
    private int _faintingState = 1;

    private Transform _child = default;
    private BoxCollider[] _boxColliders = default;

    private GameObject _golem = default;
    private GameObject _evilMage = default;
    private GameObject _fishman = default;
    private GameObject _demon = default;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("TransitionNo", -2);

        _LaserBeam = transform.Find("LaserBeam");
        _LaserBeam.gameObject.SetActive(false);

        _child = transform.Find("RigPelvis");
        _boxColliders = _child.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = false;
        }

        _golem = GameObject.Find("GolemPADefault");
        _golem.SetActive(false);
        _evilMage = GameObject.Find("EvilMagePADefault");
        _evilMage.SetActive(false);
        _fishman = GameObject.Find("FishmanPADefault");
        _fishman.SetActive(false); 
        _demon = GameObject.Find("FylingDemonPAMaskTint");
        _demon.SetActive(false);
    }

    private void Update()
    {
        if (IsAnimationFinished("Appearance"))
        {
            _animator.SetInteger("TransitionNo", -1);
        }

        if (IsAnimationFinished("Roar"))
        {
            _animator.SetInteger("TransitionNo", 0);
            isStartAction = true;
        }

        // ���[�r�[��ɍs���J�n
        if (!isStartAction)
        {
            return;
        }

        // ����&�_�E���e�X�g
        if (Input.GetKeyDown(KeyCode.S) && !isFaintg)
        {
            _hp = 50;
        }

        // ���ʃe�X�g
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (BoxCollider collider in _boxColliders)
            {
                collider.enabled = false;
            }
            _LaserBeam.gameObject.SetActive(false);
            _actionState = 4;
        }

        // �{�X�̍s���p�^�[���X�e�[�g
        switch (_actionState)
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
                StartCoroutine(DeathState(3f));

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
        if (IsAnimationFinished("WingAttack")
            || IsAnimationFinished("MagicBullet")
            || IsAnimationFinished("LaserBeam")
            || IsAnimationFinished("Heel"))
        {
            _animator.SetInteger("TransitionNo", 0);

            _LaserBeam.gameObject.SetActive(false); // ��A�N�e�B�u��
            isBulletGeneration = true;
            foreach (BoxCollider collider in _boxColliders)
            {
                collider.enabled = false;
            }

            if (_hp <= 50 && !isFaintg)
            {
                _actionState = 3;
            }
        }

        _currentTimer -= Time.deltaTime;
        if (_currentTimer <= 0)
        {
            _actionState = 2;
        }
    }

    /// <summary>
    /// �A�^�b�N���
    /// �{�X�̍U���p�^�[���𒊑I����
    /// </summary>
    private void AttackState()
    {
        Random _random = new Random();
        for (_currentLottery = 0; _currentLottery < _confirmedAttackState.Length; _currentLottery++)
        {
            _confirmedAttackState[_currentLottery] = _currentLottery + 1;
        }

        // �U���p�^�[�����V���b�t�����A�O��̔z��̍Ō�Ǝ��̔z��̍ŏ��������l�ɂȂ�Ȃ��悤�ɂ���
        do
        {
            for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
            {
                int j = _random.Next(_currentLottery + 1);
                int tmp = _confirmedAttackState[_currentLottery];
                _confirmedAttackState[_currentLottery] = _confirmedAttackState[j];
                _confirmedAttackState[j] = tmp;
            }

        } while (_confirmedAttackState[0] == _lastValue);

        _lastValue = _confirmedAttackState[^1]; // �O��̔z��̍Ō�̒l

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

        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = true;
        }
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
        isFaintg = true;

        switch (_faintingState)
        {
            case 1:
                _animator.SetInteger("TransitionNo", 4);

                if (IsAnimationFinished("Summon"))
                {
                    _golem.SetActive(true);
                    _evilMage.SetActive(true);
                    _fishman.SetActive(true);
                    _demon.SetActive(true);
                    _summonTimer = 10.0f;
                    _faintingState = 2;           
                }

                break;

            case 2:
                _animator.SetInteger("TransitionNo", 5);
                _summonTimer -= Time.deltaTime;
                if (_summonTimer <= 0f)
                {
                    _faintingState = 3;
                }
                break;

            case 3:
                _animator.SetInteger("TransitionNo", 6);
                _actionState = 1;
                _faintingState = 1;
                break;
        }
    }

    /// <summary>
    /// ���S���
    /// </summary>
    private IEnumerator DeathState(float fadeDuration)
    {
        _animator.SetInteger("TransitionNo", 7);

        yield return new WaitForSeconds(fadeDuration);

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
    /// ���̃I�u�W�F�N�g�ƏՓ˂����ۂ̏����B
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    public override void OnTriggerEnter(Collider other)
    {
        // �_���[�W��^���鏈���i��: �v���C���[�ȂǓ���̃��C���[�̏ꍇ�j
        if (other.CompareTag("Player")) // �v���C���[�ɑ΂��ă_���[�W��^����
        {
            // �v���C���[�̃_���[�W�������Ăяo���i���̗�j
            Debug.Log($"Wing Hit {other.gameObject.name}, dealt {_damage} damage.");
        }
    }
}
