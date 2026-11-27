using UnityEngine;

public class SpinCatUI : MonoBehaviour
{
    // Variable
    [SerializeField] float rotationSpeed = 100f;

    // Rotate every frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
