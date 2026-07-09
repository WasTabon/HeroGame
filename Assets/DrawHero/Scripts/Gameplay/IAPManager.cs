using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance;

    public const string POWER_PACK_ID = "com.drawhero.powerpack";

    public event System.Action OnPowerPackPurchased;

    private StoreController storeController;
    private bool ready;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        storeController = UnityIAPServices.StoreController();

        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnProductsFetchFailed += OnProductsFetchFailed;

        await storeController.Connect();

        var products = new List<ProductDefinition>
        {
            new ProductDefinition(POWER_PACK_ID, ProductType.Consumable)
        };
        storeController.FetchProducts(products);
    }

    private void OnDestroy()
    {
        if (storeController != null)
        {
            storeController.OnPurchasePending -= OnPurchasePending;
            storeController.OnPurchaseFailed -= OnPurchaseFailed;
            storeController.OnProductsFetched -= OnProductsFetched;
            storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
        }

        if (Instance == this)
            Instance = null;
    }

    private void OnProductsFetched(List<Product> products)
    {
        ready = true;
    }

    private void OnProductsFetchFailed(ProductFetchFailed fail)
    {
        Debug.LogWarning("IAP product fetch failed: " + fail.FailureReason);
    }

    public bool IsReady()
    {
        return ready;
    }

    public void BuyPowerPack()
    {
        if (storeController == null)
        {
            Debug.LogWarning("IAP not connected yet.");
            return;
        }

        storeController.PurchaseProduct(POWER_PACK_ID);
    }

    private void OnPurchasePending(PendingOrder order)
    {
        bool isPowerPack = false;

        var items = order.CartOrdered.Items();
        foreach (var item in items)
        {
            if (item.Product.definition.id == POWER_PACK_ID)
            {
                isPowerPack = true;
                break;
            }
        }

        if (isPowerPack)
        {
            PowerUpManager.GrantPowerPack();
            OnPowerPackPurchased?.Invoke();

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySfx("pop");
        }

        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseFailed(FailedOrder order)
    {
        Debug.LogWarning("Purchase failed: " + order.FailureReason);
    }
}