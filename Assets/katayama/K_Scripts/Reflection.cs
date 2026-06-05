using UnityEngine;

public class Reflection : MonoBehaviour
{
    [Header("敵を吹っ飛ばす力")]
    [SerializeField] private float knockbackPower = 10f;

    [Header("自分の反射速度")]
    [SerializeField] private float reflectSpeed = 10f;

    [Header("自分の反射距離")]
    [SerializeField] private float reflectDistance = 5f;

    [Header("速度による反射距離補正")]
    [SerializeField] private float baseSpeed = 10f;

    [SerializeField] private float maxDistanceMultiplier = 3f;

    [Header("通常時の反射距離倍率")]
    [Range(0f, 1f)]
    [SerializeField] private float reflectRate = 1f;

    [Header("後ろヒット時の反射距離倍率")]
    [Range(0f, 1f)]
    [SerializeField]
    private float backHitReflectMultiplier = 0.3f;

    [Header("最低反射角（度）")]
    [Range(0f, 89f)]
    [SerializeField] private float minimumReflectAngle = 30f;

    [Header("停止判定")]
    [SerializeField] private float stopThreshold = 0.1f;

    [Header("ホーミング設定")]
    [SerializeField] private float autoAimRadius = 10f;

    [Range(0f, 1f)]
    [SerializeField] private float homingStrength = 0.8f;

    [Header("敵タグ")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("後ろ判定距離")]
    [SerializeField] private float behindCheckRadius = 1.5f;

    private Rigidbody myRb;
    private OhajikiFlick ohajikiFlick;

    private float remainingDistance = 0f;

    private Vector3 reflectDirection;

    private float currentReflectSpeed = 0f;

    // 後ろ側にいたか
    private bool wasBehindEnemy = false;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
        ohajikiFlick = GetComponent<OhajikiFlick>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (myRb == null) return;
        if (collision.contactCount == 0) return;

        // 止まっていたら反射しない
        if (myRb.linearVelocity.magnitude < stopThreshold)
        {
            return;
        }

        Rigidbody enemyRb =
            collision.gameObject.GetComponent<Rigidbody>();

        Vector3 normal =
            collision.contacts[0].normal;

        // =========================
        // 当たった瞬間の速度
        // =========================
        Vector3 incoming =
            myRb.linearVelocity;

        float hitSpeed =
            incoming.magnitude;

        // =========================
        // 敵を吹っ飛ばす
        // =========================
        if (enemyRb != null)
        {
            enemyRb.AddForce(
                incoming.normalized *
                knockbackPower,
                ForceMode.Impulse
            );
        }

        // =========================
        // フリック攻撃状態にする
        // =========================
        ohajikiFlick.isAttacking = true;
        //ohajikiFlick.attackTimer = 0f;

        // =========================
        // 通常反射
        // =========================
        Vector3 reflected =
            Vector3.Reflect(
                incoming.normalized,
                normal
            );

        Vector3 tangent =
            Vector3.ProjectOnPlane(
                reflected,
                normal
            ).normalized;

        if (tangent.sqrMagnitude < 0.0001f)
        {
            reflectDirection =
                reflected.normalized;
        }
        else
        {
            Vector3 away =
                normal.normalized;

            float angleRad =
                minimumReflectAngle *
                Mathf.Deg2Rad;

            reflectDirection =
                tangent * Mathf.Cos(angleRad) +
                away * Mathf.Sin(angleRad);

            reflectDirection.Normalize();

            if (Vector3.Dot(
                reflectDirection,
                normal) < 0f)
            {
                reflectDirection =
                    Vector3.Reflect(
                        reflectDirection,
                        normal
                    );
            }
        }

        // =========================
        // 近くの敵を検索
        // =========================
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                autoAimRadius
            );

        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(enemyTag))
                continue;

            if (hit.gameObject == collision.gameObject)
                continue;

            if (hit.gameObject == gameObject)
                continue;

            Vector3 toEnemy =
                (
                    hit.transform.position -
                    transform.position
                ).normalized;

            float dot =
                Vector3.Dot(
                    reflectDirection,
                    toEnemy
                );

            if (dot < 0.3f)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    hit.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance =
                    distance;

                nearestEnemy =
                    hit.transform;
            }
        }

        // =========================
        // ホーミング補正
        // =========================
        if (nearestEnemy != null)
        {
            Vector3 targetDirection =
                (
                    nearestEnemy.position -
                    transform.position
                ).normalized;

            float speedHomingMultiplier =
                Mathf.Clamp(
                    hitSpeed / baseSpeed,
                    0f,
                    2f
                );

            float finalHomingStrength =
                Mathf.Clamp01(
                    homingStrength *
                    speedHomingMultiplier
                );

            reflectDirection =
                Vector3.Lerp(
                    reflectDirection,
                    targetDirection,
                    finalHomingStrength
                ).normalized;
        }

        // =========================
        // 後ろヒットなら減衰
        // =========================
        float finalReflectRate =
            reflectRate;

        if (wasBehindEnemy)
        {
            finalReflectRate *=
                backHitReflectMultiplier;
        }

        // =========================
        // 反射速度
        // =========================
        currentReflectSpeed =
            reflectSpeed;

        if (currentReflectSpeed <= 0.01f)
        {
            myRb.linearVelocity =
                Vector3.zero;

            return;
        }

        // =========================
        // 反射距離
        // =========================
        float speedMultiplier =
            Mathf.Sqrt(
                hitSpeed / baseSpeed
            );

        speedMultiplier =
            Mathf.Clamp(
                speedMultiplier,
                0.3f,
                maxDistanceMultiplier
            );

        remainingDistance =
            reflectDistance *
            finalReflectRate *
            speedMultiplier;

        // =========================
        // 反射開始
        // =========================
        myRb.linearVelocity =
            reflectDirection *
            currentReflectSpeed;
    }

    private void FixedUpdate()
    {
        // =========================
        // 後ろ側判定を事前保存
        // =========================

        Collider[] enemies =
            Physics.OverlapSphere(
                transform.position,
                behindCheckRadius
            );

        wasBehindEnemy = false;

        foreach (Collider enemy in enemies)
        {
            if (!enemy.CompareTag(enemyTag))
                continue;

            Vector3 enemyForward =
                enemy.transform.forward;

            Vector3 enemyToPlayer =
                (
                    transform.position -
                    enemy.transform.position
                ).normalized;

            float dot =
                Vector3.Dot(
                    enemyForward,
                    enemyToPlayer
                );

            // 後ろ側
            if (dot < -0.3f)
            {
                wasBehindEnemy = true;
                break;
            }
        }

        if (remainingDistance <= 0f)
            return;

        float moveDistance =
            currentReflectSpeed *
            Time.fixedDeltaTime;

        remainingDistance -=
            moveDistance;

        if (remainingDistance <= 0f)
        {
            myRb.linearVelocity =
                Vector3.zero;

            remainingDistance = 0f;

            currentReflectSpeed = 0f;
        }
        else
        {
            myRb.linearVelocity =
                reflectDirection *
                currentReflectSpeed;
        }
    }

    // ホーミング範囲表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            autoAimRadius
        );

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(
            transform.position,
            behindCheckRadius
        );
    }
}