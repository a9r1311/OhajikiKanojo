using UnityEngine;
using System.Collections;

public class UIChanger : MonoBehaviour
{
    public GameObject titleUI;
    public GameObject inGameUI;
    public GameObject configUI;
    public GameObject rankingUI;
    public GameObject resultUI;
    public GameObject gameoverUI;
    [SerializeField] GameObject resultOutputUI;

    public void ActiveTitleUI()
    {
        ChangeUI(true, false, false, false, false, false);
    }


    public void ActiveInGameUI()
    {
        ChangeUI(false, true, false, false, false, false);
    }


    public void ActiveConfigUI()
    {
        ChangeUI(true, false, true, false, false, false);
    }


    public void ActiveRankingUI()
    {
        ChangeUI(true, false, false, true, false, false);
    }

    public void ActiveResultUI()
    {
        ChangeUI(false, false, false, false, true, false);
    }

    public void ActiveGameOverUI()
    {
        ChangeUI(false, false, false, false, false, true);
    }


    public void HideAllUI()
    {
        ChangeUI(false, false, false, false, false, false);
        resultOutputUI.SetActive(false);
    }

    void ChangeUI(bool title, bool game, bool config, bool ranking, bool result, bool gameover)
    {
        titleUI.SetActive(title);
        inGameUI.SetActive(game);
        configUI.SetActive(config);
        rankingUI.SetActive(ranking);
        resultUI.SetActive(result);
        gameoverUI.SetActive(gameover);
    }

}
