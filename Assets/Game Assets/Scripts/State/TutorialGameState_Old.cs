using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;


public class TutorialStep_Old
{
    public string Title;
    public string Description;
    public Func<bool> CompletionCheck;

    public TutorialStep_Old(string title, string description, Func<bool> completionCheck)
    {
        Title = title;
        Description = description;
        CompletionCheck = completionCheck;
    }
}

public class TutorialGameState_Old : GameState
{
    public GameObject tutorialMenu;
    private TutorialMenu activeTutorialMenu;
    private Vector3 activeTutorialMenuInitEuler;

    private List<TutorialStep_Old> tutorialSteps = new List<TutorialStep_Old>();
    private int currentStepIndex = 0;

    private bool tutorialComplete = false;
    private float tutorialCompleteTimer = 0f;
    private const float completeFadeoutDelay = 15f;

    private bool tutorialMenuHiddenForPause = false;

    public AudioClip stepCompleteSound;
    private AudioSource audioSource;

    private float orbTypesLearnedTimer = 0f;
    private bool orbTypesStepStarted = false;
    private float orbTypesLearnedTimeLength = 30f;

    private int lastRecordedScore = 0;
    private bool targetHitInit = false;

    private bool orbTouchedGravityPad = false;

    private void SetupTutorial()
    {
        tutorialSteps.Add(new TutorialStep_Old(
            "Orb Types",
            "• Standard (Red) - predictable, loses momentum per bounce\n" +
            "• Pulse (Yellow) - builds momentum with each bounce\n" +
            "• Splitter (Cyan) - splits into three orbs after half a second\n" +
            "• Sticky (White) - temporarily attaches to first contacted surface\n" +
            "• Timed (Black) - explodes after its timer reaches 0\n\n" +
            "Each orb behaves differently, and mastering their abilities is key to high-level play!",
            () => LearnedAboutOrbTypes()
        ));

        tutorialSteps.Add(new TutorialStep_Old(
            "Pick up an orb",
            "Learn how to pick up orbs using your controller’s grip button, typically located along the side where your fingers naturally rest. " +
            "Move your hand close to an orb and squeeze the grip button to pick it up. " +
            "In Orb Slinger VR, picking up orbs is the core of your interaction - " +
            "you'll need to master this mechanic to prepare your shots, charge up your throws, and aim precisely at targets across the environment.",
            () => OrbPickedUp()
        ));
        tutorialSteps.Add(new TutorialStep_Old(
            "Throw an orb",
            "Practice throwing by releasing your grip at the right time while moving your hand in the desired direction. " +
            "In Orb Slinger VR, your throw’s speed and arc determine how far and how accurately the orb travels. " +
            "Learning to time your release and build momentum is key to making precise, powerful throws - " +
            "a critical skill for overcoming later challenges and maximizing your score.",
            () => OrbThrown()
        ));
        tutorialSteps.Add(new TutorialStep_Old(
            "Hit a target",
            "Strike a target by first throwing an orb and bouncing it off the environment. " +
            "In Orb Slinger VR, you can't simply throw directly - the orb must first bounce off a surface before it can score a hit on a target. " +
            "This bouncing mechanic adds a layer of strategy and skill, requiring you to plan your shots, aim creatively, " +
            "and master the game’s physics to chain together clever bank shots and tactical attacks.",
            () => TargetHit()
        ));
        tutorialSteps.Add(new TutorialStep_Old(
            "Redirectors",
            "In Orb Slinger VR, some redirectors can be rotated to change the orb's bounce direction.\n\n" +
            "• Green Redirectors are fixed\n" +
            "• Magenta Redirectors can be rotated\n\n" +
            "To rotate a magenta redirector, point your controller at it and hold the grip button - while gripping, the redirector will rotate based on your hand’s motion!",
            () => RedirectorGrabbed()
        ));
        tutorialSteps.Add(new TutorialStep_Old(
            "Gravity Pads",
            "Gravity Pads exert gravitational force on orbs!\n\n" +
            "• Arrows show the force direction and speed\n" +
            "• Use Gravity Pads to redirect or accelerate your throws\n\n" +
            "Watch how the arrows move - faster arrows mean stronger gravitational force.",
            () => OrbPassedThroughGravityPad()
        ));
        tutorialSteps.Add(new TutorialStep_Old(
            "Pause Menu",
            "Learn how to pause/resume the game at any time by pressing " +
            "the designated pause button on your controller (the menu button on your left controller for the Meta Quest). " +
            "Opening the pause menu lets you take a break, view your score, or Restart/Exit the Gameplay Level. " +
            "Knowing how to quickly access the pause menu is important for staying comfortable and in control during longer or more intense gameplay.",
            () => PauseMenuOpened()
        ));

        if (tutorialMenu != null)
        {
            GameObject menuInstance = Instantiate(tutorialMenu);
            activeTutorialMenu = menuInstance.GetComponent<TutorialMenu>();

            if (activeTutorialMenu != null)
            {
                activeTutorialMenuInitEuler = activeTutorialMenu.transform.eulerAngles;
            }

            UpdateTutorialMenu(false);

            activeTutorialMenu.SetDescriptionFontSize(5);
        }
    }

