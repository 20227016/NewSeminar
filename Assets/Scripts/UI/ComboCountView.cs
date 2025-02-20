using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class ComboCountView
{

    public void UpdateText(float value, TextMeshProUGUI text, float initializeValue)
    {
        if (text == null)
        {
            return;
        }
        text.text = value.ToString();
        text.gameObject.SetActive(value > initializeValue);
    }

}