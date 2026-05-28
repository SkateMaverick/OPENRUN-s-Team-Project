using UnityEngine;

public class SingleSpawnManager : MonoBehaviour
{
    public static SingleSpawnManager Instance { get; private set; }

    [Header("Players")]
    [SerializeField] private Transform quePlayer;
    [SerializeField] private Transform noaPlayer;

    [Header("Spawn Points")]
    [SerializeField] private Transform queSpawnPoint;
    [SerializeField] private Transform noaSpawnPoint;

    [Header("Test")]
    [SerializeField] private bool enableTestKey = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RespawnAll();
    }

    private void Update()
    {
        if (!enableTestKey)
            return;

        // 키보드 상단 0번: 두 캐릭터 모두 리스폰 테스트
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            RespawnAll();
        }
    }

    public void RespawnAll()
    {
        RespawnPlayer(quePlayer);
        RespawnPlayer(noaPlayer);
    }

    public void RespawnQue()
    {
        RespawnPlayer(quePlayer);
    }

    public void RespawnNoa()
    {
        RespawnPlayer(noaPlayer);
    }

    public void RespawnPlayer(Transform player)
    {
        if (player == null)
            return;

        Transform spawnPoint = GetSpawnPoint(player);

        if (spawnPoint == null)
            return;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        PlayerLife life = player.GetComponent<PlayerLife>();
        if (life != null)
        {
            life.ResetLife();
        }
    }

    private Transform GetSpawnPoint(Transform player)
    {
        if (player == quePlayer)
            return queSpawnPoint;

        if (player == noaPlayer)
            return noaSpawnPoint;

        // 이름으로도 보조 판별
        if (player.name.Contains("Que"))
            return queSpawnPoint;

        if (player.name.Contains("Noa") || player.name.Contains("Bow"))
            return noaSpawnPoint;

        return null;
    }
}