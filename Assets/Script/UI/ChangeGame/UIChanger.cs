using UnityEngine;
using System.Collections;

public class UIChanger : MonoBehaviour
{
    public GameObject titleUI;
    public GameObject inGameUI;
    public GameObject configUI;
    public GameObject rankingUI;
    public GameObject resultUI;


    public void ActiveTitleUI()
    {
        StartCoroutine(ChangeUI(true, false, false, false, false));
    }


    public void ActiveInGameUI()
    {
        StartCoroutine(ChangeUI(false, true, false, false, false));
    }


    public void ActiveConfigUI()
    {
        StartCoroutine(ChangeUI(true, false, true, false, false));
    }


    public void ActiveRankingUI()
    {
        StartCoroutine(ChangeUI(true, false, false, true, false));
    }


    public void HideAllUI()
    {
        StartCoroutine(ChangeUI(false, false, false, false, false));
    }

    IEnumerator ChangeUI(bool title, bool game, bool config, bool ranking, bool result)
    {
        titleUI.SetActive(title);
        inGameUI.SetActive(game);
        configUI.SetActive(config);
        rankingUI.SetActive(ranking);
        resultUI.SetActive(result);

        yield return new WaitForSeconds(0);
    }

}
