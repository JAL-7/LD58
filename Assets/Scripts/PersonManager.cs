using UnityEngine;
using System.Collections.Generic;

public class PersonManager : MonoBehaviour
{

    public static PersonManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public List<GameObject> people = new List<GameObject>();

    public List<string> firstNames;

    public GameObject personPrefab;

    public bool wanderTime;

    public void Begin()
    {
        CreatePeople(GameSettings.Instance.numberOfPeople);
    }

    void Update()
    {
        foreach (GameObject item in people)
        {
            item.GetComponent<Person>().isWandering = RoundManager.Instance.activeRound;
        }
    }

    public void CreatePeople(int count)
    {
        for (int i = 0; i < count; i++)
        {
            people.Add(GeneratePerson());
        }
    }

    public GameObject GeneratePerson()
    {
        var obj = Instantiate(personPrefab);
        Person p = obj.GetComponent<Person>();
        string newName = GenerateName();
        while (NameIsTaken(newName))
        {
            newName = GenerateName();
        }
        p.personName = newName;
        p.age = Random.Range(13, 86);
        p.speed = Random.Range(GameSettings.Instance.minSpeed, GameSettings.Instance.maxSpeed);
        p.traits = GenerateTraits();
        if (!p.TryPlaceAtRandomPosition())
        {
            Debug.LogWarning($"Failed to place {newName} without overlaps.");
        }
        return obj;
    }

    public string GenerateName()
    {
        return firstNames[Random.Range(0, firstNames.Count)] + " " + GetRandomUppercaseLetter() + ".";
    }

    List<Trait> GenerateTraits()
    {
        IReadOnlyList<Trait> availableTraits = TraitManager.Instance.Traits;
        int desiredCount = Random.Range(GameSettings.Instance.minTraits, GameSettings.Instance.maxTraits + 1);
        List<Trait> traits = new List<Trait>(desiredCount);
        HashSet<int> usedIndices = new HashSet<int>();

        while (traits.Count < desiredCount && usedIndices.Count < availableTraits.Count)
        {
            int candidateIndex = Random.Range(0, availableTraits.Count);
            if (usedIndices.Add(candidateIndex))
            {
                traits.Add(availableTraits[candidateIndex]);
            }
        }

        return traits;
    }

    bool NameIsTaken(string n)
    {
        foreach (GameObject obj in people)
        {
            if (n == obj.GetComponent<Person>().personName)
            {
                return true;
            }
        }
        return false;
    }

    public string GetRandomUppercaseLetter()
    {
        int num = Random.Range(0, 26);
        char letter = (char)('A' + num);
        return letter.ToString();
    }

}
