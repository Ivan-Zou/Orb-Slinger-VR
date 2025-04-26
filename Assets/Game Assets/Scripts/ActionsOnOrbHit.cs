using UnityEngine;

public class ActionsOnOrbHit : MonoBehaviour {
    // Set the value of this target/score zone in its prefab
    public int pointValue = 0;

    public GameObject hitExplosion;
    public AudioClip hitSound;
    public GameState state;

    OrbSpawner spawner;

    void Start() {
        state = GameObject.FindGameObjectWithTag("State").GetComponent<GameState>(); 
        GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
        if (spawnerObj != null) {
            spawner = spawnerObj.GetComponent<OrbSpawner>();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Collider collider = collision.collider;

        if (collider.CompareTag("Orb")) {
            // Get the bounce count
            OrbBounceCounter bounceCounter = collider.gameObject.GetComponent<OrbBounceCounter>();
            int bounceCount = bounceCounter.bounceCount;
            // Destroy the target/score zone if the orb bounced at least once
            if (bounceCount > 0) {
                // Play damaged sound
                AudioSource.PlayClipAtPoint(hitSound, gameObject.transform.position);
                // Create explosion particles
                Instantiate(hitExplosion, gameObject.transform.position, Quaternion.identity);
                // Destroy the target/score zone
                Destroy(gameObject);
                // Respawn the orb if its a Timed Orb
                TimedOrb timed = collider.gameObject.GetComponent<TimedOrb>();
                if (timed != null && spawner != null) {
                    spawner.OnTimedOrbDestroyed();
                }
                // Destroy the orb
                Destroy(bounceCounter.gameObject);
                // Do stuff with pointValue
                state.UpdateScore(pointValue * bounceCount);
            }
            
        }
    }
}
