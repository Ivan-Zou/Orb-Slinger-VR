using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class GravityPad : MonoBehaviour
{
    [Header("Force Settings")]
    public Vector3 forceDirection = Vector3.up;
    public float forceStrength = 100f;

    [Header("Arrow Visual Settings")]
    public GameObject arrowPrefab;
    public float arrowSpacing = 0.35f;
    public float arrowScale = 0.4f;
    public float arrowSpeed = 0.5f;

    private List<Transform> arrows = new List<Transform>();

    void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 padSize = box.size;

        float[] positions = { -arrowSpacing, 0f, arrowSpacing };

        foreach (float x in positions)
        {
            foreach (float y in new List<float>() { -arrowSpacing, 0f, arrowSpacing, arrowSpacing + arrowSpacing })
            {
                foreach (float z in positions)
                {
                    Vector3 localPos = new Vector3(x, y, z);
                    Vector3 worldPos = transform.TransformPoint(localPos);

                    GameObject arrow = Instantiate(arrowPrefab, worldPos, Quaternion.identity, transform);
                    arrow.transform.localScale = Vector3.one * arrowScale;
                    arrow.transform.rotation = Quaternion.LookRotation(forceDirection.normalized);

                    arrows.Add(arrow.transform);
                }
            }
        }
    }


    void Update()
    {
        Vector3 localForceDir = transform.InverseTransformDirection(forceDirection.normalized);
        Vector3 worldForceDir = forceDirection.normalized;

        for (int i = 0; i < arrows.Count; i++)
        {
            Transform arrow = arrows[i];

            // Move in local space
            arrow.localPosition += localForceDir * arrowSpeed * Time.deltaTime;

            // Wrap around if outside the bounds of the cube
            Vector3 p = arrow.localPosition;
            // Calculate dynamic half-extents based on parent scale
            //Vector3 halfScale = transform.localScale * 0.5f;
            Vector3 halfScale = transform.localScale * 0.75f; // giving it a buffer for wrap-around

            if (Mathf.Abs(p.x) > halfScale.x) p.x = -Mathf.Sign(p.x) * halfScale.x;
            if (Mathf.Abs(p.y) > halfScale.y) p.y = -Mathf.Sign(p.y) * halfScale.y;
            if (Mathf.Abs(p.z) > halfScale.z) p.z = -Mathf.Sign(p.z) * halfScale.z;

            arrow.localPosition = p;
        }
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector3 worldForce = transform.TransformDirection(forceDirection.normalized) * forceStrength;
            rb.AddForce(worldForce, ForceMode.Acceleration);
        }
    }
}
