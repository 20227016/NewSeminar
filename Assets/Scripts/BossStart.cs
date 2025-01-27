using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStart : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 40f; // ���m�͈�
    [SerializeField] private LayerMask _playerLayer; // �v���C���[�����m���邽�߂̃��C���[�}�X�N

    private bool isPlayerNearby = false; // �v���C���[���͈͓��ɂ��邩�ǂ���

    private GameObject _movieCamera = default;
    private GameObject _roar = default;
    private GameObject _circle = default;
    private GameObject _text = default;
    private GameObject _bar = default;

    private void Awake()
    {
        _movieCamera = GameObject.Find("MovieCamera");
        _movieCamera.SetActive(false);
        _roar = GameObject.Find("RoarEffects");
        _roar.SetActive(false);
        _circle = GameObject.Find("Magic circle Enemy");
        _circle.SetActive(false);

        //�@�{�X�����҂�
        StartCoroutine(Delay(5f));
    }

    /// <summary>
    /// ���S���
    /// </summary>
    private IEnumerator Delay(float fadeDuration)
    {
        yield return new WaitForSeconds(fadeDuration);

        _text = GameObject.Find("BossNameText");
        _text.SetActive(false);
        _bar = GameObject.Find("BossHP_Bar");
        _bar.SetActive(false);
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
            // �v���C���[��1�l�ȏ�͈͓��ɂ���
            isPlayerNearby = true;
            _movieCamera.SetActive(true);
            _roar.SetActive(true);
            _circle.SetActive(true);
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
