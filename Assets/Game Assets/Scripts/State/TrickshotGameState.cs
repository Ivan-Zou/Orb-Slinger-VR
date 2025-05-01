using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrickshotGameState : GameState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int attempts = 5;

    public int numOrbs = 0;
    public int levelNo;
    public int remainingTargets = 1;
    public int bonusPoints = 100;

    protected override void Start() {
        base.Start();
        resultTitle = "Defeat";
        attempts = GameObject.FindGameObjectsWithTag("Orb").Length;
        numOrbs = attempts;
        remainingTargets = GameObject.FindGameObjectsWithTag("Scoreable").Length;
    }

    protected override void CheckWinLoss()
    {
        gameOver = numOrbs == 0 || remainingTargets == 0;
        if (gameOver) {
            if (remainingTargets == 0) {
                resultTitle = "Victory";
                playerScore += bonusPoints * attempts;
            }
        }
        base.CheckWinLoss();
    }

    public override int GetHiScore()
    {
        return PlayerPrefs.GetInt("Level" + levelNo + "HiScore", 0);
    }

    protected override void SetHiScore(int newScore)
    {
        PlayerPrefs.SetInt("Level" + levelNo + "HiScore", Math.Max(GetHiScore(), playerScore));
    }

    public override string GetStatusText() {
        return attempts.ToString("00");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        numOrbs = orbs.Length;
        attempts = 0;
        foreach (GameObject orb in orbs) {
            SplitterOrb so = orb.GetComponent<SplitterOrb>();
            if (so == null || so.canSplit) {
                attempts += 1;
            }
        }
        // attempts = GameObject.FindGameObjectsWithTag("Orb").Length;
        remainingTargets = GameObject.FindGameObjectsWithTag("Scoreable").Length;
    }
}
