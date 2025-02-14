using UnityEngine;

[System.Serializable]
public struct EnemyStatusStruct
{

    [Header("�ő�HP")]
    public int _maxHp; // �ő�HP
    [Header("����HP")]
    public int _hp; // ����HP
    [Header("�U����")]
    public int _attackPower; // �U����
    [Header("�U�����x")]
    public int _attackPowerSpeed; // �U�����x
    [Header("�ړ����x")]
    public int _moveSpeed; // �ړ����x
    [Header("�h���")]
    public float _defensePercentage; // �h���
    [Header("�U���N�[���^�C��")]
    public int _attackCoolTime; // �U���N�[���^�C��
    [Header("�U���x���^�C��")]
    public int _attackDelayTime; // �U���x���^�C��
    [Header("�_�E���^�C��")]
    public int _downedTime; // �_�E���^�C��

}