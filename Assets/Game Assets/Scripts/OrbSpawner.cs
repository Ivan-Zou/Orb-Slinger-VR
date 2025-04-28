using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class OrbSpawner : MonoBehaviour {
    [Header("Orb Prefabs")]
    public GameObject standardOrb;
    public GameObject pulseOrb;
    public GameObject splitterOrb;
    public GameObject stickyOrb;
    public GameObject timedOrb;

    [Header("Spawn Options")]
    public bool spawnStandard = true;
    public bool spawnPulse = false;
    public bool spawnSplitter = false;
    public bool spawnSticky = false;
    public bool spawnTimed = false;

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
        if (spawnSplitter) TrySpawnOrb("Splitter", splitterOrb, spawnIndex++);
        if (spawnSticky) TrySpawnOrb("Sticky", stickyOrb, spawnIndex++);
        if (spawnTimed) TrySpawnOrb("Timed", timedOrb, spawnIndex++);
    }

    void TrySpawnOrb(string orbType, GameObject prefab, int index) {
        if (spawnedOrbs.ContainsKey(orbType) && spawnedOrbs[orbType] != null) return;

        Vector3 spawnPos = transform.position;

        if (spawnInPlace) {
            spawnPos += transform.rotation * new Vector3(index * spawnGap, 0, 0);
        } else {
            float randX = Random.Range(-spawnAreaSizeX / 2.0f, spawnAreaSizeX / 2.0f);
            float randZ = Random.Range(-spawnAreaSizeZ / 2.0f, spawnAreaSizeZ / 2.0f);
            spawnPos += transform.rotation * new Vector3(randX, 0, randZ);
        }

        spawnPos.y = spawnHeightY;

        GameObject orb = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedOrbs[orbType] = orb;

        // Set spawner reference for TimedOrb specifically
        TimedOrb timed = orb.GetComponent<TimedOrb>();
        if (timed != null) {
            timed.SetSpawner(this);
        }

        // Subscribe to grab event
        XRGrabInteractable grab = orb.GetComponent<XRGrabInteractable>();
        if (grab != null && orbType != "Timed") {
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
        else if (orbType == "Splitter") TrySpawnOrb("Splitter", splitterOrb, spawnIndex + (spawnStandard ? 1 : 0) + (spawnPulse ? 1 : 0));
        else if (orbType == "Sticky") TrySpawnOrb("Sticky", stickyOrb, spawnIndex + (spawnStandard ? 1 : 0) + (spawnPulse ? 1 : 0) + (spawnSplitter ? 1 : 0));
        else if (orbType == "Timed") TrySpawnOrb("Timed", timedOrb, spawnIndex + (spawnStandard ? 1 : 0) + (spawnPulse ? 1 : 0) + (spawnSplitter ? 1 : 0) + (spawnSticky ? 1 : 0));
    }

    public void OnTimedOrbDestroyed() {
        if (spawnedOrbs.ContainsKey("Timed")) {
            spawnedOrbs["Timed"] = null;
        }

        StartCoroutine(RespawnAfterDelay("Timed"));
    }
}
