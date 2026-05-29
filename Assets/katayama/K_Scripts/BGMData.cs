using UnityEngine;

[CreateAssetMenu(
    fileName = "BGMData",
    menuName = "Audio/BGM Data"
)]
public class BGMData : ScriptableObject
{
    [Header("•¡”BGM")]
    public AudioClip[] clips;

    [Header("‰¹—Ê")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("ƒ‹[ƒv")]
    public bool loop = true;

    // =========================
    // w’èæ“¾
    // =========================
    public AudioClip GetClip(
        int index
    )
    {
        if (clips == null ||
            clips.Length == 0)
        {
            return null;
        }

        index = Mathf.Clamp(
            index,
            0,
            clips.Length - 1
        );

        return clips[index];
    }
}