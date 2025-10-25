using System;
using System.Collections;
using UnityEngine;

public class Rim : MonoBehaviour
{
    public enum RimMode
    {
        Deactivate, // Simply turns off after delay
        Blink       // Blinks on/off before turning off
    }

    [Header("Rim Activation Settings")]
    public GameObject[] rimsToActivate;
    public RimMode rimMode = RimMode.Deactivate; // Select behavior mode

    [Header("Deactivate Settings")]
    public float deactivateDelay = 1.5f;         // Time before turning off

    [Header("Blink Settings")]
    public float blinkDuration = 2f;             // Total time to blink
    public float blinkInterval = 0.2f;           // On/off interval during blink

    private Coroutine deactivateRoutine;

    private void Start()
    {
        // Ensure rims start off
        foreach (GameObject rim in rimsToActivate)
        {
            rim.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Only react to BallController collisions
        if (other.gameObject.GetComponent<BallController>() == null) return;

        // Activate all rims
        foreach (GameObject rim in rimsToActivate)
            rim.SetActive(true);

        // Restart coroutine if one’s running
        if (deactivateRoutine != null)
            StopCoroutine(deactivateRoutine);

        deactivateRoutine = StartCoroutine(HandleRimBehavior());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react to BallController collisions
        if (other.gameObject.GetComponent<BallController>() == null) return;

        // Activate all rims
        foreach (GameObject rim in rimsToActivate)
            rim.SetActive(true);

        // Restart coroutine if one’s running
        if (deactivateRoutine != null)
            StopCoroutine(deactivateRoutine);

        deactivateRoutine = StartCoroutine(HandleRimBehavior());
    }

    private IEnumerator HandleRimBehavior()
    {
        switch (rimMode)
        {
            case RimMode.Deactivate:
                yield return new WaitForSeconds(deactivateDelay);
                foreach (GameObject rim in rimsToActivate)
                    rim.SetActive(false);
                break;

            case RimMode.Blink:
                float elapsed = 0f;
                bool isActive = true;

                while (elapsed < blinkDuration)
                {
                    isActive = !isActive;
                    foreach (GameObject rim in rimsToActivate)
                        rim.SetActive(isActive);

                    yield return new WaitForSeconds(blinkInterval);
                    elapsed += blinkInterval;
                }

                // Ensure off at end
                foreach (GameObject rim in rimsToActivate)
                    rim.SetActive(false);
                break;
        }

        deactivateRoutine = null;
    }
}
