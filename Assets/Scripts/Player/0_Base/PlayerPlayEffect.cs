using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlayEffect : IPlayEffect
{
    public void PlayEffect(ParticleSystem particleSystem, Vector3 playPosition)
    {
        particleSystem.transform.position = playPosition;

        particleSystem.gameObject.SetActive(true);
    }
}
