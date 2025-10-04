using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ViewPerson : MonoBehaviour
{
    [SerializeField] PersonManager personManager;
    [SerializeField] Camera targetCamera;

    public Person selectedPerson;

    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text traitsHeader;
    public TMP_Text traitsText;

    void Update()
    {
        if (selectedPerson == null)
        {
            nameText.text = "";
            ageText.text = "";
            traitsHeader.text = "";
            traitsText.text = "";
        }
        if (RoundManager.Instance.activeRound)
        {
            Mouse mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 screenPosition = mouse.position.ReadValue();
                SelectClosest(screenPosition);
                // foreach (GameObject personObject in personManager.people)
                // {
                //     personObject.GetComponent<SpriteRenderer>().color = Color.white;
                // }
                // selectedPerson.GetComponent<SpriteRenderer>().color = Color.green;
                nameText.text = selectedPerson.personName;
                ageText.text = selectedPerson.age.ToString() + " Years Old";
                traitsHeader.text = "Traits";
                traitsText.text = "";
                foreach (Trait item in selectedPerson.traits)
                {
                    traitsText.text += item.coreValue + "\n";
                }
            }
        }
    }

    void SelectClosest(Vector2 screenPosition)
    {
        selectedPerson = FindClosestPerson(screenPosition);
    }

    Person FindClosestPerson(Vector2 screenPosition)
    {
        Person closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (GameObject personObject in personManager.people)
        {
            Person person = personObject.GetComponent<Person>();
            if (person == null)
            {
                continue;
            }

            Vector3 screenPoint = targetCamera.WorldToScreenPoint(person.transform.position);
            Vector2 personScreen = new Vector2(screenPoint.x, screenPoint.y);

            float distance = (personScreen - screenPosition).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = person;
            }
        }

        return closest;
    }
}