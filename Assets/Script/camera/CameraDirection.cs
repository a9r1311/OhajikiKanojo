using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraDirection : MonoBehaviour
{
    public CinemachineBrain brain;

    public CinemachineCamera titleCam;
    public CinemachineCamera gameCam;
    public CinemachineCamera resultCam;
    public CinemachineCamera gameoverCam;

    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        noise = gameCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        
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

    public void GoTitleCamera(float goTitleTime)
    {
        brain.DefaultBlend.Time = goTitleTime;

        titleCam.Priority = 10;
        gameCam.Priority = 0;
        resultCam.Priority = 0;
        gameoverCam.Priority = 0;
    }

    public void GoGameCamera(float goGameTime)
    {
        brain.DefaultBlend.Time = goGameTime;

        titleCam.Priority = 0;
        gameCam.Priority = 10;
        resultCam.Priority = 0;
        gameoverCam.Priority = 0;
    }

    public void GoResultCamera(float goResultTime)
    {
        brain.DefaultBlend.Time = goResultTime;

        titleCam.Priority = 0;
        gameCam.Priority = 0;
        resultCam.Priority = 10;
        gameoverCam.Priority = 0;
    }

    public void GoGameOverCamera(float goGameOverTime)
    {
        brain.DefaultBlend.Time = goGameOverTime;

        titleCam.Priority = 0;
        gameCam.Priority = 0;
        resultCam.Priority = 0;
        gameoverCam.Priority= 10;
    }
}
