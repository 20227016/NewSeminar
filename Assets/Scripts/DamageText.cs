using UnityEngine;
using TMPro;

/// <summary>
/// �G�l�~�[�̎󂯂��_���[�W��\������
/// </summary>
public class DamageText : MonoBehaviour
{
    [SerializeField] 
    private GameObject _damageTextPrefab; // �_���[�W�\���p�̃v���n�u

    [SerializeField, Tooltip("�e�L�X�g��������܂ł̎���")]
    private float _damageTrueTime = default;

    [SerializeField, Tooltip("�e�L�X�g�̃����_���ȃY����")]
    private Vector2 _randomOffsetRange = new Vector2(30f, 20f); // X, Y�����̃Y���͈̔�

    public void ShowDamage(int damage, Vector3 damagePos)
    {
        GameObject canvasObj = GameObject.Find("CanvasDamage");

        Canvas canvas = canvasObj.GetComponent<Canvas>();

        // �e�L�X�g�𐶐�����
        GameObject damageTextObj = Instantiate(_damageTextPrefab, canvas.transform);
        TextMeshProUGUI damageText = damageTextObj.GetComponent<TextMeshProUGUI>();

        // �_���[�W�ʂ��e�L�X�g�ɏ�������
        damageText.text = damage.ToString(); 

        // **���[���h���W���X�N���[�����W�ɕϊ�**
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(damagePos);
        damageTextObj.transform.position = screenPosition;

        // **�����_���ȃY����������**
        float randomX = Random.Range(-_randomOffsetRange.x, _randomOffsetRange.x);
        float randomY = Random.Range(-_randomOffsetRange.y, _randomOffsetRange.y);
        screenPosition += new Vector2(randomX, randomY);

        // �e�L�X�g�̈ʒu��ݒ�
        damageTextObj.transform.position = screenPosition;

        Destroy(damageTextObj, _damageTrueTime);
    }

}
