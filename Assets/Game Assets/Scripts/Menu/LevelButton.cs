using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int level;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (level > PlayerPrefs.GetInt("LevelUnlocked", 1)) {
            Debug.LogFormat("Level {0} Not Unlocked", level);
            GetComponent<Button>().interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
