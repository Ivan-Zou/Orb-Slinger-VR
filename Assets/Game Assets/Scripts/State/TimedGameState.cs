using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedGameState : GameState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float timer = 120.0f;

    protected override void Start() {
        base.Start();
    }

    protected override void CheckWinLoss()
    {
        if (timer <= 0.0f) {
            Debug.Log("Time is up");
            lost = true;
        }
        base.CheckWinLoss();
    }

    public override int GetHiScore()
    {
        return PlayerPrefs.GetInt("TimedHiScore", 0);
    }

    protected override void SetHiScore(int newScore)
    {
        PlayerPrefs.SetInt("TimedHiScore", Math.Max(GetHiScore(), playerScore));
    }

    public override string GetStatusText() {
        int minutes = (int) (timer / 60);
        int seconds = (int) (timer - (60 * minutes));
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        timer -= Time.deltaTime;
    }
}
