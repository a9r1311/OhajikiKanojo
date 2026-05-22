using UnityEngine;
using UnityEngine.InputSystem;

public class TestShake : MonoBehaviour
{
    [SerializeField]CameraDirection camera;
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            camera.Shake(5.0f, 0.2f);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            camera.Shake(40.0f, 0.2f);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            camera.Shake(5.0f, 0.5f);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            camera.Shake(20.0f, 2.5f);
        }
    }
}
