using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    public RectTransform allFill;
    public Image hpFill;
    public Image hpBackFill;

    [SerializeField] PrinceController pc;

    Coroutine damageReaction;
    Coroutine hpBackCount;

    float maxHP;
    float oldHP;
    private bool goBackGauge = false;

    public float reactionTimer = 0.4f;
    public float goBackGaugeCount = 1.0f;
    public float backGaugeSpeed = 0.5f;
    public float shakePow = 16f;

    void Start()
    {
        maxHP = pc.currentHp;
        oldHP = pc.currentHp;

        ChangeColor(hpFill, Color.red);
        ChangeColor(hpBackFill, Color.black);
    }

    void Update()
    {
        if(oldHP != pc.currentHp)
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
            hpBackFill.fillAmount = Mathf.MoveTowards(hpBackFill.fillAmount, hpFill.fillAmount, backGaugeSpeed * Time.deltaTime);
            if(hpBackFill.fillAmount == hpFill.fillAmount) goBackGauge = false;
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
        hpFill.fillAmount = pc.currentHp / maxHP;
    }

    IEnumerator HPBackCount()
    {
        yield return new WaitForSeconds(goBackGaugeCount);

        goBackGauge = true;
    }

    void ChangeColor(Image fill, Color color)
    {
        fill.color = color;
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
