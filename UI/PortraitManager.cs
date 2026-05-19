using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PortraitManager : MonoBehaviour
    {
        private Image[] _portraitSegments;

        private void Awake()
        {
            _portraitSegments = GetComponentsInChildren<Image>();
        }

        public void Show(float appearSpeed, AnimationCurve fadeCurve)
        {
            StartCoroutine(ShowPortrait(appearSpeed, fadeCurve));
        }
        
        public void Hide(float appearSpeed, AnimationCurve fadeCurve)
        {
            StartCoroutine(HidePortrait(appearSpeed, fadeCurve));
        }
        
        private IEnumerator ShowPortrait(float appearSpeed, AnimationCurve fadeCurve)
        {
            float a = 0f;
            while (a < 1f)
            {
                a += Time.deltaTime / appearSpeed;
                a = a > 1f ? 1f : a;
                float evaluatedA = fadeCurve.Evaluate(a);
                foreach (var t in _portraitSegments)
                {
                    t.color = new Color(1f, 1f, 1f, evaluatedA);
                }
                
                
                yield return new WaitForEndOfFrame();
            }
        }
        
        private IEnumerator HidePortrait(float appearSpeed, AnimationCurve fadeCurve)
        {
            float a = 1f;
            while (a > 0f)
            {
                a -= Time.deltaTime / appearSpeed;
                a = a < 0f ? 0f : a;
                float evaluatedA = fadeCurve.Evaluate(a);
                foreach (var t in _portraitSegments)
                {
                    t.color = new Color(1f, 1f, 1f, evaluatedA);
                }
                
                
                yield return new WaitForEndOfFrame();
            }
        }
    }
}