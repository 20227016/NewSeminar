using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayEffect
{
    void RPC_PlayEffect(NetworkObject particleSystem, Vector3 playPosition);

    void RPC_LoopEffect(NetworkObject particleSystem, Vector3 playPosition, float effectDuration);
}
