using UnityEngine;

public class DamageUI : MonoBehaviour
{

    [SerializeField]
    private GameObject _targets = default;

    [SerializeField]
    private GameObject _canvas = default;

    [SerializeField]
    private GameObject _damageUI = default;

    /// <summary>
    /// ダメージを与えたときに、敵を取得する
    /// その取得した敵の位置を取得し、そこからランダムをかけて、その位置から与えたダメージUIを表示させる
    /// </summary>
    private void DamagePopUI()
    {
        var obj = new GameObject("Target");
        var ui = Instantiate(_damageUI);

        obj.transform.SetParent(_targets.transform);
        ui.transform.SetParent(_canvas.transform);

        ui.GetComponent<DamageUIPos>()._target = obj.transform;

        ui.SetActive(true);

        var circlePos = Random.insideUnitCircle * 1.2f;
        obj.transform.position = transform.position + Vector3.up * Random.Range(3.0f, 4.0f) + new Vector3(circlePos.x,0,circlePos.y);
        ui.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main,obj.transform.position);

        Destroy(obj,5f);
        Destroy(ui, 5f);

    }


}
