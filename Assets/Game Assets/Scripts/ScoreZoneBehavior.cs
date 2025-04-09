using UnityEngine;

public class ScoreZoneBehavior : MonoBehaviour {

    [Header("Rotation Speed (degrees/second)")]
    public float rotationSpeedX = 0.0f;
    public float rotationSpeedY = 0.0f;
    public float rotationSpeedZ = 0.0f;

    [Header("Shrink Settings")]
    public float shrinkSpeed = 0.0f;
    public float minScaleThreshold = 0.05f;

    [Header("Lifetime in Seconds (-1 means infinite)")]
    public float lifetime = -1.0f;

    private float currentLifetime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        currentLifetime = lifetime;
    }

    // Update is called once per frame
    void Update() {
        // Handle Rotation
        transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime);

        // Handle Shrinking
        if (shrinkSpeed > 0) {
            float shrinkAmount = shrinkSpeed * Time.deltaTime;
            Vector3 newScale = transform.localScale - new Vector3(shrinkAmount, shrinkAmount, 0);

            // Avoid negative or too-small scale
            if (newScale.x <= minScaleThreshold || newScale.y <= minScaleThreshold || newScale.z <= minScaleThreshold) {
                Destroy(gameObject);
                return;
            }

            transform.localScale = newScale;
        }

        // Handle Lifetime
        if (lifetime > 0) {
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
