using UnityEngine;
using UnityEngine.InputSystem;

public class OhajikiFlick : MonoBehaviour
{
    Rigidbody rb;

    Vector3 startPos;
    Vector3 currentPos;

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

    [Header("フリック再許可")]
    [SerializeField] float flickEnableSpeed = 1.0f;
    [SerializeField] float flickCooldown = 0.2f;

    [Header("キャンセル判定")]
    [SerializeField] float cancelDistance = 0.2f;

    [Header("ためショット")]
    [SerializeField] float maxChargeTime = 2f;
    [SerializeField] float chargeMultiplier = 2f;

    float chargeTime = 0f;

    float flickTimer = 0f;

    int currentFlickCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 矢印を最初は非表示
        arrow.gameObject.SetActive(false);

        // 回転固定
        rb.constraints =
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // クールタイム
        flickTimer += Time.deltaTime;

        // 減速したら再フリック可能
        if (rb.linearVelocity.magnitude < flickEnableSpeed &&
            flickTimer > flickCooldown)
        {
            canFlick = true;
        }
        else
        {
            canFlick = false;
        }

        // =========================
        // 押した瞬間
        // =========================
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!canFlick) return;

            if (currentFlickCount >= maxFlickCount) return;

            // 開始位置
            startPos = GetMouseWorldPosition();

            isDragging = true;

            // ため時間リセット
            chargeTime = 0f;

            // 矢印表示
            arrow.gameObject.SetActive(true);
        }

        // =========================
        // ドラッグ中
        // =========================
        if (isDragging)
        {
            // ため時間加算
            chargeTime += Time.deltaTime;

            // 最大値制限
            chargeTime = Mathf.Clamp(
                chargeTime,
                0f,
                maxChargeTime
            );

            currentPos = GetMouseWorldPosition();

            // 引っ張り方向
            Vector3 dir = startPos - currentPos;

            // Y無視
            dir.y = 0;

            // 距離制限
            dir = Vector3.ClampMagnitude(
                dir,
                maxDragDistance
            );

            // 感度
            dir *= flickSensitivity;

            // 最大パワー
            dir = Vector3.ClampMagnitude(
                dir,
                maxPower
            );

            // =========================
            // キャンセル判定
            // =========================
            bool isCanceling =
                dir.magnitude < cancelDistance;

            // 矢印表示切替
            arrow.gameObject.SetActive(!isCanceling);

            // =========================
            // 向き
            // =========================
            if (dir != Vector3.zero)
            {
                // 矢印方向
                arrow.rotation =
                    Quaternion.LookRotation(dir);

                // プレイヤー方向
                Vector3 lookDir = dir;
                lookDir.y = 0;

                Quaternion baseRot =
                    Quaternion.LookRotation(lookDir);

                transform.rotation =
                    baseRot *
                    Quaternion.Euler(-90f, -180f, 0f);
            }

            // =========================
            // 矢印サイズ
            // =========================
            float powerPercent =
                dir.magnitude / maxPower;

            float length =
                powerPercent * arrowMaxLength;

            // ため倍率
            float chargeRate =
                chargeTime / maxChargeTime;

            // ためるほど伸びる
            arrow.localScale = new Vector3(
                2f,
                2f,
                length * (1f + chargeRate)
            );

            // =========================
            // 矢印位置
            // =========================
            if (dir != Vector3.zero)
            {
                arrow.position =
                    transform.position +
                    dir.normalized *
                    length *
                    0.5f;
            }
        }

        // =========================
        // 離した瞬間
        // =========================
        if (Mouse.current.leftButton.wasReleasedThisFrame &&
            isDragging)
        {
            currentPos = GetMouseWorldPosition();

            Vector3 dir = startPos - currentPos;

            dir.y = 0;

            dir = Vector3.ClampMagnitude(
                dir,
                maxDragDistance
            );

            dir *= flickSensitivity;

            dir = Vector3.ClampMagnitude(
                dir,
                maxPower
            );

            // =========================
            // キャンセル
            // =========================
            if (dir.magnitude < cancelDistance)
            {
                isDragging = false;

                arrow.gameObject.SetActive(false);

                return;
            }

            // =========================
            // ため倍率
            // =========================
            float chargeRate =
                chargeTime / maxChargeTime;

            float chargePower =
                Mathf.Lerp(
                    1f,
                    chargeMultiplier,
                    chargeRate
                );

            // 最終威力
            float finalPower =
                power * chargePower;

            // 発射（逆方向）
            rb.AddForce(
                -dir.normalized * finalPower,
                ForceMode.Impulse
            );

            // 回数加算
            currentFlickCount++;

            // 状態リセット
            isDragging = false;
            canFlick = false;

            // クールタイムリセット
            flickTimer = 0f;

            // 矢印非表示
            arrow.gameObject.SetActive(false);
        }
    }

    // =========================
    // マウス位置 → ワールド座標
    // =========================
    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos =
            Mouse.current.position.ReadValue();

        Ray ray =
            Camera.main.ScreenPointToRay(mousePos);

        Plane plane =
            new Plane(Vector3.up, Vector3.zero);

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}