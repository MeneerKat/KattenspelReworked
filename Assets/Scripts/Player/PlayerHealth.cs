using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Variables
    [SerializeField] Sprite catHeart;
    [SerializeField] Sprite catHeartDamaged;
    [SerializeField] GameObject heartPrefab;

    [SerializeField] int maxHealth = 5;
    [SerializeField] int currentHealth = 5;
    Camera cam;

    private List<GameObject> catHearts = new();

    // Functions
    void Start()
    {
        // Initialize variables
        cam = Camera.main;
        maxHealth = GameManager.Instance.maxHealth;
        currentHealth = GameManager.Instance.startHealth;
    }

    // Update the entire bar based on current health
    public void UpdateBar()
    {
        currentHealth = GameManager.Instance.currentHealth;

        // Destroy old hearts
        foreach (GameObject heart in catHearts)
            Destroy(heart);
        catHearts.Clear();

        // Create new hearts
        for (int i = 0; i < maxHealth; i++)
        {
            // Get the correct sprite
            Sprite heart = catHeart;

            if (i >= currentHealth)
                heart = catHeartDamaged;

            // Positioning
            float screenSizeX = cam.orthographicSize * cam.aspect;
            float screenSizeY = cam.orthographicSize;

            float x_pos = -screenSizeX + (heartPrefab.transform.localScale.x / 2) + i * heartPrefab.transform.localScale.x;
            float y_pos = -screenSizeY + (heartPrefab.transform.localScale.y / 2);

            // Create gameobject and set correct sprite
            GameObject newHeart = GameObject.Instantiate(heartPrefab, new Vector2(x_pos, y_pos), Quaternion.Euler(0, 0, 0));
            newHeart.GetComponent<SpriteRenderer>().sprite = heart;
            catHearts.Add(newHeart);
        }
    }
}
