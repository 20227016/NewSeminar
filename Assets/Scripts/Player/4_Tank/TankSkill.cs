using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TankSkill : MonoBehaviour, ISkill
{
    [SerializeField, Tooltip("ƒXƒLƒ‹‚É‚æ‚é–hŒä—Íã¸”{—¦")]
    private float _defenceMaltiplier = 2.0f;

    public void Skill(CharacterBase characterBase, float skillTime)
    {
        // Œ³‚ÌUŒ‚‘¬“x‚ð•ÛŽ
        float originalDefence = characterBase._characterStatusStruct._defensePower;

        // UŒ‚‘¬“x‚ðˆêŽž“I‚É•ÏX
        characterBase._characterStatusStruct._defensePower *= _defenceMaltiplier;

        // skillTimeŒã‚ÉŒ³‚ÌUŒ‚‘¬“x‚É–ß‚·
        Observable.Timer(TimeSpan.FromSeconds(skillTime))
            .Subscribe(_ =>
            {
                characterBase._characterStatusStruct._defensePower = originalDefence;
            });
    }
}
