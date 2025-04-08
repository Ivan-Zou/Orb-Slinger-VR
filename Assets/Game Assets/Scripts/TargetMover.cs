using UnityEngine;

public class TargetMover : MonoBehaviour {

    public bool staticTarget = true;
    public float xDistance = 0.0f;
    public float yDistance = 0.0f;
    public float zDistance = 0.0f;
    public float movementSpeed = 1.0f;

    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (!staticTarget) {
            float x = xDistance * Mathf.Sin(Time.time * movementSpeed);
            float y = yDistance * Mathf.Sin(Time.time * movementSpeed);
            float z = zDistance * Mathf.Sin(Time.time * movementSpeed);

            transform.position = startPosition + new Vector3(x, y, z);
        }

    }
}
