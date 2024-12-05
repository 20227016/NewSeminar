using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// ���[�������i�[�X�|�i�[�i�����ɐ�������I�u�W�F�N�g�ɃN���C�A���g�Ƃ��Đ������z�X�g�Ƃ��Đ����������߂��j
/// ����������P�̃X�N���v�g�ŏ���������̂ŁAIf���ŃN���C�A���g�̎��̏����ƃz�X�g�̎��̏����𕪂���
/// �z�X�g
/// �l�b�g���[�N�Ǘ��S���i�X�e�[�g�I�[�\���e�B�[�������Ă���i���Ă�̂̓z�X�g�̂݁j�j
/// �l���Ǘ��ARPC�����A�Q�[���X�^�[�g
/// 
/// �N���C�A���g
/// �ύX���ɒʒm�𑗂�
/// 
/// ���[���}�l�[�W���[���Ȃ��Ȃ胋�[���}�l�[�W���[��RPC�������z�X�g��
/// 
/// 
/// </summary>
public class ParticipantsSpawner : MonoBehaviour, IParticipantsSpawner
{

    [SerializeField, Tooltip("Participants�̃v���n�u")]
    private GameObject _participantPrefab = default;

    /// <summary>
    /// �z�X�g�֐��������ʒm
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Spawner(StartGameArgs startGameArgs, NetworkRunner _networkRunner, RoomInfo preDefinedRoom)
    {

        print("�X�|�i�[�ւ悤����");
        try
        {

            // �w�肵���l�b�g���[�N���[�h�ł̃Q�[�����X�^�[�g������i�Z�b�V���������d�����Ă��铙�Ŏ��s����j
            StartGameResult result = await _networkRunner.StartGame(startGameArgs);
            // �Z�b�V����������������
            if (result.Ok)
            {

                Debug.Log($"���� {startGameArgs.SessionName} �ɐ���ɎQ�����܂����B");

            }
            else
            {

                Debug.LogError($"���� {startGameArgs.SessionName} �ɎQ���ł��܂���ł���: {result.ShutdownReason}");
                Debug.LogError($"�ڑ����������I�����܂�");
                await _networkRunner.Shutdown();
                return false;

            }
            // �Q���҃I�u�W�F�N�g
            NetworkObject participant = default;
            // �Q���҂̎Q���`�Ԃɂ�鏈��
            switch (_networkRunner.GameMode)
            {

                //�@�z�X�g
                case GameMode.Host:

                    // �Q���҂��z�X�g�Ƃ��Đ���
                    participant = _networkRunner.Spawn(_participantPrefab, Vector3.zero, Quaternion.identity, _networkRunner.LocalPlayer);
                    // �����Ɏ��s�����Ƃ�
                    if (participant == null)
                    {

                        Debug.LogError("�Q���҃I�u�W�F�N�g�̐����Ɏ��s");
                        return false;

                    }
                    Debug.Log("�z�X�g�Ƃ��ĕ����ɎQ�����܂����B");
                    if (participant.HasStateAuthority)
                    {

                        Debug.Log("�z�X�g�̌����擾�ɐ���");

                    }
                    else
                    {

                        Debug.LogError("�z�X�g�̌����擾�Ɏ��s");

                    }
                    // Room�����킽���C���^�[�t�F�[�X
                    IParticipantInfo iParticipantInfo = participant.GetComponent<IParticipantInfo>();
                    if (iParticipantInfo == null)
                    {

                        Debug.LogError("�z�X�g��Room����n�����߂̃C���^�[�t�F�[�X���Ȃ�");
                        return false;

                    }
                    iParticipantInfo.SetRoomInfo(preDefinedRoom);

                    break;
                // �N���C�A���g
                case GameMode.Client:

                    // �Q���҂��N���C�A���g�Ƃ��Đ���
                    participant = _networkRunner.Spawn(_participantPrefab);
                    // �����Ɏ��s�����Ƃ�
                    if (participant == null)
                    {

                        Debug.LogError("�Q���҃I�u�W�F�N�g�̐����Ɏ��s");
                        return false;

                    }
                    Debug.Log("�N���C�A���g�Ƃ��ĕ����ɎQ�����܂����B");

                    break;

            }
            return true;

        }
        catch
        {

            Debug.LogError("�G���[");
            // �������s
            return false;


        }

    }

}
