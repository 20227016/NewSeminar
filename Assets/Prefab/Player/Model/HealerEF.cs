using System.Collections;
using UnityEngine;

public class HealerEF : MonoBehaviour
{
    [SerializeField]
    private float scaleDuration = 1f; // �X�P�[���ύX�̏��v����
    [SerializeField]
    private Vector3 targetScale = new Vector3(2f, 2f, 2f); // �ŏI�I�ȃX�P�[��
    [SerializeField]
    private GameObject effect; // �G�t�F�N�g�iParticle System�j
    [SerializeField]
    private GameObject _player;

    public void StaetEF()
    {
        Debug.Log("StartEF �Ă΂�܂���");
        StartCoroutine(ScaleEffect());
    }


    private IEnumerator ScaleEffect()
    {
        effect.SetActive(true);
        Debug.Log("�X�P�[���ύX�J�n");
        Vector3 initialScale = effect.transform.localScale;
        float timeElapsed = 0f;

        while (timeElapsed < scaleDuration)
        {
            effect.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        effect.transform.localScale = targetScale;
        Debug.Log("�X�P�[���ύX����");
    }
}
