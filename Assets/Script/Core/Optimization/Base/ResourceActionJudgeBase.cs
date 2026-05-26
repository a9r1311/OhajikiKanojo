using UnityEngine;

namespace Ohajiki.Core
{
    public abstract class ResourceActionJudgeBase    //  リソース関数を判断するクラスのBaseClass
    {
        public ResourceActionJudgeBase()
        {

        }

        public virtual bool Judge()    //  判断
        {
            return true;
        }
    }
}