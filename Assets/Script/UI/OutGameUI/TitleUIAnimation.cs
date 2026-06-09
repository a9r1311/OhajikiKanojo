using System.Collections;
using UnityEngine;

public class TitleUIAnimation : MonoBehaviour
{
    [SerializeField] RectTransform title;
    [SerializeField] RectTransform start;
    //[SerializeField] RectTransform config;
    [SerializeField] RectTransform ranking;

    Vector2 titleInPos;
    Vector2 startInPos;
    Vector2 configInPos;
    Vector2 rankingInPos;

    Vector2 titleOutPos;
    Vector2 startOutPos;
    Vector2 configOutPos;
    Vector2 rankingOutPos;

    Quaternion titleInRot;
    Quaternion startInRot;
    Quaternion configInRot;
    Quaternion rankingInRot;

    Quaternion titleOutRot;
    Quaternion startOutRot;
    Quaternion configOutRot;
    Quaternion rankingOutRot;

    void Start()
    {
        titleInPos = title.anchoredPosition;
        startInPos = start.anchoredPosition;
        //configInPos = config.anchoredPosition;
        rankingInPos = ranking.anchoredPosition;

        titleOutPos = titleInPos + new Vector2(-800, 1000);
        startOutPos = startInPos + new Vector2(-500, -1200);
        configOutPos = configInPos + new Vector2(-100, -1400);
        rankingOutPos = rankingInPos + new Vector2(-300, -1600);

        titleInRot = Quaternion.Euler(0, 0, 0);
        startInRot = Quaternion.Euler(0, 0, 0);
        configInRot = Quaternion.Euler(0, 0, 0);
        rankingInRot = Quaternion.Euler(0, 0, 0);

        titleOutRot = Quaternion.Euler(0, 0, 70);
        startOutRot = Quaternion.Euler(0, 0, 80);
        configOutRot = Quaternion.Euler(0, 0, 45);
        rankingOutRot = Quaternion.Euler(0, 0, -60);
    }

    public void MoveTitleInPosition(float duration)
    {
        StartCoroutine(MTIP(duration));
    }

    public void MoveTitleOutPosition(float duration)
    {
        StartCoroutine(MTOP(duration));
    }

    IEnumerator MTIP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            title.anchoredPosition = Vector2.Lerp(titleOutPos, titleInPos, t);
            start.anchoredPosition = Vector2.Lerp(startOutPos, startInPos, t);
            //config.anchoredPosition = Vector2.Lerp(configOutPos, configInPos, t);
            ranking.anchoredPosition = Vector2.Lerp(rankingOutPos, rankingInPos, t);

            title.rotation = Quaternion.Lerp(titleOutRot, titleInRot, t);
            start.rotation = Quaternion.Lerp(startOutRot, startInRot, t);
            //config.rotation = Quaternion.Lerp(configOutRot, configInRot, t);
            ranking.rotation = Quaternion.Lerp(rankingOutRot, rankingInRot, t);

            yield return null;
        }
    }

    IEnumerator MTOP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            title.anchoredPosition = Vector2.Lerp(titleInPos, titleOutPos, t);
            start.anchoredPosition = Vector2.Lerp(startInPos, startOutPos, t);
            //config.anchoredPosition = Vector2.Lerp(configInPos, configOutPos, t);
            ranking.anchoredPosition = Vector2.Lerp(rankingInPos, rankingOutPos, t);

            title.rotation = Quaternion.Lerp(titleInRot, titleOutRot, t);
            start.rotation = Quaternion.Lerp(startInRot, startOutRot, t);
            //config.rotation = Quaternion.Lerp(configInRot, configOutRot, t);
            ranking.rotation = Quaternion.Lerp(rankingInRot, rankingOutRot, t);

            yield return null;
        }
    }

    public void SetTitleInPosition()
    {
        title.anchoredPosition = titleInPos;
        start.anchoredPosition = startInPos;
        //config.anchoredPosition = configInPos;
        ranking.anchoredPosition = rankingInPos;

        title.rotation = titleInRot;
        start.rotation = startInRot;
        //config.rotation = configInRot;
        ranking.rotation = rankingInRot;
    }

    public void SetTitleOutPosition()
    {
        title.anchoredPosition = titleOutPos;
        start.anchoredPosition = startOutPos;
        //config.anchoredPosition = configOutPos;
        ranking.anchoredPosition= rankingOutPos;

        title.rotation = titleOutRot;
        start.rotation = startOutRot;
        //config.rotation = configOutRot;
        ranking.rotation = rankingOutRot;
    }

}
