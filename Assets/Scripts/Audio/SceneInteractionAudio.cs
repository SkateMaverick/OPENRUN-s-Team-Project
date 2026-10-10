using UnityEngine;

public class SceneInteractionAudio : MonoBehaviour
{
    public static SceneInteractionAudio Instance { get; private set; }

    public AudioClip[] walkFootstep;
    public AudioClip[] runFootstep;

    private void Awake() => Instance = this;
}
