using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrickshotGameState : GameState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int attempts = 5;
    public int levelNo;
    public int remainingTargets = 1;

    protected override void Start() {
        base.Start();
        resultTitle = "Defeat";
        attempts = GameObject.FindGameObjectsWithTag("Orb").Length;
        remainingTargets = GameObject.FindGameObjectsWithTag("Scoreable").Length;
    }

    protected override void CheckWinLoss()
    {
        gameOver = attempts == 0 || remainingTargets == 0;
        if (gameOver) {
            if (remainingTargets == 0) {
                resultTitle = "Victory";
                PlayerPrefs.SetInt("LevelUnlocked", Math.Max(PlayerPrefs.GetInt("LevelUnlocked", 1), levelNo + 1));
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
        attempts = GameObject.FindGameObjectsWithTag("Orb").Length;
        remainingTargets = GameObject.FindGameObjectsWithTag("Scoreable").Length;
    }
}
