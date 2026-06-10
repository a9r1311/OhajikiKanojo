using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class OhajikiFlick : MonoBehaviour
{
    Rigidbody rb;
    private ScoreDirector scoreDirector;  //スコアディレクターへの参照
    [SerializeField] GameStop gameStop;

    Vector3 startPos;
    Vector3 currentPos;
    Vector3 startPosition;
    Quaternion startRotation;

    public bool isDragging = false;
    bool canFlick = true;
    public bool isAttacking = false;

    [Header("リスポーン地点")]
    [SerializeField]
    Transform spawnPoint;

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

    [Header("再フリック可能な速度")]
    [SerializeField]
    float stopVelocity = 0.5f;

    [Header("フリック再許可")]
    [SerializeField] float flickCooldown = 0.2f;

    [Header("キャンセル判定")]
    [SerializeField] float cancelDistance = 0.2f;

    [Header("ためショット")]
    [SerializeField] float maxChargeTime = 2f;
    [SerializeField] float chargeMultiplier = 3f;

    [Header("チャージエフェクト")]
    [SerializeField]
    GameObject chargeEffectPrefab;

    GameObject currentChargeEffect;

    [Header("モデル向き補正")]
    [Tooltip("モデルが逆を向く場合は Y を 180 にする")]
    [SerializeField]
    Vector3 modelRotationOffset =
        new Vector3(-90f, -180f, 0f);

    [Header("予測線")]
    [SerializeField]
    LineRenderer predictionLine;

    [Header("リザルトUI")]
    [SerializeField] GameObject resultUI;

    [SerializeField]
    int predictionPoints = 30;

    [SerializeField]
    float predictionStep = 0.1f;

    float chargeTime = 0f;
    float flickTimer = 0f;
    int currentFlickCount = 0;

    float attackTimer = 0f;
    float attackTime = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreDirector = GetComponent<ScoreDirector>();

        startPosition =
            transform.position;

        startRotation =
            transform.rotation;

        // 矢印を最初は非表示
        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }

        // 回転固定
        rb.constraints =
            RigidbodyConstraints.FreezeRotation |
            RigidbodyConstraints.FreezePositionY;

        if (predictionLine != null)
        {
            predictionLine.enabled = false;
        }
    }

    void Update()
    {
        if (Mouse.current == null ) return;

        // ★リザルト中は完全停止
        if (gameStop != null)
        {
            if ((resultUI != null && resultUI.activeSelf) || gameStop.isGameStop)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                isDragging = false;
                canFlick = false;

                if (arrow != null)
                    arrow.gameObject.SetActive(false);

                if (predictionLine != null)
                    predictionLine.enabled = false;

                if (currentChargeEffect != null)
                {
                    Destroy(currentChargeEffect);
                }

                return;
            }
        }

        if (Mouse.current == null) return;

        // クールタイム
        flickTimer += Time.deltaTime;

        // クールタイムだけで再フリック可能
        canFlick =
            flickTimer > flickCooldown &&
            rb.linearVelocity.magnitude < stopVelocity;

        // 攻撃状態の解除
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer > attackTime &&
                rb.linearVelocity.magnitude < 0.01f)
            {
                isAttacking = false;  // 攻撃状態解除
                scoreDirector.chainCount = 0;  // チェインリセット
                scoreDirector.playerKnockScore = 0;  // ノックバックスコアリセット
                attackTimer = 0f;
            }
        }

        // =========================
        // 押した瞬間
        // =========================
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!canFlick) return;
            if (currentFlickCount >= maxFlickCount) return;

            startPos = GetMouseWorldPosition();

            isDragging = true;
            chargeTime = 0f;

            if (arrow != null)
            {
                arrow.gameObject.SetActive(true);
            }

            if (chargeEffectPrefab != null)
            {
                currentChargeEffect =
                    Instantiate(
                        chargeEffectPrefab,
                        transform.position,
                        Quaternion.identity,
                        transform
                    );
            }
        }

        // =========================
        // ドラッグ中
        // =========================
        if (isDragging)
        {
            // ため時間
            chargeTime += Time.deltaTime;

            float chargeRate = Mathf.Clamp01(
                chargeTime /
                maxChargeTime
            );

            float chargePower =
                Mathf.Lerp(
                    1f,
                    chargeMultiplier,
                    chargeRate
                );

            float finalPower =
                power *
                chargePower;

            chargeTime = Mathf.Clamp(
                chargeTime,
                0f,
                maxChargeTime
            );

            currentPos =
                GetMouseWorldPosition();

            Vector3 dir =
                currentPos -
                transform.position;

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

            if (dir.magnitude > cancelDistance)
            {
                DrawPrediction(
                    dir,
                    finalPower
                );
            }
            else if (
                predictionLine != null)
            {
                predictionLine.enabled =
                    false;
            }

            // キャンセル判定
            bool isCanceling =
                dir.magnitude <
                cancelDistance;


            // 矢印表示
            if (arrow != null)
            {
                arrow.gameObject.SetActive(
                    !isCanceling
                );
            }

            // =========================
            // 向き
            // =========================
            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(dir) *
                    Quaternion.Euler(
                        modelRotationOffset
                    );

                if (arrow != null)
                {
                    arrow.rotation =
                        Quaternion.LookRotation(dir);
                }
            }

            // =========================
            // 矢印サイズ
            // =========================
            if (arrow != null)
            {
                float powerPercent =
                    dir.magnitude /
                    maxPower;

                float length =
                    powerPercent *
                    arrowMaxLength;

                //float chargeRate =
                //    chargeTime /
                //    maxChargeTime;

                arrow.localScale =
                    new Vector3(
                        2f,
                        2f,
                        length *
                        (1f + chargeRate)
                    );

                // 矢印位置
                if (dir.sqrMagnitude > 0.0001f)
                {
                    arrow.position =
                        transform.position +
                        dir.normalized *
                        length *
                        0.5f;
                }

                if (currentChargeEffect != null)
                {
                    currentChargeEffect.transform.position =
                        transform.position;
                }

            }
        }

        // =========================
        // 離した瞬間
        // =========================
        if (
            Mouse.current.leftButton
            .wasReleasedThisFrame &&
            isDragging
        )
        {
            currentPos =
                GetMouseWorldPosition();

            Vector3 dir =
                currentPos -
                transform.position;

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
            if (dir.magnitude <
                cancelDistance)
            {
                isDragging = false;

                if (predictionLine != null)
                {
                    predictionLine.enabled = false;
                }

                if (arrow != null)
                {
                    arrow.gameObject
                        .SetActive(false);
                }

                if (currentChargeEffect != null)
                {
                    Destroy(currentChargeEffect);
                }

                return;
            }

            // ため倍率
            float chargeRate =
                chargeTime /
                maxChargeTime;

            float chargePower =
                Mathf.Lerp(
                    1f,
                    chargeMultiplier,
                    chargeRate
                );

            // 最終威力
            float finalPower =
                power * chargePower;

            // =========================
            // ノックバック中でも
            // フリックを優先
            // =========================

            // 今の速度を消す
            rb.linearVelocity =
                Vector3.zero;

            // フリック方向へ発射
            rb.AddForce(
                dir.normalized *
                finalPower,
                ForceMode.Impulse
            );
            if (predictionLine != null)
            {
                predictionLine.enabled = false;
            }

            // 攻撃状態へ
            isAttacking = true;
            attackTimer = 0f;

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

            if (currentChargeEffect != null)
            {
                Destroy(currentChargeEffect);
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
            Camera.main
            .ScreenPointToRay(mousePos);

        Plane plane =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    transform.position.y,
                    0f
                )
            );

        if (plane.Raycast(
            ray,
            out float distance
        ))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    public void Retry()
    {
        // 一旦物理停止
        rb.isKinematic = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // リスポーン
        if (spawnPoint != null)
        {
            transform.SetPositionAndRotation(
                spawnPoint.position,
                spawnPoint.rotation
            );
        }
        else
        {
            transform.SetPositionAndRotation(
                startPosition,
                startRotation
            );
        }

        // 状態リセット
        currentFlickCount = 0;
        canFlick = true;
        flickTimer = flickCooldown;
        isDragging = false;
        isAttacking = false;
        chargeTime = 0f;

        if (arrow != null)
            arrow.gameObject.SetActive(false);

        if (currentChargeEffect != null)
            Destroy(currentChargeEffect);

        if (predictionLine != null)
            predictionLine.enabled = false;

        // 次フレームで物理再開
        StartCoroutine(EnablePhysicsNextFrame());
    }

    IEnumerator EnablePhysicsNextFrame()
    {
        yield return null;

        rb.isKinematic = false;
    }

    void DrawPrediction(
    Vector3 dir,
    float finalPower
)
    {
        if (predictionLine == null)
            return;

        predictionLine.enabled = true;

        predictionLine.positionCount =
            predictionPoints;

        Vector3 position =
            transform.position;

        Vector3 velocity =
            dir.normalized * finalPower;

        float drag =
            rb.linearDamping;

        for (int i = 0;
             i < predictionPoints;
             i++)
        {
            predictionLine.SetPosition(
                i,
                position
            );

            position +=
                velocity *
                predictionStep;

            velocity *=
                Mathf.Clamp01(
                    1f -
                    drag *
                    predictionStep
                );
        }
    }
}