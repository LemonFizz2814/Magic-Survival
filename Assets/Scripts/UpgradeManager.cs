using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public PlayerScript player;
    public UpgradeStats upgradeStats;

    public GameObject[] optionButtons;
    //public TextMeshProUGUI[] upgradeNameText;
    public Image[] upgradeIconImage;
    public Sprite[] upgradeIcons;
    public TextMeshProUGUI[] upgradeDescriptionText;
    public TextMeshProUGUI[] tierText;
    //public TextMeshProUGUI skipText;
    public Animator descriptionBox;
    public TextMeshProUGUI descriptionText;

    public int skipCoin;
    public int chanceForLastReappear;
    public int chanceForPrevReappear;

    const int amountOfOptions = 3;
    const int maxTiers = 5;

    int previousPick = 0;
    int queueOfUpgrades = 0;

    int[] options = { 0, 0, 0 };
    int[] arrayOfUpgrades;

    public void QueueUpgrades(KeyCode debugKey = KeyCode.None)
    {
        queueOfUpgrades++;

        if (queueOfUpgrades <= 1)
        {
            if (debugKey == KeyCode.None) SelectOptions();
            else DebugSelectOptions(debugKey);
        }
    }

    public bool CheckQueue()
    {
        if (queueOfUpgrades > 0)
        {
            SelectOptions();
        }
        return queueOfUpgrades > 0;
    }

    //Assigns the possible upgrade toward attack/player stat
    public void SelectOptions()
    {
        //skipText.text = "Skip +$" + skipCoin;

        List<int> listOfUpgrades = new List<int>();
        List<int> previousUpgrades = new List<int>();

        for (int i = 0; i < upgradeStats.GetUpgradeStats().Count; i++)
        {
            UpgradeStats.upgradeTiers currentUpgrade = upgradeStats.GetUpgradeStats()[i];

            //Checking if this upgrade changes the scriptable object stats
            if (currentUpgrade.upgrade == PlayerScript.UPGRADES.none)
            {
                //Making sure that this upgrade doesn't get selected if the attack doesn't even have a spawn rate
                if (currentUpgrade.attkStat != UpgradeStats.ATTACKSTAT.FIRERATE &&
                    !currentUpgrade.attackObj.enableSpawn) continue;
                
                if (currentUpgrade.tierLevel < maxTiers)
                {
                    listOfUpgrades.Add(i);
                    if (currentUpgrade.tierLevel > 0)
                    {
                        previousUpgrades.Add(i);
                    }
                }
                continue;

            }

            if (currentUpgrade.tierLevel < maxTiers)
            {
                listOfUpgrades.Add(i);

                if (currentUpgrade.tierLevel > 0)
                {
                    previousUpgrades.Add(i);
                }
            }
        }

        arrayOfUpgrades = listOfUpgrades.ToArray();

        for (int i = 0; i < amountOfOptions; i++)
        {
            //pick random upgrade
            //Matthew: I think length isn't zero based as got an error about an out of bounds index so i did a lil
            // - 1 to the length
            int num = arrayOfUpgrades[Random.Range(0, arrayOfUpgrades.Length - 1)];

            PickOption(i, num);
        }

        int random = Random.Range(0, 100);

        if (random < chanceForLastReappear && System.Array.IndexOf(arrayOfUpgrades, previousPick) != -1)
        {
            int randOption = Random.Range(0, amountOfOptions);
            PickOption(randOption, previousPick);
        }
        else if (random < chanceForPrevReappear && previousUpgrades.Count > 0)
        {
            int randOption = Random.Range(0, amountOfOptions);
            int randUpgrade = previousUpgrades[Random.Range(0, previousUpgrades.Count)];

            if (System.Array.IndexOf(arrayOfUpgrades, randUpgrade) != -1)
            {
                PickOption(randOption, randUpgrade);
            }
        }
    }

    //Choose selected options
    public void DebugSelectOptions(KeyCode _key)
    {
        List<int> listOfUpgrades = new List<int>();
        int iPicks = 0;


        List<UpgradeStats.upgradeTiers> upgrades = upgradeStats.GetUpgradeStats();
        for(int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].debugKey == _key)
            {
                listOfUpgrades.Add(i);
                iPicks++;
            }

            if (iPicks >= 3)
            {
                arrayOfUpgrades = listOfUpgrades.ToArray();
                for (int j = 0; j < listOfUpgrades.Count; j++)
                {
                    PickOption(j, listOfUpgrades[j]);
                }
                break;
            }
        }

        //Add a random upgrade to fill out the remaining UI slots
        while (iPicks < 3)
        {
            listOfUpgrades.Add(Random.Range(0, upgrades.Count));
            iPicks++;

            if (iPicks == 3)
            {
                arrayOfUpgrades = listOfUpgrades.ToArray();
                for (int j = 0; j < listOfUpgrades.Count; j++)
                {
                    PickOption(j, listOfUpgrades[j]);
                }
            }
        }
        
    }

    //Displays selected upgrade to UI
    //(i = the UI icon slot, num = the element in the array of upgrades
    void PickOption(int _i, int _num)
    {
        //remove that upgrade from the list of upgrades to choose from
        int indexOfNum = System.Array.IndexOf(arrayOfUpgrades, _num);
        arrayOfUpgrades[indexOfNum] = arrayOfUpgrades[arrayOfUpgrades.Length - 1];
        System.Array.Resize(ref arrayOfUpgrades, arrayOfUpgrades.Length - 1);

        options[_i] = _num;

        UpgradeStats.upgradeTiers upgradeTier = upgradeStats.GetUpgradeStats()[options[_i]];

        //upgradeNameText[_i].text = upgradeTier.upgradeName;
        upgradeIconImage[_i].sprite = upgradeTier.icon;
        tierText[_i].text = "" + Mathf.Clamp(upgradeTier.tierLevel + 1, 0, 5);
        //upgradeDescriptionText[i].text = upgradeTier.upgradeDescription;
    }

    //Used in UI button components: Use selected upgrade to upgrade attack/player stat
    public void UpgradeButtonPressed(int _number)
    {
        queueOfUpgrades--;
        previousPick = options[_number];
        UpgradeStats.upgradeTiers upgradeTier = upgradeStats.GetUpgradeStats()[options[_number]];

        int i = Mathf.Clamp(upgradeTier.tierLevel + 1, 0, 5);
        upgradeStats.GetUpgradeStats()[options[_number]].SetUpgradeTier(i);

        player.Upgrade(upgradeTier);


        descriptionBox.SetTrigger("Play");
        descriptionText.text = upgradeTier.upgradeDescription;
    }

    public void SkipPressed()
    {
        //player.IncreaseCoins(skipCoin);
        //player.Upgrade(PlayerScript.UPGRADES.none, 0, 0);
    }
}
