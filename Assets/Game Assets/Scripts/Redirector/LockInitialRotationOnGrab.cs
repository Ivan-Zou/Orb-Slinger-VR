using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class LockRotationToBounds : MonoBehaviour
{
    private Quaternion sceneStartRotation;
    private Quaternion grabOffsetRotation;
    private XRBaseInteractor currentInteractor;

    private const float MaxAngle = 15f;

    void Awake()
    {
        sceneStartRotation = transform.rotation;

        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    void OnDestroy()
    {
        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
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
        transform.rotation = Quaternion.Slerp(transform.rotation, clampedRotation, Time.deltaTime * 15f);
    }

    private float ClampAngleDelta(float baseAngle, float targetAngle, float maxDelta)
    {
        float delta = Mathf.DeltaAngle(baseAngle, targetAngle);
        delta = Mathf.Clamp(delta, -maxDelta, maxDelta);
        return baseAngle + delta;
    }
}
