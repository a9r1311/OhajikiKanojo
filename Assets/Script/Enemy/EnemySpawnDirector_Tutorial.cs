//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class EnemySpawnDirector_Tutorial : MonoBehaviour
//{
//    [SerializeField] private List<GameObject> spawnEnemy = new();  //スポーンする敵のリスト
//    [SerializeField] private GameObject target;  //敵の追尾ターゲット
//    [SerializeField] private GameStop gameStop;  //ゲームストップへの参照
//    [SerializeField] private ScoreDirector scoreDirector;

//    private List<Queue<GameObject>> waitingEnemies;  //オブジェクトプール用の待機中の敵のキューリスト
//    private int poolCount = 4;  //とりあえずこの数ずつ用意しておく
//    private List<GameObject> activeEnemies = new();  //現在アクティブな敵のリスト

//    private float initialSpawnInterval = 3.2f;  //スポーン間隔の初期値
//    private float currentSpawnInterval;  //スポーン間隔
//    private float minSpawnInterval = 0.35f;  //スポーン間隔の最小値
//    private float spawnIntervalDecreaseRate = 0.086f;  //スポーン間隔の減少率
//    private float spawnTimer = 0f;  //スポーンタイマー

//    [SerializeField] private bool isLongDistanceSpawn = false;  //遠距離スポーンフラグ
//    [SerializeField] private int spawnTypeCount = 1;  //スポーンさせる敵の種類数
//    private float increaseSpawnTypeInterval = 5.7f;  //スポーンさせる敵の種類を増やす間隔
//    private int spawnCounter = 0;  //スポーンカウンター

//    void Start()
//    {
//        //ターゲット確認
//        if (target == null)
//        {
//            target = GameObject.Find("Prince");

//            if (target == null)
//                Debug.LogError("Targetが設定されていません");
//        }

//        currentSpawnInterval = initialSpawnInterval;  //スポーン間隔を初期化

//        waitingEnemies = new List<Queue<GameObject>>();  //初期化
//        Vector3 waitingPos = new Vector3(100f, 0f, 100f);  //初期待機位置

//        for (int i = 0; i < spawnEnemy.Count; i++) 
//        {
//            //敵の種類ごとにオブジェクトプール用のキューを作成
//            waitingEnemies.Add(new Queue<GameObject>());

//            for (int j = 0; j < poolCount; j++)
//            {
//                //敵をスポーン
//                GameObject enemy = Instantiate(spawnEnemy[i], waitingPos, Quaternion.identity);
//                EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
//                //スポーンした敵に色々設定
//                enemyBase.target = target;
//                enemyBase.spawnDirector = this;
//                enemyBase.scoreDirector = scoreDirector;
//                enemyBase.gameStop = gameStop;
//                //敵を非アクティブにして待機キューに追加
//                waitingEnemies[enemyBase.id].Enqueue(enemy);
//                enemy.SetActive(false);
//            }
//        }
//    }

//    void Update()
//    {
//        //ゲームストップ中はスポーンさせない
//        if (gameStop == null)
//        {
//            Debug.LogWarning("GameStopへの参照が設定されていません。");
//        }
//        else if (gameStop.isGameStop)
//            return;

//        //スポーンタイマー
//        spawnTimer += Time.deltaTime;
//        if (spawnTimer >= currentSpawnInterval)
//        {
//            spawnTimer = 0f;

//            //スポーン間隔を徐々に短くする
//            if ((currentSpawnInterval -= spawnIntervalDecreaseRate) < minSpawnInterval)
//            {
//                Debug.Log(2);
//                currentSpawnInterval = minSpawnInterval;
//            }

//            if(spawnCounter >= increaseSpawnTypeInterval && spawnTypeCount < spawnEnemy.Count)
//            {
//                spawnCounter = 0;
//                spawnTypeCount++;  //スポーンさせる敵の種類を増やす
//            }
//            else
//            {
//                spawnCounter++;
//            }

//            //敵をスポーン
//            SpawnEnemy(false);

//            //遠距離スポーンの処理
//            if (isLongDistanceSpawn)
//            {
//                SpawnEnemy(true);
//            }
//        }
//    }

