using System.Collections;
using UnityEngine;

public class HitVisualSwitcher : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The default visual (active normally).")]
    public GameObject normalVisual;

    [Tooltip("The visual shown briefly on hit.")]
    public GameObject hitVisual;

    [Header("Settings")]
    [Tooltip("How long the hit visual stays active (seconds).")]
    public float hitDuration = 0.3f;

    private Coroutine hitRoutine;

    private void Start()
    {
        // Ensure correct initial state
        SetNormalState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BallController>() == null)
            return;

        // Trigger visual swap
        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        SetNormalState(false);
        yield return new WaitForSeconds(hitDuration);
        SetNormalState(true);
    }

    private void SetNormalState(bool normal)
    {
        if (normalVisual != null)
            normalVisual.SetActive(normal);
        if (hitVisual != null)
            hitVisual.SetActive(!normal);
    }
}