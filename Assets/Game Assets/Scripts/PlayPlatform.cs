using Microsoft.Win32.SafeHandles;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayPlatform : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject xrObj;
    public XROrigin xrOrigin;
    public TeleportationProvider teleportProvider;
    public Collider platformCollider;
    private float xNeg, xPos, zNeg, zPos, xMid, zMid;
    private Vector3 teleportPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Components
        xrOrigin = xrObj.GetComponent<XROrigin>();
        playerTransform = xrObj.GetNamedChild("Camera Offset").GetNamedChild("Main Camera").transform;
        teleportProvider = xrObj.GetComponent<TeleportationProvider>();
        platformCollider = GetComponent<BoxCollider>();
        Vector3 minPoint = platformCollider.bounds.min;
        Vector3 maxPoint = platformCollider.bounds.max;
        xNeg = minPoint.x;
        zNeg = minPoint.z;
        xPos = maxPoint.x;
        zPos = maxPoint.z;
        teleportPos = new Vector3(xNeg  + ((xPos - xNeg) / 2), transform.position.y, zNeg + ((zPos - zNeg) / 2));
        Debug.LogFormat("xNeg = {0}, zNeg = {1}, xPos = {2}, zPos = {3}", xNeg, zNeg, xPos, xPos);
    }

    void TeleportPlayer() {
        Debug.Log("Teleporting the player to the center of the platform");
        Debug.LogFormat("Current Position: {0} | New Position: {1}", playerTransform.position, teleportPos);
        // Create Teleport Request
        TeleportRequest req = new TeleportRequest() {
            destinationPosition = teleportPos,
            destinationRotation = playerTransform.rotation
        };
        teleportProvider.QueueTeleportRequest(req);


        // TODO: Apply penalty to player based on GameMode
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is out of play platform
        Vector3 playerPos = playerTransform.position;
        if (playerPos.x < xNeg || playerPos.x > xPos || 
            playerPos.z < zNeg || playerPos.z > zPos) {
            // Teleport the player back to the center of the platform
            TeleportPlayer();
        }
    }
}
