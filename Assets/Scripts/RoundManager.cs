using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{

    public static RoundManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public GameObject menuPanel;
    public GameObject gameOverWindow;
    public TMP_Text roundText;
    public TMP_Text timeText;

    public GameObject option;
    public Transform optionsParent;

    public int roundNumber;
    public float timeLeft;
    public bool activeRound;
    public bool endOfRound;

    public void BeginGame()
    {
        menuPanel.SetActive(false);
        PersonManager.Instance.Begin();
        AICollectiveManager.Instance.Begin();
        BeginRound();
    }

    public void BeginRound()
    {
        if (roundNumber >= GameSettings.Instance.numberOfRounds)
        {
            GameOver();
            return;
        }
        roundNumber++;
        timeLeft = GameSettings.Instance.roundLength;
        activeRound = true;
        endOfRound = false;
    }

    void Update()
    {
        if (activeRound)
        {
            timeLeft -= Time.deltaTime;
        }
        roundText.text = "Round " + roundNumber.ToString() + " of " + GameSettings.Instance.numberOfRounds.ToString();
        timeText.text = Mathf.Round(timeLeft).ToString();
        if (timeLeft < 0 && !endOfRound)
        {
            EndOfRound();
        }
    }

    void EndOfRound()
    {
        activeRound = false;
        endOfRound = true;
        ShowUserOptions();
    }

    void ShowUserOptions()
    {
        optionsParent.gameObject.SetActive(true);
        foreach (Transform item in optionsParent)
        {
            Destroy(item.gameObject);
        }
        foreach (Trait trait in TraitManager.Instance.Traits)
        {
            if (!AICollectiveManager.Instance.playerCollective.GetComponent<Collective>().traits.Contains(trait))
            {
                var t = Instantiate(option, optionsParent);
                t.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = trait.coreValue;
                t.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                t.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => Option(trait));
            }
        }
    }

    public void Option(Trait trait)
    {
        optionsParent.gameObject.SetActive(false);
        AICollectiveManager.Instance.playerCollective.GetComponent<Collective>().traits.Add(trait);
        SelectTraitAI();
        foreach (GameObject obj in PersonManager.Instance.people)
        {
            obj.GetComponent<Person>().ChooseCollective();
            if (obj.GetComponent<Person>().collective != null)
            {
                obj.GetComponent<SpriteRenderer>().color = obj.GetComponent<Person>().collective.color;

            }
            else
            {
                obj.GetComponent<SpriteRenderer>().color = Color.white;

            }
        }
        UpdateCollectiveMemberships();
        // HERE
        BeginRound();
    }

    void UpdateCollectiveMemberships()
    {
        AICollectiveManager manager = AICollectiveManager.Instance;
        if (manager == null)
        {
            return;
        }

        List<Collective> collectives = new List<Collective>();

        if (manager.playerCollective != null)
        {
            Collective player = manager.playerCollective.GetComponent<Collective>();
            if (player != null)
            {
                collectives.Add(player);
            }
        }

        List<GameObject> aiCollectives = manager.collectivesAI;
        if (aiCollectives != null)
        {
            for (int i = 0; i < aiCollectives.Count; i++)
            {
                GameObject obj = aiCollectives[i];
                if (obj == null)
                {
                    continue;
                }

                Collective aiCollective = obj.GetComponent<Collective>();
                if (aiCollective != null)
                {
                    collectives.Add(aiCollective);
                }
            }
        }

        List<GameObject> people = PersonManager.Instance != null ? PersonManager.Instance.people : null;

        for (int i = 0; i < collectives.Count; i++)
        {
            Collective collective = collectives[i];
            if (collective == null)
            {
                continue;
            }

            int memberCount = 0;
            if (people != null)
            {
                for (int j = 0; j < people.Count; j++)
                {
                    GameObject personObj = people[j];
                    if (personObj == null)
                    {
                        continue;
                    }

                    Person person = personObj.GetComponent<Person>();
                    if (person != null && person.collective == collective)
                    {
                        memberCount++;
                    }
                }
            }

            collective.currentMembers = memberCount;
            collective.points += memberCount;
        }
    }

    void SelectTraitAI()
    {
        IReadOnlyList<Trait> availableTraits = TraitManager.Instance != null ? TraitManager.Instance.Traits : null;
        List<GameObject> aiCollectives = AICollectiveManager.Instance.collectivesAI;
        for (int i = 0; i < aiCollectives.Count; i++)
        {
            GameObject obj = aiCollectives[i];
            if (obj == null)
            {
                continue;
            }

            Collective collective = obj.GetComponent<Collective>();
            if (collective == null)
            {
                continue;
            }

            Trait trait = PickRandomUnusedTrait(availableTraits, collective.traits);
            if (trait != null)
            {
                collective.traits.Add(trait);
            }
        }
    }

    static Trait PickRandomUnusedTrait(IReadOnlyList<Trait> availableTraits, List<Trait> currentTraits)
    {
        List<int> candidates = new List<int>(availableTraits.Count);
        for (int i = 0; i < availableTraits.Count; i++)
        {
            Trait candidate = availableTraits[i];
            if (candidate == null)
            {
                continue;
            }

            if (!HasTrait(currentTraits, candidate))
            {
                candidates.Add(i);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, candidates.Count);
        return availableTraits[candidates[randomIndex]];
    }

    static bool HasTrait(List<Trait> currentTraits, Trait trait)
    {
        if (currentTraits == null || currentTraits.Count == 0 || trait == null)
        {
            return false;
        }
        for (int i = 0; i < currentTraits.Count; i++)
        {
            Trait existing = currentTraits[i];
            if (existing == trait)
            {
                return true;
            }
            if (existing != null && existing.coreValue == trait.coreValue)
            {
                return true;
            }
        }
        return false;
    }

    void GameOver()
    {
        gameOverWindow.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }

}
