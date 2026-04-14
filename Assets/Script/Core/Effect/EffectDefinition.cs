using UnityEngine;

namespace Ohajiki.Core
{
    [CreateAssetMenu(fileName = "EffectDefinition", menuName = "Effect/Definition")]
    public class EffectDefinition : ScriptableObject    //  Effect定義SO
    {
        [Header("Key")]
        public EffectKey effectKey;    //  エフェクトの鍵
        [Header("再生対象")]
        public GameObject prefab;    //  再生するエフェクト
    }
}