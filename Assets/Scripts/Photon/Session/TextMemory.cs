using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TextMemory : NetworkBehaviour
{

    [Networked,Tooltip("���g�������Ă���Ƃ��Ƀe�L�X�g���X�V")]
    public string Character { get; set; } = "";

    /// <summary>
    /// �����̃e�L�X�g�R���|�[�l���g
    /// </summary>
    private Text _text = default;

    /// <summary>
    /// �X�|�[�������Ƃ��ɌĂяo�����
    /// </summary>
    public override void Spawned()
    {

        this.TryGetComponent<Text>(out _text);
        if(_text == null)
        {

            Debug.LogError($"{this.gameObject}�̂Ƀe�L�X�g�R���|�[�l���g�������Ă��܂���");

        }
        // ���g���Ȃ��Ƃ�
        if(Character == "")
        {

            // ���g������Ƃ�
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
    /// �e�L�X�g�̍X�V
    /// </summary>
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_TextUpdate()
    {

        _text.text = Character;

    }

}
