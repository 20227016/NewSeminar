using Fusion;

/// <summary>
/// �l�b�g���[�N�Ŏg�p����ϐ��̊Ǘ�
/// ���݂̖���
/// �ERoomInfo�Ō��݂̕����̐l����CurrentPlayerCount�ɑ������(�����ϐ�)
/// </summary>
public class RoomInfo
{

    [Networked] public string RoomName { get; private set; }
    [Networked] public int CurrentParticipantCount { get; set; } // ���݂̃v���C���[��[�l�b�g���[�N��œ���]
    [Networked] public int MaxParticipantCount { get; private set; } // �ő�v���C���[��[�l�b�g���[�N��œ���]

    public RoomInfo(string roomName, int maxPlayerCount)
    {
        RoomName = roomName;
        MaxParticipantCount = maxPlayerCount;
        CurrentParticipantCount = 0; // �����l��
    }

    public void UpdateParticipantCount(int newParticipantCount)
    {
        CurrentParticipantCount = newParticipantCount; // �v���C���[�����X�V����
    }
}