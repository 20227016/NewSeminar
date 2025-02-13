using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDisplay : MonoBehaviour
{
    private void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation;
    }
}
