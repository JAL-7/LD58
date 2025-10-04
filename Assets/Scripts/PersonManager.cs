using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour
{

    public List<GameObject> people = new List<GameObject>();

    public List<string> firstNames;

    public GameObject personPrefab;

    public bool wanderTime;

    void Start()
    {
        CreatePeople(25);
    }

    void Update()
    {
        foreach (GameObject item in people)
        {
            item.GetComponent<Person>().isWandering = wanderTime;
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
        p.traits = new List<Trait>();
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
