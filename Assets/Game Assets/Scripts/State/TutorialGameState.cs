using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;


public class TutorialStep
{
    public string Title;
    public string Description;
    public Func<bool> CompletionCheck;

    public TutorialStep(string title, string description, Func<bool> completionCheck)
    {
        Title = title;
        Description = description;
        CompletionCheck = completionCheck;
    }
}

public class TutorialGameState : GameState
{
    public GameObject tutorialMenu;
    private TutorialMenu activeTutorialMenu;

    private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    private int currentStepIndex = 0;

    private bool tutorialComplete = false;
    private float tutorialCompleteTimer = 0f;
    private const float completeFadeoutDelay = 15f;

    private bool tutorialMenuHiddenForPause = false;

    public AudioClip stepCompleteSound;
    private AudioSource audioSource;

    



    [Header("Orb Prefabs")]
    public GameObject standardOrb;
    public GameObject pulseOrb;
    public GameObject splitterOrb;
    public GameObject stickyOrb;
    public GameObject timedOrb;

    private GameObject currentOrbToThrow = null;
    private int thrownOrbCount = 0;
    private bool init_PickupThrowObserveOrbs = true;

    private bool init_HitTargets = true;

    private bool init_RedirectorGrabbedAndReleased = true;
    private bool redirectorGrabbedLastFrame = false;

    private bool init_RedirectorHitTargets = true;

    private bool init_OrbPassedThroughGravityPad = true;
    private bool orbTouchedGravityPad = false;

    private bool init_GravityPadsHitTargets = true;

    private bool init_PauseMenuOpened = true;

    private void SetupTutorial()
    {
        tutorialSteps.Add(new TutorialStep(
            "Pick up, throw, & observe Orbs",
            "1) Place hand on orb\n" +
            "2) Hold down grip button to hold orb\n" +
            "3) Release grip button during throwing motion\n" +
            "4) Observe Orb behavior",
            () => PickupThrowObserveOrbs()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Hit Targets to Score",
            "1) Orb must bounce before hitting a target\n" +
            "2) More bounces = higher score",
            () => HitTargets()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Redirector Walls",
            "1) Green Redirectors CANNOT be rotated\n" +
            "2) Magenta Redirectors CAN be rotated\n\n" +
            "Point controller at Magenta Redirector and hold grip button to rotate",
            () => RedirectorGrabbedAndReleased()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Redirector Walls",
            "1) Use them to hit targets!",
            () => RedirectorHitTargets()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Gravity Pads",
            "1) Exert gravitational force on orbs!",
            () => OrbPassedThroughGravityPad()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Gravity Pads",
            "1) Use them to hit targets!",
            () => GravityPadsHitTargets()
        ));

        tutorialSteps.Add(new TutorialStep(
            "Pause Menu",
            "Press the menu button on your left controller",
            () => PauseMenuOpened()
        ));

        if (tutorialMenu != null)
        {
            GameObject menuInstance = Instantiate(tutorialMenu);
            activeTutorialMenu = menuInstance.GetComponent<TutorialMenu>();

            UpdateTutorialMenu(false);
        }
    }

    private void UpdateTutorialMenu(bool withSoundAndAnimation)
    {
        if (activeTutorialMenu == null)
            return;

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
            activeTutorialMenu.SetDescription("Great Job! You are now ready to play Orb Slinger VR! Press the Menu button to Exit the Tutorial.");
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
            float deltaRotation = (rotationAmount / duration) * elapsed;
            activeTutorialMenu.transform.rotation = Quaternion.Euler(0f, deltaRotation, 0f);

            elapsed += Time.unscaledDeltaTime; // Important: unscaled so it still works if Time.timeScale == 0 (paused)
            yield return null;
        }

        activeTutorialMenu.transform.rotation = Quaternion.identity;
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
        gameOver = GameObject.FindGameObjectsWithTag("Scoreable").Length <= -1;
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

