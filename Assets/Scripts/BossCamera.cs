using UnityEngine;
using System.Collections;

public class BossCamera : MonoBehaviour
{
    // 設定する位置と回転
    private Vector3 targetPosition = new Vector3(-18, 18, 0);
    private Vector3 targetRotation = new Vector3(30, 70, 0);

    Animator _animator = default;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        // 10秒後に設定するコルーチンを開始
        StartCoroutine(ChangeTransformAfterDelay(10f));
    }

    private IEnumerator ChangeTransformAfterDelay(float delay)
    {
        // 指定された秒数待つ
        yield return new WaitForSeconds(delay);

        // 位置と回転を変更
        gameObject.transform.position = targetPosition;
        gameObject.transform.rotation = Quaternion.Euler(targetRotation);
    }

    private void AnimatorFinished()
    {
        _animator.enabled = false;
    }
}
