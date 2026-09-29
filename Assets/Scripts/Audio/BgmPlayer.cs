using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip bgm;

    private void Start()
    {
        if (bgm == null) AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM(bgm);
    }
}
