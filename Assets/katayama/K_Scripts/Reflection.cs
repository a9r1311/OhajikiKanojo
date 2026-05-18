using UnityEngine;

public class Reflection : MonoBehaviour
{
    [Header("敵を吹っ飛ばす力")]
    [SerializeField] private float knockbackPower = 10f;

    [Header("自分の反射速度")]
    [SerializeField] private float reflectSpeed = 10f;

    [Header("自分の反射距離")]
    [SerializeField] private float reflectDistance = 5f;

    [Header("反射時の減衰率（0～1）")]
    [Tooltip("1 = 減衰なし / 0.5 = 半分の速度 / 0 = 反射しない")]
    [Range(0f, 1f)]
    [SerializeField] private float reflectRate = 0.9f;

    [Header("最低反射角（度）")]
    [Tooltip("浅い角度で当たっても、この角度以上で跳ね返る")]
    [Range(0f, 89f)]
    [SerializeField] private float minimumReflectAngle = 30f;

    private Rigidbody myRb;
    private float remainingDistance = 0f;
    private Vector3 reflectDirection;
    private float currentReflectSpeed = 0f;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

        if (myRb == null) return;
        if (collision.contactCount == 0) return;

        Vector3 normal = collision.contacts[0].normal;

        // =========================
        // 敵を吹っ飛ばす
        // =========================
        if (enemyRb != null && myRb.linearVelocity.sqrMagnitude > 0.01f)
        {
            enemyRb.AddForce(
                myRb.linearVelocity.normalized * knockbackPower,
                ForceMode.Impulse
            );
        }

        // =========================
        // 自分を反射させる
        // =========================
        Vector3 incoming = myRb.linearVelocity;

        if (incoming.sqrMagnitude < 0.01f) return;

        // 通常の反射方向
        Vector3 reflected = Vector3.Reflect(incoming.normalized, normal);

        // 壁に沿う方向（法線成分を除いた方向）
        Vector3 tangent = Vector3.ProjectOnPlane(reflected, normal).normalized;

        // tangent がゼロの場合（真正面衝突）は通常反射をそのまま使用
        if (tangent.sqrMagnitude < 0.0001f)
        {
            reflectDirection = reflected.normalized;
        }
        else
        {
            // 壁から離れる方向（法線方向）
            Vector3 away = normal.normalized;

            // 最低角度を使って方向を再構成
            // 0° = 壁に沿う
            // 90° = 法線方向
            float angleRad = minimumReflectAngle * Mathf.Deg2Rad;

            reflectDirection =
                tangent * Mathf.Cos(angleRad) +
                away * Mathf.Sin(angleRad);

            reflectDirection.Normalize();

            // 反射方向が壁の外側を向くように保証
            if (Vector3.Dot(reflectDirection, normal) < 0f)
            {
                reflectDirection = Vector3.Reflect(reflectDirection, normal);
            }
        }

        // 減衰率を反映した反射速度
        currentReflectSpeed = reflectSpeed * reflectRate;

        if (currentReflectSpeed <= 0.01f)
        {
            myRb.linearVelocity = Vector3.zero;
            return;
        }

        // 指定した距離だけ移動
        remainingDistance = reflectDistance;

        // 反射開始
        myRb.linearVelocity = reflectDirection * currentReflectSpeed;

        Debug.Log(reflectDirection);
    }

    private void FixedUpdate()
    {
        if (remainingDistance <= 0f) return;

        float moveDistance = currentReflectSpeed * Time.fixedDeltaTime;
        remainingDistance -= moveDistance;

        if (remainingDistance <= 0f)
        {
            myRb.linearVelocity = Vector3.zero;
            remainingDistance = 0f;
            currentReflectSpeed = 0f;
        }
        else
        {
            myRb.linearVelocity = reflectDirection * currentReflectSpeed;
        }
    }
}