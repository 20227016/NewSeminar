using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("パッシブによるガード時の防御力上昇率( * 防御力)")]
    private float _guardMultiplier = 2.0f;

    // ガードフラグ
    private bool _isGuard = false;

    // 元の防御力を保持
    private float _originalDefensePower;

    public void Passive(CharacterBase characterBase)
    {
        Debug.Log("タンクのパッシブ");

        // 初回実行時に元の防御力を記録
        if (!_isGuard && _originalDefensePower == 0)
        {
            _originalDefensePower = characterBase._characterStatusStruct._defensePower;
        }

        // 状態に応じて防御力を変更
        if (!_isGuard)
        {
            // 防御力を上昇
            characterBase._characterStatusStruct._defensePower *= _guardMultiplier;
        }
        else
        {
            // 防御力を元に戻す
            characterBase._characterStatusStruct._defensePower = _originalDefensePower;
        }

        // 状態を切り替える
        _isGuard = !_isGuard;
    }
}
