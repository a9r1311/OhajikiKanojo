using UnityEngine;

public class ScoreDirector : MonoBehaviour
{
    private Rigidbody playerRb;
    [SerializeField] private int score = 0;  //スコア

    private const float maxSpeed = 110.2917f;  //プレイヤーの最大速度
    private float currentSpeed = 0f;  //現在の速度
    private const int maxBonus = 400;  //最大ボーナス

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        //スコア初期化
        score = 0;
    }

    private void FixedUpdate()
    {
        currentSpeed = playerRb.linearVelocity.magnitude;
    }

    public int AddScore()
    {
        //Debug.Log("Speed: " + currentSpeed);
        //ボーナス計算(０～４００点)
        int bonus = Mathf.RoundToInt(currentSpeed / maxSpeed * maxBonus);
        bonus = Mathf.Clamp(bonus, 0, maxBonus);
        Debug.Log("Bonus: " + bonus);

        //スコア加算
        score += 100 + bonus;

        return 100 + bonus;
    }

    public int ChainScore(int enemyScore) 
    {
        //スコア加算(二倍)
        score += enemyScore * 2;
        Debug.Log("Chain Score: " + (enemyScore * 2));

        return enemyScore * 2;
    }
}
