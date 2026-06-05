namespace Ohajiki.Core
{
    public class ScrennFadeJudge : ScreenFadeJudgeBase    //  ScreenFadeExcuteのFadeリクエストを通していいかを判断する
    {
        public override bool Judge()    //  書ける条件増えたら追記していく
        {
            return true;
        }
    }
}