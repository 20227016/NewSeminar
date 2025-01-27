using UnityEngine;
using System.Collections;

public class MovieCamera : MonoBehaviour
{
    private Camera _camera = default;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        // 10�b��ɐݒ肷��R���[�`�����J�n
        StartCoroutine(ChangeTransformAfterDelay(10f));
    }

    private IEnumerator ChangeTransformAfterDelay(float delay)
    {
        // �w�肳�ꂽ�b���҂�
        yield return new WaitForSeconds(delay);

        _camera.enabled = false;
    }
}
