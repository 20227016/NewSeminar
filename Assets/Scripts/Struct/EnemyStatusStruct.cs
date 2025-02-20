using UnityEngine;

[System.Serializable]
public struct EnemyStatusStruct
{

    [Header("ÅåHP")]
    public int _maxHp; // ÅåHP
    [Header("»İHP")]
    public int _hp; // »İHP
    [Header("UÍ")]
    public int _attackPower; // UÍ
    [Header("U¬x")]
    public int _attackPowerSpeed; // U¬x
    [Header("Ú®¬x")]
    public int _moveSpeed; // Ú®¬x
    [Header("häÍ")]
    public float _defensePercentage; // häÍ
    [Header("UN[^C")]
    public int _attackCoolTime; // UN[^C
    [Header("Ux^C")]
    public int _attackDelayTime; // Ux^C
    [Header("_E^C")]
    public int _downedTime; // _E^C

}