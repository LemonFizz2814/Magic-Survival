using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI highScoreText;

    public PlayerScript player;
    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public ObstacleSpawner obstacleSpawner;
    public CoinSpawner coinSpawner;

    public GameObject prizePrompt;
    public GameObject purchasePrompt;
    public GameObject watchAdPrompt;
    public GameObject background;

    public PurchaseScript purchaseScript;
    public PrizeScript prizeScript;
    public PoolingManager poolingManager;
    public TextMeshProUGUI[] coinTexts;

    public Animator cameraAnimator;
    public Animator fadeAnimator;
    public Animator gameOverAnimator;
    public AdvertisingScript advertisingScript;

    public int coinsToPurchase;

    [Header("Audio")]
    public GameObject BGM;
    public bool hasSound = true;
    public GameObject MusicButton;
    public GameObject SoundButton;

    public void RestartPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateScoreText(int _score)
    {
        scoreText.text = "Score: " + _score;
    }
    public void UpdateHighScoreText(int _highScore)
    {
        highScoreText.text = "HighScore: " + _highScore;
    }
    public void StartPressed()
    {
        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {
        cameraAnimator.SetTrigger("PlayPosition");
        poolingManager.SpawnIntialObjects();
        uiManager.ShowPlayScreen(false);

        yield return new WaitForSeconds(1.5f);

        background.SetActive(false);
        uiManager.ShowInGameUI(true);
        player.StartPressed();
        enemySpawner.StartPressed();
        coinSpawner.StartPressed();
        obstacleSpawner.StartPressed();
    }
    public void CustomizePressed()
    {
        uiManager.ShowCustomizeUI(true);
        uiManager.ShowPlayScreen(false);
    }
    public void SettingsPressed()
    {
        uiManager.ShowSettingUI(true);
        uiManager.ShowPlayScreen(false);
    }
    public void BackPressed()
    {
        uiManager.ShowPlayScreen(true);
        uiManager.ShowCustomizeUI(false);
        uiManager.ShowPurchaseUI(false);
        uiManager.ShowSettingUI(false);
    }
    public void AcceptPressed()
    {
        uiManager.ShowPurchaseUI(false);
        uiManager.ShowPrizeUI(false);
    }
    public void PurchasePressed()
    {
        uiManager.ShowPurchaseUI(true);
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - coinsToPurchase);
        purchasePrompt.SetActive(PlayerPrefs.GetInt("Coins") >= coinsToPurchase);
        purchaseScript.UnlockPrize();
        UpdateCoinTexts();
    }
    public void PrizePressed()
    {
        uiManager.ShowPrizeUI(true);
        prizePrompt.SetActive(false);
        prizeScript.GivePrize();
        PlayerPrefs.SetInt("Day", System.DateTime.Today.Day);
        UpdateCoinTexts();
    }

    public void GameOver(int _coins)
    {
        purchasePrompt.SetActive(_coins >= coinsToPurchase);
        watchAdPrompt.SetActive(Random.Range(0, 100) < advertisingScript.chanceToShowWatchAd);
        prizePrompt.SetActive(PlayerPrefs.GetInt("Day", 0) != System.DateTime.Today.Day);
        UpdateCoinTexts();
        //fadeAnimator.SetTrigger("Fade");
        gameOverAnimator.SetTrigger("Play");

        if (Random.Range(0, 100) < advertisingScript.chanceForAd)
        {
            advertisingScript.ShowAd();
        }
    }

    void UpdateCoinTexts()
    {
        for(int i = 0; i < coinTexts.Length; i++)
        {
            coinTexts[i].text = "Volts: " + PlayerPrefs.GetInt("Coins");
        }
    }

    public void ToggleMusic()
    {
        if (BGM.activeSelf)
        {
            Color32 Red = new Color32(236, 61, 86, 255);
            MusicButton.GetComponent<Image>().color = Red;

            BGM.SetActive(false);
        }
        else
        {
            Color32 Cyan = new Color32(68, 229, 227, 255);
            MusicButton.GetComponent<Image>().color = Cyan;

            BGM.SetActive(true);
        }
    }
    public void ToggleSound()
    {
        if (hasSound)
        {
            Color32 Red = new Color32(236, 61, 86, 255);
            SoundButton.GetComponent<Image>().color = Red;

            hasSound = false;
        }
        else
        {
            Color32 Cyan = new Color32(68, 229, 227, 255);
            SoundButton.GetComponent<Image>().color = Cyan;

            hasSound = true;
        }
    }
}