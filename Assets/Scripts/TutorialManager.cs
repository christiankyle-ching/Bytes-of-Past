using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TutorialManager : MonoBehaviour
{
    bool userTapped
    {
        get
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonUp(0);
#else
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended) return true;
                return false;
            }
            else
            {
                return false;
            }
#endif
        }
    }

    [Header("Steps - Overlays")]
    public GameObject[]
    popups;
    int currentPopupIndex = 0;

    [Header("UI References")]
    public SceneLoader sceneLoader;
    public Transform playerArea;

    private void Start()
    {
        UpdateCanvas();
    }

    private void Update()
    {
        //Step 0 : Welcome;
        //Step 1 : ShowHand;
        //Step 2 : ShowTimeline;
        //Step 3 : PlaceCorrect;
        //Step 4 : PlaceWrong;
        //Step 5 : PlacePlayer;
        //Step 6 : PlayQuiz;
        //Step 7 : WinOrLose;

        switch (currentPopupIndex)
        {
            case 0:
                if (userTapped)
                {
                    SoundManager.Instance.PlayDefaultSFX();
                    NextStep();
                }
                break;
            case 1:
                if (userTapped)
                {
                    SoundManager.Instance.PlayDefaultSFX();
                    NextStep();
                }
                break;
            case 2:
                if (userTapped)
                {
                    SoundManager.Instance.PlayDefaultSFX();
                    NextStep();
                }
                break;
            // Step 3 to 5 are HandleDropInTimeline driven
            // Step 6 is Quiz driven
            // Step 7 is Button driven
            default:
                break;
        }
    }

    void UpdateCanvas()
    {
        foreach (GameObject popup in popups)
        {
            popup.SetActive(popup == popups[currentPopupIndex]);
        }
    }

    public bool IsRightDrop(CardData card, int dropPos)
    {
        // Check right drop depending on what step
        if (currentPopupIndex == 3)
        {
            return dropPos == 2 && card.ID == "CC40"; // Correct Drop
        }
        else if (currentPopupIndex == 4)
        {
            return dropPos == 1 && card.ID == "CC01"; // Wrong Drop
        }
        else
        {
            return true;
        }
    }

    public void NextStep()
    {
        if (currentPopupIndex < popups.Length - 1)
        {
            currentPopupIndex++;
        }

        UpdateCanvas();
    }

    public void EndTutorial()
    {
        SoundManager.Instance.PlayClickedSFX();
        sceneLoader.GoToTutorial();
    }

}
