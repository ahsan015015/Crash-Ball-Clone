using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{   
    
    private AudioSource audioSource;
    public PlayerType.TypeOfPlayer playerType;
     void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball)
        {
            Player hitPlayer = GameManager.Instance.GetPlayerByType(playerType);
            if (hitPlayer != null)
            {
                // Play hit sound

                hitPlayer.TakeDamage();
            }
             audioSource.PlayOneShot(GameManager.Instance.loseHealthSound);
            Destroy(other.gameObject);
        }
    }
}
