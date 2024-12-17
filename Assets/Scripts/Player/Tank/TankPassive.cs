using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    [SerializeField, Tooltip("ƒpƒbƒVƒu‚É‚æ‚éƒK[ƒh‚Ì–hŒäã¸—¦( * –hŒä—Í)")]
    private float _guardMultiplier = 2.0f;

    // Às‰ñ”‚ğŠÇ—
    private int _executionCount = 0;

    // Œ³‚Ì–hŒä—Í‚ğ•Û
    private float _originalDefensePower;

    public void Passive(CharacterBase characterBase)
    {
        Debug.Log("ƒ^ƒ“ƒN‚ÌƒpƒbƒVƒu");

        // ‰‰ñÀs‚ÉŒ³‚Ì–hŒä—Í‚ğ‹L˜^
        if (_executionCount == 0)
        {
            _originalDefensePower = characterBase._characterStatusStruct._defensePower;
        }

        _executionCount++;

        // Šï”‰ñ–Ú: –hŒä—Í‚ğã¸
        if (_executionCount % 2 == 1)
        {
            characterBase._characterStatusStruct._defensePower *= _guardMultiplier;
            Debug.Log($"–hŒä—Í‚ğã¸: {characterBase._characterStatusStruct._defensePower}");
        }
        // ‹ô”‰ñ–Ú: –hŒä—Í‚ğŒ³‚É–ß‚·
        else
        {
            characterBase._characterStatusStruct._defensePower = _originalDefensePower;
            Debug.Log($"–hŒä—Í‚ğŒ³‚É–ß‚·: {characterBase._characterStatusStruct._defensePower}");
        }

    }
}
