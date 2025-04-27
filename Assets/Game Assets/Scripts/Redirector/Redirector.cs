using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Redirector : MonoBehaviour
{
    [Header("Redirector Settings")]
    public bool IsRotatable = false;

    [Header("Materials")]
    public Material NonRotatableMaterial;
    public Material RotatableMaterial;

    [Header("Rotation Settings")]
    [Range(0f, 45f)]
    public float MaxAngle = 15f;

    [Range(1f, 100f)]
    public float RotationSpeed = 15f;

    private XRGrabInteractable grabInteractable;
    private Quaternion sceneStartRotation;
    private Quaternion grabOffsetRotation;
    private XRBaseInteractor currentInteractor;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Enable/disable grab based on IsRotatable
        grabInteractable.enabled = IsRotatable;

        sceneStartRotation = transform.rotation;

        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = IsRotatable ? RotatableMaterial : NonRotatableMaterial;
        }

        if (IsRotatable)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDestroy()
    {
        if (IsRotatable)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as XRBaseInteractor;

        // Store the offset between the hand and the object when first grabbed
        grabOffsetRotation = Quaternion.Inverse(currentInteractor.transform.rotation) * transform.rotation;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    void LateUpdate()
    {
        if (currentInteractor == null) return;

        // Intended target rotation (based on hand rotation * initial offset at grab)
        Quaternion desiredRotation = currentInteractor.transform.rotation * grabOffsetRotation;

        // Clamp the rotation to ±MaxAngle from the sceneStartRotation
        Vector3 baseEuler = sceneStartRotation.eulerAngles;
        Vector3 desiredEuler = desiredRotation.eulerAngles;

        Vector3 clampedEuler = new Vector3(
            ClampAngleDelta(baseEuler.x, desiredEuler.x, MaxAngle),
            ClampAngleDelta(baseEuler.y, desiredEuler.y, MaxAngle),
            ClampAngleDelta(baseEuler.z, desiredEuler.z, MaxAngle)
        );

        Quaternion clampedRotation = Quaternion.Euler(clampedEuler);

        // Smoothly rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, clampedRotation, Time.deltaTime * RotationSpeed);
    }

    private float ClampAngleDelta(float baseAngle, float targetAngle, float maxDelta)
    {
        float delta = Mathf.DeltaAngle(baseAngle, targetAngle);
        delta = Mathf.Clamp(delta, -maxDelta, maxDelta);
        return baseAngle + delta;
    }
}
