using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbBounceCounter : MonoBehaviour {
    public int bounceCount;
    private XRGrabInteractable grabInteractable;

    public bool hasBeenThrown = false;

    void Awake() {
        grabInteractable = GetComponent<XRGrabInteractable>();
        bounceCount = 0;

        // Listen to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        hasBeenThrown = false;
        bounceCount = 0;
    }

    private void OnReleased(SelectExitEventArgs args) {
        hasBeenThrown = true;
        bounceCount = 0;
    }

    private void OnCollisionEnter(Collision collision) {
        Collider collider = collision.collider;
        // increment bounce count if the orb has been thrown and bounces on something thats not a scoreable object
        if (hasBeenThrown && collision.relativeVelocity.magnitude > 0.01f & !collider.CompareTag("Scoreable")) {
            bounceCount++;
        }
    }

    private void OnDestroy() {
        // Clean up event listeners
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
