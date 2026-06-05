using UnityEngine;
using UnityEngine.UI;

namespace Ohajiki.Core
{
    public class ScrennFadeSystemBootstrap : MonoBehaviour    //  画面をフェードさせるシステムの初期化役
    {
        ScreenFadeFacade screenFadeFacade;    //  システム窓口Class
        ScreenFadeJudgeBase fadeJudge;    //  フェードしていい状態か判断するクラス
        ScreenFade screenFader;    //  フェード関数持ちクラス

        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Canvas canvas;
        [SerializeField] Image fadeImage;

        private void Awake()
        {
            screenFader = new ScreenFade(canvasGroup, canvas, fadeImage);
            fadeJudge = new ScrennFadeJudge();
            screenFadeFacade = new ScreenFadeFacade(screenFader, fadeJudge);
            
            DontDestroyOnLoad(canvas);
            ServiceLocator.Register<IScreenFadeFacade>(screenFadeFacade);
        }

        private void Start()
        {
        }
    }
}