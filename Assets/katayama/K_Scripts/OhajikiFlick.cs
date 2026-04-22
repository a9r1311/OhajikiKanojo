using UnityEngine;
using UnityEngine.InputSystem;

public class OhajikiFlick3D : MonoBehaviour
{
    Rigidbody rb;

    Vector3 startPos;   // フリック開始位置
    Vector3 currentPos; // 現在位置

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

    [Header("フリック再許可（減速判定）")]
    [SerializeField] float flickEnableSpeed = 1.0f; // この速度以下で再フリック可能
    [SerializeField] float flickCooldown = 0.2f;    // 連続フリック防止の待ち時間

    float flickTimer = 0f; // フリック後の経過時間

    int currentFlickCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 矢印を最初は非表示
        arrow.gameObject.SetActive(false);

        // 回転とY位置を固定（転がり防止＆床固定）
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // フリック後の経過時間をカウント
        flickTimer += Time.deltaTime;

        if (rb.linearVelocity.magnitude < flickEnableSpeed && flickTimer > flickCooldown)
        {
            canFlick = true;
        }
        else
        {
            canFlick = false;
        }

        // ===== 押した瞬間 =====
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // フリック不可なら何もしない
            if (!canFlick) return;

            // 回数制限チェック
            if (currentFlickCount >= maxFlickCount) return;

            // 開始位置を取得
            startPos = GetMouseWorldPosition();

            isDragging = true;

            // 矢印を表示
            arrow.gameObject.SetActive(true);
        }

        //  ドラッグ中
        if (isDragging)
        {
            currentPos = GetMouseWorldPosition();

            // 引っ張り方向（スタート → 現在）
            Vector3 dir = startPos - currentPos;

            // Y方向は無視（床のみ移動）
            dir.y = 0;

            // 最大ドラッグ距離を制限
            dir = Vector3.ClampMagnitude(dir, maxDragDistance);

            // 感度調整
            dir *= flickSensitivity;

            // 最大パワー制限
            dir = Vector3.ClampMagnitude(dir, maxPower);

            //  矢印の向き
            if (dir != Vector3.zero)
            {
                // 矢印の向き
                arrow.rotation = Quaternion.LookRotation(dir);

                // プレイヤーの向き（補正付き）
                Vector3 lookDir = dir;
                lookDir.y = 0;

                // 基本の向き
                Quaternion baseRot = Quaternion.LookRotation(lookDir);
 
                transform.rotation = baseRot * Quaternion.Euler(-90f, -180f, 0f);
            }
            //  矢印の長さ 
            float powerPercent = dir.magnitude / maxPower;
            float length = powerPercent * arrowMaxLength;

            arrow.localScale = new Vector3(2f, 2f, length);

            //  矢印の位置 
            arrow.position = transform.position + dir.normalized * length * 0.5f;
        }

        //  離した瞬間 
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            currentPos = GetMouseWorldPosition();

            Vector3 dir = startPos - currentPos;
            dir.y = 0;

            // ドラッグ制限
            dir = Vector3.ClampMagnitude(dir, maxDragDistance);
            dir *= flickSensitivity;
            dir = Vector3.ClampMagnitude(dir, maxPower);

            // 力を加える（逆方向に飛ばす）
            rb.AddForce(-dir * power, ForceMode.Impulse);

            // フリック回数を増やす
            currentFlickCount++;

            // 状態リセット
            isDragging = false;

            // フリック直後は一旦不可にする
            canFlick = false;

            // クールタイムリセット
            flickTimer = 0f;

            // 矢印を非表示
            arrow.gameObject.SetActive(false);
        }
    }

    // ===== マウス位置 → ワールド座標（床）=====
    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        // Y=0の床との交点を取得
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}