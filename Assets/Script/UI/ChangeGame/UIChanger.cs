using UnityEngine;
using System.Collections;

public class UIChanger : MonoBehaviour
{
    public GameObject titleUI;
    public GameObject inGameUI;
    public GameObject configUI;
    public GameObject rankingUI;


    public void ActiveTitleUI()
    {
        StartCoroutine(TitleUI(true));
        StartCoroutine(InGameUI(false));
        StartCoroutine(ConfigUI(false));
        StartCoroutine(RankingUI(false));
    }


    public void ActiveInGameUI()
    {
        StartCoroutine(TitleUI(false));
        StartCoroutine(InGameUI(true));
        StartCoroutine(ConfigUI(false));
        StartCoroutine(RankingUI(false));
    }


    public void ActiveConfigUI()
    {
        StartCoroutine(TitleUI(true));
        StartCoroutine(InGameUI(false));
        StartCoroutine(ConfigUI(true));
        StartCoroutine(RankingUI(false));
    }


    public void ActiveRankingUI()
    {
        StartCoroutine(TitleUI(true));
        StartCoroutine(InGameUI(false));
        StartCoroutine(ConfigUI(false));
        StartCoroutine(RankingUI(true));
    }

    IEnumerator TitleUI(bool isBool)
    {
        titleUI.SetActive(isBool);
        yield return new WaitForSeconds(0);
    }

    IEnumerator InGameUI(bool isBool)
    {
        inGameUI.SetActive(isBool);
        yield return new WaitForSeconds(0);
    }

    IEnumerator ConfigUI(bool isBool)
    {
        configUI.SetActive(isBool);
        yield return new WaitForSeconds(0);
    }

    IEnumerator RankingUI(bool isBool)
    {
        rankingUI.SetActive(isBool);
        yield return new WaitForSeconds(0);
    }
}
