using UnityEngine;

public class GameSettings : MonoBehaviour
{

    public static GameSettings Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);

    }

    public int minTraits, maxTraits;
    public int numberOfPeople;
    public int numberOfAICollectives;
    public float minSpeed, maxSpeed;
    public int numberOfPossibleTraits;
    public int roundLength;
    public int numberOfRounds;
    public Color playerColor;

}
