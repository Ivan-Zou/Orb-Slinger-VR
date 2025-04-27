using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class GravityPad : MonoBehaviour
{
    [Header("Force Settings")]
    public float forceStrength = 100f;

    [Header("Arrow Movement Settings")]
    [Range(0f, 1f)]
    public float arrowSpeedToForceStrengthRatio = 0.5f;

    [Header("Arrow Layout Settings")]
    public GameObject arrowPrefab;

    [Tooltip("Number of additional strips horizontally. 0 = 1x1 centered, 1 = 3x3 grid, 2 = 5x5 grid, etc.")]
    [Min(0)]
    public int additionalStripsHorizontal = 1;
    public float horizontalStripSpacing = 0.33f;

    [Tooltip("Number of additional arrows vertically along each strip. 0 = center arrow only, 1 = 3 arrows, 2 = 5 arrows, etc.")]
    [Min(0)]
    public int additionalArrowsVertical = 3;
    public float verticalArrowSpacing = 0.33f;

    // Constants
    private const float _arrowLocalScale = 0.3f;

    // Internal Buffers
    private List<Transform> _arrowInstances = new List<Transform>();

    // Precomputed Values
    private float _arrowSpeed;
    private float _localWrapThreshold;
    private float _localWrapResetOffset;

    private MaterialPropertyBlock _mpb;
    private int _gravityPadMatrixID;

    void Start()
    {
        _gravityPadMatrixID = Shader.PropertyToID("_GravityPadWorldToLocal");
        _mpb = new MaterialPropertyBlock();
        _mpb.SetMatrix(_gravityPadMatrixID, transform.worldToLocalMatrix);

        // Assume your BoxCollider defines pad size
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 halfSize = Vector3.Scale(box.size, transform.localScale) * 0.5f;

        _mpb.SetVector("_PadHalfSize", halfSize);

        // Remove any manually placed arrows under GravityPad
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Arrow"))
            {
                Destroy(child.gameObject);
            }
        }

        _arrowSpeed = arrowSpeedToForceStrengthRatio * forceStrength * 0.03f;
        _localWrapThreshold = (verticalArrowSpacing / transform.lossyScale.y) * (additionalArrowsVertical + 1);
        _localWrapResetOffset = (verticalArrowSpacing / transform.lossyScale.y) * ((additionalArrowsVertical * 2) + 1);

        for (int xIdx = -additionalStripsHorizontal; xIdx <= additionalStripsHorizontal; ++xIdx)
        {
            for (int zIdx = -additionalStripsHorizontal; zIdx <= additionalStripsHorizontal; ++zIdx)
            {
                for (int yIdx = -additionalArrowsVertical; yIdx <= additionalArrowsVertical; ++yIdx)
                {
                    Vector3 localPos = new Vector3(
                        xIdx * horizontalStripSpacing,
                        yIdx * verticalArrowSpacing,
                        zIdx * horizontalStripSpacing
                    );

                    Vector3 worldPos = transform.position + (transform.rotation * localPos);

                    GameObject arrow = Instantiate(arrowPrefab, worldPos, Quaternion.identity);

                    arrow.transform.SetParent(transform, worldPositionStays: true);
                    arrow.transform.localScale = new Vector3(
                            _arrowLocalScale / transform.lossyScale.x,
                            _arrowLocalScale / transform.lossyScale.y,
                            _arrowLocalScale / transform.lossyScale.z
                    );
                    arrow.transform.rotation = transform.rotation * Quaternion.Euler(-90, 0, 0);    // Arrows point "up" relative to GravityPad rotation

                    var renderer = arrow.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.SetPropertyBlock(_mpb);
                    }

                    _arrowInstances.Add(arrow.transform);
                }
            }
        }
    }

    void Update()
    {
        // TODO: Move this to Start() if we end up not rotating GravityPads dynamically
        Vector3 localUp = transform.InverseTransformDirection(transform.up);
        // TODO: Remove this if we end up not rotating GravityPads dynamically
        _mpb.SetMatrix(_gravityPadMatrixID, transform.worldToLocalMatrix);

        foreach (Transform arrow in _arrowInstances)
        {
            arrow.localPosition += localUp * _arrowSpeed * Time.deltaTime;

            // Smooth wrap-around on Y
            if (arrow.localPosition.y > _localWrapThreshold)
            {
                Vector3 p = arrow.localPosition;
                p.y -= _localWrapResetOffset;
                arrow.localPosition = p;
            }

            // TODO: Remove this if we end up not rotating GravityPads dynamically
            var renderer = arrow.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.SetPropertyBlock(_mpb);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            Vector3 worldForce = transform.up * forceStrength;
            rb.AddForce(worldForce, ForceMode.Acceleration);
        }
    }

    private HashSet<Rigidbody> _bouncedBodies = new HashSet<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        var orbCounter = other.GetComponent<OrbBounceCounter>();
        Rigidbody rb = other.attachedRigidbody;

        if (orbCounter != null && rb != null && orbCounter.hasBeenThrown && !_bouncedBodies.Contains(rb))
        {
            orbCounter.bounceCount++;
            _bouncedBodies.Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            _bouncedBodies.Remove(rb);
        }
    }
}
