using System.Collections;
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

    [Header("ワープ前エフェクト")]
    [SerializeField]
    private GameObject warpEffectPrefab;

    [SerializeField]
    private float effectDestroyTime = 2f;

    [Header("ワープまでの待機時間")]
    [SerializeField]
    private float warpDelay = 0.5f;

    private Rigidbody rb;

    public bool isWarped = false;
    private bool isWarping = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame &&
            !isWarping)
        {
            StartCoroutine(WarpRoutine());
        }
    }

    private IEnumerator WarpRoutine()
    {
        isWarping = true;

        // ワープ前エフェクト
        if (warpEffectPrefab != null)
        {
            GameObject effect =
                Instantiate(
                    warpEffectPrefab,
                    transform.position,
                    Quaternion.identity
                );

            Destroy(effect, effectDestroyTime);
        }

        // 指定時間待つ
        yield return new WaitForSeconds(warpDelay);

        WarpToEscort();

        isWarped = true;
        isWarping = false;
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