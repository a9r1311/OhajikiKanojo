using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM AudioSource")]
    [SerializeField]
    AudioSource bgmSource;

    [Header("çƒê∂Ç∑ÇÈBGM")]
    [SerializeField]
    BGMData bgmData;

    [Header("çƒê∂Ç∑ÇÈî‘çÜ")]
    [SerializeField]
    int clipIndex = 0;

    [Header("BGMëSëÃâπó ")]
    [Range(0f, 1f)]
    [SerializeField]
    float masterBGMVolume = 1f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(
                gameObject
            );
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGM();
    }

    // =========================
    // BGMçƒê∂
    // =========================
    public void PlayBGM()
    {
        if (bgmData == null)
            return;

        AudioClip clip =
            bgmData.GetClip(
                clipIndex
            );

        if (clip == null)
            return;

        bgmSource.clip =
            clip;

        bgmSource.volume =
            bgmData.volume *
            masterBGMVolume;

        bgmSource.loop =
            bgmData.loop;

        bgmSource.Play();
    }

    // =========================
    // BGMí‚é~
    // =========================
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // =========================
    // àÍéûí‚é~
    // =========================
    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    // =========================
    // çƒäJ
    // =========================
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    // =========================
    // âπó ïœçX
    // =========================
    public void SetBGMVolume(
        float volume
    )
    {
        masterBGMVolume =
            Mathf.Clamp01(volume);

        bgmSource.volume =
            masterBGMVolume;
    }
}