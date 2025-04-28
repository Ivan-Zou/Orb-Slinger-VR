using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;

public abstract class GameState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected int playerScore = 0;
    public GameObject resultMenu;
    protected bool gameOver = false;
    protected string resultTitle = "";

    GameObject[] grabControllers;
    GameObject[] menuControllers;

    public InputActionReference pauseAction;
    private bool isPaused = false;
    private GameObject activePauseMenuInstance = null;

    protected virtual void Start() {
        // Set TimeScale to 1.0f
        Time.timeScale = 1.0f;
        gameOver = false;
        if (grabControllers == null) {
            grabControllers = GameObject.FindGameObjectsWithTag("GameController");
        }
        if (menuControllers == null) {
            menuControllers = GameObject.FindGameObjectsWithTag("MenuController");
        }
        // Enable Grab Controllers
        foreach (GameObject grabController in grabControllers) {
            grabController.SetActive(true);
        }
        // Disable Menu Controllers
        foreach (GameObject menuController in menuControllers) {
            menuController.SetActive(false);
        }

        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePressed;
        }
    }

    public void UpdateScore(int delta) {
        playerScore += delta;
        Debug.LogFormat("New Score: {0}", playerScore);
    }

    public string GetScoreText() {
        return playerScore.ToString("0000");
    }

    public abstract string GetStatusText();

    protected virtual void CheckWinLoss() {
        if (gameOver) {
            ShowResult();
        }
    }

    public abstract int GetHiScore();

    protected abstract void SetHiScore(int newScore);

    protected virtual void ShowResult() {
        SetHiScore(playerScore);
        Time.timeScale = 0.0f;
        // Disable Grab Interactors
       foreach (GameObject grabController in grabControllers) {
            grabController.SetActive(false);
        }
        // Enable Menu Controllers
        foreach (GameObject menuController in menuControllers) {
            menuController.SetActive(true);
        }
        // Destroy All Orbs
        foreach (GameObject orb in GameObject.FindGameObjectsWithTag("Orb")) {
            Destroy(orb);
        }
        // Destroy All Targets
        foreach (GameObject orb in GameObject.FindGameObjectsWithTag("Scoreable")) {
            Destroy(orb);
        }
        // Destroy Redirectors
        foreach (GameObject redirector in GameObject.FindGameObjectsWithTag("Redirector")) {
            Destroy(redirector);
        }
        // Destroy Gravity Pads
        foreach (GameObject gravityPad in GameObject.FindGameObjectsWithTag("GravityPad"))
        {
            Destroy(gravityPad);
        }
        GameObject menu = Instantiate(resultMenu);
        Debug.Log(resultTitle);
        menu.GetComponent<ResultMenu>()?.SetTitle(resultTitle);
        this.enabled = false;
    }

    // Update is called once per frame
    protected virtual void Update() {
        CheckWinLoss();
    }

    private void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePressed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (gameOver) return;

        TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;

            // Disable Grab Controllers
            foreach (GameObject grabController in grabControllers)
            {
                grabController.SetActive(false);
            }
            // Enable Menu Controllers
            foreach (GameObject menuController in menuControllers)
            {
                menuController.SetActive(true);
            }

            // Spawn the Pause Menu only if not already open
            if (activePauseMenuInstance == null && resultMenu != null)
            {
                SetHiScore(playerScore);
                activePauseMenuInstance = Instantiate(resultMenu);
                activePauseMenuInstance.GetComponent<ResultMenu>()?.SetTitle("Paused");
            }
        }
        else
        {
            Time.timeScale = 1f;
            
            // Disable Menu Controllers
            foreach (GameObject menuController in menuControllers)
            {
                menuController.SetActive(false);
            }
            // Enable Grab Controllers
            foreach (GameObject grabController in grabControllers)
            {
                grabController.SetActive(true);
            }

            // Destroy the Pause Menu if it exists
            if (activePauseMenuInstance != null)
            {
                Destroy(activePauseMenuInstance);
                activePauseMenuInstance = null;
            }
        }
    }
}
