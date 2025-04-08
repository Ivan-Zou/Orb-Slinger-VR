using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbTrailController : MonoBehaviour {
    private XRGrabInteractable grabInteractable;
    private TrailRenderer trailRenderer;

    private void Awake() {
        grabInteractable = GetComponent<XRGrabInteractable>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        if (trailRenderer != null) {
            trailRenderer.enabled = false;
        } else {
            print("Trail is null on awake");
        }

        // Listen to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        // Reset trail in case it still has old data
        if (trailRenderer != null)  {
            trailRenderer.Clear();
            trailRenderer.enabled = false;
        } else {
            print("Trail is null on grab");
        }
    }

    private void OnReleased(SelectExitEventArgs args) {
        // Activate trail only after release
        if (trailRenderer != null) {
            trailRenderer.enabled = true;
        } else {
            print("Trail is null on release");
        }
    }

    private void OnDestroy() {
        // Clean up event listeners
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
