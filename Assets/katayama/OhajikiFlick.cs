using UnityEngine;
using UnityEngine.InputSystem; // 新Input System

public class OhajikiFlick3D : MonoBehaviour
{
    // ===== 物理 =====
    Rigidbody rb;

    // ===== 入力位置 =====
    Vector3 startPos;   // フリック開始位置
    Vector3 currentPos; // 現在位置

    // ===== 状態 =====
    bool isDragging = false; // ドラッグ中か
    bool canFlick = true;    // フリック可能か

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
    [SerializeField] float stopThreshold = 0.05f; // 速度がこれ以下なら停止候補
    [SerializeField] float stopTime = 0.3f;       // この時間止まれば完全停止

    float stopTimer = 0f; // 停止している時間

    int currentFlickCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 矢印非表示
        arrow.gameObject.SetActive(false);

        // 回転固定（転がらないように）
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // ===== 押した瞬間 =====
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // ▼重要：動いてたら絶対にフリックさせない
            if (rb.linearVelocity.magnitude > stopThreshold) return;

            // フリック不可状態なら無視
            if (!canFlick) return;

            // 回数制限
            if (currentFlickCount >= maxFlickCount) return;

            // 開始位置取得
            startPos = GetMouseWorldPosition();

            isDragging = true;

            // 矢印表示
            arrow.gameObject.SetActive(true);
        }

        // ===== ドラッグ中 =====
        if (isDragging)
        {
            currentPos = GetMouseWorldPosition();

            // 引っ張り方向
            Vector3 dir = startPos - currentPos;

            // 床だけ動くようにする
            dir.y = 0;

            // 距離制限
            dir = Vector3.ClampMagnitude(dir, maxDragDistance);

            // 感度調整
            dir *= flickSensitivity;

            // 最大パワー制限
            dir = Vector3.ClampMagnitude(dir, maxPower);

            // ===== 矢印の向き =====
            if (dir != Vector3.zero)
                arrow.rotation = Quaternion.LookRotation(dir);

            // ===== 矢印の長さ =====
            float powerPercent = dir.magnitude / maxPower;
            float length = powerPercent * arrowMaxLength;

            arrow.localScale = new Vector3(0.3f, 0.3f, length);

            // ===== 矢印の位置 =====
            arrow.position = transform.position + dir.normalized * length * 0.5f;
        }

        // ===== 離した瞬間 =====
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            currentPos = GetMouseWorldPosition();

            Vector3 dir = startPos - currentPos;
            dir.y = 0;

            // 同じ制限処理
            dir = Vector3.ClampMagnitude(dir, maxDragDistance);
            dir *= flickSensitivity;
            dir = Vector3.ClampMagnitude(dir, maxPower);

            // 力を加える（逆方向）
            rb.AddForce(-dir * power, ForceMode.Impulse);

            // フリック回数加算
            currentFlickCount++;

            // 状態リセット
            isDragging = false;
            canFlick = false;

            // 停止タイマーリセット（ここ重要）
            stopTimer = 0f;

            // 矢印非表示
            arrow.gameObject.SetActive(false);
        }

        // ===== 停止判定（改良版）=====
        if (rb.linearVelocity.magnitude < stopThreshold)
        {
            // 止まっている時間をカウント
            stopTimer += Time.deltaTime;

            // 一定時間止まったら完全停止
            if (stopTimer >= stopTime)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // フリック可能にする
                if (!canFlick)
                    canFlick = true;
            }
        }
        else
        {
            // 動いている間はタイマーリセット
            stopTimer = 0f;

            // 念のためフリック不可にする（安全策）
            canFlick = false;
        }
    }

    // ===== マウス位置 → ワールド座標（床）=====
    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Y=0の床との交点
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}