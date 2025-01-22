using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public GameObject playerPrefab; // �v���C���[�̃v���n�u
    private NetworkRunner _networkRunner; // NetworkRunner�̎Q��

    private void Start()
    {
        // NetworkRunner�̎Q�Ƃ��擾
        _networkRunner = FindObjectOfType<NetworkRunner>();

        // �T�[�o�[���ł̂ݎ��s
        if (Object.HasStateAuthority) // �T�[�o�[����������ꍇ
        {
            // �T�[�o�[�ŃI�u�W�F�N�g���X�|�[������
            SpawnPlayerObject();
        }
    }

    private void SpawnPlayerObject()
    {
        // NetworkRunner�����݂��Ă��邱�Ƃ��m�F
        if (_networkRunner != null)
        {
            // �v���C���[�I�u�W�F�N�g���C���X�^���X��
            GameObject playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            // �T�[�o�[���ŃX�|�[��
            _networkRunner.Spawn(playerObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("NetworkRunner is not found in the scene.");
        }
    }
}
