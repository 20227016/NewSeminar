
using UnityEngine;

public interface IAttackLight
{
    /// <summary>
    /// ��U��
    /// </summary>
    void AttackLight(CharacterBase characterBase, float attackPower, float attackMultiplier, float delay, float range);

}