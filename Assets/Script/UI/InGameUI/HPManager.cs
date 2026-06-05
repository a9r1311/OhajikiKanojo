using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    public RectTransform allFill;
    public GameObject mainHP;
    public GameObject backHP;
    Image[] hpFill;
    Image[] hpBackFill;

    [SerializeField] PrinceController pc;
    [SerializeField] HeartChecker hc;

    Coroutine damageReaction;
    Coroutine hpBackCount;

    float maxHP;
    float oldHP;
    private bool goBackGauge = false;

    float rate;
    public int heartCount = 5;
    public float heartLine;
    public float reactionTimer = 0.4f;
    public float goBackGaugeCount = 1.0f;
    public float backGaugeSpeed = 0.5f;
    public float shakePow = 16f;

    void Start()
    {
        maxHP = pc.currentHp;
        oldHP = pc.currentHp;
        heartLine = 100f / heartCount;

        hpFill = mainHP.GetComponentsInChildren<Image>();
        hpBackFill = backHP.GetComponentsInChildren<Image>();

        ChangeColor(hpFill, Color.red);
        ChangeColor(hpBackFill, Color.black);
    }

    void Update()
    {
        if (oldHP != pc.currentHp)
        {
            if(damageReaction != null)
            {
                StopCoroutine(damageReaction);
                if(hpBackCount != null) StopCoroutine(hpBackCount);
            }

            damageReaction = StartCoroutine(DamageReaction());

            oldHP = pc.currentHp;
        }

        if(goBackGauge)
        {
            bool complete = true;

            for(int i = heartCount - 1; i >= 0; i--)
            {
                if (hpBackFill[i].fillAmount != hpFill[i].fillAmount)
                {
                    float before = hpBackFill[i].fillAmount;

                    hpBackFill[i].fillAmount = Mathf.MoveTowards(hpBackFill[i].fillAmount, hpFill[i].fillAmount, backGaugeSpeed * Time.deltaTime);
                    complete = false;

                    if(before > 0f && hpBackFill[i].fillAmount <= 0f) hc.UpdateHeart();

                    break;
                }
            }
            if(complete) goBackGauge = false;
        }
    }

    IEnumerator DamageReaction()
    {
        GaugeReaction();
        ChangeColor(hpFill, Color.yellow);
        hpBackCount = StartCoroutine(HPBackCount());
        StartCoroutine(HPShake());

        yield return new WaitForSeconds(reactionTimer);

        ChangeColor(hpFill, Color.red);
    }

    void GaugeReaction()
    {
        rate = pc.currentHp / maxHP * 100;
        for(int i = 0; i < heartCount; i++)
        {
            hpFill[i].fillAmount = Mathf.Clamp01((rate - heartLine * i) / heartLine);
        }
    }

    IEnumerator HPBackCount()
    {
        yield return new WaitForSeconds(goBackGaugeCount);

        goBackGauge = true;
    }

    public void ChangeColor(Image[] fills, Color color)
    {
        foreach(Image img in fills) img.color = color;
    }

    IEnumerator HPShake()
    {
        Vector2 basePos = allFill.anchoredPosition;

        for(float time = 0; time <= reactionTimer;)
        {
            allFill.anchoredPosition = basePos + Random.insideUnitCircle * shakePow;

            yield return null;

            time += Time.deltaTime;
        }

        allFill.anchoredPosition = basePos;
    }
}
