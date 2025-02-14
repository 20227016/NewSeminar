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

        Destroy(damageText, _damageTrueTime);
    }

}
