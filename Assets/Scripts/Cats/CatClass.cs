using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CatClass
{
    // Variables
    public bool isActive;

    float screenSizeX;
    float screenSizeY;
    float horizontalSpeed;
    float verticalSpeed;
    float rotationSpeed;

    GameObject catPrefab;
    GameObject catObject;
    List<Sprite> catSprites;
    Rigidbody2D rb;

    // Constructor
    public CatClass(
        GameObject CatPrefab, List<Sprite> CatSprites, 
        float HorizontalSpeed, float VerticalSpeed, float RotationSpeed) 
    {
        this.catPrefab = CatPrefab;
        this.catSprites = CatSprites;

        this.horizontalSpeed = HorizontalSpeed;
        this.verticalSpeed = VerticalSpeed;
        this.rotationSpeed = RotationSpeed;

        GetScreenBorders();
        CreateGameObject();

        this.isActive = true;
    }

    // Get screen borders
    private void GetScreenBorders()
    {
        Camera cam = Camera.main;
        this.screenSizeX = cam.orthographicSize * cam.aspect;
        this.screenSizeY = cam.orthographicSize;
    }

    // Create the cat
    public virtual void CreateGameObject()
    {
        // Create prefab
        this.catObject = GameObject.Instantiate(catPrefab);
        this.rb = catObject.GetComponent<Rigidbody2D>();

        // Set sprite
        int randomSpriteIndex = UnityEngine.Random.Range(0, catSprites.Count);
        catObject.GetComponent<SpriteRenderer>().sprite = catSprites[randomSpriteIndex];

        // Connect collision script
        CatCollision collision = catObject.GetComponent<CatCollision>();
        collision.SetCatClass(this);

        // Move to spawn position
        SpawnPosition();
    }

    // Move to spawn position
    public virtual void SpawnPosition()
    {
        // Random vertical pos above screen
        float verticalPos = 0;
        float horizontalPos = screenSizeY + catObject.transform.localScale.y/2;
        catObject.transform.position = new Vector2(verticalPos, horizontalPos);
    }

    // Add score on hit
    public virtual void OnHit()
    {
        GameManager.Instance.AddScore();
        UnityEngine.Object.Destroy(catObject);
        isActive = false;
    }

    // Check if cat reached other side
    public virtual void BorderCheck()
    {
        if (catObject.transform.position.y <= -screenSizeY - catObject.transform.localScale.x)
        {
            GameManager.Instance.RemoveHealth(GameManager.Instance.damagePerHit);
            UnityEngine.Object.Destroy(catObject);
            isActive = false;
        }
    }

    public virtual void MoveCat()
    {
        rb.AddForce(new Vector2(horizontalSpeed, verticalSpeed),ForceMode2D.Force);
        catObject.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);

        BorderCheck();
    }
}

