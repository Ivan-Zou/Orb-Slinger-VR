using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialGameState : GameState
{
    protected override void Start() {
        base.Start();
        resultTitle = "Result";
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
    }
}
