using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayEffect
{
    void RPC_PlayEffect(ParticleSystem particleSystem, Vector3 playPosition);

    void RPC_LoopEffect(ParticleSystem particleSystem, Vector3 playPosition, float effectDuration);
}
