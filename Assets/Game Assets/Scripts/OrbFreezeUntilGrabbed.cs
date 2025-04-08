using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbFreezeUntilGrabbed : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Freeze movement
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Listen for the grab event
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        // Unfreeze when grabbed
        rb.constraints = RigidbodyConstraints.None;
    }

    private void OnDestroy() {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }
}
