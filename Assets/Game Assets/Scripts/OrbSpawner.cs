using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class OrbSpawner : MonoBehaviour {
    [Header("Orb Prefabs")]
    public GameObject standardOrb;
    public GameObject pulseOrb;

    [Header("Spawn Options")]
    public bool spawnStandard = true;
    public bool spawnPulse = false;

    public bool spawnInPlace = true;
    public float spawnGap = 0.25f;

    [Header("If spawnInPlace is false, spawn orbs randomly in area")]
    public float spawnAreaSizeX = 1.0f;
    public float spawnAreaSizeZ = 1.0f;
    public float spawnHeightY = 1.0f;

    [Header("Respawn Delay")]
    public float respawnDelay = 2.0f;

    private Dictionary<string, GameObject> spawnedOrbs = new Dictionary<string, GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        TrySpawnAll();
    }
    void TrySpawnAll() {
        int spawnIndex = 0;

        if (spawnStandard) TrySpawnOrb("Standard", standardOrb, spawnIndex++);
        if (spawnPulse) TrySpawnOrb("Pulse", pulseOrb, spawnIndex++);
    }

    void TrySpawnOrb(string orbType, GameObject prefab, int index) {
        if (spawnedOrbs.ContainsKey(orbType) && spawnedOrbs[orbType] != null) return;

        Vector3 spawnPos = transform.position;

        if (spawnInPlace) {
            spawnPos += new Vector3(index * spawnGap, 0, 0);
        } else {
            float randX = Random.Range(-spawnAreaSizeX / 2.0f, spawnAreaSizeX / 2.0f);
            float randZ = Random.Range(-spawnAreaSizeZ / 2.0f, spawnAreaSizeZ / 2.0f);
            spawnPos += new Vector3(randX, 0, randZ);
        }

        spawnPos.y = spawnHeightY;

        GameObject orb = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedOrbs[orbType] = orb;

        // Subscribe to grab event
        XRGrabInteractable grab = orb.GetComponent<XRGrabInteractable>();
        if (grab != null) {
            grab.selectEntered.AddListener((args) => OnOrbGrabbed(orbType));
        }
    }

    void OnOrbGrabbed(string orbType) {
        if (spawnedOrbs.ContainsKey(orbType)) {
            // Remove reference
            spawnedOrbs[orbType] = null;
        }

        // Respawn after delay
        StartCoroutine(RespawnAfterDelay(orbType));
    }

    IEnumerator RespawnAfterDelay(string orbType) {
        yield return new WaitForSeconds(respawnDelay);

        int spawnIndex = 0;
        if (orbType == "Standard") TrySpawnOrb("Standard", standardOrb, spawnIndex);
        else if (orbType == "Pulse") TrySpawnOrb("Pulse", pulseOrb, spawnIndex + (spawnStandard ? 1 : 0));
    }
}