    private void UpdateTutorialMenu(bool withSoundAndAnimation)
    {
        if (activeTutorialMenu == null)
            return;

        activeTutorialMenu.SetDescriptionFontSize(6);

        if (withSoundAndAnimation)
        {
            PlayStepCompleteFeedback();
        }

        if (!tutorialComplete && currentStepIndex < tutorialSteps.Count)
        {
            activeTutorialMenu.SetTitle(tutorialSteps[currentStepIndex].Title);
            activeTutorialMenu.SetDescription(tutorialSteps[currentStepIndex].Description);
        }
        else
        {
            activeTutorialMenu.SetTitle("Tutorial Complete!");
            activeTutorialMenu.SetDescription("Great Job! You are now ready to play Orb Slinger VR! Either hit all of the Score Zones or press the Menu button to Exit the Tutorial.");
        }
    }

    private void PlayStepCompleteFeedback()
    {
        if (audioSource != null && stepCompleteSound != null && activeTutorialMenu != null)
        {
            AudioSource.PlayClipAtPoint(stepCompleteSound, activeTutorialMenu.transform.position);
            StartCoroutine(RotateMenuCoroutine());
        }
    }

    private IEnumerator RotateMenuCoroutine()
    {
        float elapsed = 0f;
        float duration = 1f;
        float rotationAmount = 360f;

        while (elapsed < duration)
        {
            float deltaRotation = (rotationAmount / duration) * Time.unscaledDeltaTime;
            //activeTutorialMenu.transform.Rotate(0f, deltaRotation, 0f, Space.World);
            activeTutorialMenu.transform.Rotate(Vector3.up, deltaRotation, Space.Self);

            elapsed += Time.unscaledDeltaTime; // Important: unscaled so it still works if Time.timeScale == 0 (paused)
            yield return null;
        }

        activeTutorialMenu.transform.eulerAngles = activeTutorialMenuInitEuler;
    }

    protected override void Start() {
        base.Start();
        resultTitle = "Result";

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        SetupTutorial();
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

        // Hide Tutorial Menu if Pause Menu is open
        if (activePauseMenuInstance != null && activeTutorialMenu != null && !tutorialMenuHiddenForPause)
        {
            activeTutorialMenu.gameObject.SetActive(false);
            tutorialMenuHiddenForPause = true;
        }
        // Show Tutorial Menu again if Pause Menu is closed
        else if (activePauseMenuInstance == null && activeTutorialMenu != null && tutorialMenuHiddenForPause)
        {
            activeTutorialMenu.gameObject.SetActive(true);
            tutorialMenuHiddenForPause = false;
        }

        CheckForStepCompletion();
    }

    private void CheckForStepCompletion()
    {
        if (tutorialComplete)
        {
            tutorialCompleteTimer += Time.deltaTime;
            if (tutorialCompleteTimer >= completeFadeoutDelay && activeTutorialMenu != null)
            {
                Destroy(activeTutorialMenu.gameObject);
                activeTutorialMenu = null;
            }
            return;
        }

        if (currentStepIndex < tutorialSteps.Count)
        {
            if (tutorialSteps[currentStepIndex].CompletionCheck())
            {
                currentStepIndex++;
                UpdateTutorialMenu(true);
            }
        }
        else
        {
            tutorialComplete = true;
            UpdateTutorialMenu(true);
        }
    }

    private bool LearnedAboutOrbTypes()
    {
        if (!orbTypesStepStarted)
        {
            orbTypesStepStarted = true;
            orbTypesLearnedTimer = 0f;
        }

        orbTypesLearnedTimer += Time.deltaTime;

        return orbTypesLearnedTimer >= orbTypesLearnedTimeLength;
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

    private bool RedirectorGrabbed()
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
                        if (interactable.transform.CompareTag("Redirector"))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    // Called from GravityPad.cs
    public void OnOrbTouchedGravityPad()
    {
        orbTouchedGravityPad = true;
    }

    private bool OrbPassedThroughGravityPad()
    {
        return orbTouchedGravityPad;
    }

    private bool PauseMenuOpened()
    {
        return activePauseMenuInstance != null;
    }
}
