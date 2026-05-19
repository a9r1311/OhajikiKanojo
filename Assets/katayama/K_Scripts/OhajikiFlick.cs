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
    [SerializeField] float chargeMultiplier = 3f;

    [Header("モデル向き補正")]
    [Tooltip("モデルが逆を向く場合は Y を 180 にする")]
    [SerializeField]
    Vector3 modelRotationOffset =
        new Vector3(-90f, -180f, 0f);

    float chargeTime = 0f;
    float flickTimer = 0f;
    int currentFlickCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 矢印を最初は非表示
        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }

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

            // 開始位置（ため時間用）
            startPos = GetMouseWorldPosition();

            isDragging = true;
            chargeTime = 0f;

            if (arrow != null)
            {
                arrow.gameObject.SetActive(true);
            }
        }

        // =========================
        // ドラッグ中
        // =========================
        if (isDragging)
        {
            // ため時間加算
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(
                chargeTime,
                0f,
                maxChargeTime
            );

            currentPos = GetMouseWorldPosition();

            // プレイヤー → 現在のマウス位置の方向
            Vector3 dir =
                currentPos - transform.position;

            // Y無視
            dir.y = 0f;

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

            // キャンセル判定
            bool isCanceling =
                dir.magnitude < cancelDistance;

            // 矢印表示切替
            if (arrow != null)
            {
                arrow.gameObject.SetActive(!isCanceling);
            }

            // =========================
            // 向き
            // =========================
            if (dir.sqrMagnitude > 0.0001f)
            {
                // プレイヤー方向
                transform.rotation =
                    Quaternion.LookRotation(dir) *
                    Quaternion.Euler(
                        modelRotationOffset
                    );

                // 矢印方向（マウスと逆向きになる場合は180度回転）
                if (arrow != null)
                {
                    arrow.rotation =
                        Quaternion.LookRotation(dir) *
                        Quaternion.Euler(0f, 0f, 0f);
                }
            }

            // =========================
            // 矢印サイズ
            // =========================
            if (arrow != null)
            {
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
                // 矢印をプレイヤーの前方に配置
                if (dir.sqrMagnitude > 0.0001f)
                {
                    arrow.position =
                        transform.position +
                        dir.normalized *
                        length *
                        0.5f;
                }
            }
        }

        // =========================
        // 離した瞬間
        // =========================
        if (Mouse.current.leftButton.wasReleasedThisFrame &&
            isDragging)
        {
            currentPos = GetMouseWorldPosition();

            // =====================================
            // プレイヤー → マウス位置の方向
            // （クリック開始位置に依存しない）
            // =====================================
            Vector3 dir =
                currentPos - transform.position;

            dir.y = 0f;

            dir = Vector3.ClampMagnitude(
                dir,
                maxDragDistance
            );

            dir *= flickSensitivity;

            dir = Vector3.ClampMagnitude(
                dir,
                maxPower
            );

            // キャンセル
            if (dir.magnitude < cancelDistance)
            {
                isDragging = false;

                if (arrow != null)
                {
                    arrow.gameObject.SetActive(false);
                }

                return;
            }

            // ため倍率
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

            // 現在の速度をリセット
            rb.linearVelocity = Vector3.zero;

            // マウスの方向へ発射
            rb.AddForce(
                dir.normalized * finalPower,
                ForceMode.Impulse
            );

            // 回数加算
            currentFlickCount++;

            // 状態リセット
            isDragging = false;
            canFlick = false;
            flickTimer = 0f;

            // 矢印非表示
            if (arrow != null)
            {
                arrow.gameObject.SetActive(false);
            }
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

        // プレイヤーの高さに合わせた平面
        Plane plane =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    transform.position.y,
                    0f
                )
            );

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
}