using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMenu : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    //public float offsetRadius = 2f;
    //public float distanceToHead = 4;
    //public float yPos = 2.5f;
    //Camera head;

    void Start()
    {
        //head = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //transform.eulerAngles = new Vector3(0.0f, head.transform.eulerAngles.y, 0.0f);
        //Vector3 headCenter = head.transform.position + head.transform.forward * distanceToHead;
        //Vector3 direction = transform.position - headCenter;
        //Vector3 targetPos = headCenter + direction.normalized * offsetRadius;
        //transform.position = new Vector3(targetPos.x, yPos, targetPos.z);

        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0.0f, 4.0f, 9.49f);
    }

    public void SetTitle(string title)
    {
        titleText.SetText(title);
    }

    public void SetDescription(string description)
    {
        descriptionText.SetText(description);
    }

    public void SetDescriptionFontSize(float fontSize)
    {
        descriptionText.fontSize = fontSize;
    }
}
