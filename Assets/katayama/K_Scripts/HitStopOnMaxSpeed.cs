using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitStopOnMaxSpeed : MonoBehaviour
{
    [Header("プレイヤーの Rigidbody")]
    [SerializeField] Rigidbody playerRb;

    [Header("カメラシェイク")]
    [SerializeField] CameraDirection cameraDirection;

    [Header("エフェクト用カメラ")]
    [SerializeField] Camera effectCamera;

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

    [Header("通常ヒットエフェクト")]
    [SerializeField]
    GameObject normalHitEffect;

    [Header("ヒットエフェクト")]
    [Tooltip("0番目 = 衝突地点エフェクト")]
    [SerializeField]
    GameObject[] hitEffectPrefabs;

    [SerializeField]
    float effectDestroyTime = 2.0f;

    [Header("初期エフェクト数")]
    [SerializeField]
    int startEffectCount = 1;

    [Header("連鎖終了までの時間")]
    [SerializeField]
    float comboResetTime = 2f;

    // 現在のエフェクト数
    int currentEffectCount;

    // 最後にヒットした時間
    float lastHitTime = 0f;

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
            cameraDirection =
                FindAnyObjectByType<CameraDirection>();
        }

        // カメラ未設定なら MainCamera
        if (effectCamera == null)
        {
            effectCamera = Camera.main;
        }

        // 初期化
        currentEffectCount =
            startEffectCount;
    }

    void Update()
    {
        // 毎フレーム、
        // 衝突直前の速度を保存
        if (playerRb != null)
        {
            previousSpeed =
                playerRb.linearVelocity.magnitude;
        }

        // =========================
        // 連鎖終了判定
        // =========================
        if (Time.time - lastHitTime >
            comboResetTime)
        {
            currentEffectCount =
                startEffectCount;
        }
    }

    private void OnCollisionEnter(
        Collision collision
    )
    {
        // Enemyタグ以外は無視
        if (!collision.gameObject
            .CompareTag("Enemy"))
            return;

        if (playerRb == null)
            return;

        GameObject enemy =
            collision.gameObject;

        // 同じ敵への連続ヒット防止
        if (lastHitTimes.ContainsKey(enemy))
        {
            if (Time.time -
                lastHitTimes[enemy]
                < sameEnemyCooldown)
            {
                return;
            }
        }

        // 衝突直前の速度
        float currentSpeed =
            previousSpeed;

        // =========================
        // 通常ヒット
        // =========================
        if (currentSpeed <
            maxSpeed - speedTolerance)
        {
            if (normalHitEffect != null &&
                collision.contactCount > 0)
            {
                AudioManager.Instance.PlaySE(0);

                Vector3 hitPoint =
                    collision.contacts[0].point;

                GameObject effect =
                    Instantiate(
                        normalHitEffect,
                        hitPoint,
                        Quaternion.identity
                    );

                Destroy(
                    effect,
                    effectDestroyTime
                );
            }

            return;
        }

        // =========================
        // ここから最高速度ヒット
        // =========================

        // 最後のヒット時刻記録
        lastHitTimes[enemy] =
            Time.time;

        lastHitTime =
            Time.time;

        // =========================
        // ヒットストップエフェクト
        // =========================
        if (hitEffectPrefabs != null &&
            hitEffectPrefabs.Length > 0 &&
            collision.contactCount > 0)
        {
            Vector3 hitPoint =
                collision.contacts[0].point;

            AudioManager.Instance.PlaySE(1);

            // 0番目のPrefab
            if (hitEffectPrefabs[0] != null)
            {
                GameObject firstEffect =
                    Instantiate(
                        hitEffectPrefabs[0],
                        hitPoint,
                        Random.rotation
                    );

                Destroy(
                    firstEffect,
                    effectDestroyTime
                );
            }

            // カメラ演出用
            for (int i = 1;
                 i < currentEffectCount;
                 i++)
            {
                Vector3 viewportPos =
                    new Vector3(
                        Random.Range(0.15f, 0.85f),
                        Random.Range(0.15f, 0.85f),
                        10f
                    );

                Vector3 spawnPos =
                    effectCamera
                    .ViewportToWorldPoint(
                        viewportPos
                    );

                int randomIndex =
                    Random.Range(
                        0,
                        hitEffectPrefabs.Length
                    );

                GameObject randomPrefab =
                    hitEffectPrefabs[
                        randomIndex
                    ];

                if (randomPrefab == null)
                    continue;

                GameObject effect =
                    Instantiate(
                        randomPrefab,
                        spawnPos,
                        Random.rotation
                    );

                Destroy(
                    effect,
                        effectDestroyTime
                    );
            }

            currentEffectCount++;
        }

        // カメラシェイク
        if (cameraDirection != null)
        {
            cameraDirection.Shake(
                shakePower,
                shakeTime
            );
        }

        // ヒットストップ
        if (!isHitStopping)
        {
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

        // 待機
        yield return
            new WaitForSecondsRealtime(
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