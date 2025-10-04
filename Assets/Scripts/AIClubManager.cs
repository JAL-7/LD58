using System.Collections.Generic;
using UnityEngine;

public class AIClubManager : MonoBehaviour
{

    public static AIClubManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public List<GameObject> clubs = new List<GameObject>();
    public Transform rightPanel;

    public GameObject clubPrefab;

    public void Begin()
    {
        CreateClubs(GameSettings.Instance.numberOfAIClubs);
    }

    public void CreateClubs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            clubs.Add(GenerateClub());
        }
    }

    public GameObject GenerateClub()
    {
        var obj = Instantiate(clubPrefab, rightPanel);
        obj.GetComponent<Collective>().color = PickAcceptableColor();
        return obj;
    }

    public Color PickAcceptableColor()
    {
        return Color.green;
    }

}
