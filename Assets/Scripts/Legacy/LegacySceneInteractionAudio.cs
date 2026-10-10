// 52aad02 커밋 기준 SceneInteractionAudio 백업 (발사 연출·발소리 변경 전)
using UnityEngine;

public class LegacySceneInteractionAudio : MonoBehaviour
{
    public static LegacySceneInteractionAudio Instance { get; private set; }

    public AudioClip walkFootStep;
    public AudioClip runFootStep;

    private void Awake() => Instance = this;
}
