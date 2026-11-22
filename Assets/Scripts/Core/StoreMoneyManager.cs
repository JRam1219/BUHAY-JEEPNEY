using UnityEngine;

public class StoreMoneyManager : MonoBehaviour
{
    public static StoreMoneyManager Instance;

    public int currentMoney = 1700;
    public int itemsBought = 0;
    public int totalItemsToBuy = 4; // Match the number of shelves you place

    void Awake()
    {
        Instance = this;
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        itemsBought++;
        Debug.Log("Spent ₱" + amount + ". Money left: ₱" + currentMoney);
    }

    public bool HasBoughtAll()
    {
        return itemsBought >= totalItemsToBuy;
    }
}