using UnityEngine;

public class DoorFollowHandle : MonoBehaviour
{
    public Transform handle; // Assign your handle here
    public Transform doorHinge; // The pivot point of the door

    private Quaternion initialHandleRotation;
    private Quaternion initialDoorRotation;

    void Start()
    {
        // Store the initial local rotations
        initialHandleRotation = handle.localRotation;
        initialDoorRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate the rotation difference of the handle from its starting rotation
        Quaternion handleRotationDelta = Quaternion.Inverse(initialHandleRotation) * handle.localRotation;

        // Apply that rotation difference to the door
        transform.rotation = initialDoorRotation * handleRotationDelta;
    }
}
