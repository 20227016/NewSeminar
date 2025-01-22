using Fusion;
using UnityEngine;

public class PlayerAttackTest : NetworkBehaviour
{
    [SerializeField] private float raycastDistance = 10f; // Raycast�̋���

    private int health = 100;

    // ����������
    public override void Spawned()
    {
        base.Spawned();

        Debug.Log("a");
        // �I�u�W�F�N�g���T�[�o�[�ɂ���ĊǗ�����Ă��邩�ǂ������`�F�b�N
        if (HasStateAuthority)
        {
            // �T�[�o�[���ł̂ݎ��s����鏉����
            health = 100;
            Debug.Log("Player spawned with health: " + health);
        }
        else
        {
            // �N���C�A���g���ł̏������i�K�v�ɉ����āj
            Debug.Log("Player object is controlled by another player.");
        }
    }

    private void Update()
    {
        // if (!Object.HasInputAuthority) return; // ���[�J���v���C���[�̂ݏ���

        if (Input.GetMouseButtonDown(1)) // �E�N���b�N�ōU��
        {
            RaycastHit hit;

            // �v���C���[�̑O���������擾�iplayerTransform�̓v���C���[��Transform�j
            Vector3 forwardDirection = transform.forward;

            // �v���C���[�̑O����Ray�𔭎�
            Ray ray = new Ray(transform.position, forwardDirection); // �v���C���[�̈ʒu����O���ɔ���

            // Raycast�����s
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                var enemyHealth = hit.collider.GetComponent<EnemyNetworkManager>();
                if (enemyHealth != null)
                {
                    // Raycast�Ńq�b�g�����G�������ꍇ
                    Debug.Log($"Hit enemy: {enemyHealth.gameObject.name}");

                    // �T�[�o�[�ɍU�����������N�G�X�g
                    RPC_AttackEnemy(enemyHealth, 20);
                }

                // Raycast�����������ʒu��\��
                Debug.Log($"Ray hit at: {hit.point}");
            }

            // Raycast�����o���i�V�[���r���[��Ray��`��j
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red, 2f); // 2�b�ԕ\��
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_AttackEnemy(EnemyNetworkManager enemy, int damage)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // �T�[�o�[���Ń_���[�W����
        }
    }
}
