using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbLifetimeManager : MonoBehaviour {
    public float speedThreshold = 0.5f;

    // Wait this long after throw before checking
    public float checkDelay = 1.0f; 
    public int maxBounces = 30;

    // Time under speed threshold before destroy
    public float minSpeedDuration = 1.0f; 

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool hasBeenThrown = false;
    private int bounceCount = 0;
    private float lowSpeedTimer = 0;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args) {
        hasBeenThrown = true;
        // Start checking speed after delay
        InvokeRepeating(nameof(CheckSpeed), checkDelay, 0.1f); 
    }

    private void OnCollisionEnter(Collision collision) {
        if (hasBeenThrown && collision.relativeVelocity.magnitude > 0.01f) {
            bounceCount++;
            if (bounceCount >= maxBounces) {
                Destroy(gameObject);
            }
        }
    }

    void CheckSpeed() {
        if (!hasBeenThrown) return;

        float speed = rb.linearVelocity.y;
        if (speed < speedThreshold) {
            lowSpeedTimer += 0.1f;
            if (lowSpeedTimer >= minSpeedDuration) {
                Destroy(gameObject);
            }
        } else {
            // Reset timer if speed picks up again
            lowSpeedTimer = 0; 
        }
    }

    void OnDestroy() {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
