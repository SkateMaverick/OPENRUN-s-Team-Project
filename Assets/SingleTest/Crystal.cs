using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField] private float colorChangeSpeed = 1f;
    private Renderer _crystalRenderer;

    private void Awake()
    {
        _crystalRenderer = GetComponent<Renderer>();
    }
    
    private void Update()
    {
        Rotate();
        ChangeColor();
    }

    private void Rotate()
    {
        transform.Rotate(0f, 0f, 90f * Time.deltaTime);
    }

    private void ChangeColor()
    {
        float hue = (Time.time * colorChangeSpeed) % 1.0f;
        Color targetColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        
        _crystalRenderer.material.SetColor("_BaseColor", targetColor);
        //_crystalRenderer.material.color = targetColor;
    }
}
