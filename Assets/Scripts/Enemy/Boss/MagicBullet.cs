using UnityEngine;

/// <summary>
/// MagicBullet.cs
/// ���@�w����]������
/// �쐬��: 11/17
/// �쐬��: �Έ䒼�l 
/// </summary>
public class MagicBullet : BaseEnemy
{
    private Vector3 _targetScale = new Vector3(5f, 5f, 5f); // �ŏI�I�ȑ傫��
    private float _scaleSpeed = 2f; // �傫���Ȃ鑬�x
    private float _moveSpeed = 5f; // ��ԑ��x

    private float _currentTimer = 0f; // ���ݎ���
    private float _displayTime = 5f; // �\������

    private bool isMoving = false; // �ړ���ԃt���O

    private void Update()
    {
        // �܂��ڕW�T�C�Y�ɒB���Ă��Ȃ��ꍇ�A���X�ɑ傫������
        if (!isMoving)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, _targetScale, _scaleSpeed * Time.deltaTime);

            // �ڕW�T�C�Y�ɒB������ړ����J�n
            if (transform.localScale == _targetScale)
            {
                isMoving = true;
            }
        }
        else
        {
            // �����Ă�������Ɉړ�
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;

            // �\�����Ԃ��v��
            _currentTimer += Time.deltaTime;

            // ��莞�Ԍ�ɔ�\��
            if (_currentTimer >= _displayTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// HP��0�ȉ��ɂȂ�����Ă΂�鏈��(Base�Q��)
    /// </summary>
    protected override void OnDeath()
    {

    }
}
