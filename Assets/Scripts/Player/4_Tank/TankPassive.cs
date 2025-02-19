using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("パッシブによるガード時の防御力上昇量(+ 防御力)")]
    private float _guardDefencePower = 60f;

    // ガードフラグ
    private bool _isGuard = false;

    public void Passive(CharacterBase characterBase)
    {
        TankCharacter tankCharacter = characterBase.GetComponent<TankCharacter>();

        // 状態に応じて防御力を変更
        if (!_isGuard)
        {
            // ガード時防御力を適用
            tankCharacter.GuardDefencePower = _guardDefencePower;
        }
        else
        {
            // 元に戻す
            tankCharacter.GuardDefencePower = 0;
        }

        // 状態を切り替える
        _isGuard = !_isGuard;
    }
}
