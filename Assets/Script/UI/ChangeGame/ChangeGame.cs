using System.Collections;
using UnityEngine;

public class ChangeGame : MonoBehaviour
{
    [SerializeField] TitleUIAnimation animeT;
    [SerializeField] InGameUIAnimation animeIG;
    [SerializeField] CameraDirection direction;
    [SerializeField] UIChanger changeUI;
    [SerializeField] GameStop gs;

    public float animeTTime = 1.6f;
    public float animeIGTime = 1.0f;
    public float cameraTime = 2.0f;

    public float earlyTimer = 0.5f;

    public void GoTitle()
    {
        StartCoroutine(GT());
    }

    public void GoInGame()
    {
       StartCoroutine(GIG());
    }

    public void GoResult()
    {
        //StartCoroutine(GR());
    }

    IEnumerator GT()
    {
        gs.StopGame();

        animeIG.MoveInGameOutPosition(animeIGTime * earlyTimer);

        yield return new WaitForSeconds(animeIGTime * earlyTimer);

        changeUI.HideAllUI();
        animeIG.SetInGameInPosition();
        animeT.SetTitleOutPosition();
        changeUI.ActiveTitleUI();
        direction.GoTitleCamera(cameraTime * earlyTimer);

        yield return new WaitForSeconds((cameraTime - animeTTime) * earlyTimer);

        animeT.MoveTitleInPosition(animeTTime * earlyTimer);

        yield return new WaitForSeconds(animeTTime * earlyTimer);
    }


    IEnumerator GIG()
    {
        direction.GoGameCamera(cameraTime);
        animeT.MoveTitleOutPosition(animeTTime);

        yield return new WaitForSeconds(CheckLongTime(animeTTime, cameraTime));

        changeUI.HideAllUI();
        animeT.SetTitleInPosition();
        animeIG.SetInGameOutPosition();
        changeUI.ActiveInGameUI();
        animeIG.MoveInGameInPosition(animeIGTime);

        yield return new WaitForSeconds(animeIGTime);

        gs.StartGame();
    }

    /*IEnumerator GR()
    {

    }*/

    float CheckLongTime(float a, float b)
    {
        return Mathf.Max(a, b);
    }
}
