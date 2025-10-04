using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Collective : MonoBehaviour
{

    public bool isPlayer;
    public Color color;

    public List<Trait> traits = new List<Trait>();

    public TMP_Text collectiveName;
    public TMP_Text currentMembersText;
    public TMP_Text pointsText;

    public int currentMembers;
    public int points;

    void Update()
    {
        if (isPlayer)
        {
            color = GameSettings.Instance.playerColor;
        }
        GetComponent<RawImage>().color = color;
        collectiveName.text = GetCollectiveLabel(true);

        if (currentMembersText != null)
        {
            currentMembersText.SetText($"{currentMembers} Current Members");
        }

        if (pointsText != null)
        {
            pointsText.SetText($"{points} Points");
        }
    }

    public string GetCollectiveLabel(bool upperCaseT)
    {
        if (traits == null || traits.Count == 0)
        {
            if (upperCaseT)
            {
                return "An empty collective";
            }
            return "an empty collective";
        }

        List<string> parts = new List<string>(traits.Count);
        for (int i = 0; i < traits.Count; i++)
        {
            Trait trait = traits[i];
            parts.Add(trait.adjective.Trim());
        }

        if (parts.Count == 0)
        {
            if (upperCaseT)
            {
                return "An empty collective";
            }
            return "an empty collective";
        }

        StringBuilder builder = new StringBuilder();
        if (upperCaseT)
        {
            builder.Append("The ");
        }
        else
        {
            builder.Append("the ");
        }

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

        builder.Append(" collective");
        return builder.ToString();
    }

}
