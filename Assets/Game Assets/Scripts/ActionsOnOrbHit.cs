using UnityEngine;

public class ActionsOnOrbHit : MonoBehaviour {
    // Set the value of this target/score zone in its prefab
    public float pointValue = 0;
    private void OnCollisionEnter(Collision collision) {
        Collider collider = collision.collider;

        if (collider.CompareTag("Orb")) {
            // Get the bounce count
            OrbBounceCounter bounceCounter = collider.gameObject.GetComponent<OrbBounceCounter>();
            int bounceCount = bounceCounter.bounceCount;
            // Destroy the target/score zone if the orb bounced at least once
            if (bounceCount > 0) {
                Destroy(gameObject);
                // Do stuff with pointValue
            }
            
        }
    }
}
