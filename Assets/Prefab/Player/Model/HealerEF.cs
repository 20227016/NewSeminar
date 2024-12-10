using System.Collections;
using UnityEngine;

public class HealerEF : MonoBehaviour
{
    [SerializeField]
    private float scaleDuration = 1f; // スケール変更の所要時間
    [SerializeField]
    private Vector3 targetScale = new Vector3(2f, 2f, 2f); // 最終的なスケール
    [SerializeField]
    private GameObject effect; // エフェクト（Particle System）
    [SerializeField]
    private GameObject _player;

    public void StaetEF()
    {
        Debug.Log("StartEF 呼ばれました");
        StartCoroutine(ScaleEffect());
    }


    private IEnumerator ScaleEffect()
    {
        effect.SetActive(true);
        Debug.Log("スケール変更開始");
        Vector3 initialScale = effect.transform.localScale;
        float timeElapsed = 0f;

        while (timeElapsed < scaleDuration)
        {
            effect.transform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        effect.transform.localScale = targetScale;
        Debug.Log("スケール変更完了");
    }
}
