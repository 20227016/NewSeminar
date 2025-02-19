using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("ƒXƒLƒ‹‚É‚æ‚é–hŒä—Íã¸”{—¦(* –hŒä—Í)")]
    private float _defenceMaltiplier = 2f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        float originalDefence = characterBase._characterStatusStruct._defensePower;

        characterBase._characterStatusStruct._defensePower *= _defenceMaltiplier;

        // skillTimeŒã‚ÉŒ³‚Ì–hŒä—Í‚É–ß‚·
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._defensePower = originalDefence;
            });
    }
}
