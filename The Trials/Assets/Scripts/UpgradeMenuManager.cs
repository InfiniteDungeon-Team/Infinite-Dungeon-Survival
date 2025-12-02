using System.Linq; // needed for GetRandomNumbers()
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuManager : MonoBehaviour
{
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] PlayerUpgradeManager playerUpgradeManager;
    [SerializeField] WaveManager waveManager;

    // LeanTween things
    private Vector2 targetInPos;
    private Vector2 targetOutPos;
    private float timeToTweenIn = 1.50f;
    private float timeToTweenOut = 1f;

    [SerializeField] RectTransform upgradeMenuRectTrans;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Sprite[] iconsArray;
    [SerializeField] Image[] upgradeSlotsIcons;
    [SerializeField] TMP_Text[] currentUpgradeValues;
    [SerializeField] TMP_Text[] newUpgradeValues;


    private int[] selectedUpgrades;

    private bool upgradeSelected = false;



    private void Start()
    {
        targetInPos = new Vector2(cameraTransform.position.x, cameraTransform.position.y);
        targetOutPos = new Vector2(upgradeMenu.transform.position.x, 2000f);
    }

    public void InitiateUpgradeMenu()
    {
        TweenIn();
    }


    public void FetchNewUpgadeOptions()
    {
        // Get 3 random numbers, 0 - 5
        selectedUpgrades = GetRandomNumbers();
        //Debug.Log("3 Nums: [" + string.Join(", ", selectedUpgrades) + "]");



        // Cycle through which upgrades were chosen randomly and apply the UI elements
        for (int i = 0; i < selectedUpgrades.Length; i++)
        {
            switch (selectedUpgrades[i])
            {
                case 0:
                    //Debug.Log($"Selected upgrade #{i + 1} is Attack Damage!");
                    upgradeSlotsIcons[i].sprite = iconsArray[selectedUpgrades[i]];
                    currentUpgradeValues[i].text = playerUpgradeManager.GetCurrentDamage().ToString("F2");
                    newUpgradeValues[i].text = playerUpgradeManager.GetNextUpgradeDamage().ToString("F2");
                    break;
                case 1:
                    //Debug.Log($"Selected upgrade #{i + 1} is Magazine Size!");
                    upgradeSlotsIcons[i].sprite = iconsArray[selectedUpgrades[i]];
                    currentUpgradeValues[i].text = playerUpgradeManager.GetCurrentPlayerMagazineSize().ToString();
                    newUpgradeValues[i].text = playerUpgradeManager.GetNextUpgradePlayerMagazineSize().ToString();
                    break;
                case 2:
                    //Debug.Log($"Selected upgrade #{i + 1} is Max Health!");
                    upgradeSlotsIcons[i].sprite = iconsArray[selectedUpgrades[i]];
                    currentUpgradeValues[i].text = playerUpgradeManager.GetCurrentHP().ToString();
                    newUpgradeValues[i].text = playerUpgradeManager.GetNextUpgradeHP().ToString();
                    break;
                case 3:
                    //Debug.Log($"Selected upgrade #{i + 1} is Movement Speed!");
                    upgradeSlotsIcons[i].sprite = iconsArray[selectedUpgrades[i]];
                    currentUpgradeValues[i].text = playerUpgradeManager.GetCurrentMoveSpeed().ToString("F2");
                    newUpgradeValues[i].text = playerUpgradeManager.GetNextUpgradeMoveSpeed().ToString("F2");
                    break;
                case 4:
                    //Debug.Log($"Selected upgrade #{i + 1} is Reload Speed!");
                    upgradeSlotsIcons[i].sprite = iconsArray[selectedUpgrades[i]];
                    currentUpgradeValues[i].text = playerUpgradeManager.GetCurrentPlayerReloadSpeed().ToString("F2");
                    newUpgradeValues[i].text = playerUpgradeManager.GetNextUpgradePlayerReloadSpeed().ToString("F2");
                    break;
                case 5:
                    //Debug.Log($"Selected upgrade #{i + 1} is Special Attack!");
                    upgradeSlotsIcons[i].sprite = iconsArray[selectedUpgrades[i]];
                    currentUpgradeValues[i].text = playerUpgradeManager.GetCurrentPlayerSpecialAttacks().ToString();
                    newUpgradeValues[i].text = playerUpgradeManager.GetNextUpgradePlayerSpecialAttacks().ToString();
                    break;
            }
        }
    }

    public void ClickUpgrade_1()
    {
        Debug.Log("Upgrade #1 Selected!");
        if (upgradeSelected)
            return;

        switch (selectedUpgrades[0])
        {
            case 0:
                // Attack Damage
                playerUpgradeManager.SetPlayerDamage();
                break;
            case 1:
                // Magazine Size
                playerUpgradeManager.SetPlayerMagazineSize();
                break;
            case 2:
                // Max Health
                playerUpgradeManager.SetPlayerMaxHP();
                break;
            case 3:
                // Movement Speed
                playerUpgradeManager.SetPlayerMoveSpeed();

                break;
            case 4:
                // Max Health
                playerUpgradeManager.SetPlayerReloadSpeed();

                break;
            case 5:
                // Special Attack
                playerUpgradeManager.SetPlayerSpecialAttacks();
                break;
        }
        TweenOut();
    }

    public void ClickUpgrade_2()
    {
        Debug.Log("Upgrade #2 Selected!");
        if (upgradeSelected)
            return;

        switch (selectedUpgrades[1])
        {
            case 0:
                // Attack Damage
                playerUpgradeManager.SetPlayerDamage();
                break;
            case 1:
                // Magazine Size
                playerUpgradeManager.SetPlayerMagazineSize();
                break;
            case 2:
                // Max Health
                playerUpgradeManager.SetPlayerMaxHP();
                break;
            case 3:
                // Movement Speed
                playerUpgradeManager.SetPlayerMoveSpeed();

                break;
            case 4:
                // Max Health
                playerUpgradeManager.SetPlayerReloadSpeed();

                break;
            case 5:
                // Special Attack
                playerUpgradeManager.SetPlayerSpecialAttacks();
                break;
        }
        TweenOut();
    }

    public void ClickUpgrade_3()
    {
        Debug.Log("Upgrade #3 Selected!");
        if (upgradeSelected)
            return;

        switch (selectedUpgrades[2])
        {
            case 0:
                // Attack Damage
                playerUpgradeManager.SetPlayerDamage();
                break;
            case 1:
                // Magazine Size
                playerUpgradeManager.SetPlayerMagazineSize();
                break;
            case 2:
                // Max Health
                playerUpgradeManager.SetPlayerMaxHP();
                break;
            case 3:
                // Movement Speed
                playerUpgradeManager.SetPlayerMoveSpeed();
                break;
            case 4:
                // Max Health
                playerUpgradeManager.SetPlayerReloadSpeed();
                break;
            case 5:
                // Special Attack
                playerUpgradeManager.SetPlayerSpecialAttacks();
                break;
        }
        TweenOut();
    }
    private void SetUpgradeSelected(bool _state)
    {
        upgradeSelected = _state;
    }

    public int[] GetRandomNumbers()
    {
        // Used AI to assist with learning this... returns an array of 3 random numbers from a range without repeating values
        System.Random rng = new System.Random();
        int[] threeNumbers = Enumerable.Range(0, 6).OrderBy(x => rng.Next()).Take(3).ToArray();
        return threeNumbers;
    }
    private void TweenIn()
    {
        // start off screen above camera view
        upgradeMenuRectTrans.anchoredPosition = new Vector2(0f, 2000f);

        SetUpgradeSelected(false); // allow upgrade selections
        FetchNewUpgadeOptions(); // get new upgrade choices
        upgradeMenu.SetActive(true);

        // tween to center
        LeanTween.move(upgradeMenuRectTrans, Vector3.zero, timeToTweenIn).setEaseOutBounce();
    }

    private void TweenOut()
    {
        SetUpgradeSelected(true);

        Vector3 offScreenPos = new Vector3(0f, 2000f, 0f);

        LeanTween.move(upgradeMenuRectTrans, offScreenPos, timeToTweenOut).setEaseInBack()
                 .setOnComplete(() =>
                 {
                     upgradeMenu.SetActive(false);
                     waveManager.InitiateWaveStart();
                 });

        playerUpgradeManager.PrintPlayerUpgrades();
    }
}