    private bool PickupThrowObserveOrbs()
    {
        if (init_PickupThrowObserveOrbs)
        {
            init_PickupThrowObserveOrbs = false;
            currentOrbToThrow = Instantiate(standardOrb, new Vector3(0f, 1f, 1f), Quaternion.identity);
        }

        if (currentOrbToThrow != null)
        {
            OrbLifetimeManager lifetimeManager = currentOrbToThrow.GetComponent<OrbLifetimeManager>();
            if (lifetimeManager != null && lifetimeManager.hasBeenThrown)
            {
                thrownOrbCount++;
                if (thrownOrbCount == 1)
                {
                    currentOrbToThrow = Instantiate(pulseOrb, new Vector3(0f, 1f, 1f), Quaternion.identity);
                }
                else if (thrownOrbCount == 2)
                {
                    currentOrbToThrow = Instantiate(splitterOrb, new Vector3(0f, 1f, 1f), Quaternion.identity);
                }
                else if (thrownOrbCount == 3)
                {
                    currentOrbToThrow = Instantiate(stickyOrb, new Vector3(0f, 1f, 1f), Quaternion.identity);
                }
                else if (thrownOrbCount == 4)
                {
                    currentOrbToThrow = Instantiate(timedOrb, new Vector3(0f, 1f, 1f), Quaternion.identity);
                }
                else if (thrownOrbCount == 5)
                {
                    currentOrbToThrow = null;
                    GameObject.Find("PlayPlatform").transform.Find("SpawnOrbsInPlace").gameObject.SetActive(true);

                    return true;
                }
            }
        }

        return false;
    }

    private bool HitTargets()
    {
        if (init_HitTargets)
        {
            init_HitTargets = false;
            GameObject hitTargets = GameObject.Find("HitTargets");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("HitTargets not found!");
            }
        }

        return GameObject.FindGameObjectsWithTag("Scoreable").Length <= 0;
    }

    private bool RedirectorGrabbedAndReleased()
    {
        if (init_RedirectorGrabbedAndReleased)
        {
            init_RedirectorGrabbedAndReleased = false;
            GameObject hitTargets = GameObject.Find("Redirectors_Part1");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("Redirectors_Part1 not found!");
            }
        }

        bool isCurrentlyGrabbing = false;

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
                            isCurrentlyGrabbing = true;
                            break;
                        }
                    }
                }
            }
        }

        bool didRelease = redirectorGrabbedLastFrame && !isCurrentlyGrabbing;
        redirectorGrabbedLastFrame = isCurrentlyGrabbing;

        return didRelease;
    }

    private bool RedirectorHitTargets()
    {
        if (init_RedirectorHitTargets)
        {
            init_RedirectorHitTargets = false;
            GameObject hitTargets = GameObject.Find("Redirectors_Part2");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("Redirectors_Part2 not found!");
            }
        }

        return GameObject.FindGameObjectsWithTag("Scoreable").Length <= 0;
    }

    // Called from GravityPad.cs
    public void OnOrbTouchedGravityPad()
    {
        orbTouchedGravityPad = true;
    }

    private bool OrbPassedThroughGravityPad()
    {
        if (init_OrbPassedThroughGravityPad)
        {
            init_OrbPassedThroughGravityPad = false;

            GameObject hitTargets = GameObject.Find("Redirectors_Part1");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Redirectors_Part1 not found!");
            }

            hitTargets = GameObject.Find("Redirectors_Part2");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Redirectors_Part2 not found!");
            }

            hitTargets = GameObject.Find("GravityPads_Part1");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("GravityPads_Part1 not found!");
            }
        }

        return orbTouchedGravityPad;
    }

    private bool GravityPadsHitTargets()
    {
        if (init_GravityPadsHitTargets)
        {
            init_GravityPadsHitTargets = false;

            GameObject hitTargets = GameObject.Find("GravityPads_Part2");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("GravityPads_Part2 not found!");
            }
        }

        return GameObject.FindGameObjectsWithTag("Scoreable").Length <= 0;
    }

    private bool PauseMenuOpened()
    {
        if (init_PauseMenuOpened)
        {
            init_PauseMenuOpened = false;

            GameObject hitTargets = GameObject.Find("GravityPads_Part1");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("GravityPads_Part1 not found!");
            }

            hitTargets = GameObject.Find("GravityPads_Part2");
            if (hitTargets != null)
            {
                foreach (Transform child in hitTargets.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("GravityPads_Part2 not found!");
            }
        }

        return activePauseMenuInstance != null;
    }
}
