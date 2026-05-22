using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private RankingManager rankingManager;

    public void OnClickSendButton()
    {
        string playerName = nameInputField.text;

        //未入力対策
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "NoName";
        }

        rankingManager.SendScore(playerName, ScoreData.FinalScore);
    }
}
