using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public GameObject overlay;
    public GameObject setting;
    public GameObject log;

    //タイトルシーンからホームシーンへ
    public void OnCrickStratButton()
    {
        SceneLoader.Load("HomeScene");
    }

    //オーバーレイ並びに設定かログを開く
    public void OnClickOverlayButton(GameObject selectObject)
    {
        overlay.SetActive(true);
        selectObject.SetActive(true);
    }

    //設定とログを閉じる
    public void OnClickCloseButton()
    {
        overlay.SetActive(false);
        setting.SetActive(false);
        log.SetActive(false);
    }
}
