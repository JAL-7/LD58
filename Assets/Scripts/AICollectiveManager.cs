using System.Collections.Generic;
using UnityEngine;

public class AICollectiveManager : MonoBehaviour
{

    public static AICollectiveManager Instance { get; private set; }

    private const int MaxColorAttempts = 256;
    private const float MinRgbDistance = 0.28f;
    private const float MinHueDifference = 0.05f;
    private const float MinValueDifference = 0.1f;
    private const float MinContrastWithWhite = 4.5f;
    private const float MinContrastWithGrey = 1.4f;
    private static readonly Color DarkGreyReference = new Color(0.15f, 0.15f, 0.15f);

    private readonly List<Color> usedAIColors = new List<Color>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public GameObject playerCollective;
    public List<GameObject> collectivesAI = new List<GameObject>();
    public Transform rightPanel;

    public GameObject collectivePrefab;

    public void Begin()
    {
        usedAIColors.Clear();
        CreateCollectives(GameSettings.Instance.numberOfAICollectives);
    }

    public void CreateCollectives(int count)
    {
        for (int i = 0; i < count; i++)
        {
            collectivesAI.Add(GenerateCollective());
        }
    }

    public GameObject GenerateCollective()
    {
        var obj = Instantiate(collectivePrefab, rightPanel);
        Color chosenColor = PickAcceptableColor();
        obj.GetComponent<Collective>().color = chosenColor;
        usedAIColors.Add(chosenColor);
        return obj;
    }

    public Color PickAcceptableColor()
    {
        Color bestCandidate = new Color(0.2f, 0.4f, 0.65f);
        float bestDistanceScore = -1f;

        for (int attempt = 0; attempt < MaxColorAttempts; attempt++)
        {
            Color candidate = GenerateRandomCandidateColor();

            if (!HasSufficientContrast(candidate, Color.white, MinContrastWithWhite))
            {
                continue;
            }

            if (!HasSufficientContrast(candidate, DarkGreyReference, MinContrastWithGrey))
            {
                continue;
            }

            if (IsDistinctFromAll(candidate, out float distanceScore))
            {
                return candidate;
            }

            if (distanceScore > bestDistanceScore)
            {
                bestDistanceScore = distanceScore;
                bestCandidate = candidate;
            }
        }

        Debug.LogWarning($"AICollectiveManager.PickAcceptableColor fell back after {MaxColorAttempts} attempts. Using closest candidate.");
        return bestCandidate;
    }

    private Color GenerateRandomCandidateColor()
    {
        float hue = Random.value;
        float saturation = Random.Range(0.7f, 1f);
        float value = Random.Range(0.35f, 0.62f);
        Color color = Color.HSVToRGB(hue, saturation, value, true);
        color.a = 1f;
        return EnsureSufficientDarkness(color);
    }

    private static Color EnsureSufficientDarkness(Color color)
    {
        const float targetContrast = 4.8f;
        float contrast = CalculateContrastRatio(color, Color.white);
        if (contrast >= targetContrast)
        {
            return color;
        }

        Color.RGBToHSV(color, out float h, out float s, out float v);
        float minValueForContrast = FindValueForContrast(h, s, targetContrast);
        v = Mathf.Min(v, minValueForContrast);
        Color adjusted = Color.HSVToRGB(h, s, v, true);
        adjusted.a = 1f;
        return adjusted;
    }

    private static float FindValueForContrast(float hue, float saturation, float desiredContrast)
    {
        float low = 0f;
        float high = 0.7f;
        for (int i = 0; i < 6; i++)
        {
            float mid = (low + high) * 0.5f;
            Color test = Color.HSVToRGB(hue, saturation, mid, true);
            float contrast = CalculateContrastRatio(test, Color.white);
            if (contrast >= desiredContrast)
            {
                high = mid;
            }
            else
            {
                low = mid;
            }
        }
        return high;
    }

    private bool IsDistinctFromAll(Color candidate, out float minDistance)
    {
        minDistance = float.MaxValue;
        bool hasReference = false;

        if (GameSettings.Instance != null)
        {
            if (!IsDistinctFromColor(candidate, GameSettings.Instance.playerColor, ref minDistance))
            {
                return false;
            }
            hasReference = true;
        }

        for (int i = 0; i < usedAIColors.Count; i++)
        {
            if (!IsDistinctFromColor(candidate, usedAIColors[i], ref minDistance))
            {
                return false;
            }
            hasReference = true;
        }

        if (!hasReference)
        {
            minDistance = 1f;
        }

        return true;
    }

    private bool IsDistinctFromColor(Color candidate, Color reference, ref float minDistance)
    {
        float distance = CalculateColorDistance(candidate, reference);
        if (distance < minDistance)
        {
            minDistance = distance;
        }

        Color.RGBToHSV(candidate, out float candidateHue, out float candidateSat, out float candidateVal);
        Color.RGBToHSV(reference, out float referenceHue, out float referenceSat, out float referenceVal);

        float hueDifference = GetHueDifference(candidateHue, referenceHue);
        float valueDifference = Mathf.Abs(candidateVal - referenceVal);

        if (distance < MinRgbDistance || (hueDifference < MinHueDifference && valueDifference < MinValueDifference))
        {
            return false;
        }

        if (candidateSat < 0.3f && referenceSat < 0.3f && hueDifference < 0.15f)
        {
            return false;
        }

        return true;
    }

    private static float GetHueDifference(float hueA, float hueB)
    {
        float diff = Mathf.Abs(hueA - hueB);
        return diff > 0.5f ? 1f - diff : diff;
    }

    private static float CalculateColorDistance(Color a, Color b)
    {
        float dr = a.r - b.r;
        float dg = a.g - b.g;
        float db = a.b - b.b;
        return Mathf.Sqrt(dr * dr + dg * dg + db * db);
    }

    private static bool HasSufficientContrast(Color foreground, Color background, float minimumRatio)
    {
        float contrast = CalculateContrastRatio(foreground, background);
        return contrast >= minimumRatio;
    }

    private static float CalculateContrastRatio(Color a, Color b)
    {
        float luminanceA = CalculateRelativeLuminance(a);
        float luminanceB = CalculateRelativeLuminance(b);
        float brighter = Mathf.Max(luminanceA, luminanceB);
        float darker = Mathf.Min(luminanceA, luminanceB);
        return (brighter + 0.05f) / (darker + 0.05f);
    }

    private static float CalculateRelativeLuminance(Color color)
    {
        float r = LinearizeChannel(color.r);
        float g = LinearizeChannel(color.g);
        float b = LinearizeChannel(color.b);
        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    private static float LinearizeChannel(float channel)
    {
        if (channel <= 0.04045f)
        {
            return channel / 12.92f;
        }
        return Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
    }

}
