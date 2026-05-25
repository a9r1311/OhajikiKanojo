using UnityEngine;

public class CullingGroupManager : MonoBehaviour    //  カメラ外のオブジェクトを非表示
{
    private CullingGroup _cullingGroup;
    private BoundingSphere[] _spheres;

    // 管理したいゲームオブジェクトのリスト
    public GameObject[] targets;

    void Awake()
    {
        
    }

    void Start()
    {
        // 1. CullingGroupの初期化
        _cullingGroup = new CullingGroup();
        _cullingGroup.targetCamera = Camera.main;

        // 2. 判定用の球体配列を作成（ターゲットと同じ数）
        _spheres = new BoundingSphere[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            // 各オブジェクトの座標と「判定半径」を設定
            _spheres[i] = new BoundingSphere(targets[i].transform.position, 1.0f);
        }

        // 3. 配列をセット
        _cullingGroup.SetBoundingSpheres(_spheres);
        _cullingGroup.SetBoundingSphereCount(targets.Length);

        // 4. 状態が変わった時のイベントを登録
        _cullingGroup.onStateChanged = OnStateChanged;
    }

    // 5. 状態が変化した個体だけ、インデックス番号で通知が来る
    private void OnStateChanged(CullingGroupEvent ev)
    {
        // ev.index が、配列の何番目かを示している
        GameObject target = targets[ev.index];

        if (ev.isVisible)
        {
            // 画面内に入った：レンダラーやスクリプトをONにする
            target.SetActive(true);
        }
        else
        {
            // 画面外に出た：OFFにする
            target.SetActive(false);
        }
    }

    void Update()
    {
        // 6. オブジェクトが動く場合は、球体の座標も同期させる
        for (int i = 0; i < _spheres.Length; i++)
        {
            _spheres[i].position = targets[i].transform.position;
        }
    }

    void OnDestroy()
    {
        // 7. メモリリーク防止のため必ず破棄
        _cullingGroup.Dispose();
        _cullingGroup = null;
    }
}