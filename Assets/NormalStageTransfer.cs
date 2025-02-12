using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections;

public class NormalStageTransfer : NetworkBehaviour
{

    /// <summary>
    ///  �e���|�[�g���̃v���C���[���Ǘ����郊�X�g
    /// </summary>
    private List<GameObject> _playersInPortal = new List<GameObject>();

    /// <summary>
    /// �m�[�}���X�e�[�W�̃|�[�^���ɓ���X�e�[�W�ړ�������̈ʒu
    /// </summary>
    private GameObject _normalTeleportPosition = default;
    /// <summary>
    /// �{�X�X�e�[�W�̃|�[�^���ɓ���X�e�[�W�ړ�������̈ʒu
    /// </summary>
    private GameObject _bossTeleportPosition = default;

    [Tooltip("�m�[�}���X�e�[�W�̃e���|�[�g���W")]
    private Transform _normalStageteleportPos = default;

    [Tooltip("�{�X�X�e�[�W�̃e���|�[�g���W")]
    private Transform _bossStageteleportPos = default;

    /// <summary>
    /// �G�Ǘ��N���X
    /// </summary>
    private EnemySpawner _enemySpawner = default;

    /// <summary>
    /// �m�[�}���X�e�[�W���N���A�������̃t���O
    /// </summary>
    [Networked]
    public bool ClearNormalStage { get; set; } = false;

    /// <summary>
    /// �X�|�[���������Ƃ����邩�̃t���O
    /// </summary>
    [Networked]
    public bool HasTeleport { get; set; } = false;

    [Header("�m�[�}���X�e�[�W�̃X�J�C�{�b�N�X")]
    private Material _normalStageSkyBox = default;

    [Header("�{�X�X�e�[�W�̃X�J�C�{�b�N�X")]
    private Material _bossStageSkyBox = default;

    /// <summary>
    /// �m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��
    /// </summary>
    [Networked, Tooltip("�m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��")]
    public int StageRequiredPlayers { get; set; }

    [SerializeField, Header("���g�̃T�E���h�\�[�X")]
    private AudioSource _audioSource = default;
    [SerializeField, Header("�e���|�[�g�̉�")]
    private AudioClip _audioClip = default;

    private ISound _sound = new SoundManager();

    public override void Spawned()
    {

        _enemySpawner = FindObjectOfType<EnemySpawner>();

        _normalTeleportPosition = GameObject.Find("NormalTeleportPosition");
        _bossTeleportPosition = GameObject.Find("BossTeleportPosition");

        _normalStageSkyBox = Resources.Load<Material>("Day1");
        _bossStageSkyBox = Resources.Load<Material>("Sunset3");
        if(_normalStageSkyBox != null)
        {
            print("�擾�ł�����");
        }
        else
        {
            print("�ł��ĂȂ���");
        }

        _normalStageteleportPos = _normalTeleportPosition.transform;
        _bossStageteleportPos = _bossTeleportPosition.transform;

        _enemySpawner.OnAllEnemiesDefeatedObservable.Subscribe(_ =>
        {
            // �G�l�~�[�S�Ŏ��̏������L�q
            Debug.Log("���̃X�N���v�g�ŃG�l�~�[�S�ŃC�x���g���󂯎��܂����I");
            HandleAllEnemiesDefeated();
        }).AddTo(this);
    }

    /// <summary>
    /// �]���|�[�^���Ƀv���C���[���������Ƃ��Ƀ��X�g�ɒǉ�����
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {

        if (collider.CompareTag("Player") && !_playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Add(collider.gameObject);
            print($"�v���C���[�����m�B���݂̐l���� {_playersInPortal.Count}/{StageRequiredPlayers} �ł�");
        }
        // �K�v�l������������S�����e���|�[�g
        if ((_playersInPortal.Count >= StageRequiredPlayers) && !ClearNormalStage || HasTeleport && !ClearNormalStage )
        {
            Debug.Log($"<color=red>�|�[�^���l���F{_playersInPortal.Count}</color>");
            Debug.Log($"<color=red>�Q���l���F{StageRequiredPlayers}</color>");
            print("�����̃e���|�[�g");
            NormalTeleportAllPlayers();
            if (Runner.IsServer)
            {
                ChecgeBool();
            }
        }
        else if ((_playersInPortal.Count >= StageRequiredPlayers) && ClearNormalStage ||  HasTeleport &&  ClearNormalStage)
        {
            print("�{�X�e���|�[�g�A����");
            BossTeleportAllPlayers();

        }
    }

    /// <summary>
    /// �e���|�[�g�����Ƃ��Ƀt���O��ύX����
    /// </summary>
    private void ChecgeBool()
    {

        Debug.Log("Bool�ύX");
        HasTeleport = true;

    }

    /// <summary>
    /// �]���|�[�^������v���C���[���������Ƃ��Ƀ��X�g����폜����
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player") && _playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Remove(collider.gameObject);
            print($"<color=red>�v���C���[���|�[�^���𗣂�܂����B���݂̐l���� {_playersInPortal.Count} �ł�</color>");
        }
    }

    /// <summary>
    /// �S�Ẵv���C���[���e���|�[�g������
    /// </summary>
    private void NormalTeleportAllPlayers()
    {
        // �m�[�}���X�e�[�W�Ƀe���|�[�g
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _normalStageteleportPos.position;
            print($"{player.name} ���m�[�}���X�e�[�W�Ƀe���|�[�g���܂���");
        }
        Debug.Log($"<color=red>�X�J�C�{�b�N�X</color>{_normalStageSkyBox}");
        RenderSettings.skybox = _normalStageSkyBox;
        _sound.ProduceSE(_audioSource,_audioClip,1,1,0);

    }

    private void BossTeleportAllPlayers()
    {
        // �{�X�X�e�[�W�Ƀe���|�[�g
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _bossStageteleportPos.position;
            print($"{player.name} ���{�X�X�e�[�W�ɓr���Q���Ƃ��ăe���|�[�g���܂���");

        }
        RenderSettings.skybox = _bossStageSkyBox;
        _sound.ProduceSE(_audioSource, _audioClip, 1, 1, 0);
    }


    private void HandleAllEnemiesDefeated()
    {
        RPC_AllEnemiesDefeated();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_AllEnemiesDefeated()
    {
        ClearNormalStage = true;
        print("�G�S�ł̒ʒm���󂯎��܂���" + ClearNormalStage);
    }
}
