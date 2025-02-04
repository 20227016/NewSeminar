using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISound 
{

    public void ProduceSE(AudioSource audioSource,AudioClip audioClip ,float audioSpeed, float audioVolume);

}
