using Fusion;
using UnityEngine;

public struct PlayerNetworkInput : INetworkInput
{
    public Vector2 MoveDirection;
    public bool IsRunning;
    public bool IsAttackLight;
    public bool IsAttackStrong;
    public bool IsAvoidance;
    public bool IsTargetting;
    public bool IsSkill;
    public bool IsResurrection;
}
