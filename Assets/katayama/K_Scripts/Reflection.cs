using UnityEngine;

public class Reflection : MonoBehaviour
{
    [Header("敵を吹っ飛ばす力")]
    [SerializeField] float knockbackPower = 10f;

    [Header("自分の反射係数")]
    [SerializeField] float reflectPower = 0.9f;

    private void OnCollisionEnter(Collision collision)
    {
        // ===== 相手のRigidbody =====
        Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();

        // ===== 自分のRigidbody =====
        Rigidbody myRb = GetComponent<Rigidbody>();

        // Rigidbodyどっちも無かったら何もしない
        if (enemyRb == null || myRb == null) return;

        // ===== 衝突面の法線 =====
        Vector3 normal = collision.contacts[0].normal;

        //  敵を吹っ飛ばす
        // 法線方向に飛ばす（壁から離れる方向）
        enemyRb.AddForce(myRb.linearVelocity.normalized * knockbackPower, ForceMode.Impulse);

        //  自分を反射させる
        Vector3 incoming = myRb.linearVelocity;

        Vector3 reflectDir = Vector3.Reflect(incoming, normal);

        myRb.linearVelocity = reflectDir * reflectPower;
    }
}