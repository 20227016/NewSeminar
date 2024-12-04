using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPassive : MonoBehaviour, IPassive
{
    public void Passive(CharacterBase characterBase)
    {
        Debug.Log("タンクのパッシブ");
    }
}
