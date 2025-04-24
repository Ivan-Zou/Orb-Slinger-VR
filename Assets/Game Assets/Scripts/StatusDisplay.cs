using TMPro;
using UnityEngine;

public class StatusDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TMP_Text statusText;
    public bool addLabel = false;
    public string label;
    GameState state;

    void Start()
    {
        statusText = GetComponent<TMP_Text>();
        state = GameObject.FindGameObjectWithTag("State").GetComponent<GameState>(); 
    }

    // Update is called once per frame
    void Update()
    {
        statusText.text = (addLabel ? label : "") + state.GetStatusText();
    }
}
