using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public int roundNumber;
    public float timeLeft;
    public bool activeRound;

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
    }

    void Update()
    {
        if (activeRound)
        {
            timeLeft -= Time.deltaTime;
        }
        roundText.text = "Round " + roundNumber.ToString() + " of " + GameSettings.Instance.numberOfRounds.ToString();
        timeText.text = Mathf.Round(timeLeft).ToString();
        if (timeLeft <= 0)
        {
            activeRound = false;
        }
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
