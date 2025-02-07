using System.Collections.Generic;
using UnityEngine;

public class BossStageTransfer : MonoBehaviour
{
    // �e���|�[�g���̃v���C���[���Ǘ����郊�X�g
    private List<GameObject> _playersInPortal = new List<GameObject>();

    // �K�v�ȃv���C���[��
    [ Tooltip("�m�[�}���X�e�[�W�Ƀe���|�[�g���邽�߂ɕK�v�Ȑl��")]
    public int BossStageRequiredPlayers { get; set; }// �K�v�ȃv���C���[��

    [SerializeField, Tooltip("�{�X�X�e�[�W�̃e���|�[�g���W")]
    private Transform _bossTeleportPos = default;

    [SerializeField, Header("�{�X�X�e�[�W�̃X�J�C�{�b�N�X")]
    private Material _bossStageSkyBox = default;

    [SerializeField, Header("���g�̃T�E���h�\�[�X")]
    private AudioSource _audioSource = default;
    [SerializeField, Header("�e���|�[�g�̉�")]
    private AudioClip _audioClip = default;

    private ISound _sound = new SoundManager();

    /// <summary>
    /// �]���|�[�^���Ƀv���C���[���������Ƃ��Ƀ��X�g�ɒǉ�����
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !_playersInPortal.Contains(collider.gameObject))
        {
            _playersInPortal.Add(collider.gameObject);
            print($"�v���C���[�����m�B���݂̐l���� {_playersInPortal.Count} /{BossStageRequiredPlayers}�ł�");
        }

        // �K�v�l������������S�����e���|�[�g
        if (_playersInPortal.Count >= BossStageRequiredPlayers)
        {
            BossTeleportAllPlayers();
        }
    }

    private void BossTeleportAllPlayers()
    {
        // �{�X�X�e�[�W�Ƀe���|�[�g
        foreach (GameObject player in _playersInPortal)
        {

            player.transform.position = _bossTeleportPos.position;
            print($"{player.name} ���{�X�����Ƀe���|�[�g���܂���");
        }
        RenderSettings.skybox = _bossStageSkyBox;
        _sound.ProduceSE(_audioSource, _audioClip, 1, 1, 0);
    }
}
