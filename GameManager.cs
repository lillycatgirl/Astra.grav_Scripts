using System;
using System.Collections;
using System.Collections.Generic;
using FX;
using Gameplay.Shop;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Shop,
        Setup,
        Round,
        PostRound
    }
    [Header("Game Information")]
    public static GameManager Instance;
    public long scoreRequirement = 80;
    public double scoreRequirementScaling = 1.4d;
    public long score;
    public int round;
    public bool paused;
    public GameState gameState;
    public int maxStars;
    public int stars;
    public bool canScore;
    public bool inTutorial = true;
    
    public List<PlanetObject> PlayerPlanets{ get; private set; } = new List<PlanetObject>();
    [Header("Global Settings")]
    public Vector2 despawnLimit;
    public Vector2 planetPositionLimit;
    public float wallRestitution;
    public float wallDamage;
    public float planetDamageMultiplier;

    [Header("UI Elements")]
    public GameObject startButton;
    public Sprite buttonStart;
    public Sprite buttonEnd;
    public GameObject pauseButton;
    public GameObject scoreDisplay;
    public Gradient scoreDisplayGradient;
    private TMP_Text _scoreDisplayComponent;
    public GameObject[] healthBarStars;
    
    private ObjectGravityManager _gravityManager;
    private ObjectDraggableManager _draggableManager;
    private TransitionManager _transitionManager;
    private ShopManager _shopManager;
    private DialogueManager _dialogueManager;
    private DarkenEffect _darkenEffect;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Application.targetFrameRate = 60;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameState =  GameState.Setup;
        //startButton.GetComponent<Image>().sprite = buttonStart;
        
        _scoreDisplayComponent = scoreDisplay.GetComponent<TMP_Text>();
        _gravityManager = ObjectGravityManager.Instance;
        _draggableManager = ObjectDraggableManager.Instance;
        _transitionManager = TransitionManager.Instance;
        _shopManager = ShopManager.Instance;
        _dialogueManager = DialogueManager.Instance;
        _darkenEffect = DarkenEffect.Instance;
        
        UpdateScoreDisplay();
        _transitionManager.TransitionOut();
        stars = maxStars;
        if (inTutorial)
        {
            StartCoroutine(PlayTutorial());
        }
        else
        {
            StartSetup(false);
        }
    }

    void ShowPlanetVectors(bool shown)
    {
        foreach (var p in PlayerPlanets)
        {
            p.planetArrowController.enabled = shown;
        }
    }
    private void UpdateHealthDisplay()
    {
        switch (stars)
        {
            default:
            case 0:
                healthBarStars[0].SetActive(false);
                healthBarStars[1].SetActive(false);
                healthBarStars[2].SetActive(false);
                break;
            case 1:
                healthBarStars[0].SetActive(true);
                healthBarStars[1].SetActive(false);
                healthBarStars[2].SetActive(false);
                break;
            case 2:
                healthBarStars[0].SetActive(true);
                healthBarStars[1].SetActive(true);
                healthBarStars[2].SetActive(false);
                break;
            case 3:
                healthBarStars[0].SetActive(true);
                healthBarStars[1].SetActive(true);
                healthBarStars[2].SetActive(true);
                break;
        }
    }
    public void UpdateScoreDisplay()
    {
        _scoreDisplayComponent.text = $"SCORE: {score.ToString()}/{scoreRequirement.ToString()} \nROUND: {round.ToString()}";
    }
    public void AddPlayerPlanet(PlanetObject planet)
    {
        PlayerPlanets.Add(planet);
    }
    public void RemovePlayerPlanet(PlanetObject planet)
    {
        PlayerPlanets.Remove(planet);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PlayTutorial()
    {
        yield return new WaitForSeconds(0.2f);
        _darkenEffect.EnableDarken();
        
        yield return new WaitForSeconds(2.3f);
        yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine(0));
        
        StartSetup(false);
        yield return WaitForRoundResult();
        // Tutorial Instructions round 1
        if (stars != maxStars)
        {
            yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_fail_1"));
            while (true)
            {
                yield return WaitForRoundResult();
                if (stars != maxStars)
                    yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_fail_2"));
                else
                {
                    yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_succeed_2"));
                    break;
                }
                yield return WaitForRoundResult();
                if (stars != maxStars)
                    yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_fail_3"));
                else
                {
                    yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_succeed_2"));
                    break;
                }
                yield return WaitForRoundResult();
                if (stars != maxStars)
                    yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_fail_4"));
                else
                {
                    yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_succeed_2"));
                    break;
                }
                yield return WaitForRoundResult();
            }
        }
        else
        {
            yield return StartCoroutine(_dialogueManager.StartDialogueCoroutine("tutorial_1_succeed_1"));
        }
        
    }

    private IEnumerator WaitForRoundResult()
    {
        
        yield return new WaitUntil(() => gameState == GameState.Setup);
        yield return new WaitUntil(() => gameState == GameState.PostRound);
        yield return new WaitForEndOfFrame();
    }
    
    private void Update()
    {
        switch (gameState)
        {
            case GameState.Setup:
                break;
            case GameState.Round:
                Color c0 = _scoreDisplayComponent.color;
                Color c1 = scoreDisplayGradient.Evaluate((float)(score * 15)/(scoreRequirement * 100));
                _scoreDisplayComponent.color = Color.Lerp(c0, c1, 1f - Mathf.Exp(-2 * Time.deltaTime));
                break;
            case GameState.Shop:
                break;
            case GameState.PostRound:
                Color c2 = _scoreDisplayComponent.color;
                Color c3 = scoreDisplayGradient.Evaluate(0f);
                _scoreDisplayComponent.color = Color.Lerp(c2, c3, 1f - Mathf.Exp(-2 * Time.deltaTime));
                _gravityManager.timeScale = Mathf.Lerp(_gravityManager.timeScale, 0f, 1f - Mathf.Exp(-2 * Time.deltaTime));
                break;
            default:
                break;
        }
    }

    public void ButtonStart()
    {
        if (_dialogueManager.inDialogue) return;
        switch (gameState)
        {
            case GameState.Shop:
                StartSetup();
                break;
            case GameState.Setup:
                StartRound();
                break;
            case GameState.Round:
                StartPostRound();
                break;
            case GameState.PostRound:
                if (score >= scoreRequirement && !inTutorial)
                {
                    StartShop();
                }
                else
                {
                    StartSetup(false);
                }
                break;
        }
    }
    

    private void StartSetup(bool increaseScore = true)
    {
        if (increaseScore)
            _transitionManager.TransitionInOut();
        _gravityManager.timeScale = 1;
        _gravityManager.simulateGravity = false;
        _gravityManager.OnRoundEnd();
        _shopManager.StartCoroutine(_shopManager.ExitShop());
        _draggableManager.enableDragging = true;
        gameState = GameState.Setup;
        score = 0;
        if (increaseScore)
        {
            scoreRequirement = (long)(Math.Round(scoreRequirement * scoreRequirementScaling / 10.0) * 10);
            round++;
            stars = maxStars;
        }
        
        UpdateScoreDisplay();
        UpdateHealthDisplay();
        ShowPlanetVectors(true);
    }


    private void StartRound()
    {
        gameState = GameState.Round;
        _gravityManager.OnRoundStart();
        _gravityManager.simulateGravity = true;
        _draggableManager.enableDragging = false;
        canScore = true;
        
        UpdateScoreDisplay();
        ShowPlanetVectors(false);
    }


    private void StartShop()
    {
        // TODO make the shop do something lmao
        gameState = GameState.Shop;
        _gravityManager.simulateGravity = false;
        _gravityManager.timeScale = 1;
        _draggableManager.enableDragging = true;
        ShowPlanetVectors(false);
        _transitionManager.TransitionInOut();
        _shopManager.StartCoroutine(_shopManager.EnterShop());

    }

    private void StartPostRound()
    {
        canScore = false;
        gameState =  GameState.PostRound;
        _gravityManager.simulateGravity = true;
        _draggableManager.enableDragging = false;
        if (score >= scoreRequirement)
        {
            _scoreDisplayComponent.color = new Color(0.94f, 0.61f, 0.99f);
            stars = maxStars;
        }
        else
        {
            _scoreDisplayComponent.color = new Color(0.99f, 0.37f, 0.37f);
            stars -= 1;
        }
        UpdateHealthDisplay();
        ShowPlanetVectors(false);
    }
    
    private void Pause()
    {
        if  (gameState != GameState.Round) return;
        paused = !paused;
        UpdateScoreDisplay();
        _gravityManager.simulateGravity = !paused;
    }

    public void AddScore(long amount)
    {
        if (!canScore) return;
        score += amount;
        UpdateScoreDisplay();
    }
}
