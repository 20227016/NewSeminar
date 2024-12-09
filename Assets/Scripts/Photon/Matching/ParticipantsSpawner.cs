using Fusion;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public async Task<bool> Spawner(NetworkRunner networkRunner, RoomInfo preDefinedRoom)
    {

        print("�X�|�i�[�ւ悤����");

        // �Z�b�V�������@
        StartGameArgs startGameArgs = default;
        // �Z�b�V�������s����
        StartGameResult result = default;
        // �T�[�o�[���iArgs�������j
        startGameArgs = new StartGameArgs()
        {

            // �Q�[�����[�h
            GameMode = GameMode.Host,
            // �Z�b�V������
            SessionName = "Room",
            // �l�b�g���[�N��ł̃V�[���J�ړ����H
            SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            // �Z�b�V�����쐬���ɁA���݂̃V�[���ɒu���ꂽ�V�[���I�u�W�F�N�g���X�|�[������
            Scene = SceneManager.GetActiveScene().buildIndex

        };
        Debug.Log($"�z�X�g�@��{startGameArgs}�@�̐ݒ�ŎQ���������s");
        // �w�肵���l�b�g���[�N���[�h�ł̃Q�[�����X�^�[�g������i�Z�b�V���������d�����Ă��铙�Ŏ��s����j
        result = await networkRunner.StartGame(startGameArgs);
        Debug.Log($"���ʁF{result}");
        // �Z�b�V����������������
        if (result.Ok)
        {

            Debug.Log($"�z�X�g�@�Ƃ��ĕ��� {startGameArgs.SessionName} �ɐ���ɎQ�����܂����B");

        }
        else
        {

            Debug.LogError($"�z�X�g�@�Ƃ��ĕ��� {startGameArgs.SessionName} �ɎQ���ł��܂���ł���: {result.ShutdownReason}");
            Debug.LogError($"�Z�b�V�������������I�����܂�");
            await networkRunner.Shutdown();
            // �T�[�o�[���iArgs�������j
            startGameArgs = new StartGameArgs()
            {

                // �Q�[�����[�h
                GameMode = GameMode.Client,
                // �Z�b�V������
                SessionName = "Room",
                // �l�b�g���[�N��ł̃V�[���J�ړ����H
                SceneManager = this.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                // �Z�b�V�����쐬���ɁA���݂̃V�[���ɒu���ꂽ�V�[���I�u�W�F�N�g���X�|�[������
                Scene = SceneManager.GetActiveScene().buildIndex

            };
            Debug.Log($"�N���C�A���g�@��{startGameArgs}�@�̐ݒ�ŎQ���������s");

            // �w�肵���l�b�g���[�N���[�h�ł̃Q�[�����X�^�[�g������i�Z�b�V���������d�����Ă��铙�Ŏ��s����j
            result = await networkRunner.StartGame(startGameArgs);
            Debug.Log($"���ʁF{result}");
            if (result.Ok)
            {

                Debug.Log($"�N���C�A���g�@�Ƃ��ĕ��� {startGameArgs.SessionName} �ɐ���ɎQ�����܂����B");

            }
            else
            {

                Debug.LogError($"�N���C�A���g�Ƃ��ā@���� {startGameArgs.SessionName} �ɎQ���ł��܂���ł���: {result.ShutdownReason}");
                Debug.LogError($"�ڑ����������I�����܂�");
                await networkRunner.Shutdown();
                return false;

            }

        }
        if (startGameArgs.GameMode == GameMode.Client)
        {

            Debug.Log($"��������");
            Debug.Log($"�Z�b�V������ԁF{networkRunner.SessionInfo}");
            Debug.Log($"���݂̎Q����: {preDefinedRoom.CurrentParticipantCount}");
            Debug.Log($"�ő�Q���l��: {preDefinedRoom.MaxParticipantCount}");
            if (preDefinedRoom.CurrentParticipantCount == preDefinedRoom.MaxParticipantCount + 1)
            {

                Debug.LogError($"�l�����ő�ɒB���Ă��邽�ߕ��� {startGameArgs.SessionName} �ɎQ���ł��܂���ł���: {result.ShutdownReason}");
                await networkRunner.Shutdown();
                return false;

            }
            Debug.Log($"�l���ɗ]�T�����邽�ߓ����ł��܂�");

        }

        Debug.Log($"�z�X�g�E�N���C�A���g�e����");
        // �Q���҃I�u�W�F�N�g
        NetworkObject participant = default;
        // �Q���҂̎Q���`�Ԃɂ�鏈��
        switch (networkRunner.GameMode)
        {

            //�@�z�X�g
            case GameMode.Host:

                // �Q���҂��z�X�g�Ƃ��Đ���
                participant = networkRunner.Spawn(_participantPrefab, Vector3.zero, Quaternion.identity, networkRunner.LocalPlayer);
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
                    return false;

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
                participant = networkRunner.Spawn(_participantPrefab);
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
}
