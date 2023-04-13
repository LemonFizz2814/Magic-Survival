using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    public bool toggleFPS;
    public Slider xpBar;
    public Slider healthBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fpsCounter;
    int avgFrameRate;

    private void Update()
    {
        if (toggleFPS)
        {
            float current = 0;
            current = Time.frameCount / Time.time;
            avgFrameRate = (int)current;
            fpsCounter.text = "FPS: " + avgFrameRate.ToString();
        }
        else
        {
            fpsCounter.text = "";
        }
    }

    public void UpdateLevelText(int _level)
    {
        levelText.text = "" + _level;
    }
    public void UpdateScoreText(int _score)
    {
        scoreText.text = "" + _score;
    }
    public void UpdateXPBar(int _xp, int _maxXp)
    {
        xpBar.maxValue = _maxXp;
        xpBar.value = _xp;
    }
    public void UpdateHealthBar(float _health, float _maxHealth)
    {
        healthBar.maxValue = _maxHealth;
        healthBar.value = _health;
        healthBar.transform.localScale = new Vector3((float)_maxHealth/800, healthBar.transform.localScale.y, 1);
    }

    public void UpdateCoinText(int _coins)
    {
        coinText.text = "$" + _coins;
    }
}