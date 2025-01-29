using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("ƒpƒbƒVƒu‚É‚æ‚éƒK[ƒh‚Ì–hŒä—Íã¸—¦( * –hŒä—Í)")]
    private float _guardMultiplier = 2.0f;

    // ƒK[ƒhƒtƒ‰ƒO
    private bool _isGuard = false;

    // Œ³‚Ì–hŒä—Í‚ğ•Û
    private float _originalDefensePower;

    public void Passive(CharacterBase characterBase)
    {
        // ‰‰ñÀs‚ÉŒ³‚Ì–hŒä—Í‚ğ‹L˜^
        if (!_isGuard && _originalDefensePower == 0)
        {
            _originalDefensePower = characterBase._characterStatusStruct._defensePower;
        }

        // ó‘Ô‚É‰‚¶‚Ä–hŒä—Í‚ğ•ÏX
        if (!_isGuard)
        {
            // –hŒä—Í‚ğã¸
            characterBase._characterStatusStruct._defensePower *= _guardMultiplier;
        }
        else
        {
            // –hŒä—Í‚ğŒ³‚É–ß‚·
            characterBase._characterStatusStruct._defensePower = _originalDefensePower;
        }

        // ó‘Ô‚ğØ‚è‘Ö‚¦‚é
        _isGuard = !_isGuard;
    }
}
