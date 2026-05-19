using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private GameObject dialogueBox;
        private Image _dialogueBoxImage;
        private TMP_Text[] _dialogueTextDisplays;
        [SerializeField] private float appearSpeed;
        [SerializeField] private AnimationCurve fadeCurve;
        private Coroutine _dialogueBoxAppearCoroutine;

        [Header("Typing Settings")]
        [SerializeField] private float typingSpeed = 0.03f;
        private bool _isTyping;

        [Header("Reader Settings")]
        public static DialogueManager Instance;
        public bool inDialogue = false;
        [SerializeField] private string[] dialogueCsvPaths;

        private List<List<string>> _dialogueLines;
        private int _dialogueProgressIndex;
        private DarkenEffect _darkenEffect;
        private Coroutine _typingCoroutine;
        
        [Header("Portraits")]
        public PortraitManager seraphim;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            _dialogueBoxImage = dialogueBox.GetComponent<Image>();
            _darkenEffect = DarkenEffect.Instance;
            _dialogueTextDisplays = dialogueBox.GetComponentsInChildren<TMP_Text>();
        }

        private void Update()
        {
            if (inDialogue)
            {
                CheckForClick();
            }
        }

        private void CheckForClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProgressDialogue();
            }
        }

        private IEnumerator TypeDialogue(string text)
        {
            _isTyping = true;

            dialogueText.text = "";

            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            _isTyping = false;
        }

        private IEnumerator ShowDialogueBox()
        {
            seraphim.Show(appearSpeed, fadeCurve);
            dialogueBox.SetActive(true);
            float a = 0f;
            while (a < 1f)
            {
                a += Time.deltaTime / appearSpeed;
                a = a > 1f ? 1f : a;
                float evaluatedA = fadeCurve.Evaluate(a);
                _dialogueBoxImage.color = new Color(1f, 1f, 1f, evaluatedA);
                foreach (var t in _dialogueTextDisplays)
                {
                    t.color = new Color(1f, 1f, 1f, evaluatedA);
                }
                
                
                yield return new WaitForEndOfFrame();
            }
        }
        
        private IEnumerator HideDialogueBox()
        {
            seraphim.Hide(appearSpeed, fadeCurve);
            float a = 1f;
            while (a > 0f)
            {
                a -= Time.deltaTime / appearSpeed;
                a = a < 0f ? 0f : a;
                float evaluatedA = fadeCurve.Evaluate(a);
                _dialogueBoxImage.color = new Color(1f, 1f, 1f, evaluatedA);
                foreach (var t in _dialogueTextDisplays)
                {
                    t.color = new Color(1f, 1f, 1f, evaluatedA);
                }
                
                
                
                yield return new WaitForEndOfFrame();
            }
            dialogueBox.SetActive(false);
        }

        private void SetInDialogue(bool value)
        {
            inDialogue = value;

            if (value)
            {
                _darkenEffect.EnableDarken();
                if (_dialogueBoxAppearCoroutine != null)
                    StopCoroutine(_dialogueBoxAppearCoroutine);
                StartCoroutine(ShowDialogueBox());
            }
            else
                _darkenEffect.DisableDarken();
            
            
        }

        private void ProgressDialogue()
        {
            if (_isTyping)
            {
                StopCoroutine(_typingCoroutine);

                dialogueText.text = _dialogueLines[0][_dialogueProgressIndex];

                _isTyping = false;
                return;
            }
            _dialogueProgressIndex++;
            if (_dialogueProgressIndex >= _dialogueLines[0].Count)
            {
                SetInDialogue(false);
                StartCoroutine(HideDialogueBox());
                return;
            }

            _typingCoroutine =
                StartCoroutine(TypeDialogue(_dialogueLines[0][_dialogueProgressIndex]));
        }

        public IEnumerator StartDialogueCoroutine(int dialogueID)
        {
            SetInDialogue(true);
            _dialogueLines = ReadFromCsv.ReadCsv(dialogueCsvPaths[dialogueID]);

            _dialogueProgressIndex = 0;

            _typingCoroutine =
                StartCoroutine(TypeDialogue(_dialogueLines[0][_dialogueProgressIndex]));
            yield return new WaitUntil(() => !inDialogue); 
        }
        
        public IEnumerator StartDialogueCoroutine(string dialogueName)
        {
            SetInDialogue(true);
            _dialogueLines = ReadFromCsv.ReadCsv(dialogueName);

            _dialogueProgressIndex = 0;

            _typingCoroutine =
                StartCoroutine(TypeDialogue(_dialogueLines[0][_dialogueProgressIndex]));
            yield return new WaitUntil(() => !inDialogue); 
        }

        public void StartDialogue(int dialogueID)
        {
            SetInDialogue(true);
            _dialogueLines = ReadFromCsv.ReadCsv(dialogueCsvPaths[dialogueID]);

            _dialogueProgressIndex =  0;

            _typingCoroutine =
                StartCoroutine(TypeDialogue(_dialogueLines[0][_dialogueProgressIndex]));
            
        }
    }
}