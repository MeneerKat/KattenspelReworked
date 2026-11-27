using UnityEngine;

public class NukeClass
{
    GameObject nukeObject;
    GameObject nukeTargetObject;
    float fallSpeed;
    Vector2 fallPos;
    public bool isActive = false;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    // Screen
    float screenSizeX;
    float screenSizeY;

    public NukeClass(
        GameObject NukePrefab,
        GameObject NukeTargetPrefab,
        float FallSpeed,
        Vector2 FallPos
        )
    {
        this.fallSpeed = FallSpeed;
        this.fallPos = FallPos;

        GetScreenBorders();
        CreateGameObject(NukePrefab);
        CreateTarget(NukeTargetPrefab);
        SpawnPosition();
        isActive = true;
    }

    // Create the nuke
    public virtual void CreateGameObject(GameObject NukePrefab)
    {
        nukeObject = GameObject.Instantiate(NukePrefab);

        rb = nukeObject.GetComponent<Rigidbody2D>();
        spriteRenderer = nukeObject.GetComponent<SpriteRenderer>();
    }

    // Create the spinning target
    public virtual void CreateTarget(GameObject TargetPrefab)
    {
        nukeTargetObject = GameObject.Instantiate(TargetPrefab);
    }

    // Get screen borders
    public void GetScreenBorders()
    {
        Camera cam = Camera.main;

        screenSizeX = cam.orthographicSize * cam.aspect;
        screenSizeY = cam.orthographicSize;
    }

    public virtual void SpawnPosition()
    {
        nukeObject.transform.position = fallPos + new Vector2(0, screenSizeY * 1.5f);
        nukeTargetObject.transform.position = fallPos;
    }

    public void Tick(float deltaTime)
    {
        rb.AddForce(new Vector2(0, -fallSpeed * deltaTime));
        nukeTargetObject.transform.Rotate(0f, 0f, GameManager.Instance.nukeTargetRotationSpeed * deltaTime, Space.Self);

        if (nukeObject.transform.position.y < fallPos.y)
        {
            Explode();
        }
    }

    public void DestroyObject()
    {
        UnityEngine.GameObject.Destroy(nukeObject);
        UnityEngine.GameObject.Destroy(nukeTargetObject);
    }

    public void Explode()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(nukeObject.transform.position, GameManager.Instance.nukeRange);

        foreach (Collider2D col in nearbyObjects)
        {
            // Check if the object implements INukable
            INukable nukable = col.GetComponent<INukable>();

            if (nukable != null)
            {
                nukable.NukeEffect();
            }
        }

        isActive = false;
    }
}