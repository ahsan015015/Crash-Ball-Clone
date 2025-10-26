using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpawnAnimation
{
    public SpriteRenderer spriteRenderer;  // Assign the sprite renderer for this spawn
    public List<Sprite> frames;            // The frames for this spawn animation
}

public class BallSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject ballPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn Animations")]
    public List<SpawnAnimation> spawnAnimations; // One per spawn point
    [Tooltip("Time per frame (controls animation speed)")]
    public float frameTime = 0.1f;
    [Tooltip("How many times the animation loops")]
    public int loops = 2;
    [Tooltip("Delay (after animation start) before spawning the ball")]
    public float spawnDelayAfterAnimStart = 0.5f;

    [Header("Settings")]
    public float minSpawnDelay = 1.5f;
    public float maxSpawnDelay = 3.5f;
    public int maxActiveBalls = 10;

    private int activeBalls = 0;
    private bool spawningActive = true;
    private List<GameObject> spawnedBalls = new List<GameObject>();

    public void StartGame()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (spawningActive)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            
            if (!spawningActive) yield break; // stop if game over

            if (activeBalls >= maxActiveBalls)
                continue;

            int randomIndex = Random.Range(0, spawnPoints.Length);
            StartCoroutine(SpawnWithAnimation(randomIndex));
        }
    }

    IEnumerator SpawnWithAnimation(int index)
    {
        if (index >= spawnAnimations.Count) yield break;
        SpawnAnimation anim = spawnAnimations[index];

        // Start playing animation
        StartCoroutine(PlaySpawnAnimation(anim));

        // Wait for configured delay before spawning the ball
        yield return new WaitForSeconds(spawnDelayAfterAnimStart);

        // Spawn the ball mid-animation
        Transform spawnPoint = spawnPoints[index];
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedBalls.Add(ball);
        activeBalls++;

        BallController ballController = ball.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.OnBallDestroyed += () =>
            {
                activeBalls--;
                spawnedBalls.Remove(ball);
            };
        }
    }

    IEnumerator PlaySpawnAnimation(SpawnAnimation anim)
    {
        // Loop the animation frames
        for (int l = 0; l < loops; l++)
        {
            foreach (var frame in anim.frames)
            {
                anim.spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(frameTime);
            }
        }
    }
    
    /// <summary>
    /// Stop any new balls from spawning.
    /// </summary>
    public void StopSpawning()
    {
        spawningActive = false;
    }

    /// <summary>
    /// Destroys all currently spawned balls instantly.
    /// </summary>
    public void DestroyAllBalls()
    {
        Debug.Log("Destroying all spawned balls");
        foreach (var ball in spawnedBalls)
        {
            if (ball != null)
                Destroy(ball);
        }
        spawnedBalls.Clear();
        activeBalls = 0;
    }
}
