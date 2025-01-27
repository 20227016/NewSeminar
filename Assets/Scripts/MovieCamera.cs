using UnityEngine;
using System.Collections;

public class MovieCamera : MonoBehaviour
{
    private Camera _camera = default;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        // 10秒後に設定するコルーチンを開始
        StartCoroutine(ChangeTransformAfterDelay(10f));
    }

    private IEnumerator ChangeTransformAfterDelay(float delay)
    {
        // 指定された秒数待つ
        yield return new WaitForSeconds(delay);

        _camera.enabled = false;
    }
}
