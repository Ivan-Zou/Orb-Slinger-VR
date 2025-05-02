using System;
using UnityEngine;

public class EndlessGameState : GameState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int lives = 5;

    public void SubtractLife() {
        --lives;
    }

    protected override void Start() {
        base.Start();
        resultTitle = "Result";
    }

    protected override void CheckWinLoss()
    {
        if (lives == 0) {
            Debug.Log("Lives over");
            gameOver = true;
        }
        base.CheckWinLoss();
    }

    public override int GetHiScore()
    {
        return PlayerPrefs.GetInt("EndlessHiScore", 0);
    }

    protected override void SetHiScore(int newScore)
    {
        PlayerPrefs.SetInt("EndlessHiScore", Math.Max(GetHiScore(), playerScore));
    }

    public override string GetStatusText() {
        return lives.ToString("00");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }
}
