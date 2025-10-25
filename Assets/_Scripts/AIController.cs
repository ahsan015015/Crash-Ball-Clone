using UnityEngine;

[RequireComponent(typeof(Transform))]
public class AIController : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 6f;

    // If true => AI moves on X axis (left-right) and Z is locked.
    // If false => AI moves on Z axis (forward-back) and X is locked.
    public bool moveOnX = true;

    // Separate limits for each axis so top/bottom and left/right can have different ranges
    public float moveLimitX = 4f;
    public float moveLimitZ = 4f;

    [Tooltip("Which side this AI defends. Should point toward the arena center from this paddle.")]
    public Vector3 defenseDirection = Vector3.back;

    // Optional: small threshold so AI doesn't jitter when target is nearly aligned
    public float stopThreshold = 0.05f;

    private BallController targetBall;
    
    /* ---------------------------------------------------------
     *  HUMAN-LIKE ERRORS
     *  Add these lines anywhere inside AIController
     *  (they plug straight into the existing Update loop)
     *---------------------------------------------------------*/
    [Header("Human-like flaws")]
    [Range(0,1)] public float reactionTime = 0.18f;   // avg seconds before it notices
    [Range(0,1)] public float overshoot   = 0.12f;    // 0 = perfect stop, 1 = 100 % overshoot
    [Range(0,1)] public float indecision  = 0.07f;    // chance per frame to freeze
    [Range(0,1)] public float misread     = 0.05f;    // chance to aim at the wrong offset

    float nextReaction;          // countdown until it “sees” the ball again
    float fakeTargetOffset;      // how far it thinks the ball is from its real axis

    void Start()                 // initialise randomness
    {
        nextReaction = Random.Range(0f, reactionTime);
        fakeTargetOffset = 0f;
    }

    Vector3 HumaniseTarget(Vector3 realTarget)
    {
        /* 1. reaction delay ---------------------------------------------------- */
        nextReaction -= Time.deltaTime;
        if (nextReaction > 0) return transform.position;   // still “blind”
        nextReaction = Random.Range(0.7f, 1.3f) * reactionTime;

        /* 2. mis-read ball position ------------------------------------------- */
        if (Random.value < misread)
            fakeTargetOffset = Random.Range(-0.6f, 0.6f);
        else
            fakeTargetOffset = Mathf.MoveTowards(fakeTargetOffset, 0, Time.deltaTime * 2f);

        Vector3 flawed = realTarget;
        if (moveOnX) flawed.x += fakeTargetOffset;
        else         flawed.z += fakeTargetOffset;

        /* 3. occasional freeze (“oops, which way?”) --------------------------- */
        if (Random.value < indecision) return transform.position;

        return flawed;
    }


    void Update()
    {
        FindTargetBall();
        if (targetBall == null) return;
        
        /* >>>>>>>>>>  NEW LINE  <<<<<<<<<< */
        Vector3 targetPos = HumaniseTarget(targetBall.transform.position);

        // Ball direction (velocity); if zero, do nothing
        Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
        if (ballRb == null) return;

        Vector3 ballDir = ballRb.velocity;
        if (ballDir.sqrMagnitude < 0.01f) return;

        // If the ball is moving away from this AI (dot > 0), ignore it
        // (defenseDirection should point outward from paddle toward the arena center,
        //  so dot < 0 means ball moving toward that paddle)
        if (Vector3.Dot(ballDir.normalized, defenseDirection.normalized) > 0f)
            return;

        // Get the target position (we only care about one axis)
        //Vector3 targetPos = targetBall.transform.position;
        Vector3 currentPos = transform.position;
        Vector3 desiredPos = currentPos;

        if (moveOnX)
        {
            float deltaX = targetPos.x - currentPos.x;
            if (Mathf.Abs(deltaX) > stopThreshold)
            {
                float move = Mathf.Sign(deltaX) * moveSpeed * Time.deltaTime;
                // don't overshoot: clamp move to not pass target in this frame
                if (Mathf.Abs(move) > Mathf.Abs(deltaX)) move = deltaX;
                desiredPos.x = currentPos.x + move;
            }

            // lock Z and clamp X to limits
            desiredPos.z = currentPos.z; // ensure Z never changes
            desiredPos.x = Mathf.Clamp(desiredPos.x, -moveLimitX, moveLimitX);
        }
        else
        {
            float deltaZ = targetPos.z - currentPos.z;
            if (Mathf.Abs(deltaZ) > stopThreshold)
            {
                float move = Mathf.Sign(deltaZ) * moveSpeed * Time.deltaTime;
                if (Mathf.Abs(move) > Mathf.Abs(deltaZ)) move = deltaZ;
                desiredPos.z = currentPos.z + move;
            }

            // lock X and clamp Z to limits
            desiredPos.x = currentPos.x; // ensure X never changes
            desiredPos.z = Mathf.Clamp(desiredPos.z, -moveLimitZ, moveLimitZ);
        }
        
        /* >>>>>>>>>>  NEW LINE  <<<<<<<<<< */
        desiredPos = Vector3.Lerp(transform.position, desiredPos, 1f - overshoot);

        transform.position = desiredPos;
    }

    void FindTargetBall()
    {
        BallController[] balls = FindObjectsOfType<BallController>();
        if (balls.Length == 0)
        {
            targetBall = null;
            return;
        }

        // find the closest ball (simple, cheap)
        float minDist = Mathf.Infinity;
        BallController closest = null;
        foreach (var b in balls)
        {
            float dist = (b.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = b;
            }
        }
        targetBall = closest;
    }
    
    // ✅ Draw movement boundary only in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position;

        // Lift line a bit above ground so it's visible
        float yOffset = 0.2f;
        pos.y += yOffset;

        if (moveOnX)
        {
            Vector3 left = new Vector3(-moveLimitX, pos.y, pos.z);
            Vector3 right = new Vector3(moveLimitX, pos.y, pos.z);
            Gizmos.DrawLine(left, right);
        }
        else
        {
            Vector3 front = new Vector3(pos.x, pos.y, moveLimitZ);
            Vector3 back = new Vector3(pos.x, pos.y, -moveLimitZ);
            Gizmos.DrawLine(front, back);
        }
    }
}
