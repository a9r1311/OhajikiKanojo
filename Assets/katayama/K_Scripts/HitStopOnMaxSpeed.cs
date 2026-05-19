using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitStopOnMaxSpeed : MonoBehaviour
{
    [Header("プレイヤーの Rigidbody")]
    [SerializeField] Rigidbody playerRb;

    [Header("カメラシェイク")]
    [SerializeField] CameraDirection cameraDirection;

    [Header("カメラシェイク設定")]
    [SerializeField] float shakePower = 5.0f;
    [SerializeField] float shakeTime = 0.2f;

    [Header("最高速度")]
    [SerializeField] float maxSpeed = 20f;

    [Header("最高速度とみなす許容範囲")]
    [SerializeField] float speedTolerance = 0.5f;

    [Header("ヒットストップ時間")]
    [SerializeField] float hitStopDuration = 0.15f;

    [Header("同じ敵への再ヒット待機時間")]
    [SerializeField] float sameEnemyCooldown = 1.0f;

    [Header("ヒットエフェクト")]
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] float effectDestroyTime = 2.0f;

    // 現在ヒットストップ中か
    bool isHitStopping = false;

    // 衝突直前の速度を保存
    float previousSpeed = 0f;

    // 敵ごとの最後のヒット時刻
    Dictionary<GameObject, float> lastHitTimes =
        new Dictionary<GameObject, float>();

    void Start()
    {
        if (playerRb == null)
        {
            playerRb = GetComponent<Rigidbody>();
        }

        if (cameraDirection == null)
        {
            cameraDirection = FindAnyObjectByType<CameraDirection>();
        }
    }

    void Update()
    {
        // 毎フレーム、衝突直前の速度を保存
        if (playerRb != null)
        {
            previousSpeed = playerRb.linearVelocity.magnitude;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Enemyタグ以外は無視
        if (!collision.gameObject.CompareTag("Enemy"))
            return;

        if (playerRb == null)
            return;

        GameObject enemy = collision.gameObject;

        // 同じ敵への連続ヒットを防止
        if (lastHitTimes.ContainsKey(enemy))
        {
            if (Time.time - lastHitTimes[enemy] < sameEnemyCooldown)
            {
                return;
            }
        }

        // 衝突直前の速度で判定
        float currentSpeed = previousSpeed;

        // 最高速度付近で衝突した場合のみ発動
        if (currentSpeed < maxSpeed - speedTolerance)
            return;

        // 最後のヒット時刻を記録
        lastHitTimes[enemy] = Time.time;

        // =========================
        // エフェクト再生
        // =========================
        if (hitEffectPrefab != null &&
            collision.contactCount > 0)
        {
            // 衝突地点
            Vector3 hitPoint =
                collision.contacts[0].point;

            // 衝突面の法線方向に向ける
            Quaternion hitRotation =
                Quaternion.LookRotation(
                    collision.contacts[0].normal
                );

            // エフェクト生成
            GameObject effect =
                Instantiate(
                    hitEffectPrefab,
                    hitPoint,
                    hitRotation
                );

            // 一定時間後に削除
            Destroy(effect, effectDestroyTime);
        }

        // カメラシェイク
        if (cameraDirection != null)
        {
            Debug.Log("カメラシェイク確認");
            cameraDirection.Shake(
                shakePower,
                shakeTime
            );
        }

        // ヒットストップ
        if (!isHitStopping)
        {
            Debug.Log("ヒットストップ確認");
            Rigidbody enemyRb =
                collision.rigidbody;

            StartCoroutine(
                HitStop(
                    playerRb,
                    enemyRb
                )
            );
        }
    }

    IEnumerator HitStop(
        Rigidbody player,
        Rigidbody enemy
    )
    {
        isHitStopping = true;

        Vector3 playerVelocity =
            player != null
            ? player.linearVelocity
            : Vector3.zero;

        Vector3 enemyVelocity =
            enemy != null
            ? enemy.linearVelocity
            : Vector3.zero;

        // 停止
        if (player != null)
        {
            player.linearVelocity =
                Vector3.zero;
            player.isKinematic = true;
        }

        if (enemy != null)
        {
            enemy.linearVelocity =
                Vector3.zero;
            enemy.isKinematic = true;
        }

        // 実時間で待機
        yield return new WaitForSecondsRealtime(
            hitStopDuration
        );

        // 復帰
        if (player != null)
        {
            player.isKinematic = false;
            player.linearVelocity =
                playerVelocity;
        }

        if (enemy != null)
        {
            enemy.isKinematic = false;
            enemy.linearVelocity =
                enemyVelocity;
        }

        isHitStopping = false;
    }
}