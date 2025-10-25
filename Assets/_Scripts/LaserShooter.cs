using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ContinuousAutoLaser : MonoBehaviour
{
    public float laserLength = 30f;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.enabled = true;
        lr.positionCount = 2;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.useWorldSpace = true;
        lr.startColor = Color.red;
        lr.endColor = Color.red;
    }

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        if (lr == null) return;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * laserLength;

        // Optional: check collisions (will only work in play mode)
        if (Application.isPlaying)
        {
            if (Physics.Raycast(startPos, transform.forward, out RaycastHit hit, laserLength))
                endPos = hit.point;
        }

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    // Draw a gizmo for editor preview
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * laserLength);
        Gizmos.DrawSphere(transform.position + transform.forward * laserLength, 0.05f);
    }
}