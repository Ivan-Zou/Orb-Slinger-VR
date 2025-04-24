using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public abstract class GameState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected int playerScore = 0;
    public GameObject resultMenu;
    protected bool lost = false;

    GameObject[] grabControllers;
    GameObject[] menuControllers;

    protected virtual void Start() {
        // Set TimeScale to 1.0f
        Time.timeScale = 1.0f;
        lost = false;
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
        if (lost) {
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
        Instantiate(resultMenu);
        this.enabled = false;
    }

    // Update is called once per frame
    protected virtual void Update() {
        CheckWinLoss();
    }
}
