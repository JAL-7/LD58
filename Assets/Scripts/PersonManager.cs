using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour
{

    public List<Person> people;
    public List<string> firstNames;

    public Person GeneratePerson()
    {
        Person p = new Person();
        string newName = GenerateName();
        while (people.Exists(person => person.personName == newName))
        {
            newName = GenerateName();
        }
        p.personName = newName;
        p.age = Random.Range(13, 86);
        p.traits = new List<Trait>();
        return p;
    }

    public string GenerateName()
    {
        return firstNames[Random.Range(0, firstNames.Count)] + " " + GetRandomUppercaseLetter() + ".";
    }

    public void CreatePeople(int count)
    {
        for (int i = 0; i < count; i++)
        {
            people.Add(GeneratePerson());
        }
    }

    public string GetRandomUppercaseLetter()
    {
        int num = Random.Range(0, 26);
        char letter = (char)('A' + num);
        return letter.ToString();
    }

}
