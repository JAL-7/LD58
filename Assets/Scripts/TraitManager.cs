using UnityEngine;
using System.Collections.Generic;

public class TraitManager : MonoBehaviour
{
    public static TraitManager Instance { get; private set; }

    static readonly Trait[] DefaultTraitDefinitions = new[]
    {
        new Trait { coreValue = "Hates cyclists", adjective = "cyclist-hating" },
        new Trait { coreValue = "Cyclist", adjective = "lycra-clad" },
        new Trait { coreValue = "Has a dog", adjective = "dog-owning" },
        new Trait { coreValue = "Dog lover", adjective = "dog-loving" },
        new Trait { coreValue = "Dog hater", adjective = "dog-averse" },
        new Trait { coreValue = "Unemployed", adjective = "between gigs" },
        new Trait { coreValue = "Team Edward", adjective = "Team Edward" },
        new Trait { coreValue = "Team Jacob", adjective = "Team Jacob" },
        new Trait { coreValue = "Gum chewers", adjective = "gum-chewing" },
        new Trait { coreValue = "Cat person", adjective = "cat-loving" },
        new Trait { coreValue = "Bird watcher", adjective = "bird-watching" },
        new Trait { coreValue = "Night owl", adjective = "night owl" },
        new Trait { coreValue = "Early riser", adjective = "dawn-loving" },
        new Trait { coreValue = "Board game hoarder", adjective = "board game hoarding" },
        new Trait { coreValue = "Puzzle solver", adjective = "puzzle-solving" },
        new Trait { coreValue = "Retro gamer", adjective = "retro gaming" },
        new Trait { coreValue = "Speed runner", adjective = "speedrunning" },
        new Trait { coreValue = "True crime junkie", adjective = "true crime bingeing" },
        new Trait { coreValue = "Succulent collector", adjective = "succulent collecting" },
        new Trait { coreValue = "Minimalist", adjective = "minimalist" },
        new Trait { coreValue = "Maximalist", adjective = "maximalist" },
        new Trait { coreValue = "Conspiracy theorist", adjective = "conspiratorial" },
        new Trait { coreValue = "Storm chaser", adjective = "storm chasing" },
        new Trait { coreValue = "Wannabe influencer", adjective = "ring light carrying" },
        new Trait { coreValue = "Podcast host", adjective = "podcast hosting" },
        new Trait { coreValue = "Amateur magician", adjective = "sleight of hand practicing" },
        new Trait { coreValue = "Drone pilot", adjective = "drone flying" },
        new Trait { coreValue = "Beekeeper", adjective = "beekeeping" },
        new Trait { coreValue = "Hot sauce collector", adjective = "hot sauce hoarding" },
        new Trait { coreValue = "Ice bather", adjective = "ice bathing" },
        new Trait { coreValue = "Roller skater", adjective = "roller skating" },
        new Trait { coreValue = "Talks to houseplants", adjective = "plant whispering" },
        new Trait { coreValue = "Reality TV fan", adjective = "reality TV studying" },
        new Trait { coreValue = "Competitive air guitarist", adjective = "air guitar shredding" },
        new Trait { coreValue = "Unironic kazooist", adjective = "kazoo tooting" },
        new Trait { coreValue = "Volunteer traffic warden", adjective = "traffic warden volunteering" },
    };

    [SerializeField]
    List<Trait> traitDefinitions = new List<Trait>();

    public IReadOnlyList<Trait> Traits => traitDefinitions;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        traitDefinitions = BuildDefaultTraits();

    }

    static List<Trait> BuildDefaultTraits()
    {
        var defaults = new List<Trait>(DefaultTraitDefinitions.Length);
        for (int i = 0; i < DefaultTraitDefinitions.Length; i++)
        {
            Trait definition = DefaultTraitDefinitions[i];
            defaults.Add(new Trait
            {
                coreValue = definition.coreValue,
                adjective = definition.adjective
            });
        }

        return defaults;
    }
}

[System.Serializable]
public class Trait
{
    public string coreValue;
    public string adjective;
}
