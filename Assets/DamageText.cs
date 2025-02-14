using UnityEngine;
using TMPro;

/// <summary>
/// エネミーの受けたダメージを表示する
/// </summary>
public class DamageText : MonoBehaviour
{
    [SerializeField] 
    private GameObject _damageTextPrefab; // ダメージ表示用のプレハブ

    [SerializeField, Tooltip("テキストが消えるまでの時間")]
    private float _damageTrueTime = default;

    [SerializeField, Tooltip("テキストのランダムなズレ幅")]
    private Vector2 _randomOffsetRange = new Vector2(30f, 20f); // X, Y方向のズレの範囲

    public void ShowDamage(int damage, Vector3 damagePos)
    {
        GameObject canvasObj = GameObject.Find("CanvasDamage");

        Canvas canvas = canvasObj.GetComponent<Canvas>();

        // テキストを生成する
        GameObject damageTextObj = Instantiate(_damageTextPrefab, canvas.transform);
        TextMeshProUGUI damageText = damageTextObj.GetComponent<TextMeshProUGUI>();

        // ダメージ量をテキストに書き込む
        damageText.text = damage.ToString(); 

        // **ワールド座標をスクリーン座標に変換**
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(damagePos);
        damageTextObj.transform.position = screenPosition;

        // **ランダムなズレを加える**
        float randomX = Random.Range(-_randomOffsetRange.x, _randomOffsetRange.x);
        float randomY = Random.Range(-_randomOffsetRange.y, _randomOffsetRange.y);
        screenPosition += new Vector2(randomX, randomY);

        // テキストの位置を設定
        damageTextObj.transform.position = screenPosition;

        Destroy(damageTextObj, _damageTrueTime);
    }

}
