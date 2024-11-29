
/// <summary>
/// NormalCharacter.cs
/// クラス説明
/// ノーマルキャラクター
///
/// 作成日: 9/3
/// 作成者: 山田智哉
/// </summary>
public class NormalCharacter : CharacterBase
{

    public override void Skill(CharacterBase characterBase, float skillTime, float skillCoolTime)
    {
        _currentSkillPoint.Value = 0f;
        _skill.Skill(this, skillTime, skillCoolTime);
    }
}