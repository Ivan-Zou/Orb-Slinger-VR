using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    GameState state;
    TMP_Text hiScoreText;
    TMP_Text scoreText;
    
    public float offsetRadius = 0.3f;
    public float distanceToHead = 4;
    public float yPos = 2.5f;
    Camera head;

    void Start()
    {
        state = GameObject.FindGameObjectWithTag("State").GetComponent<GameState>(); 
        head = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        foreach (Transform child in gameObject.transform) {
            if (child.CompareTag("Score")) {
                scoreText = child.GetComponent<TMP_Text>();
                scoreText.text = "Score: " + state.GetScoreText();
            } else if (child.CompareTag("HiScore")) {
                hiScoreText = child.GetComponent<TMP_Text>();
                hiScoreText.text = "Hi-Score: " + state.GetHiScore().ToString("0000");
            }
        }
        transform.eulerAngles = new Vector3(0.0f, head.transform.eulerAngles.y, 0.0f);
        Vector3 headCenter = head.transform.position + head.transform.forward * distanceToHead;
        Vector3 direction = transform.position - headCenter;
        Vector3 targetPos = transform.position = headCenter + direction.normalized * offsetRadius;
        transform.position = new Vector3(targetPos.x, yPos, targetPos.z);
    }

    public void Exit() {
        SceneManager.LoadSceneAsync(0);
    }
    

    public void Restart() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    
    void Update()
    {

    }
}
