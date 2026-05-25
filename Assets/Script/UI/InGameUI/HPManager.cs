using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{

    public Image hpFill;

    public float maxHP = 3;
    public float currentHP;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }

    public void GetDamage()
    {
        currentHP--;
        if(currentHP < 0) currentHP = 0;

        UpdateHPBar();

        if (currentHP == 0) Debug.Log("君の負け～～～");
    }

    void UpdateHPBar()
    {
        hpFill.fillAmount = currentHP / maxHP;
    }
}
