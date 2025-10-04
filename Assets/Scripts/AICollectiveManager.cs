using System.Collections.Generic;
using UnityEngine;

public class AICollectiveManager : MonoBehaviour
{

    public static AICollectiveManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public List<GameObject> collectives = new List<GameObject>();
    public Transform rightPanel;

    public GameObject collectivePrefab;

    public void Begin()
    {
        CreateCollectives(GameSettings.Instance.numberOfAICollectives);
    }

    public void CreateCollectives(int count)
    {
        for (int i = 0; i < count; i++)
        {
            collectives.Add(GenerateCollective());
        }
    }

    public GameObject GenerateCollective()
    {
        var obj = Instantiate(collectivePrefab, rightPanel);
        obj.GetComponent<Collective>().color = PickAcceptableColor();
        return obj;
    }

    public Color PickAcceptableColor()
    {
        return Color.green;
    }

}
