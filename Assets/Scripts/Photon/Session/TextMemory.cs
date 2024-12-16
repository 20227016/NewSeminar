using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TextMemory : NetworkBehaviour
{

    [Networked,Tooltip("中身が入っているときにテキストを更新")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 自分のテキストコンポーネント
    /// </summary>
    private Text _text = default;

    /// <summary>
    /// スポーンしたときに呼び出される
    /// </summary>
    public override void Spawned()
    {

        this.TryGetComponent<Text>(out _text);
        if(_text == null)
        {

            Debug.LogError($"{this.gameObject}のにテキストコンポーネントが入っていません");

        }
        // 中身があるとき
        if(Name != "")
        {

            _text.text = Name;


        }

    }

    /// <summary>
    /// テキストの更新
    /// </summary>
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_TextUpdate()
    {

        _text.text = Name;

    }

}
