using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WalkingIK))]
public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("SingleMain Sound (Minecraft Walk)")]
    [Tooltip("SingleMain 씬에서 한걸음 걸을 때마다 재생될 Minecraft 발소리 클립들")]
    [SerializeField] private AudioClip[] singleMainStepClips;

    [Header("Other Scenes Sound (e.g. Dungeon)")]
    [Tooltip("SingleMain 외 다른 씬(던전 등)에서 사용할 기본 발소리 클립들")]
    [SerializeField] private AudioClip[] defaultStepClips;

    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.7f;
    [SerializeField] private float minPitch = 0.94f;
    [SerializeField] private float maxPitch = 1.06f;
    [SerializeField, Range(0f, 1f)] private float spatialBlend = 0.25f;

    [Header("Scene Restriction")]
    [SerializeField] private string targetSceneName = "SingleMain";

    [Header("Sprint / Run Settings")]
    [SerializeField] private float runStepInterval = 0.25f;

    private AudioSource _audioSource;
    private WalkingIK _walkingIK;
    private PlayerAnimator _playerAnimator;
    private int _lastClipIndex = -1;
    private float _runStepTimer;

    public AudioClip[] SingleMainStepClips
    {
        get => singleMainStepClips;
        set => singleMainStepClips = value;
    }

    public AudioClip[] DefaultStepClips
    {
        get => defaultStepClips;
        set => defaultStepClips = value;
    }

    private void Awake()
    {
        _walkingIK = GetComponent<WalkingIK>();
        _playerAnimator = GetComponent<PlayerAnimator>();

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = spatialBlend;

        _runStepTimer = runStepInterval * 0.8f;
    }

    private void OnEnable()
    {
        if (_walkingIK != null)
        {
            _walkingIK.OnStepTaken += HandleWalkStep;
        }
    }

    private void OnDisable()
    {
        if (_walkingIK != null)
        {
            _walkingIK.OnStepTaken -= HandleWalkStep;
        }
    }

    private void Update()
    {
        // 걷기는 WalkingIK.OnStepTaken 이벤트로 발동하고,
        // 달리기(Sprint) 상태일 때는 애니메이션 주기에 맞춰 발소리를 재생
        if (_playerAnimator != null && _playerAnimator.isMoving && _playerAnimator.isSprint)
        {
            _runStepTimer += Time.deltaTime;
            if (_runStepTimer >= runStepInterval)
            {
                _runStepTimer = 0f;
                PlayFootstep();
            }
        }
        else
        {
            _runStepTimer = runStepInterval * 0.8f;
        }
    }

    private void HandleWalkStep()
    {
        // 달리기 중에는 Update의 달리기 타이머가 담당
        if (_playerAnimator != null && _playerAnimator.isSprint)
            return;

        PlayFootstep();
    }

    public void PlayFootstep()
    {
        if (!IsGrounded())
            return;

        bool inSingleMain = IsInSingleMain();

        // singleMain에서만 minecraft walk 사운드를 사용
        AudioClip[] clipsToUse = inSingleMain ? singleMainStepClips : defaultStepClips;

        if (clipsToUse == null || clipsToUse.Length == 0)
            return;

        int index = Random.Range(0, clipsToUse.Length);
        if (clipsToUse.Length > 1 && index == _lastClipIndex)
        {
            index = (index + 1) % clipsToUse.Length;
        }
        _lastClipIndex = index;

        AudioClip clip = clipsToUse[index];
        if (clip == null)
            return;

        _audioSource.pitch = Random.Range(minPitch, maxPitch);
        _audioSource.PlayOneShot(clip, volume);
    }

    public bool IsInSingleMain()
    {
        if (string.Equals(gameObject.scene.name, targetSceneName, StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(SceneManager.GetActiveScene().name, targetSceneName, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 1.5f, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider col = hits[i].collider;
            if (col == null || col.isTrigger) continue;
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        LoadDefaultClipsInEditor();
    }

    public void LoadDefaultClipsInEditor()
    {
        System.Collections.Generic.List<AudioClip> minecraftClips = new System.Collections.Generic.List<AudioClip>();
        for (int i = 1; i <= 11; i++)
        {
            string path = string.Format("Assets/Sound/MinecraftWalk/minecraft_step_{0:D2}.wav", i);
            var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null) minecraftClips.Add(clip);
        }
        if (minecraftClips.Count > 0)
        {
            singleMainStepClips = minecraftClips.ToArray();
        }

        var dungeonClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/던전 발소리/steps_platform.ogg");
        if (dungeonClip != null)
        {
            defaultStepClips = new AudioClip[] { dungeonClip };
        }
    }
#endif
}
