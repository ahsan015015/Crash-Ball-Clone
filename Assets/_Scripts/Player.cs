using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Info")]
    public PlayerType.TypeOfPlayer playerType;
    public TMP_Text playerHealthText;
    public int playerHealth;
    
    [Header("Death Settings")]
    [Tooltip("Direction the player moves backward when dying.")]
    public Vector3 deathMoveDirection = Vector3.back;
    
    [Tooltip("How far backward the player moves when dying.")]
    public float deathMoveDistance = 0.5f;

    [Tooltip("How fast the player moves backward when dying.")]
    public float deathMoveSpeed = 3f;
    
    [Tooltip("Time to destroy (seconds).")]
    public float destroyDelay = 2f;

    [Tooltip("Particle system attached to this player that plays when dying.")]
    public ParticleSystem deathParticle;

    [Tooltip("Extra object to activate (like explosion mesh or animation).")]
    public GameObject onDeathActivateObject;

    private bool isDead = false;
    

    private void Start()
    {
        playerHealth = GameManager.Instance.healthManager.playerMaxHealth;
        UpdateHealthUI();
        
        if (onDeathActivateObject != null)
            onDeathActivateObject.SetActive(false);
    }
    
    public void TakeDamage(int amount = 1)
    {
        playerHealth -= amount;
        playerHealth = Mathf.Max(0, playerHealth);
        UpdateHealthUI();

        if (playerHealth <= 0)
        {
            Debug.Log($"{playerType} is eliminated!");
            HandlePlayerEliminated();
        }
    }
    
    private void HandlePlayerEliminated()
    {
        Debug.Log($"{name} is eliminated!");
        
        // Activate linked object
        if (onDeathActivateObject != null)
            onDeathActivateObject.SetActive(true);

        if (gameObject.TryGetComponent(out PlayerController playerController))
        {
            playerController.enabled = false;
        }

        if (gameObject.TryGetComponent(out AIController aiController))
        {
            aiController.enabled = false;
        }
        
        // Start death sequence
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Move backward slightly
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (deathMoveDirection.normalized * deathMoveDistance);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * deathMoveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Play attached particle system if exists
        if (deathParticle != null)
            deathParticle.Play();

        // Small delay before destroying
        yield return new WaitForSeconds(destroyDelay);

        // Notify GameManager
        GameManager.Instance.OnPlayerEliminated(this);

        // 🧠 Destroy logic depends on win type
        if (GameManager.Instance.winCondition == GameManager.WinConditionType.LastAlive)
        {
            // In LastAlive mode, dead players should be destroyed
            Destroy(gameObject);
        }
        else if (GameManager.Instance.winCondition == GameManager.WinConditionType.AllDead)
        {
            // In AllDead mode, only destroy if this is NOT the last player
            if (GameManager.Instance.players.Count > 1)
                Destroy(gameObject);
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealthText)
            playerHealthText.text = playerHealth.ToString();
    }
}