//    //敵をスポーンさせる
//    private void SpawnEnemy(bool longDistance)
//    {
//        if (spawnEnemy.Count > 0)
//        {
//            int index;
//            //出現させる敵をランダムに選択
//            if (!longDistance)
//                index = Random.Range(0, spawnTypeCount);
//            else
//                //遠距離スポーンは特定の敵しか出さないようにする
//                do
//                    index = Random.Range(0, spawnTypeCount);
//                while (index == spawnEnemy.Count - 1);  //遠距離スポーンは最後の敵を出さない
            

//            //出現座標をランダムに決定
//            Vector3 spawnPos = GetSpawnPosition(index, longDistance);

//            //敵をスポーン
//            if (waitingEnemies[index].Count <= 0)  //オブジェクトプールが足りない場合は新たにスポーン
//            {
//                //敵を生成
//                GameObject enemy = Instantiate(spawnEnemy[index], spawnPos, Quaternion.identity);
//                EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

//                //スポーンした敵に色々設定
//                enemyBase.target = target;
//                enemyBase.spawnDirector = this;
//                enemyBase.scoreDirector = scoreDirector;
//                enemyBase.gameStop = gameStop;
//                activeEnemies.Add(enemy);  //スポーンした敵をアクティブな敵のリストに追加
//            }
//            else  //オブジェクトプールから敵を出してスポーン
//            {
//                GameObject enemy = waitingEnemies[index].Dequeue();
//                enemy.transform.position = spawnPos;
//                enemy.SetActive(true);
//                enemy.GetComponent<EnemyBase>().moveState = EnemyBase.movePattern.Walk;  //行動パターンを歩きにする
//                activeEnemies.Add(enemy);  //スポーンした敵をアクティブな敵のリストに追加
//            }
//        }
//    }

//    //スポーン位置をランダムに決定
//    private Vector3 GetSpawnPosition(int id, bool longDistance)
//    {
//        float targetRadius = 9f;  //ターゲットを中心とした半径
//        float width = 25f;        //スポーン位置の横幅
//        float height = 18f;       //スポーン位置の奥行き
//        Vector3 targetPos = target.transform.position;  //ターゲットの位置
//        Vector3 spawnPos = Vector3.zero;

//        if (longDistance)
//        {
//            targetRadius = 20f;  //ターゲットを中心とした半径
//            width = 28f;         //スポーン位置の横幅
//            height = 35f;        //スポーン位置の奥行き
//        }

//        do
//        {
//            float x = Random.Range(targetPos.x - width / 2f, targetPos.x + width / 2f);
//            float z = Random.Range(targetPos.z - height / 2f, targetPos.z + height / 2f) + 6f;

//            spawnPos = new Vector3(x, 0.9f, z);

//        } while (Vector3.Distance(spawnPos, targetPos + new Vector3(0f, 0f, 2f)) < targetRadius
//                 || (id == 3 && spawnPos.z < targetPos.z));  //ターゲットから一定距離以上の位置かつ、id3はターゲットより前方なら通す

//        return spawnPos;
//    }

//    //敵をオブジェクトプールに戻す
//    public void ReturnEnemyToPool(GameObject enemy, int index)
//    {
//        waitingEnemies[index].Enqueue(enemy);
//    }

//    //アクティブな敵のリストから敵を削除
//    public void RemoveActiveEnemy(GameObject enemy)
//    {
//        activeEnemies.Remove(enemy);
//    }

//    //スポーン設定を初期化
//    public void SpawnReset()
//    {
//        spawnTimer = 0f;
//        currentSpawnInterval = initialSpawnInterval;
//        spawnCounter = 0;
//        spawnTypeCount = 1;

//        //アクティブな敵をすべてオブジェクトプールに戻す
//        foreach (var enemy in activeEnemies)
//        {
//            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();  //敵のEnemyBaseコンポーネントを取得

//            enemyBase.ResetState();  //敵の状態をリセット
//            ReturnEnemyToPool(enemy, enemyBase.id);  //敵をオブジェクトプールに戻す
//        }

//        activeEnemies.Clear();  //アクティブな敵のリストをクリア
//    }
//}
