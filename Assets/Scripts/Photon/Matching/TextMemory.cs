using Fusion;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TextMemory : NetworkBehaviour
{

    [Networked,Tooltip("���g�������Ă���Ƃ��Ƀe�L�X�g���X�V")]
    public String Character { get; set; } = "";

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
        // ���g������Ƃ�
        if(Character != "")
        {

            _text.text = Character;


        }

    }

    /// <summary>
    /// �e�L�X�g�̍X�V
    /// </summary>
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_TextUpdate()
    {

        _text.text = Character;

    }

}
