using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    public SettingsSave SettingsSave;
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

    //public Animator cameraAnimator;
    public CameraTracking camTrack;
    public Animator fadeAnimator;
    public Animator gameOverAnimator;
    public AdvertisingScript advertisingScript;

    public int coinsToPurchase;

    [Header("Audio")]
    public GameObject BGM;
    public bool hasSound = true;
    public bool hasMusic = true;
    public GameObject MusicButton;
    public GameObject SoundButton;
    private bool Reset;
    private float elapsedtime;

    private void Awake()
    {
        SettingsSave = GameObject.FindGameObjectWithTag("SettingsSave").GetComponent<SettingsSave>();

        if (!SettingsSave.HasMusic)
        {
            Color32 Red = new Color32(236, 61, 86, 255);
            MusicButton.GetComponent<Image>().color = Red;

            //BGM.SetActive(false);
            BGM.GetComponent<AudioSource>().volume = 0;
            
        }
        else
        {
            Color32 Cyan = new Color32(68, 229, 227, 255);
            MusicButton.GetComponent<Image>().color = Cyan;

            //BGM.SetActive(true);
            BGM.GetComponent<AudioSource>().volume = 1;
        }

        hasMusic = SettingsSave.HasMusic;


        if (!SettingsSave.HasSound)
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

        hasSound = SettingsSave.HasSound;
    }

    private void Update()
    {
        if (SettingsSave.HasMusic && Reset)
        {
            BGM.GetComponent<AudioSource>().volume = 0;
        }

        if (fadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && Reset)
        {
            Reset = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestartPressed()
    {
        uiManager.ShowCustomizeUI(false);
        uiManager.ShowPlayScreen(false);

        fadeAnimator.SetBool("Fade", true);
        
        Reset = true;

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        camTrack.camAnim.SetTrigger("PlayPosition");
        poolingManager.SpawnIntialObjects();
        uiManager.ShowPlayScreen(false);

        yield return new WaitForSeconds(1.5f);

        camTrack.EnableTrack = true;
        camTrack.camAnim.enabled = false;
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
        if (hasMusic)
        {
            Color32 Red = new Color32(236, 61, 86, 255);
            MusicButton.GetComponent<Image>().color = Red;

            //BGM.SetActive(false);
            BGM.GetComponent<AudioSource>().volume = 0;
            hasMusic = false;
        }
        else
        {
            Color32 Cyan = new Color32(68, 229, 227, 255);
            MusicButton.GetComponent<Image>().color = Cyan;

            //BGM.SetActive(true);
            BGM.GetComponent<AudioSource>().volume = 1;
            hasMusic = true;
        }

        SettingsSave.HasMusic = hasMusic;
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

        SettingsSave.HasSound = hasSound;
    }
}