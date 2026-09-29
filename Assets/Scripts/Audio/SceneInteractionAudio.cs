using UnityEngine;

public class SceneInteractionAudio : MonoBehaviour
{
    public static SceneInteractionAudio Instance { get; private set; }

    public AudioClip walkFootStep;
    public AudioClip runFootStep;

    private void Awake() => Instance = this;
}
