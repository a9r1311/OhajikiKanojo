using TMPro;
using UnityEngine;

public class ScoreDirector : MonoBehaviour
{
    private Rigidbody playerRb;
    [SerializeField] private TextMeshProUGUI scoreText;  //スコア表示用テキスト
    private int score = 0;  //スコア
    public int chainCount = 0;  //連鎖数

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

    //スコア表示更新
    private void ScoreTextUpdate()
    {
        string scoreString = "";  //スコアを7桁表示するための文字列

        //スコアを7桁表示するための0埋め
        for (int i = 7; i > score.ToString().Length; i--)
        {
            scoreString += "0";
        }

        //スコア表示更新
        scoreText.text = scoreString + score;
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

        //スコア表示更新
        ScoreTextUpdate();

        return 100 + bonus;
    }

    public int ChainScore(int enemyScore, int magnification)
    {
        //スコア加算
        score += enemyScore * magnification;
        Debug.Log("Chain Score: " + (enemyScore * magnification));

        //スコア表示更新
        ScoreTextUpdate();

        return enemyScore * magnification;
    }

    public int GetScore()
    {
        return score;
    }
}
