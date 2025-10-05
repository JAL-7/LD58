using Michsky.MUIP;
using UnityEngine;

public class GameSettings : MonoBehaviour
{

    public static GameSettings Instance { get; private set; }

    public SliderManager minSpeedSlider, maxSpeedSlider, numberOfPeopleSlider, minTraitsSlider, maxTraitsSlider, animationSpeedSlider, roundLengthSlider, numberOfRoundsSlider, numberOfAICollectivesSlider, numberOfPossibleTraitsSlider, cityDenistySlider;
    public SliderManager red, green, blue;

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

    void Start()
    {
        minSpeedSlider.mainSlider.value = minSpeed;
        maxSpeedSlider.mainSlider.value = maxSpeed;
        //numberOfPeopleSlider.mainSlider.value = numberOfPeople;
        minTraitsSlider.mainSlider.value = minTraits;
        maxTraitsSlider.mainSlider.value = maxTraits;
        roundLengthSlider.mainSlider.value = roundLength;
        numberOfRoundsSlider.mainSlider.value = numberOfRounds;
        //numberOfAICollectivesSlider.mainSlider.value = numberOfAICollectives;
        //numberOfPossibleTraitsSlider.mainSlider.value = numberOfPossibleTraits;
        //animationSpeedSlider.mainSlider.value = animationSpeed;
        red.mainSlider.value = playerColor.r;
        green.mainSlider.value = playerColor.g;
        blue.mainSlider.value = playerColor.b;
        
    }

    public int minTraits, maxTraits;
    public int numberOfPeople;
    public int numberOfAICollectives;
    public float minSpeed, maxSpeed;
    public int numberOfPossibleTraits;
    public int roundLength;
    public int numberOfRounds;
    public Color playerColor;
    public int animationSpeed;

    public void UpdateMaxSpeedSlider()
    {
        maxSpeedSlider.mainSlider.minValue = minSpeedSlider.mainSlider.value;
    }

    public void UpdateMaxTraitsSlider()
    {
        maxTraitsSlider.mainSlider.minValue = minTraitsSlider.mainSlider.value;
    }

    public void UpdateColor()
    {
        playerColor = new Color(red.mainSlider.value, green.mainSlider.value, blue.mainSlider.value);
    }

}
