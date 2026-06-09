using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM AudioSource")]
    [SerializeField]
    AudioSource bgmSource;

    [Header("SE AudioSource")]
    [SerializeField]
    AudioSource seSource;

    [Header("BGMデータ")]
    [SerializeField]
    BGMData bgmData;

    [Header("SEデータ")]
    [SerializeField]
    SEData seData;

    [Header("再生するBGM番号")]
    [SerializeField]
    int clipIndex = 0;

    [Header("BGM全体音量")]
    [Range(0f, 1f)]
    [SerializeField]
    float masterBGMVolume = 1f;

    [Header("SE全体音量")]
    [Range(0f, 1f)]
    [SerializeField]
    float masterSEVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(
            //    gameObject
            //);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGM();
    }

    // =========================
    // BGM再生
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
    // BGM停止
    // =========================
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // =========================
    // BGM一時停止
    // =========================
    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    // =========================
    // BGM再開
    // =========================
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    // =========================
    // BGM音量変更
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

    // =========================
    // SE再生
    // =========================
    public void PlaySE(int index)
    {
        if (seData == null)
            return;

        AudioClip clip =
            seData.GetClip(index);

        if (clip == null)
            return;

        seSource.PlayOneShot(
            clip,
            seData.volume *
            masterSEVolume
        );
    }

    // =========================
    // SE音量変更
    // =========================
    public void SetSEVolume(
        float volume
    )
    {
        masterSEVolume =
            Mathf.Clamp01(volume);
    }
}