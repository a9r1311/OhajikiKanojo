using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    [SerializeField] private Transform target; // 追跡するキャラクター
    [SerializeField] private float margin = 0.05f; // 画面端からの余裕

    private RectTransform rectTransform;
    public Camera mainCam;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. ワールド座標をビューポート座標(0~1)に変換
        Vector3 viewportPos = mainCam.WorldToViewportPoint(target.position);

        // 2. 画面内かどうかの判定
        bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;

        if (isOffScreen)
        {
            // UIを表示
            rectTransform.gameObject.SetActive(true);

            // カメラの背後にいる場合の補正
            if (viewportPos.z < 0)
            {
                viewportPos.x = 1f - viewportPos.x;
                viewportPos.y = 1f - viewportPos.y;
            }

            // 3. 画面端にクランプ（固定）
            float screenX = Mathf.Clamp(viewportPos.x, margin, 1f - margin);
            float screenY = Mathf.Clamp(viewportPos.y, margin, 1f - margin);

            // 4. ビューポート座標をスクリーン（Canvas）座標に変換して代入
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(screenX, screenY);
            rectTransform.anchoredPosition = Vector2.zero;

            // 5. 矢印をキャラクターの方に向かせる（オプション）
            Vector2 center = new Vector2(0.5f, 0.5f);
            Vector2 direction = (new Vector2(screenX, screenY) - center).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90); // 画像の向きに合わせて調整
        }
        else
        {
            // 画面内ならUIを隠す
            rectTransform.gameObject.SetActive(false);
        }
    }
}