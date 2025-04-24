using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StickyOrb : MonoBehaviour {
    bool canStick = true;
    public float launchForce = 5.0f;
    public float delayBeforeLaunch = 1.0f;

    private bool hasBeenThrown = false;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        hasBeenThrown = false;
    }

    private void OnReleased(SelectExitEventArgs args) {
        hasBeenThrown = true;
    }

    void OnCollisionEnter(Collision collision) {
        if (canStick && hasBeenThrown) {
            canStick = false;

            // Freeze all motion
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Get the surface normal to launch in that direction
            Vector3 launchDirection = collision.contacts.Length > 0
                ? collision.contacts[0].normal
                : transform.up;

            StartCoroutine(LaunchAfterDelay(launchDirection));
        }
    }

    IEnumerator LaunchAfterDelay(Vector3 direction) {
        yield return new WaitForSeconds(delayBeforeLaunch);

        // Unfreeze
        rb.constraints = RigidbodyConstraints.None;

        // Apply force in surface normal direction
        rb.AddForce(direction.normalized * launchForce, ForceMode.Impulse);
    }

    void OnDestroy() {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
