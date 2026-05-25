using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraDirection : MonoBehaviour
{
    public CinemachineBrain brain;

    GameStop stop;

    public CinemachineCamera titleCam;
    public CinemachineCamera gameCam;

    public float goTitleTime;
    public float goGameTime;

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

    public void GoTitleCamera()
    {
        StartCoroutine(ChangeTitleCamera());
    }

    public void GoGameCamera()
    {
        StartCoroutine(ChangeGameCamera());
    }

    IEnumerator ChangeTitleCamera()
    {
        brain.DefaultBlend.Time = goTitleTime;

        titleCam.Priority = 10;
        gameCam.Priority = 0;

        yield return new WaitForSeconds(goTitleTime); 

        stop.StopGame();
    }

    IEnumerator ChangeGameCamera()
    {
        brain.DefaultBlend.Time = goGameTime;

        titleCam.Priority = 0;
        gameCam.Priority = 10;

        yield return new WaitForSeconds(goGameTime);

        stop.StartGame();
    }
}
