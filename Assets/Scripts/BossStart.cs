using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using System.Linq;
using UniRx;

public class BossStart : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 40f; // ���m�͈�
    [SerializeField] private LayerMask _playerLayer; // �v���C���[�����m���邽�߂̃��C���[�}�X�N

    private bool isPlayerNearby = false; // �v���C���[���͈͓��ɂ��邩�ǂ���

    private GameObject _text = default;
    private GameObject _bar = default;

    private PlayableDirector _playableDirector; // Timeline��PlayableDirector

    private void Awake()
    {
        // MovieCamera
        // RoarEffects
        // Magic circle Enemy
        _playableDirector = GetComponent<PlayableDirector>();

    }
    private void Update()
    {
        if (!isPlayerNearby)
        {
            // �v���C���[���͈͓��ɂ��邩�ǂ������`�F�b�N
            CheckForPlayers();
            return;
        }
    }

    /// <summary>
    /// �v���C���[���͈͓��ɂ��邩�ǂ��������m
    /// </summary>
    private void CheckForPlayers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);

        if (hitColliders.Length > 0)
        {
            Debug.Log($"<color=red> �{�XUI�\�� </color>");
            _text = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BossNameText");
            _bar = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "BossHP_Bar");
            if (_text != null)
            {
                _text.SetActive(false);
            }
            if (_bar != null)
            {
                _bar.SetActive(false);
            }
            // �v���C���[��1�l�ȏ�͈͓��ɂ���
            isPlayerNearby = true;
            _playableDirector.Play();
            _text.SetActive(true);
            _bar.SetActive(true);
        }
    }

    /// <summary>
    /// ���m�͈͂��V�[���r���[�ŉ���
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
