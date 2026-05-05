using UnityEngine;

public class Fly : MonoBehaviour
{
    [Header("Flying Settings")]
    public float maxSpeed = 5.0f;
    
    [Header("References")]
    public Transform centerEyeAnchor; 

    void Update()
    {
        if (Teleport.isAiming) return;
        Vector2 thumbstick = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        if (thumbstick.magnitude > 0.1f)
        {
            Vector3 gazeForward = centerEyeAnchor.forward;
            Vector3 gazeRight = centerEyeAnchor.right;

            Vector3 moveDir = (gazeForward * thumbstick.y + gazeRight * thumbstick.x).normalized;

            Vector3 movement = moveDir * maxSpeed * thumbstick.magnitude * Time.deltaTime;

            transform.position += movement;
        }
    }
}

