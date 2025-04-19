using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text scoreText;
    GameState state;

    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        state = GameObject.FindGameObjectWithTag("State").GetComponent<GameState>();  
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = state.GetScoreText();
    }
}
