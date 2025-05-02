using UnityEngine;

public class EndlessTargetSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] targetPrefabs;
    public GameObject scoreZonePrefab;
    public GameObject playPlat;
    public AudioClip spawnSound;
    GameObject target;

    void Start () {
        if (target == null) SpawnTarget(false);
    }
    
    void SpawnTarget(bool playSound = true) {
        target = Instantiate(
                Random.Range(0.0f, 1.0f) < 0.75f ? targetPrefabs[Random.Range(0, targetPrefabs.Length)] : scoreZonePrefab,
                gameObject.transform.position, 
                Quaternion.identity);
        target.transform.LookAt(playPlat.transform);
        target.transform.eulerAngles = new Vector3(90, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
        TargetMover targetMover = target.GetComponent<TargetMover>();
        if (targetMover != null) {
            targetMover.staticTarget = false;
            targetMover.xDistance = Random.Range(0, 3);
            targetMover.yDistance = Random.Range(0, 3);
            targetMover.zDistance = Random.Range(0, 3);
        } else {
            ScoreZoneBehavior scoreZoneBehavior = target.GetComponent<ScoreZoneBehavior>();
            if (scoreZoneBehavior != null) {
                scoreZoneBehavior.shrinkSpeed = 0.1f;
                scoreZoneBehavior.rotationSpeedX = 30;
                scoreZoneBehavior.rotationSpeedY = 30;
                scoreZoneBehavior.rotationSpeedZ = 30;
            }
        }
        if (playSound) {
            AudioSource.PlayClipAtPoint(spawnSound, gameObject.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) SpawnTarget();
    }
}
