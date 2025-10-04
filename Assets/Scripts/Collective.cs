using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class Collective : MonoBehaviour
{

    public bool isPlayer;
    public Color color;

    public List<Trait> traits = new List<Trait>();

    public bool run;

    void Update()
    {
        if (run)
        {
            Debug.Log(GetClubLabel());
        }
    }

    public string GetClubLabel()
    {
        if (traits == null || traits.Count == 0)
        {
            return "The club";
        }

        List<string> parts = new List<string>(traits.Count);
        for (int i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];
            parts.Add(trait.adjective.Trim());
        }

        if (parts.Count == 0)
        {
            return "The club";
        }

        StringBuilder builder = new StringBuilder("The ");

        for (int i = 0; i < parts.Count; i++)
        {
            if (i > 0)
            {
                // if (parts.Count == 2)
                // {
                //     builder.Append(" and ");
                // }
                // else if (i == parts.Count - 1)
                // {
                //     builder.Append(", and ");
                // }
                // else
                // {
                    builder.Append(", ");
                // }
            }

            builder.Append(parts[i]);
        }

        builder.Append(" club");
        return builder.ToString();
    }

}
