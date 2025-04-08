using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OrbSoundEffects : MonoBehaviour {
    public AudioClip throwSound;
    public AudioClip bounceSound;

    private XRGrabInteractable grabInteractable;

    private bool hasBeenThrown = false;

    void Awake() {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args) {
        hasBeenThrown = true;
        PlaySound(throwSound);
    }

    private void OnCollisionEnter(Collision collision) {
        if (hasBeenThrown && collision.relativeVelocity.magnitude > 0.1f) {
            PlaySound(bounceSound);
        }
    }

    private void PlaySound(AudioClip clip) {
        if (clip != null) {
            AudioSource.PlayClipAtPoint(clip, gameObject.transform.position);
        }
    }

    private void OnDestroy() {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
