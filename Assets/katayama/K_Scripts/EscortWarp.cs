using UnityEngine;
using UnityEngine.InputSystem;

public class EscortWarp : MonoBehaviour
{
    [Header("護衛対象")]
    [SerializeField]
    private Transform escortTarget;

    [Header("ワープ位置オフセット")]
    [SerializeField]
    private Vector3 warpOffset =
        new Vector3(0f, 0f, -2f);

    private Rigidbody rb;

    public bool isWarped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            WarpToEscort();
            isWarped = true;
        }
    }

    private void WarpToEscort()
    {
        if (escortTarget == null)
            return;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position =
            escortTarget.position +
            warpOffset;

        transform.rotation =
            Quaternion.Euler(
                -90f,
                escortTarget.eulerAngles.y,
                0f
            );
    }
}