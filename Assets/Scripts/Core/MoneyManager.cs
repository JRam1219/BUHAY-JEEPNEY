using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int currentMoney = 0;
    public int targetMoney = 3000;

    [Header("UI")]
    public TextMeshProUGUI moneyText;

    void Awake()
    {
        Instance = this;
        UpdateMoneyDisplay();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;

        Debug.Log("Money earned: ₱" + amount);
        Debug.Log("Current money: ₱" + currentMoney + " / ₱" + targetMoney);

        UpdateMoneyDisplay();
    }

    void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: ₱" + currentMoney.ToString("N0")
                + " / ₱" + targetMoney.ToString("N0");
        }
    }

    public bool HasReachedTarget()
    {
        return currentMoney >= targetMoney;
    }
}