using System.Collections;
using UnityEngine;

public class ChangeGame : MonoBehaviour
{
    [SerializeField] TitleUIAnimation animeT;
    [SerializeField] InGameUIAnimation animeIG;
    [SerializeField] ResultUIAnimation animeR;
    [SerializeField] CameraDirection direction;
    [SerializeField] UIChanger changeUI;
    [SerializeField] GameStop gs;

    public float cameraTime = 2.0f;

    public float animeTTime = 1.6f;
    public float animeIGTime = 1.0f;
    public float animeRTime = 1.2f;
    public float animeGOTime = 1.2f;

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
        StartCoroutine(GR());
    }

    public void GoGameOver()
    {
        StartCoroutine(GGo());
    }

    public void GoRetry()
    {
        StartCoroutine(GRt());
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

    IEnumerator GR()
    {
        gs.StopGame();

        animeIG.MoveInGameOutPosition(animeIGTime);

        yield return new WaitForSeconds(animeIGTime);

        changeUI.HideAllUI();
        animeIG.SetInGameInPosition();
        animeR.SetResultOutPosition();
        changeUI.ActiveResultUI();
        direction.GoResultCamera(cameraTime);
        animeR.MoveResultInPosition(animeRTime);

        yield return new WaitForSeconds(animeRTime);
    }

    IEnumerator GGo()
    {
        gs.StopGame();

        animeIG.MoveInGameOutPosition(animeIGTime);

        yield return new WaitForSeconds(animeIGTime);

        changeUI.HideAllUI();
        animeIG.SetInGameInPosition();
        animeR.SetResultOutPosition();
        changeUI.ActiveResultUI();
        direction.GoGameOverCamera(cameraTime);
        animeR.MoveResultInPosition(animeRTime);

        yield return new WaitForSeconds(animeGOTime);
    }

    IEnumerator GRt()
    {
        direction.GoGameCamera(cameraTime);
        animeR.MoveResultOutPosition(animeRTime * earlyTimer);

        yield return new WaitForSeconds(animeRTime * earlyTimer);

        changeUI.HideAllUI();
        animeR.SetResultInPosition();
        animeIG.SetInGameOutPosition();
        changeUI.ActiveInGameUI();
        animeIG.MoveInGameInPosition(animeIGTime);

        yield return new WaitForSeconds(animeIGTime);

        gs.StartGame();
    }

    float CheckLongTime(float a, float b)
    {
        return Mathf.Max(a, b);
    }
}
