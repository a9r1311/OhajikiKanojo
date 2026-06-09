using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeartChecker : MonoBehaviour
{
    [SerializeField] HPManager hm;
    [SerializeField] PrinceController pc;

    [SerializeField] GameObject[] heart;

    int nowHeartCount;
    float maxHP;

    void Start()
    {
        nowHeartCount = hm.heartCount;
        maxHP = pc.currentHp;
    }

    public void UpdateHeart()
    {
        //最大HPを100とした時の今あるハートの個数分の最大HP　＜　pcの最大HPを100とした時の現在HP
        if (nowHeartCount * hm.heartLine < pc.currentHp / maxHP * 100)
        {
            StartCoroutine(ActiveTrue(nowHeartCount));
            nowHeartCount++;
        }

        //最大HPを100とした時の今あるハートの個数ー１分の最大HP　＜　pcの最大HPを100とした時の現在HP
        if ((nowHeartCount - 1) * hm.heartLine >= pc.currentHp / maxHP * 100)
        {
            nowHeartCount--;
            StartCoroutine(ActiveFalse(nowHeartCount));
        }
    }

    IEnumerator ActiveTrue (int n)
    {
        yield return null;
        heart[n].SetActive(true);
    }

    IEnumerator ActiveFalse(int n)
    {
        yield return null;
        heart[n].SetActive(false);
    }

    public void ReStart()
    {
        for(int i = 0; i < hm.heartCount; i++) heart[i].SetActive(true);
        nowHeartCount = hm.heartCount;
    }
}
