using UnityEngine;
using TMPro;

public class TimedOrb : MonoBehaviour {

    public float lifetime = 15.0f;
    public GameObject destructionParticles;
    public AudioClip explosionSound;
    public TMP_Text countdownText;

    float timeRemaining;

    OrbSpawner spawner;
    bool hasExploded = false;
    EndlessGameState state;

    public void SetSpawner(OrbSpawner spawnerRef) {
        spawner = spawnerRef;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        // countdownText = GetComponent<TMP_Text>();
        timeRemaining = lifetime;
        hasExploded = false;
        state = GameObject.FindGameObjectWithTag("State").GetComponent<EndlessGameState>(); 
    }

    // Update is called once per frame
    void Update() {
        if (hasExploded) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining > 0) {
            // Update countdown text
            countdownText.text = ((int) timeRemaining + 1).ToString();
        } else {
            // Destroy the orb
            Destroy(gameObject);
            
            OnTimedOrbExplosion();
        }
    }

    public void OnTimedOrbExplosion() {
        hasExploded = true;

        // Play explosion sound
        AudioSource.PlayClipAtPoint(explosionSound, gameObject.transform.position);

        if (state != null) {
            state.SubtractLife();
        }

        // Respawn the orb
        if (spawner != null)
        {
            spawner.OnTimedOrbDestroyed();
        }
    }
}
