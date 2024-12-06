using UnityEngine;
using Fusion;
using UniRx;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField, Tooltip("��������G�l�~�[��Prefab")]
    private NetworkPrefabRef enemyPrefab;


    [SerializeField, Tooltip("��������G�l�~�[�̐�")]
    private int enemyCount = default;

    [SerializeField, Tooltip("�����G���A�̒��S")]
    private Transform spawnAreaCenter;

    [SerializeField, Tooltip("�����G���A�͈̔�")]
    private float spawnRadius = 10f;

    private NetworkRunner _runner;

    // �Q�[���J�n���ɃG�l�~�[�𐶐�����
    private void Start()
    {
        print("�X�^�[�g�͎��s����Ă��܂�");
        GameInitializer.Instance.OnEnemySpawnRequested
            .Subscribe(_ =>
            {
                Debug.Log("EnemySpawner: �G�l�~�[�X�|�[���J�n");
                SpawnerStart();
                GameInitializer.Instance.NetworkEnemySpawn();
            })
            .AddTo(this); // �I�u�W�F�N�g�j�����Ɏ�������

    }

    private async void SpawnerStart()
    {

        // NetworkRunner�𓮓I�ɐ���
        _runner = gameObject.AddComponent<NetworkRunner>();
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host, // �T�[�o�[�Ƃ��ċN��
            SessionName = "TestSession",
        });

        if (!result.Ok)
        {
            Debug.LogError($"NetworkRunner�̋N���Ɏ��s���܂���: {result.ShutdownReason}");
            return;
        }
        if (_runner.IsServer) // �T�[�o�[�̂݃G�l�~�[�𐶐�
        {
            print("�T�[�o�[�Ń��\�b�h�����s�B�G�l�~�[���X�|�[�����܂��B");
            SpawnEnemies();
        }
    }

    /// <summary>
    /// �G�l�~�[�𐶐����鏈��
    /// </summary>
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            // �����_���Ȉʒu���v�Z
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // �G�l�~�[�𐶐�
            Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// �����ʒu�������_���Ɏ擾
    /// </summary>
    /// <returns>�����_���ȍ��W</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            spawnAreaCenter.position.x + randomCircle.x,
            spawnAreaCenter.position.y,
            spawnAreaCenter.position.z + randomCircle.y
        );
    }
}
