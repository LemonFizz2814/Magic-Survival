using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;

public class AdvertisingScript : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IStoreListener, IUnityAdsInitializationListener
{
    [Header("Ad Settings")]
    public string _androidGameId;
    public string _iOSGameId;
    public int coinsFromWatchingAd;
    public int chanceForAd;
    public int chanceForBanner;
    public int chanceToShowWatchAd;
    public float adTime;

    public string kRemoveAds;
    static IStoreController m_StoreController;
    static IExtensionProvider m_StoreExtensionProvider;
    string _gameId;
    string _adUnitId;
    string _bannerAdUnitId;
    bool isIOS;

    float previousTimeFromAd;

    enum EADTYPE
    {
        nothing,
        coin,
    }

    EADTYPE adType;

    string environment = "production";

    void Awake()
    {
        InitializeAds();
        InitializePurchasing();
    }
    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"exception {exception.Message}");
            // An error occurred during services initialization.
        }
    }

    // Initalization crap
    public void InitializeAds()
    {
        isIOS = Application.platform == RuntimePlatform.IPhonePlayer;
        _gameId = (isIOS)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, false, this);

        _adUnitId = (isIOS) ? "Rewarded_iOS" : "Rewarded_Android";
        Advertisement.Load(_adUnitId, this);
        _adUnitId = (isIOS) ? "Interstitial_iOS" : "Interstitial_Android";
        Advertisement.Load(_adUnitId, this);
    }
    private bool IsStoreInitialized()
    {
        // Check if both the Purchasing references are set
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"LOADED");
    }
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId)
    {
        Debug.Log($"OnUnityAdsShowStart");
    }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"UnityAdsShowCompletionState {showCompletionState}");

        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.COMPLETED:
                switch (adType)
                {
                    case EADTYPE.coin:
                        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coinsFromWatchingAd);
                        //StartCoroutine(uiManager.UpdateInGameCoins(PlayerPrefs.GetInt("Coins")));
                        //FindObjectOfType<UIManager>().UpdateCustomizeCoins(PlayerPrefs.GetInt("Coins"));
                        break;
                }
                break;
        }
    }

    public void InitializePurchasing()
    {
        if (IsStoreInitialized())
        {
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(kRemoveAds, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) { }
    public void OnInitializeFailed(InitializationFailureReason error) { }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (Equals(args.purchasedProduct.definition.id, kRemoveAds))
        {
            Debug.Log("Purchasing Product");
            PlayerPrefs.SetInt("boughtAds", 1);
        }
        else
        {
            Debug.Log("Unrecognized Product");
        }
        return PurchaseProcessingResult.Complete;
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: { 1}", product.definition.storeSpecificId, failureReason));
    }

    // proper stuff

    public void BuyRemoveAds(string productId)
    {
        if (IsStoreInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log("Purchasing Product");
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("Product either is not found or not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void ShowAd()
    {
        if (PlayerPrefs.GetInt("boughtAds") == 0)
        {
            _adUnitId = (isIOS) ? "Interstitial_iOS" : "Interstitial_Android";
            adType = EADTYPE.nothing;
            Advertisement.Show(_adUnitId, this);
        }
    }
    public void WatchAd()
    {
        _adUnitId = (isIOS) ? "Rewarded_iOS" : "Rewarded_Android";
        Debug.Log("Showing Ad: " + _adUnitId);
        adType = EADTYPE.coin;
        Advertisement.Show(_adUnitId, this);
    }

    // banner
    public void ShowBanner()
    {
        if (UnityEngine.Random.Range(0, 100) < chanceForBanner)
        {
            _bannerAdUnitId = "Banner_Android";
            Advertisement.Banner.SetPosition(BannerPosition.TOP_LEFT);

            LoadBanner();
        }
        else
        {
            Advertisement.Banner.Hide();
        }
    }
    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_bannerAdUnitId, options);
    }
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
        ShowBannerAd();
    }
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
    }
    void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(_bannerAdUnitId, options);
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }

    public void TimerCheckAd()
    {
        if (Time.time - PlayerPrefs.GetFloat("previousTimeFromAd") > adTime)
        {
            PlayerPrefs.SetFloat("previousTimeFromAd", Time.time);
            ShowAd();
        }
        else
        {
            RandomAdChance();
        }
    }
    public void RandomAdChance()
    {
        if (UnityEngine.Random.Range(0, 100) < chanceForAd)
        {
            ShowAd();
        }
    }
}
