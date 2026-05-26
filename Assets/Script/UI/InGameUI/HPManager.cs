using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    public Image hpFill;
    [SerializeField] TargetToProtect ttp;
    private float maxHP;

    void Start()
    {
        maxHP = ttp.currentHp;
    }

    void Update()
    {
        hpFill.fillAmount = ttp.currentHp / maxHP;
    }
}
