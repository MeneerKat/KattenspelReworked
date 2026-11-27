using NUnit.Framework.Internal.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;


public class MoveCat : MonoBehaviour
{
    // [[ VARIABLES ]]
    // Components
    [SerializeField] private Rigidbody2D rb;

    // Sprite variables
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite catImage1;
    [SerializeField] private Sprite catImage2;
    [SerializeField] private Sprite catImage3;
    [SerializeField] private Sprite evilCatImage1;
    [SerializeField] private Sprite evilCatImage2;
    [SerializeField] private Sprite evilCatImage3;
    [SerializeField] private Sprite coolCatImage1;
    [SerializeField] private AudioClip purrSound;
    [SerializeField] private AudioClip hissSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip boingSound;
    [SerializeField] private AudioClip evilMeowSound;
    [SerializeField] private AudioClip crashSound;

    // Transform variables
    private float verticalSpeed;
    private float horizontalSpeed;
    private float rotationSpeed;
    private float catSize;

    // Camera
    private Camera cam;

    // Bouncing variables
    private bool bouncing = false;
    private float bounceHeightExponent = 1f;
    private int bounces = 0;

    // Evil variables
    private bool evilCats = false;
    private float evilPercentage = 0f;
    private bool evil = false;

    // Cool variables
    private bool coolCats = false;
    private float coolPercentage = 0f;
    private bool cool = false;

    // Invisible variables
    private bool invisibleCats = false;
    private float invisibilityTime = 0f;
    private float currentTransparency = 0f;

    // SpawnSide
    private string spawnSide;


    // [[ FUNCTIONS ]]
    void Start()
    {
        // Check for Bouncing setting
        bouncing = GameManager.bouncing;
        if (bouncing)
        {
            bounceHeightExponent = GameManager.Instance.bounceHeightExponent;
            bounces = GameManager.Instance.bounces;
        }

        // Check for Evil setting
        evilCats = GameManager.evilCats;
        if (evilCats)
        { 
            evilPercentage = GameManager.Instance.evilPercentage;
        }

        // Check for Cool setting
        coolCats = GameManager.coolCats;
        if (coolCats)
        {
            coolPercentage = GameManager.Instance.coolPercentage;
            if (evilCats)
                coolPercentage /= (evilPercentage / 100);
        }

        // Check for Invisible setting
        invisibleCats = GameManager.invisibleCats;
        if (invisibleCats)
        {
            invisibilityTime = GameManager.Instance.invisibilityTime;
        }

        // Set camera for border calculations
        cam = Camera.main;
        float screenSizeX = cam.orthographicSize * cam.aspect;
        float screenSizeY = cam.orthographicSize;

        // Set Speed
        horizontalSpeed = GameManager.Instance.horizontalSpeed;
        verticalSpeed = GameManager.Instance.verticalSpeed;

        // Create random position list
        List<String> spawnPositions = new() {"Top"}; // Base spawn position

        if (GameManager.surroundingCats) // Apply other spawn positions
        {
            spawnPositions.Add("Bottom");
            spawnPositions.Add("Right");
            spawnPositions.Add("Left");
        }

        spawnSide = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];

        // Apply spawn pos and speed
        float x_pos = 0f;
        float y_pos = 0f;

        if (spawnSide == "Bottom") // Move up only
        {
            x_pos = UnityEngine.Random.Range(-screenSizeX + 0.5f, screenSizeX - 0.5f);
            y_pos = -screenSizeY - 1f;
            horizontalSpeed = 0f;
        }
        else if (spawnSide == "Top") // Move down only
        {
            x_pos = UnityEngine.Random.Range(-screenSizeX + 0.5f, screenSizeX - 0.5f);
            y_pos = screenSizeY + 1f;
            verticalSpeed *= -1f;
            horizontalSpeed = 0f;
        }
        else if (spawnSide == "Right") // Move left only
        {
            x_pos = screenSizeX + 1f;
            y_pos = UnityEngine.Random.Range(-screenSizeY + 0.5f, screenSizeY - 0.5f);
            verticalSpeed = 0f;
            horizontalSpeed *= -1f;
        }
        else if (spawnSide == "Left") // Move right only
        {
            x_pos = -screenSizeX - 1f;
            y_pos = UnityEngine.Random.Range(-screenSizeY + 0.5f, screenSizeY - 0.5f);
            verticalSpeed = 0f;
        }

