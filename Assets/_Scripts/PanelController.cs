using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    [Tooltip("Panel Name with ID Numbers.")]
    [SerializeField] private GameObject MainMenu0, GamePlay1, Setting3;

    public void ChangePanel(int PanelId)
    {
        switch (PanelId)
        {
            case 0:
                MainMenu0.SetActive(true);
                GamePlay1.SetActive(false);
                Setting3.SetActive(false);
                break;
            case 1:
                MainMenu0.SetActive(false);
                GamePlay1.SetActive(true);
                Setting3.SetActive(false);
                break;
            case 2:
                MainMenu0.SetActive(false);
                GamePlay1.SetActive(false);
                Setting3.SetActive(true);
                break;
            default:
                Debug.LogWarning("Invalid PanelId: " + PanelId);
                break;
        }
    }
}
