using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Header("References")]
    public HealthManager healthManager;
    public BallSpawner ballSpawner;
    public List<Player> players = new List<Player>();
    
    [Header("Win Settings")]
    public WinConditionType winCondition = WinConditionType.LastAlive;
    public enum WinConditionType { LastAlive, AllDead }
    
    [Tooltip("The Transform where the winner will move to.")]
    public Transform winnerSpot;
    
    [Tooltip("Winning panel UI to activate after winner determined.")]
    public GameObject winPanel;

    [Tooltip("Text component to show winner name.")]
    public TMP_Text winnerText;

    public bool gameEnded = false;
   

    public Player GetPlayerByType(PlayerType.TypeOfPlayer type)
    {
        return players.Find(p => p.playerType == type);
    }
    
    public void OnPlayerEliminated(Player player)
    {
        if (gameEnded) return;

        Debug.Log($"GameManager detected elimination of {player.playerType}");

        // Remove the player from the list of alive players
        players.Remove(player);

        switch (winCondition)
        {
            case WinConditionType.LastAlive:
                CheckForWinner_LastAlive();
                break;

            case WinConditionType.AllDead:
                CheckForWinner_AllDead(player);
                break;
        }
    }

    private void CheckForWinner_LastAlive()
    {
        // When only 1 player remains alive
        if (players.Count == 1 && !gameEnded)
        {
            Player winner = players[0];
            EndGame(winner);
            HandleWinner(winner);
        }
    }

    private void CheckForWinner_AllDead(Player lastEliminated)
    {
        // In this mode, winner is the last player *before* all died
        // So we only trigger win if this is the final elimination
        if (players.Count == 0 && !gameEnded)
        {
            // Last eliminated player is winner
            EndGame(lastEliminated, skipDestroy: true);
            
            // We can track the "last man standing" using player elimination order.
            // But since all are dead now, let's keep reference of the last eliminated one
            Player winner = lastEliminated;
            HandleWinner(winner, skipDestroy: true);
        }
    }
    
    private void EndGame(Player winner, bool skipDestroy = false)
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log($"🏆 Game Over! Winner: {winner.name}");
       
        // Play win sound
        if (SoundManager.Instance.winSound != null)
            AudioSource.PlayClipAtPoint(SoundManager.Instance.winSound, Camera.main.transform.position);

        // Stop all spawning and destroy existing balls
        if (ballSpawner != null)
        {
            ballSpawner.StopSpawning();
            ballSpawner.DestroyAllBalls();
            
            StartCoroutine(ConfirmBallCleanup());
        }

        // Disable winner controls
        if (winner.TryGetComponent(out PlayerController pc))
            pc.enabled = false;
        if (winner.TryGetComponent(out AIController ai))
            ai.enabled = false;

        // Move winner to center spot
        if (winnerSpot != null)
        {
            winner.StartCoroutine(MoveWinnerToSpot(winner, winnerSpot.position));
        }

        // // Show winner panel
        // if (winPanel != null)
        //     winPanel.SetActive(true);
        //
        // if (winnerText != null)
        //     winnerText.text = $"{winner.name} Wins!";
    }

    private void HandleWinner(Player winner, bool skipDestroy = false)
    {
        gameEnded = true;
        Debug.Log($"🏆 Winner is: {winner.name}");

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winnerText != null)
            winnerText.text = $"{winner.name} Wins!";

        if (winnerSpot != null)
        {
            // Move winner to the middle spot (only X and Z)
            Vector3 targetPos = new Vector3(winnerSpot.position.x, winner.transform.position.y, winnerSpot.position.z);
            winner.StartCoroutine(MoveWinnerToSpot(winner, targetPos));
        }

        if (!skipDestroy)
        {
            // Stop all other players’ logic
            foreach (Player p in FindObjectsOfType<Player>())
            {
                if (p != winner)
                    p.enabled = false;
            }
        }
    }

    private IEnumerator<WaitForEndOfFrame> MoveWinnerToSpot(Player winner, Vector3 targetPos)
    {
        float speed = 2f;
        while (Vector3.Distance(winner.transform.position, targetPos) > 0.05f)
        {
            winner.transform.position = Vector3.Lerp(winner.transform.position, targetPos, Time.deltaTime * speed);
            // Reset rotation every frame
            winner.transform.rotation = Quaternion.Euler(Vector3.zero);
            yield return null;
            
            //yield return new WaitForEndOfFrame();
        }
        // Ensure final position and rotation are exact
        winner.transform.position = targetPos;
        winner.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    
    private IEnumerator ConfirmBallCleanup()
    {
        // Wait one short frame in case any coroutines were spawning
        yield return new WaitForSeconds(0.5f);

        // Check for any stragglers and destroy them
        BallController[] remainingBalls = FindObjectsOfType<BallController>();
        if (remainingBalls.Length > 0)
        {
            Debug.LogWarning($"⚠️ Found {remainingBalls.Length} leftover balls. Cleaning up...");
            foreach (var ball in remainingBalls)
            {
                if (ball != null)
                    Destroy(ball.gameObject);
            }
        }

        // One final cleanup sweep on the spawner list
        if (ballSpawner != null)
            ballSpawner.DestroyAllBalls();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
