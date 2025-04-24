using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Update is called once per frame

    public GameObject buttonPanel;
    public GameObject levelPanel;

    void Start()
    {
        buttonPanel.SetActive(true);
        levelPanel.SetActive(false);
    }

    public void togglePanels() {
        buttonPanel.SetActive(!buttonPanel.activeSelf);
        levelPanel.SetActive(!levelPanel.activeSelf);
    }

    public void loadLevel(int buildIdx) {
        SceneManager.LoadSceneAsync(buildIdx);
    }
}
