using UnityEngine;

namespace Ohajiki.Core
{
    public static class EffectAPIWindow    //  エフェクトAPI窓口
    {
        public static void Play(EffectKey key, Vector3 pos)    //  エフェクト再生
        {
            ServiceLocator.Resolve<IEffectSystem>().Play(key, pos);
        }
        public static void Stop(EffectKey key)    //  エフェクト停止
        {
            ServiceLocator.Resolve<IEffectSystem>().Stop(key);
        }
    }
}