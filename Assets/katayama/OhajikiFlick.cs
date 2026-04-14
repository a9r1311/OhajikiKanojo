using UnityEngine;
using UnityEngine.InputSystem; // ★これ重要

public class OhajikiFlick3D : MonoBehaviour
{
    // ===== 物理 =====
    Rigidbody rb;

    // 入力位置
    Vector3 startPos;
    Vector3 currentPos;

    // 状態管理
    bool isDragging = false;
    bool canFlick = true;

    [Header("矢印")]
    [SerializeField] Transform arrow;
    [SerializeField] float arrowMaxLength = 2f;

    [Header("パワー")]
    [SerializeField] float power = 10f;
    [SerializeField] float maxPower = 3f;

    [Header("フリック調整")]
    [SerializeField] float flickSensitivity = 0.3f;
    [SerializeField] float maxDragDistance = 1.5f;

    [Header("回数制限")]
    [SerializeField] int maxFlickCount = 5;

    [Header("停止判定")]
    [SerializeField] float stopThreshold = 0.05f;

    int currentFlickCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 矢印非表示
        arrow.gameObject.SetActive(false);

        // 回転固定（おはじきが転がらないように）
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // ===== 押した瞬間 =====
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!canFlick) return;
            if (currentFlickCount >= maxFlickCount) return;

            startPos = GetMouseWorldPosition();
            isDragging = true;
            arrow.gameObject.SetActive(true);
        }

        // ===== ドラッグ中 =====
        if (isDragging)
        {
            currentPos = GetMouseWorldPosition();

            Vector3 dir = startPos - currentPos;
            dir.y = 0; // 床に沿って動かす

            // 制限
            dir = Vector3.ClampMagnitude(dir, maxDragDistance);
            dir *= flickSensitivity;
            dir = Vector3.ClampMagnitude(dir, maxPower);

            // 向き
            if (dir != Vector3.zero)
                arrow.rotation = Quaternion.LookRotation(dir);

            // 長さ
            float powerPercent = dir.magnitude / maxPower;
            float length = powerPercent * arrowMaxLength;

            arrow.localScale = new Vector3(0.3f, 0.3f, length);

            // 位置
            arrow.position = transform.position + dir.normalized * length * 0.5f;
        }

        // ===== 離した瞬間 =====
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            currentPos = GetMouseWorldPosition();

            Vector3 dir = startPos - currentPos;
            dir.y = 0;

            dir = Vector3.ClampMagnitude(dir, maxDragDistance);
            dir *= flickSensitivity;
            dir = Vector3.ClampMagnitude(dir, maxPower);

            // 力を加える
            rb.AddForce(-dir * power, ForceMode.Impulse);

            currentFlickCount++;

            isDragging = false;
            canFlick = false;

            arrow.gameObject.SetActive(false);
        }

        // ===== 停止判定 =====
        if (rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (!canFlick)
                canFlick = true;
        }
    }

    // ===== マウス位置 → ワールド座標（床）=====
    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}