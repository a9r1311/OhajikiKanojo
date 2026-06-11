using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private GameObject resultInput;
    [SerializeField] private GameObject resultOutput;
    [SerializeField] private GameObject clearText;
    [SerializeField] private GameObject gameoverText;
    [SerializeField] private UIChanger changer;
    [SerializeField] private ScoreDirector scoreDirector;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject dangerText;
    [SerializeField] private TextMeshProUGUI inputScoreResultText;
    [SerializeField] private TextMeshProUGUI outputScoreResultText;
    [SerializeField] private RankingManager rankingManager;

    private int finalScore;  //最終スコア

    void Start()
    {
        //リザルトとかを非表示
        resultInput.SetActive(false);
        resultOutput.SetActive(false);
        changer.rankingUI.SetActive(false);
        dangerText.SetActive(false);
    }

    //リザルト+名前入力UI表示
    public void ResultView(int hp, bool isClear)
    {
        //名前入力UIを表示
        resultInput.SetActive(true);

        //クリアかゲームオーバーか
        clearText.SetActive(isClear);
        gameoverText.SetActive(!isClear);

        //スコアを取得
        finalScore = scoreDirector.GetScore();

        //HPが残っている場合はボーナスポイントを加算
        if (hp >= 0)
        {
            finalScore += hp * 1000 + 10000;
        }

        //最終スコアをテキストに表示
        inputScoreResultText.text = $"Result:" + finalScore;
        outputScoreResultText.text = finalScore.ToString();
    }

    //送信ボタンがクリックされたときの処理
    public void ClickSendButton()
    {
        string playerName = nameInputField.text;  //入力された名前を取得

        //名前の文字数制限
        if (playerName.Length > 10)
        {
            dangerText.SetActive(true);
            return;
        }
        dangerText.SetActive(false);

        //未入力対策
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "NoName";
        }

        //スコア送信
        rankingManager.SendScore(playerName, finalScore);

        //名前入力UIを非表示
        resultInput.SetActive(false);
        //ランキング表示
        resultOutput.SetActive(true);
    }

    bool beforeUIIsTitle;  //前のUIを記憶する変数

    //ランキング表示
    public void ClickRankingButton(bool beforeIsTitle)
    {
        //ランキングUIを表示
        changer.rankingUI.SetActive(true);

        //前のUIを非表示にして記憶
        if (!beforeIsTitle)
        {
            resultOutput.SetActive(false);
            beforeUIIsTitle = false;
        }
        else
        {
            changer.titleUI.SetActive(false);
            beforeUIIsTitle = true;
        }
    }

    //ランキングから元のUIに戻る
    public void ClickBackRankingButton()
    {
        changer.rankingUI.SetActive(false);  //ランキングUIを非表示

        //前のUIを再表示
        if (!beforeUIIsTitle)
        {
            resultOutput.SetActive(true);
        }
        else
        {
            changer.titleUI.SetActive(true);
        }
    }

    public void ClickTitleButton()
    {

    }
}