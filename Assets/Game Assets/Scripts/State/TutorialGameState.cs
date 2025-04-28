using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;


public class TutorialStep
{
    public string Description;
    public Func<bool> CompletionCheck;

    public TutorialStep(string description, Func<bool> completionCheck)
    {
        Description = description;
        CompletionCheck = completionCheck;
    }
}

public class TutorialGameState : GameState
{
    private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    private int currentStepIndex = 0;

    private void SetupTutorialSteps()
    {
        tutorialSteps.Add(new TutorialStep(
            "Pick up an orb",
            () => OrbPickedUp()
        ));
        tutorialSteps.Add(new TutorialStep(
            "Throw an orb",
            () => OrbThrown()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Hit a target",
            () => TargetHit()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Open the pause menu",
            () => PauseMenuOpened()
        ));
    }

    protected override void Start() {
        base.Start();
        resultTitle = "Result";

        SetupTutorialSteps();
    }

    protected override void CheckWinLoss()
    {
        gameOver = GameObject.FindGameObjectsWithTag("Scoreable").Length <= 0;
        base.CheckWinLoss();
    }

    public override int GetHiScore()
    {
        return PlayerPrefs.GetInt("TutorialHiScore", 0);
    }

    protected override void SetHiScore(int newScore)
    {
        PlayerPrefs.SetInt("TutorialHiScore", Math.Max(GetHiScore(), playerScore));
    }

    public override string GetStatusText() {
        return "\u221E";
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        CheckForStepCompletion();
    }

    private void CheckForStepCompletion()
    {
        if (currentStepIndex < tutorialSteps.Count)
        {
            if (tutorialSteps[currentStepIndex].CompletionCheck())
            {
                currentStepIndex++;
            }
            else
            {
                Debug.Log($"Tutorial Step: {tutorialSteps[currentStepIndex].Description}");
            }
        }
        else
        {
            Debug.Log("Tutorial Complete!");
        }
    }

    private bool OrbPickedUp()
    {
        foreach (GameObject grabController in grabControllers)
        {
            var interactors = grabController.GetComponentsInChildren<XRBaseInteractor>(true);
            foreach (var interactor in interactors)
            {
                if (interactor != null && interactor.hasSelection)
                {
                    foreach (var interactable in interactor.interactablesSelected)
                    {
                        if (interactable.transform.CompareTag("Orb"))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool OrbThrown()
    {
        foreach (GameObject orb in GameObject.FindGameObjectsWithTag("Orb"))
        {
            OrbLifetimeManager lifetimeManager = orb.GetComponent<OrbLifetimeManager>();
            if (lifetimeManager != null && lifetimeManager.hasBeenThrown)
            {
                return true;
            }
        }
        return false;
    }

    private int lastRecordedScore = 0;
    private bool targetHitInit = false;

    private bool TargetHit()
    {
        if (!targetHitInit)
        {
            lastRecordedScore = playerScore;
            targetHitInit = true;
        }
        else if (playerScore > lastRecordedScore)
        {
            return true;
        }
        return false;
    }

    private bool PauseMenuOpened()
    {
        return activePauseMenuInstance != null;
    }
}
