using UnityEngine;
using Unity.Cinemachine;

public class CameraDirection : MonoBehaviour
{
    public CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        
        if (noise == null)
        {
            Debug.LogError("Noiseが見つからない！");
        }
    }

    Coroutine shakeCoroutine;

    public void Shake(float power, float time)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(power, time));
    }

    System.Collections.IEnumerator ShakeCoroutine(float power, float time)
    {
        noise.AmplitudeGain = power;
        noise.FrequencyGain = 10f;

        yield return new WaitForSeconds(time);

        noise.AmplitudeGain = 0;
    }
}
