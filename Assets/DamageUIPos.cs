using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUIPos : MonoBehaviour
{

    RectTransform _rectTransform = null;

    public Transform _target = null;

    [SerializeField]
    private Vector2 offset = Vector2.zero;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _target.position) + offset;
    }
}
