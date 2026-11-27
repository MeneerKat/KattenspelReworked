
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CatClass
{
    // Variables
    public bool isActive;

    // Screen
    float screenSizeX;
    float screenSizeY;

    // Movement
    float horizontalSpeed;
    float verticalSpeed;
    float rotationSpeed;
    float catSize;

    // GameObjects
    GameObject catPrefab;
    public GameObject catObject;

    // Sprites
    List<Sprite> catSprites;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    // Spawn
    string spawnSide;

    // Bouncing
    bool bouncing;
    float bounceHeightExponent = 1f;
    int bounces;

    // Invisible
    bool invisibleCats;
    float invisibilityTime;
    float currentTransparency;

    // Audio
    AudioClip purrSound;
    AudioClip boingSound;
    AudioClip crashSound;

    // Constructor
    public CatClass(
        GameObject CatPrefab,
        List<Sprite> CatSprites,
        float HorizontalSpeed,
        float VerticalSpeed,
        float RotationSpeed,
        AudioClip PurrSound,
        AudioClip BoingSound,
        AudioClip CrashSound)
    {
        catPrefab = CatPrefab;
        catSprites = CatSprites;

        horizontalSpeed = HorizontalSpeed;
        verticalSpeed = VerticalSpeed;
        rotationSpeed = RotationSpeed;

        purrSound = PurrSound;
        boingSound = BoingSound;
        crashSound = CrashSound;

        // Bouncing settings
        bouncing = GameManager.bouncing;

        if (bouncing)
        {
            bounceHeightExponent = GameManager.Instance.bounceHeightExponent;
            bounces = GameManager.Instance.bounces;
        }

        // Invisible settings
        invisibleCats = GameManager.invisibleCats;

        if (invisibleCats)
        {
            invisibilityTime = GameManager.Instance.invisibilityTime;
        }

        GetScreenBorders();
        CreateGameObject();

        isActive = true;
    }

    // Get screen borders
    public void GetScreenBorders()
    {
        Camera cam = Camera.main;

        screenSizeX = cam.orthographicSize * cam.aspect;
        screenSizeY = cam.orthographicSize;
    }

    // Create the cat
    public virtual void CreateGameObject()
    {
        catObject = GameObject.Instantiate(catPrefab);

        rb = catObject.GetComponent<Rigidbody2D>();
        spriteRenderer = catObject.GetComponent<SpriteRenderer>();

        // Random rotation
        rotationSpeed = Random.Range(-180f, 180f);

        // Random size
        catSize = Random.Range(0.25f, 0.35f);
        catObject.transform.localScale = new Vector2(catSize, catSize);

        // Random sprite
        int randomSpriteIndex = Random.Range(0, catSprites.Count);
        spriteRenderer.sprite = catSprites[randomSpriteIndex];

        // Connect collision script
        CatCollision collision = catObject.GetComponent<CatCollision>();
        collision.SetCatClass(this);

        // Spawn
        SpawnPosition();
    }

    // Move to spawn position
    public virtual void SpawnPosition()
    {
        // Available spawn positions
        List<string> spawnPositions = new() { "Top" };

        if (GameManager.surroundingCats)
        {
            spawnPositions.Add("Bottom");
            spawnPositions.Add("Right");
            spawnPositions.Add("Left");
        }

        spawnSide = spawnPositions[Random.Range(0, spawnPositions.Count)];

        float xPos = 0f;
        float yPos = 0f;

        // Bottom
        if (spawnSide == "Bottom")
        {
            xPos = Random.Range(-screenSizeX + 0.5f, screenSizeX - 0.5f);
            yPos = -screenSizeY - 1f;

            horizontalSpeed = 0f;
        }

        // Top
        else if (spawnSide == "Top")
        {
            xPos = Random.Range(-screenSizeX + 0.5f, screenSizeX - 0.5f);
            yPos = screenSizeY + 1f;

            verticalSpeed *= -1f;
            horizontalSpeed = 0f;
        }

        // Right
        else if (spawnSide == "Right")
        {
            xPos = screenSizeX + 1f;
            yPos = Random.Range(-screenSizeY + 0.5f, screenSizeY - 0.5f);

            verticalSpeed = 0f;
            horizontalSpeed *= -1f;
        }

        // Left
        else if (spawnSide == "Left")
        {
            xPos = -screenSizeX - 1f;
            yPos = Random.Range(-screenSizeY + 0.5f, screenSizeY - 0.5f);

            verticalSpeed = 0f;
        }

        catObject.transform.position = new Vector3(xPos, yPos, 1f);
    }

    // On Miss
    public virtual void Miss()
    {
        GameManager.Instance.RemoveHealth(GameManager.Instance.damagePerHit);
        AudioSource.PlayClipAtPoint(crashSound, catObject.transform.position);
    }

    // On Hit
    public virtual void Hit()
    {
        GameManager.Instance.AddScore();
        AudioSource.PlayClipAtPoint(purrSound, catObject.transform.position);
    }

    // Add score on hit
    public virtual void OnHit()
    {
        if (bouncing && bounces > 0)
        {
            // Bounce formula
            float bounceHeightMultiplier = Mathf.Pow(bounces,bounceHeightExponent);
            rb.linearVelocity = new Vector2((bounceHeightMultiplier * -horizontalSpeed) / 40f, (bounceHeightMultiplier * -verticalSpeed) / 40f);

            AudioSource.PlayClipAtPoint(boingSound, catObject.transform.position);

            bounces--;
        }
        else
        {
            Hit();
            DestroyCat();
        }
    }

    // Destroy cat
    public void DestroyCat()
    {
        Object.Destroy(catObject);
        isActive = false;
    }

    // Check if cat reached the other side
    public virtual void BottomCheck()
    {
        bool outOfBounds = false;

        if (spawnSide == "Top" && catObject.transform.position.y <= -screenSizeY - catSize)
            outOfBounds = true;

        else if (spawnSide == "Bottom" && catObject.transform.position.y >= screenSizeY + catSize)
            outOfBounds = true;

        else if (spawnSide == "Right" && catObject.transform.position.x <= -screenSizeX - catSize)
            outOfBounds = true;

        else if (spawnSide == "Left" && catObject.transform.position.x >= screenSizeX + catSize)
            outOfBounds = true;

        if (outOfBounds)
        {
            Miss();
            DestroyCat();
        }
    }

    // Check if cat is out of bounds
    public virtual bool OutOfBoundsCheck()
    {
        if (!isActive)
            return true;

        BottomCheck();

        return !isActive;
    }

    // Move the cat and rotate it
    public virtual void MoveCat(float deltaTime)
    {
        if (!isActive)
            return;

        rb.AddForce(new Vector2(horizontalSpeed * deltaTime, verticalSpeed * deltaTime));

        catObject.transform.Rotate(0f, 0f, rotationSpeed * deltaTime, Space.Self);

        // Invisible cats
        if (invisibleCats)
        {
            currentTransparency += (1f / invisibilityTime) * deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f - currentTransparency);
        }

        BottomCheck();
    }

    public void NukeEffect()
    {
        float nukeForce = GameManager.Instance.nukeMaxForce;
        float nukeRange = GameManager.Instance.nukeRange;
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(catObject.transform.position, nukeRange);

        foreach (Collider2D obj in nearbyObjects)
        {
            if (!obj.CompareTag("Nuke"))
                continue;

            Vector2 direction = (Vector2)catObject.transform.position - (Vector2)obj.transform.position;
            float distance = direction.magnitude;

            if (distance <= 0.01f)
                continue;

            // Closer = stronger force
            float force = Mathf.Lerp(nukeForce, 0f, distance / nukeRange);
            rb.AddForce(direction.normalized * force, ForceMode2D.Force);
        }
    }
}
