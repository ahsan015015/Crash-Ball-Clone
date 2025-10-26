using UnityEngine;
using System;
using Unity.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Physics Settings")]
    public float launchForce = 12f;
    [Tooltip("Multiplier applied to velocity on each collision")]
    public float boostMultiplier = 1.05f;
    [Tooltip("Minimum speed the ball can have")]
    public float minVelocity = 5f;
    [Tooltip("Maximum speed the ball can reach")]
    public float maxVelocity = 25f;

    [Header("Debug")]
    [ReadOnly] public float currentVelocity; // Current speed of the ball

    private Rigidbody rb;
    private bool launched = false;
    public event Action OnBallDestroyed;

    [Header("Tags")]
    public string groundTag = "Ground";
    private AudioSource audioSource;
     public GameObject Last_HitByPlayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

         audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        Launch();
    }

    void Update()
    {
        // Update current speed
        currentVelocity = rb.velocity.magnitude;

        // Clamp velocity between min and max
        if (currentVelocity < minVelocity)
        {
            rb.velocity = rb.velocity.normalized * minVelocity;
        }
        else if (currentVelocity > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    public void Launch()
    {
        if (launched) return;
        launched = true;

        Vector3 dir = (transform.forward + Random.insideUnitSphere * 0.15f).normalized;
        dir.y = 0f;
        dir.Normalize();

        rb.velocity = dir * launchForce;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger || collision.collider.CompareTag(groundTag))
            return;

        // Boost velocity
        rb.velocity *= boostMultiplier;
        
            // Play hit sound
         audioSource.PlayOneShot(SoundManager.Instance.BallhitSound);

        // Keep movement on XZ plane
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Last_HitByPlayer = collision.gameObject;
        }
    }

    void OnDestroy()
    {
        OnBallDestroyed?.Invoke();
    }
}
