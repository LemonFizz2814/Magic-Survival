using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStats : MonoBehaviour
{
    public enum ATTACKSTAT
    {
        DAMAGE,
        SPEED,
        FIRERATE,
        DURATION,
        RANGE
    }

    [System.Serializable]
    public class upgradeTiers
    {
        public string upgradeName;
        public PlayerScript.UPGRADES upgrade;
        public string upgradeDescription;
        public float positiveUpgrade;
        public float negativeUpgrade;
        public BaseAttack attackObj;
        public List<ATTACKSTAT> allStatUpgrades;
        [System.NonSerialized]
        public int tierLevel;

        public void SetUpgradeTier(int _tier)
        {
            tierLevel = _tier;
        }
    }

    public List<upgradeTiers> upgradeStats = new List<upgradeTiers>();

    public List<upgradeTiers> GetUpgradeStats()
    {
        return upgradeStats;
    }
}
