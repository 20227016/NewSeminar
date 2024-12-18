using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TextMemory : NetworkBehaviour
{

    [Networked,Tooltip("中身が入っているときにテキストを更新")]
    public string Character { get; set; } = "";

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
        // 中身がないとき
        if(Character == "")
        {

            // 中身があるとき
            if (_text.text != "")
            {

                Character = _text.text;

            }

        }
        else
        {

            _text.text = Character;

        }

    }

    /// <summary>
    /// テキストの更新
    /// </summary>
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_TextUpdate()
    {

        _text.text = Character;

    }

}
