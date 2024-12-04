using Fusion;

/// <summary>
/// �l�b�g���[�N�Ŏg�p����ϐ��̊Ǘ�
/// ���݂̖���
/// �ERoomInfo�Ō��݂̕����̐l����CurrentPlayerCount�ɑ������(�����ϐ�)
/// </summary>
public class RoomInfo
{

    [Networked] public string RoomName { get; private set; }
    [Networked] public int CurrentPlayerCount { get; set; } // ���݂̃v���C���[��[�l�b�g���[�N��œ���]
    [Networked] public int MaxPlayerCount { get; private set; } // �ő�v���C���[��[�l�b�g���[�N��œ���]

    public RoomInfo(string roomName, int maxPlayerCount)
    {
        RoomName = roomName;
        MaxPlayerCount = maxPlayerCount;
        CurrentPlayerCount = 0; // �����l��
    }

    public void UpdatePlayerCount(int newPlayerCount)
    {
        CurrentPlayerCount = newPlayerCount; // �v���C���[�����X�V����
    }
}