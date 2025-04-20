using UnityEngine;

public class RedirectorManager : MonoBehaviour
{
    public GameObject redirectorPrefab;

    private Vector3[] positions = new Vector3[]
    {
        new Vector3(-1.5f, .75f, -1f),
        new Vector3(-1.5f, .75f,  0f),
        new Vector3(-1.5f, .75f,  1f),
        new Vector3(-1f, .75f,  1.5f),
        new Vector3( 0f, .75f,  1.5f),
        new Vector3( 1f, .75f,  1.5f),
    };

    private Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
    private Vector3 scale = new Vector3(0.5f, 0.5f, 0.1f);

    void Start()
    {
        foreach (Vector3 pos in positions)
        {
            GameObject redirector = Instantiate(redirectorPrefab, pos, rotation);
            redirector.transform.localScale = scale;
        }
    }
}
