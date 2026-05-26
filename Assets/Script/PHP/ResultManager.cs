using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private ScoreDirector scoreDirector;
    [SerializeField] private GameObject resultInput;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI scoreResultText;
    [SerializeField] private RankingManager rankingManager;

    private int finalScore;  //最終スコア

    void Start()
    {
        resultInput.SetActive(false);
    }

    //リザルト+名前入力UI表示
    public void ResultView()
    {
        //名前入力UIを表示
        resultInput.SetActive(true);

        finalScore = scoreDirector.GetScore();

        scoreResultText.text = $"Result:" + finalScore;
    }

    //送信ボタンがクリックされたときの処理
    public void OnClickSendButton()
    {
        string playerName = nameInputField.text;  //入力された名前を取得

        //未入力対策
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "NoName";
        }

        //スコア送信
        rankingManager.SendScore(playerName, finalScore);
        //名前入力UIを非表示
        resultInput.SetActive(false);
    }
}
