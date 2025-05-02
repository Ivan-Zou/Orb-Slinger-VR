using UnityEngine;

public class TimedTargetSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] targetPrefabs;
    public GameObject scoreZonePrefab;
    public GameObject playPlat;
    public AudioClip spawnSound;
    public float xMin, xMax, yMin, yMax, zMin, zMax = 0.0f;
    public int maxTargets = 5;
    public int maxScoreZone = 2;

    void Start() {
        for (int i = 0; i < maxTargets; ++i) {
            SpawnTarget(false);
        }
    }
    
    void SpawnTarget(bool playSound = true) {
        int targetIdx = Random.Range(0, targetPrefabs.Length);
        float xPos = Random.Range(xMin, xMax);
        float yPos = Random.Range(yMin, yMax);
        float zPos = Random.Range(zMin, zMax);
        Vector3 initialPos = new Vector3(xPos, yPos, zPos);
        GameObject target = Instantiate(targetPrefabs[targetIdx], initialPos, Quaternion.identity);
        target.transform.LookAt(playPlat.transform);
        target.transform.eulerAngles = new Vector3(90, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
        if (Random.Range(0, 2) < 1) {
            TargetMover targetMover = target.GetComponent<TargetMover>();
            if (targetMover != null) {
                targetMover.staticTarget = false;
                targetMover.xDistance = Random.Range(0, 3);
                targetMover.yDistance = Random.Range(0, 3);
                targetMover.zDistance = Random.Range(0, 3);
            }
        }
        if (playSound) {
            AudioSource.PlayClipAtPoint(spawnSound, initialPos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] scoreables = GameObject.FindGameObjectsWithTag("Scoreable");
        int numTargets = 0;
        int numZones = 0;
        foreach (GameObject scorable in scoreables) {
            if (scorable.GetComponent<ScoreZoneBehavior>() != null) {
                ++numZones;
            } else {
                ++numTargets;
            }
        }
        while (numTargets < maxTargets) {
            SpawnTarget();
            ++numTargets;
        }
    }
}
