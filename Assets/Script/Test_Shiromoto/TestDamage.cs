using UnityEngine;
using UnityEngine.InputSystem;

public class TestDamage : MonoBehaviour
{
    [SerializeField] HPManager hp;
    void Update()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame) hp.GetDamage();
    }
}
