using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Settings")]
    public float maxRayLength = 15f;
    public float rotationSpeed = 90f; 
    public LayerMask validTeleportLayers; 
    [Header("References")]
    public Transform ovrCameraRig;
    public Transform centerEyeAnchor;
    public Transform leftControllerAnchor; 
    public GameObject teleportIndicatorPrefab;

    private LineRenderer lineRenderer;
    private GameObject currentIndicator;
    
    public static bool isAiming = false;
    private Vector3 targetPosition;
    private float targetRotationY;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;
        lineRenderer.enabled = false;

        currentIndicator = Instantiate(teleportIndicatorPrefab);
        currentIndicator.SetActive(false);
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            AimTeleport();
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            ExecuteTeleport();
        }
        else
        {
            CancelTeleport();
        }
    }

    void AimTeleport()
    {
        Ray ray = new Ray(leftControllerAnchor.position, leftControllerAnchor.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayLength, validTeleportLayers))
        {
            isAiming = true;
            targetPosition = hit.point;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, leftControllerAnchor.position);
            lineRenderer.SetPosition(1, targetPosition);

            currentIndicator.SetActive(true);
            currentIndicator.transform.position = targetPosition;

            Vector2 leftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            if (Mathf.Abs(leftThumbstick.x) > 0.1f)
            {
                targetRotationY += leftThumbstick.x * rotationSpeed * Time.deltaTime;
            }

            currentIndicator.transform.rotation = Quaternion.Euler(0, targetRotationY, 0);
        }
        else
        {
            CancelTeleport();
        }
    }

    void ExecuteTeleport()
    {
        if (isAiming)
        {
            Vector3 offsetToCenter = new Vector3(centerEyeAnchor.localPosition.x, 0, centerEyeAnchor.localPosition.z);
            ovrCameraRig.position = targetPosition - offsetToCenter;

            float currentHeadRotY = centerEyeAnchor.eulerAngles.y;
            float rotationDifference = targetRotationY - currentHeadRotY;

            ovrCameraRig.RotateAround(centerEyeAnchor.position, Vector3.up, rotationDifference);
        }
        
        CancelTeleport();
    }

    void CancelTeleport()
    {
        isAiming = false;
        lineRenderer.enabled = false;
        currentIndicator.SetActive(false);
    }
}

