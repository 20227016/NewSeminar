using UnityEngine;

/// <summary>
/// TankCharacter.cs
/// クラス説明
///
///
/// 作成日: 9/13
/// 作成者: 山田智哉
/// </summary>
public class TankCharacter : CharacterBase
{

    protected override void AvoidanceAction(Transform transform)
    {
        _passive.Passive(this);
    }
}