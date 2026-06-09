using UnityEngine;

[CreateAssetMenu(
    fileName = "SEData",
    menuName = "Audio/SE Data"
)]
public class SEData : ScriptableObject
{
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    public AudioClip GetClip(int index)
    {
        if (clips == null)
            return null;

        if (index < 0 ||
            index >= clips.Length)
            return null;

        return clips[index];
    }
}