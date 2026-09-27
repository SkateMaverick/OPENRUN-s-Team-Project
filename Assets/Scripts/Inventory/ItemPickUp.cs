using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    [Header("Visual Effects")]
    public bool bobAndRotate = true;
    public float rotateSpeed = 60f;
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.15f;

    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        if (bobAndRotate)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
            float newY = _startPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (InventoryManager.Instance == null)
            return;

        bool success = InventoryManager.Instance.AddItem(itemData, amount);

        if (success)
        {
            Destroy(gameObject);
        }
    }
}