using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class TooltipManager : MonoBehaviour
    {
        private GameManager _gameManager;
        private DialogueManager _dialogueManager;
        
        // This code is a mess
        private Camera _camera;
        public GameObject tooltip;
        public TMP_Text tooltipTitle;
        public TMP_Text tooltipSubTitle;
        public TMP_Text tooltipBody;
        private RectTransform _tooltipRect;
        public Canvas canvas;
        private GameObject _prevHit;
        [SerializeField]private Image[] imageRenderers;
        [SerializeField]private TMP_Text[] textRenderers;
        private Tooltip _prevHitTooltip;
        private float _stickyTimer;
        public float stickyTimerMax;
        
        private Coroutine _currentFade;
        private bool _isVisible;

        private void Awake()
        {
            _camera = Camera.main;
            _tooltipRect = tooltip.GetComponent<RectTransform>();
        }
        void StartFade(IEnumerator routine)
        {
            if (_currentFade != null)
                StopCoroutine(_currentFade);

            _currentFade = StartCoroutine(routine);
        }

        private void Start()
        {
            _dialogueManager = DialogueManager.Instance;
            _gameManager = GameManager.Instance;
        }

        private IEnumerator FadeOut(float duration = 0.2f)
        {
            var a = 1.0f;
            while (a > 0)
            {
                a -= Time.deltaTime / duration;
                foreach (var r in imageRenderers)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, a);
                }
                foreach (var r in textRenderers)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, a);
                }
                
                yield return new WaitForEndOfFrame();
            }
            tooltip.SetActive(false);
            _prevHit = null;
        }
        private IEnumerator FadeIn(float duration = 0.2f)
        {
            tooltip.SetActive(true);
            var a = 0f;
            while (a < 1)
            {
                a += Time.deltaTime / duration;
                foreach (var r in imageRenderers)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, a);
                }
                foreach (var r in textRenderers)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, a);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        
        void Update()
        {
            if (_gameManager.gameState == GameManager.GameState.Round || _dialogueManager.inDialogue)
            {
                if (_isVisible)
                {
                    StartFade(FadeOut());
                    _isVisible = false;
                }
                return;
            };
            Vector2 mouseScreenPos = Input.mousePosition;

            Vector2 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenPos);

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            
            if (hit != null && hit.TryGetComponent<Tooltip>(out var tt))
            {
                _stickyTimer = 0f;
                if (hit.gameObject != _prevHit)
                {
                    _prevHit = hit.gameObject;
                    _prevHitTooltip = tt;

                    if (!_isVisible)
                    {
                        StartFade(FadeIn());
                        _isVisible = true;
                    }

                    tooltipTitle.text = tt.title;
                    tooltipSubTitle.text = tt.subtitle;
                    tooltipBody.text = tt.message;
                }
            }
            if (_stickyTimer >= stickyTimerMax && !_prevHitTooltip.forceEnabled)
            {
                if (_isVisible)
                {
                    StartFade(FadeOut());
                    _isVisible = false;
                }
            }

            if (_prevHit != null)
            {
                
                Vector2 screenPos = _camera.WorldToScreenPoint(_prevHitTooltip.location.position);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPos,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                    out Vector2 uiPos
                );

                _tooltipRect.localPosition =  uiPos;
                _stickyTimer +=  Time.deltaTime;
            }
        }
        
    }
}