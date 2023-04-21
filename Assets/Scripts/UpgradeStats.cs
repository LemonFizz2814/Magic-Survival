using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStats : MonoBehaviour
{
    public enum ATTACKSTAT
    {
        NONE,
        DAMAGE,
        SPEED,
        FIRERATE,
        DURATION,
        RANGE
    }

    [System.Serializable]
    public class upgradeTiers
    {
        [Header("UI and name")]
        public string upgradeName;
        public Sprite icon;
        public string upgradeDescription;

        [Header("Player & special stat upgrade")]
        public PlayerScript.UPGRADES upgrade;

        [Header("Attack stat upgrade")]
        public BaseAttack attackObj;
        public ATTACKSTAT attkStat;

        [Header("Upgrade values")]
        public float positiveUpgrade;
        public float negativeUpgrade;

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
