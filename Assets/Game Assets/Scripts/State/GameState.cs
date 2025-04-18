using UnityEngine;

public abstract class GameState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int playerScore = 0;

    void Start() {
        
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

    }

    // Update is called once per frame
    protected virtual void Update() {
        CheckWinLoss();
    }
}
