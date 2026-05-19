using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FX
{
    public class DarkenEffect : MonoBehaviour
    {
        [SerializeField] private Image darkenEffectImage;
        private Coroutine _darkenFXCoroutine;
        private bool _isDarkened = false;
        public static DarkenEffect Instance;
        [SerializeField] private float alphaMax = 225f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        
        private IEnumerator FadeOut(float duration = 0.2f)
        {
            _isDarkened = false;
            var a = alphaMax/255f;
            while (a > 0)
            {
                a -= Time.deltaTime / duration;
                a = a < 0 ? 0 : a;
                darkenEffectImage.color = new Color(darkenEffectImage.color.r, darkenEffectImage.color.g, darkenEffectImage.color.b, a);
                
                yield return new WaitForEndOfFrame();
            }
            darkenEffectImage.enabled = false;
            darkenEffectImage.raycastTarget = false;
        }
        private IEnumerator FadeIn(float duration = 0.2f)
        {
            _isDarkened = true;
            darkenEffectImage.enabled = true;
            darkenEffectImage.raycastTarget = true;
            var a = 0f;
            while (a < alphaMax/255f)
            {
                a += Time.deltaTime / duration;
                a = a > alphaMax/255f ? alphaMax/255f : a;
                darkenEffectImage.color = new Color(darkenEffectImage.color.r, darkenEffectImage.color.g, darkenEffectImage.color.b, a);
                
                yield return new WaitForEndOfFrame();
            }
        }
        
        
        public void EnableDarken()
        {
            if (_darkenFXCoroutine != null)
                StopCoroutine(_darkenFXCoroutine);
            if (!_isDarkened)
                _darkenFXCoroutine = StartCoroutine(FadeIn());
        }
        public void DisableDarken()
        {
            if (_darkenFXCoroutine != null)
                StopCoroutine(_darkenFXCoroutine);
            if (_isDarkened)
                _darkenFXCoroutine = StartCoroutine(FadeOut());
        }
    }
}