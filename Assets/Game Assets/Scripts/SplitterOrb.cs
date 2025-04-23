using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SplitterOrb : MonoBehaviour {
    public GameObject splitterOrbPrefab;
    public bool canSplit = true;

    private Rigidbody rb;
    private bool hasBeenThrown = false;
    private XRGrabInteractable grabInteractable;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        // Listen to grab and release events
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
        if (canSplit && hasBeenThrown) {
            canSplit = false;
            StartCoroutine(SplitAfterDelay(0.5f));
        }
    }

    IEnumerator SplitAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);

        Vector3 velocity = rb.linearVelocity;

        // Early out if orb isn't moving to prevent weird behavior
        if (velocity.magnitude < 0.01f) yield break;

        // Normalize the velocity direction to rotate it
        Vector3 baseDir = velocity.normalized;

        Vector3[] directions = new Vector3[] {
            Quaternion.Euler(0, -30.0f, 0) * baseDir,
            baseDir,
            Quaternion.Euler(0, 30.0f, 0) * baseDir
        };

        float spawnOffset = 0.25f;

        foreach (var dir in directions) {
            Vector3 spawnPos = transform.position + dir.normalized * spawnOffset;

            GameObject splitterOrb = Instantiate(splitterOrbPrefab, spawnPos, Quaternion.identity);
            Rigidbody splitterRb = splitterOrb.GetComponent<Rigidbody>();

            // Set velocity in same direction
            if (splitterRb != null) {

                splitterRb.constraints = RigidbodyConstraints.None;
                splitterRb.linearVelocity = dir.normalized * velocity.magnitude;
                
            }

            // disable freezing
            splitterOrb.GetComponent<OrbFreezeUntilGrabbed>().enabled = false;

            // enable trails
            splitterOrb.GetComponentInChildren<TrailRenderer>().enabled = true;

            // Prevent split again
            splitterOrb.GetComponent<SplitterOrb>().canSplit = false;

            // Add one bounce
            OrbBounceCounter counter = splitterOrb.GetComponent<OrbBounceCounter>();
            if (counter != null) {
                counter.hasBeenThrown = true;
                counter.bounceCount++;
            }

            // set lifetime manager
            OrbLifetimeManager lifeManger = splitterOrb.GetComponent<OrbLifetimeManager>();
            lifeManger.hasBeenThrown = true;
            lifeManger.StartMonitoring();

            // set sound effects
            splitterOrb.GetComponent<OrbSoundEffects>().hasBeenThrown = true;
        }

        // Destroy the original orb
        Destroy(gameObject);
    }

    private void OnDestroy() {
        // Clean up event listeners
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
