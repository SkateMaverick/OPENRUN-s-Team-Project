using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    [Header("Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;

    private GameObject _currentInstance;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSpawner();
        }
    }
    
    public void Spawner()
    {
        if (prefabToSpawn != null && spawnPoint != null)
        {
            if (_currentInstance != null) Destroy(_currentInstance);

            _currentInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Prefab or SpawnPoint is missing!");
        }
    }

    public void ResetSpawner()
    {
        if (_currentInstance != null)
        {
            Destroy(_currentInstance);
            _currentInstance = null;
        }
    }
}