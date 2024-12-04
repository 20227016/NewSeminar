
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Fusion;

/// <summary>
/// MenuOperation.cs
/// クラス説明
/// クリックしたときに呼び出す
///
/// 作成日: 10/7
/// 作成者: 高橋光栄
/// </summary>
public class ClickEventCall : MonoBehaviour
{
    [SerializeField, Tooltip("カメラ格納用")]
    private Camera _camera = default;

    [SerializeField, Tooltip("UI用Raycaster")]
    private GraphicRaycaster _raycaster = default;

    [SerializeField, Tooltip("イベントシステム")]
    private EventSystem _eventSystem = default;

    RaycastHit _hit = default;

    private string _roomName;
    private NetworkRunner _runner;
    private List<SessionInfo> _sessionList;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // まず3Dオブジェクトをクリックするか確認
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit))
            {
                print("3Dオブジェクトをクリック");
                // クリックした際にメソッド実行
                ClickEvent(_roomName,_runner,_sessionList);
            }
            else
            {
                // UI要素をクリックしたか確認
                CheckUIElementClick(_roomName, _runner, _sessionList);
            }
        }
    }

    /// <summary>
    /// クリック処理
    /// </summary>
    private void ClickEvent(string roomName, NetworkRunner runner, List<SessionInfo> sessionList)
    {
        print("クリック処理実行");
        // クリック専用インターフェースを保持しているオブジェクトのみ、クリックイベントを実行する
        IClickEvent clickable = _hit.collider.gameObject.GetComponent<IClickEvent>();

        if (clickable != null)
        {
            clickable.VoidClickEventAction();
            clickable.StringClickEventAction(roomName);
            clickable.NetworkClickEventAction(runner, sessionList);
        }
    }

    /// <summary>
    /// UI要素がクリックされたか確認する
    /// </summary>
    private void CheckUIElementClick(string roomName, NetworkRunner runner, List<SessionInfo> sessionList)
    {
        PointerEventData pointerData = new PointerEventData(_eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            foreach (var result in results)
            {
                IClickEvent clickableUI = result.gameObject.GetComponent<IClickEvent>();
                if (clickableUI != null)
                {
                    clickableUI.VoidClickEventAction();
                    clickableUI.StringClickEventAction(roomName);
                    clickableUI.NetworkClickEventAction(runner, sessionList);
                }
            }
        }
    }
}