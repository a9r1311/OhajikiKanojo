using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private GameObject resultInput;
    [SerializeField] private GameObject resultOutput;
    [SerializeField] private UIChanger changer;
    [SerializeField] private ScoreDirector scoreDirector;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject dangerText;
    [SerializeField] private TextMeshProUGUI scoreResultText;
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
    public void ResultView()
    {
        //名前入力UIを表示
        resultInput.SetActive(true);

        finalScore = scoreDirector.GetScore();

        scoreResultText.text = $"Result:" + finalScore;
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
        //rankingManager.SendScore(playerName, finalScore);

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

    public void ClickRetryButton()
    {

    }

    public void ClickTitleButton()
    {

    }
}