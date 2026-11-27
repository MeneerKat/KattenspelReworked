using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // [[ VARIABLES AND SETTINGS ]]
    [Header("Game Settings")]

    // Score settings
    [Space]
    [Header("Score Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] static public int highScore = 0;
    [SerializeField] private int baseScore = 100;
    [SerializeField] private float scoreMultiplier = 1f;
    [SerializeField] private TextMeshProUGUI scoreText;

    // Health settings
    [Space]
    [Header("Player Settings")]
    [SerializeField] public GameObject healthBar;
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public int startHealth = 5;
    [SerializeField] public int currentHealth = 5;
    [SerializeField] public int damagePerHit = 1;
    [SerializeField] public Vector2 baseStartPosition = new Vector2(0, -4);

    // User Interface settings
    [Space]
    [Header("User Interface Settings")]
    [SerializeField] private GameObject backgroundCanvas;
    [SerializeField] private GameObject scoreCanvas;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    // Cat settings
    [Space]
    [Header("Cat Settings")]
    [SerializeField] private GameObject catPrefab;
    [SerializeField] private float baseCatsPerSecond = 1f;
    [SerializeField] private float scoreCooldownDivider = 4f;
    [SerializeField] private int maximumCats = 10;
    [SerializeField] public float horizontalSpeed = 50f;
    [SerializeField] public float verticalSpeed = 50f;
    [SerializeField] public float rotationSpeed = 50f;

    // Bouncy cats settings
    [Space]
    [Header("Bouncy Cat Settings")]
    [SerializeField] public int bounces = 2;
    [SerializeField] public float bounceHeightExponent = 0.75f;

    // Evil cats settings
    [Space]
    [Header("Evil Cat Settings")]
    [SerializeField] public float evilPercentage = 50f;
    [SerializeField] public int evilDamage = 1;

    // Cool cats settings
    [Space]
    [Header("Cool Cat Settings (Healing)")]
    [SerializeField] public float coolPercentage = 5f;
    [SerializeField] public int coolHealing = 1;

    // Invisible cats settings
    [Space]
    [Header("Invisible Cats Settings")]
    [SerializeField] public float invisibilityTime = 1f;

    // Surrounding cat settings
    [Space]
    [Header("Surrounding Cats Settings")]
    [SerializeField] public float surroundingSpawnRateIncrease = 2.5f;
    [SerializeField] public Vector2 surroundingStartPosition = new Vector2(0, 0);

    // Images
    [Space]
    [Header("Cat Image Settings")]
    [SerializeField] private Sprite catImage1;
    [SerializeField] private Sprite catImage2;
    [SerializeField] private Sprite catImage3;
    [SerializeField] private Sprite evilCatImage1;
    [SerializeField] private Sprite evilCatImage2;
    [SerializeField] private Sprite evilCatImage3;
    [SerializeField] private Sprite coolCatImage1;

    // Sounds
    [Space]
    [Header("Sound Effects")]
    [SerializeField] private AudioClip purrSound;
    [SerializeField] private AudioClip hissSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip boingSound;
    [SerializeField] private AudioClip evilMeowSound;
    [SerializeField] private AudioClip crashSound;

    // Statics (Ik weet niet hoe je een static kan serializen :sob:)
    static public bool bouncing = false;
    static public bool evilCats = false;
    static public bool coolCats = false;
    static public bool invisibleCats = false;
    static public bool surroundingCats = false;
    static public float bounceMultiplier = 2.5f;
    static public float evilMultiplier = 1.7f;
    static public float coolMultiplier = 0.6f;
    static public float invisibleMultiplier = 1.7f;
    static public float surroundingMultiplier = 2.0f;

    // Private Variables
    private float cooldown;
    private float timer;
    private List<CatClass> activeCats = new();
    private Camera cam;

    // Singleton
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    // [[ FUNCTIONS ]]
    void Start()
    {
        // Lock fps to 60
        Application.targetFrameRate = 60;

        // Camera
        cam = Camera.main;
        
        // Cooldown
        cooldown = 1 / baseCatsPerSecond;
        if (surroundingCats)
            cooldown /= surroundingSpawnRateIncrease;

        // Score multiplier based on active modifiers
        if (bouncing) 
            scoreMultiplier *= bounceMultiplier;
        if (evilCats)
            scoreMultiplier *= evilMultiplier;
        if (coolCats)
            scoreMultiplier *= coolMultiplier;
        if (invisibleCats)
            scoreMultiplier *= invisibleMultiplier;
        if (surroundingCats)
            scoreMultiplier *= surroundingMultiplier;

        // Create the health bar
        healthBar.GetComponent<PlayerHealth>().UpdateBar();

        // Set the score text to 0
        UpdateScoreText();
    }

    void Update()
    {
        // Update all cats
        for (int i = activeCats.Count - 1; i >= 0; i--)
        {
            // Move local cat
            CatClass localCat = activeCats[i];

            if (localCat.isActive == false)
            {
                localCat.DestroyCat();
                activeCats.RemoveAt(i);
            }
            else
            {
                localCat.MoveCat(Time.deltaTime);
            }
        }

        // Spawn new kitties
        float newCooldown = MathF.Min(cooldown / MathF.Max(MathF.Log(score, 20) / scoreCooldownDivider, 1), cooldown);
        if (timer >= newCooldown)
        {
            if (activeCats.Count >= maximumCats) { return; }

            if (evilCats == true && UnityEngine.Random.Range(1, 100) < evilPercentage)
            {
                CatClass newEvilCat = new EvilCat(
                    catPrefab,
                    new List<Sprite> { evilCatImage1, evilCatImage2, evilCatImage3 },
                    horizontalSpeed,
                    verticalSpeed,
                    rotationSpeed,
                    purrSound,
                    boingSound,
                    crashSound,
                    hissSound,
                    evilMeowSound);
                activeCats.Add(newEvilCat);
            }
            else if (coolCats == true && UnityEngine.Random.Range(1, 50) < coolPercentage)
            {
                CatClass newCoolCat = new CoolCat(
                    catPrefab,
                    new List<Sprite> { coolCatImage1 },
                    horizontalSpeed,
                    verticalSpeed,
                    rotationSpeed,
                    purrSound,
                    boingSound,
                    crashSound,
                    healSound);
                activeCats.Add(newCoolCat);
            }
            else
            {
                CatClass newCat = new CatClass(
                    catPrefab,
                    new List<Sprite> { catImage1, catImage2, catImage3 },
                    horizontalSpeed,
                    verticalSpeed,
                    rotationSpeed,
                    purrSound,
                    boingSound,
                    crashSound);
                activeCats.Add(newCat);
            }

            timer = 0f;
        }

        timer += Time.deltaTime;
    }

    // Add correct score
    public void AddScore()
    {
        score += (int)(scoreMultiplier * baseScore);
        UpdateScoreText();
    }

    // Update the score text
    public void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    // Heal health
    public void HealHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Math.Min(currentHealth, maxHealth);
        healthBar.GetComponent<PlayerHealth>().UpdateBar(); // Update the health bar accordingly
    }

    // Damage health
    public void RemoveHealth(int amount)
    {
        currentHealth -= amount;
        healthBar.GetComponent<PlayerHealth>().UpdateBar();

        // End the game when player is at 0 health
        if (currentHealth == 0)
            EndGame();
    }

    // Ends the game
    void EndGame()
    {
        // Destroy all active cats
        maximumCats = 0;

        foreach (CatClass cat in activeCats)
        {
            cat.DestroyCat();
            cat.isActive = false;
        }

        // Switch UI
        backgroundCanvas.SetActive(false);
        scoreCanvas.SetActive(true);

        // Set highscore
        if (score > highScore)
        {
            highScore = score;
            finalScoreText.text = score.ToString();
            highscoreText.text = "Highscore!!!";
        }
        else
        {
            finalScoreText.text = score.ToString();
            highscoreText.text = "Highscore: " + highScore.ToString();
        }

        // Reset score
        score = 0;
    }
}
