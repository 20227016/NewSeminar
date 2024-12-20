using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayEffect
{
    void PlayEffect(ParticleSystem particleSystem, Vector3 playPosition);
}