        Vector3 catPosition = new(x_pos, y_pos, 1f);
        gameObject.transform.position = catPosition;

        // Random Rotationspeed
        rotationSpeed = UnityEngine.Random.value * 360 - 180;

        // Random Size
        catSize = (UnityEngine.Random.value * 0.1f + 0.25f);
        transform.localScale = new UnityEngine.Vector2(catSize, catSize);
        GameManager.Instance.FreezePositions(rb, horizontalSpeed != 0, verticalSpeed != 0);

        // Determine type (normal/evil/cool) and apply sprite
        ArrayList catSprites;
        if (evilCats && (UnityEngine.Random.value < evilPercentage / 100))
        {
            // Evil cat sprites
            catSprites = new() {evilCatImage1, evilCatImage2, evilCatImage3};
            evil = true;
            bounces = 0;
        }
        else if (coolCats && (UnityEngine.Random.value < coolPercentage / 100))
        {
            // Cool cat sprite
            catSprites = new() {coolCatImage1};
            cool = true;
        }
        else 
        {
            // Normal cat sprites
            catSprites = new() {catImage1, catImage2, catImage3};
        }

        // Random sprite
        int catSpriteIndex = UnityEngine.Random.Range(0, catSprites.Count);
        spriteRenderer.sprite = (Sprite)catSprites[catSpriteIndex];
    }

    // Handles cats that fall below the bottom of the screen. Called in GameManager
    public bool OutOfBoundsCheck() {
        // Bounds
        float screenSizeX = cam.orthographicSize * cam.aspect;
        float screenSizeY = cam.orthographicSize;

        // If out of bounds (different bounds per spawn side)
        if ((spawnSide == "Top" && transform.position.y <= -screenSizeY - catSize)
            || (spawnSide == "Bottom" && transform.position.y >= screenSizeY + catSize)
            || (spawnSide == "Right" && transform.position.x <= -screenSizeX - catSize)
            || (spawnSide == "Left" && transform.position.x >= screenSizeX + catSize))
        {
            // Lose health if not evil
            if (evil)
                AudioSource.PlayClipAtPoint(evilMeowSound, transform.position, 1f);
            else
            {
                GameManager.Instance.RemoveHealth(GameManager.Instance.damagePerHit);
                AudioSource.PlayClipAtPoint(crashSound, transform.position, 1f);
            }

            // Destroy cat
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    // Colission event
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If collide with a player
        if (collision.collider.CompareTag("Player"))
        {
            // Different impacts
            if (bouncing && bounces > 0 && !evil)
            {
                // Bounce formula when hitting bouncy cat
                float bounceHeightMultiplier = Mathf.Pow(bounces, bounceHeightExponent);
                rb.linearVelocity = new Vector2((bounceHeightMultiplier * -horizontalSpeed) / 40, (bounceHeightMultiplier * -verticalSpeed) / 40);

                AudioSource.PlayClipAtPoint(boingSound, transform.position, 1f);
                bounces--;
            }
            else if (evil)
            {
                // Remove health
                GameManager.Instance.RemoveHealth(GameManager.Instance.evilDamage);
                AudioSource.PlayClipAtPoint(hissSound, transform.position, 1f);
                Destroy(gameObject);
            }
            else if (cool)
            {
                // Add health
                GameManager.Instance.HealHealth(GameManager.Instance.coolHealing);
                AudioSource.PlayClipAtPoint(healSound, transform.position, 1f);
                Destroy(gameObject);
            }
            else
            {
                // Add score
                GameManager.Instance.AddScore();
                AudioSource.PlayClipAtPoint(purrSound, transform.position, 1f);
                Destroy(gameObject);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move and rotate kitty
        rb.AddForce(new Vector2(horizontalSpeed * Time.deltaTime, verticalSpeed * Time.deltaTime));
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        // Decrease alpha to become invisible over time
        if (invisibleCats)
        {
            currentTransparency += 1/invisibilityTime * Time.deltaTime;
            spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1f - currentTransparency);
        }
    }
}
