using SLS.UI;
using System.Collections;
using UnityEngine;

namespace ROTools.UI
{
    public class LoadingViewController : ViewController<LoadingView>
    {
        public LoadingViewController(LoadingView view) : base(view)
        {

        }

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {

        }

        protected override void OnInit()
        {
            
        }

        protected override void OnDispose()
        {

        }

        public void HideWithFadeTime(float fadeTime = 0.2f)
        {
            view.StopAllCoroutines();
            view.StartCoroutine(HideLoadViewRoutine(fadeTime));
        }

        private IEnumerator HideLoadViewRoutine(float fadeTime)
        {
            yield return new WaitForSeconds(fadeTime);
            Hide();
            yield return null;
        }
    }
}