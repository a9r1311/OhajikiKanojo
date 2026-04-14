using UnityEngine;

namespace Ohajiki.Core
{
    public interface IEffectSystem    //  エフェクトシステムのインターフェース
    {
        void Play(EffectKey key, Vector3 pos);    //  エフェクト再生
        void Stop(EffectKey key);    //  再生停止
    }
}