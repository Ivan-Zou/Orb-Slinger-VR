using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedGameState : GameState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float timer = 120.0f;


    void Start() {
        
    }

    protected override void CheckWinLoss()
    {
        base.CheckWinLoss();
        if (timer <= 0.0f) {
            Debug.Log("Time is up");
            // Load Result Scene Here
        }
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
