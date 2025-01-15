using UnityEngine;
using System.Collections;

public class BossCamera : MonoBehaviour
{
    // �ݒ肷��ʒu�Ɖ�]
    private Vector3 targetPosition = new Vector3(-18, 18, 0);
    private Vector3 targetRotation = new Vector3(30, 70, 0);

    Animator _animator = default;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        // 10�b��ɐݒ肷��R���[�`�����J�n
        StartCoroutine(ChangeTransformAfterDelay(10f));
    }

    private IEnumerator ChangeTransformAfterDelay(float delay)
    {
        // �w�肳�ꂽ�b���҂�
        yield return new WaitForSeconds(delay);

        // �ʒu�Ɖ�]��ύX
        gameObject.transform.position = targetPosition;
        gameObject.transform.rotation = Quaternion.Euler(targetRotation);
    }

    private void AnimatorFinished()
    {
        _animator.enabled = false;
    }
}
