using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // 씬마다 배치해둬도 처음 만들어진 하나만 남음
            Destroy(gameObject);
            return;
        }

        bgmSource.loop = true;
    }

    // 효과음 재생. 여러 번 호출해도 겹쳐서 재생됨
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    // 배경음 재생. 같은 곡이 이미 나오고 있으면 처음부터 다시 틀지 않음
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();
}
