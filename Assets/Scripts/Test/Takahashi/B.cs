
using UnityEngine;
using System.Collections;

public class B : MonoBehaviour
{

    private CharacterStatusStruct _characterStatusStruct;

    private IMove _move;
    private IAvoidance _avoidance;
    private void Start()
    {
        // 最初は歩く
        _move = GetComponent<PlayerWalk>();
        _avoidance = GetComponent<PlayerAvoidance>();

        // null チェック
        if (_move == null)
        {
            Debug.LogError("Runコンポーネントがアタッチされていません");
        }

        if (_avoidance == null)
        {
            Debug.LogError("PlayerAvoidanceコンポーネントがアタッチされていません");
        }
    }

    private void Update()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


    }
}