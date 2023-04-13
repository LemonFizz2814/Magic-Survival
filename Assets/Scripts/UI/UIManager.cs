using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject gameScreen;
    public GameObject upgradeScreen;
    public GameObject customizeScreen;
    public GameObject purchaseScreen;
    public GameObject settingsButton;
    public GameObject playScreen;
    public GameObject prizeScreen;

    private void Start()
    {
        ShowPlayScreen(true);
        ShowGameOverScreen(false);
        ShowInGameUI(false);
        ShowUpgradeUI(false);
        ShowCustomizeUI(false);
        ShowPurchaseUI(false);
        ShowSettingUI(false);
    }

    public void ShowGameOverScreen(bool _active)
    {
        gameOverScreen.SetActive(_active);
    }
    public void ShowPlayScreen(bool _active)
    {
        playScreen.SetActive(_active);
    }
    public void ShowInGameUI(bool _active)
    {
        gameScreen.SetActive(_active);
    }
    public void ShowUpgradeUI(bool _active)
    {
        upgradeScreen.SetActive(_active);
    }
    public void ShowCustomizeUI(bool _active)
    {
        customizeScreen.SetActive(_active);
    }
    public void ShowPurchaseUI(bool _active)
    {
        purchaseScreen.SetActive(_active);
    }
    public void ShowPrizeUI(bool _active)
    {
        prizeScreen.SetActive(_active);
    }
    public void ShowSettingUI(bool _active)
    {
        settingsButton.SetActive(_active);
    }
}
