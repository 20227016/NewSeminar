
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A.cs
/// クラス説明
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class A : MonoBehaviour
{

    [SerializeField]
    private bool _isAll = true;

    [SerializeField,Header("無視するレイヤー")]
    private LayerMask ignoreLayer = default;

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {

        BoxCastStruct boxCastStruct = default;
        // 自分の目の前から
        // 中心点
        boxCastStruct._originPos = this.transform.position + this.transform.localScale/2;
        // 半径（直径ではない）
        boxCastStruct._size = (transform.localScale - Vector3.forward * transform.localScale.z);
        boxCastStruct._size += Vector3.right * boxCastStruct._size.x * 2;
        boxCastStruct._size -= Vector3.one / 100;
        // 方向
        boxCastStruct._direction = transform.forward;
        // 距離
        boxCastStruct._distance = 5f;
        // 無視するレイヤー
        boxCastStruct._layerMask = ~ignoreLayer;
        if (Input.GetKeyDown(KeyCode.A))
        {

            _isAll = !_isAll;

        }
        if (_isAll)
        {

            RaycastHit[] hits = Search.BoxCastAll(boxCastStruct);
            if (hits.Length != 0)
            {

                foreach (RaycastHit hit in hits)
                {

                    print(hit.collider);

                }

            }

        }
        else
        {

            RaycastHit hit = Search.BoxCast(boxCastStruct);
            if (hit.collider)
            {

                print(hit.collider);

            }

        }
       
    }

}