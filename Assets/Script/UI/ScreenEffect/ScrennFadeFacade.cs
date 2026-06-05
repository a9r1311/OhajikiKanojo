using System.Threading.Tasks;
using UnityEngine;

namespace Ohajiki.Core
{
    public class ScreenFadeFacade : IScreenFadeFacade    //  BootstrapからServiceLocatorに登録されて、使われる窓口
    {
        ScreenFade screenFade;    //  フェード関数クラス
        ScreenFadeJudgeBase fadeJudge;    //  フェードをしていい状態か判断するクラス

        public ScreenFadeFacade(ScreenFade screenFade, ScreenFadeJudgeBase fadeJudge)    //  Bootstrapから呼ばれる
        {
            this.screenFade = screenFade;
            this.fadeJudge = fadeJudge;
        }

        public Task FadeIn(float duration)
        {
            if (fadeJudge.Judge())
            {
                return screenFade.FadeIn(duration);
            }
            return null;
        }

        public Task FadeOut(float Duration, Color? fadeColor = null)
        {
            if (fadeJudge.Judge())
            {
                return screenFade.FadeOut(Duration);
            }
            return null;
        }
    }
}