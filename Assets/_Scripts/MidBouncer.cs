using System.Collections;
using UnityEngine;

public class MidBouncer : MonoBehaviour
{
    [SerializeField] private float minDelayTime = 1f, maxDelayTime = 3f;
    [SerializeField] private float stayUpTime = 2f;
    [SerializeField] private float moveSpeed = 2f; // speed of bounce

    private Vector3 startPosition;
    private Vector3 targetPosition;

    [SerializeField] private Material HitMaterial, idleMaterial;
    private GameObject DeadLine;
    private bool DeadLineActive = false;

    void Start()
    {
        startPosition = new Vector3(0f, -1f, 0f);
        targetPosition = new Vector3(0f, 1f, 0f);
        transform.position = startPosition;
        PlayAnimation();
    }

    void PlayAnimation()
    {
        float delayTime = Random.Range(minDelayTime, maxDelayTime);
        StartCoroutine(BounceRoutine(delayTime));
    }

    IEnumerator BounceRoutine(float delay)
    {
        if (GameManager.Instance.gameEnded)
            yield break;
        // Wait before starting
        yield return new WaitForSeconds(delay);

        // Move up
        yield return StartCoroutine(MoveToPosition(targetPosition));

        // Stay at the top
        yield return new WaitForSeconds(stayUpTime);

        // Move down
        yield return StartCoroutine(MoveToPosition(startPosition));

        // Repeat
        PlayAnimation();
    }

     IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BallController>() != null && !GameManager.Instance.gameEnded)
        {

            GameObject Last_HitByPlayer = collision.gameObject.GetComponent<BallController>().Last_HitByPlayer;

            if (Last_HitByPlayer != null)
            {
                GameObject Deadline = Last_HitByPlayer.GetComponent<Player>().onDeathActivateObject;
                if (Deadline != null && !DeadLineActive)
                {
                    Deadline.SetActive(true);
                    DeadLineActive = true;
                     
                    StartCoroutine(DeactivateDeadlineAfterDelay(Deadline, 3f, 5f));
                }
                
            }
            // Change material on hit
            GetComponent<Renderer>().material = HitMaterial;

            // Revert back to idle material after a short delay
            StartCoroutine(RevertMaterialAfterDelay(0.5f));

        }
    }
    IEnumerator RevertMaterialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Renderer>().material = idleMaterial;
    }
    IEnumerator DeactivateDeadlineAfterDelay(GameObject Deadline, float MinDelay, float MaxDelay)
    {   
       
        float delay = Random.Range(MinDelay, MaxDelay);
        yield return new WaitForSeconds(delay);
        
           
            DeadLineActive = false;
            Deadline.SetActive(false);
        
    }
}
