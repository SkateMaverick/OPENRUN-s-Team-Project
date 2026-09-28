using UnityEngine;

[ExecuteAlways]
public class HorizonBackdropController : MonoBehaviour
{
    [Header("Tracking")]
    [Tooltip("Target to follow horizontally (usually Main Camera or Player). If null, Camera.main is used.")]
    public Transform target;
    [Tooltip("If true, follows target X and Z position like a skybox so horizon distance remains constant.")]
    public bool followTargetXZ = true;
    [Tooltip("Base world Y level of the horizon bottom.")]
    public float baseWorldY = -30f;

    [Header("Dimensions")]
    [Tooltip("Radius of the horizon cylinder.")]
    [Range(100f, 1000f)]
    public float radius = 380f;

    [Tooltip("Total vertical height of the cylinder.")]
    [Range(50f, 600f)]
    public float height = 220f;

    [Header("Slow Ambient Rotation")]
    [Tooltip("Degrees per second to rotate slowly (0 = static).")]
    public float rotationSpeed = 0f;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        FindTargetIfNull();
    }

    private void OnEnable()
    {
        FindTargetIfNull();
        UpdateTransform();
    }

    private void Update()
    {
        if (Application.isPlaying && rotationSpeed != 0f)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        UpdateTransform();
    }

    private void LateUpdate()
    {
        UpdateTransform();
    }

    private void OnValidate()
    {
        UpdateTransform();
    }

    private void FindTargetIfNull()
    {
        if (target == null)
        {
            if (Camera.main != null)
                target = Camera.main.transform;
            else
            {
                var cam = Object.FindAnyObjectByType<Camera>();
                if (cam != null) target = cam.transform;
            }
        }
    }

    public void UpdateTransform()
    {
        // Scale cylinder: diameter in X/Z, height in Y
        transform.localScale = new Vector3(radius * 2f, height, radius * 2f);

        if (followTargetXZ)
        {
            if (target == null) FindTargetIfNull();
            if (target != null)
            {
                transform.position = new Vector3(target.position.x, baseWorldY + height * 0.5f, target.position.z);
                return;
            }
        }

        // Static mode: centered at (0, Y, 0)
        transform.position = new Vector3(0f, baseWorldY + height * 0.5f, 0f);
    }
}
