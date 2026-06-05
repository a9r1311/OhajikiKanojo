using System.Threading.Tasks;
using UnityEngine;

namespace Ohajiki.Core
{
    public interface IScreenFadeFacade
    {
        public Task FadeIn(float duration);

        public Task FadeOut(float Duration, Color? fadeColor = null);
    }
}