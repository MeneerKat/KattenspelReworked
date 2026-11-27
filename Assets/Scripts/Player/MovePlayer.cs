using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class MovePlayer : MonoBehaviour
{
    // Variables
    [SerializeField] private bool horizontalMovement = false;
    [SerializeField] private bool verticalMovement = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera cam;

    // Functions
    void Start()
    {
        // Create random position list
        List<String> spawnPositions = new();
        if (GameManager.surroundingCats)
        {
            // Surrounding settings
            horizontalMovement = true;
            verticalMovement = true;
            gameObject.transform.position = GameManager.Instance.surroundingStartPosition;
        }
        else
        {
            // Base settings
            horizontalMovement = true;
            verticalMovement = false;
            gameObject.transform.position = GameManager.Instance.baseStartPosition;
        }

        // Allocate main camera for border calculations
        cam = Camera.main;

        // Freeze positions to prevent going off the axis
        GameManager.Instance.FreezePositions(rb, horizontalMovement, verticalMovement);
    }

    // Cat collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<ICanHit>(out var hittable))
        {
            hittable.OnHit();
        }
    }

    void Update()
    {
        // Get mouse position (px)
        UnityEngine.Vector2 mousePosition = new (Input.mousePosition.x, Input.mousePosition.y);
        mousePosition = cam.ScreenToWorldPoint(mousePosition);
        float mouseX = mousePosition.x;
        float mouseY = mousePosition.y;

        // Get mouse position (transform scale)
        if (horizontalMovement == false)
            mouseX = transform.localPosition.x;
        if (verticalMovement == false)
            mouseY = transform.localPosition.y;

        // Calculate position
        float playerPositionX = mouseX;
        float playerPositionY = mouseY;

        // Keep position within screen
        float playerOffsetX = transform.localScale.x / 2;
        float playerOffsetY = transform.localScale.y / 2;
        float screenSizeY = cam.orthographicSize;
        float screenSizeX = cam.orthographicSize * cam.aspect;

        playerPositionX = Mathf.Min(screenSizeX - playerOffsetX, playerPositionX);
        playerPositionX = Mathf.Max(-screenSizeX + playerOffsetX, playerPositionX);
        playerPositionY = Mathf.Min(screenSizeY - playerOffsetY, playerPositionY);
        playerPositionY = Mathf.Max(-screenSizeY + playerOffsetY, playerPositionY);

        // Apply position
        transform.localPosition = new UnityEngine.Vector2(playerPositionX, playerPositionY);
    }
}
