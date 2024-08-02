using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Subjects
{
    private static PlayerStatus instance;
    public static PlayerStatus Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerStatus>();
            }
            return instance;
        }
    }


    [SerializeField] protected bool isUsingWeapon = false;
    public bool IsUsingWeapon => isUsingWeapon;

    [Header("Duration")]
    [SerializeField] protected float durationOfItem = 5f;
    public float DurationOfItem => durationOfItem;
    [SerializeField] protected float durationOfWeapon = 8f;
    public float DurationOfWeapon => durationOfWeapon;

    [Header("Speed")]
    [SerializeField] protected float speedDefault = 5f;
    public float SpeedDefault { get { return speedDefault; } }

    [SerializeField] protected float speedAdded = 3f;
    [SerializeField] protected float speedRealTime;

    [Header("HP")]
    [SerializeField] protected int HP = 10;
    public int CurrentHP => HP;
    [SerializeField] protected int maxHP = 10;
    public int MaxHP => maxHP;

    [Header("Shield")]
    [SerializeField] protected bool hasShield = false;

    [Header("Bomb")]
    [SerializeField] protected int radiusDefault = 2;
    public int RadiusDefault => radiusDefault;
    [SerializeField] protected int radiusRealtime = 3;
    [SerializeField] protected int bombRadiusAdded = 2;

    [SerializeField] protected int bombAmount = 3;
    public int BombAmount => bombAmount;


    [Header("Item and Weapon Dictionary")]
    [SerializeField] protected Dictionary<Item, float> items = new();
    public Dictionary<Item, float> Items => items;
    [SerializeField] protected Dictionary<Item, int> weapons = new();
    public Dictionary<Item, int> Weapons => weapons;

    [Header("References")]
    [SerializeField] protected GameObject spinningAxe;
    [SerializeField] protected GameObject excalibur;
    [SerializeField] protected GameObject darkExcalibur;

    [SerializeField] protected GameObject excaliburIcon;
    [SerializeField] protected GameObject darkExcaliburIcon;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        // DontDestroyOnLoad(this.gameObject);

        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        items = new Dictionary<Item, float>()
        {
            {Item.SpinningAxe, 0},
            {Item.SuperBlastRadius, 0},
            {Item.Shield, 0},
            {Item.SpeedIncrease,0 },
            {Item.Heal, 0},
        };
        weapons = new Dictionary<Item, int>()
        {
            {Item.Excalibur, 0},
            {Item.DarkExcalibur, 0},
        };
    }



    private void Update()
    {
        UpdateIsUsingWeapon();

        HandlingSpeedUp();
        HandlingShield();
        HandlingBlastRadius();
        HandlingSpinningAxe();
        HandlingExcalibur();
        HandlingDarkExcalibur();

        HandleHeal(2);
        if (HP <= 0)
        {
            this.gameObject.SetActive(false) ;
            NotifyObservers(PlayerAction.Lose, 0);
        }
    }

    public void AddItemTime(Item item, int time)
    {
        if (!items.ContainsKey(item))
        {
            Debug.LogError($"Item {item} not found in weapons dictionary.");
            return;
        }
        items[item] = time * durationOfItem;
    }
    public void AddAxeTime(Item item, int time)
    {
        if (!items.ContainsKey(item))
        {
            Debug.LogError($"Item {item} not found in weapons dictionary.");
            return;
        }
        items[item] = time * durationOfWeapon;
    }
    public void AddItemQuantity(Item item, int quality)
    {
        if (!items.ContainsKey(item))
        {
            Debug.LogError($"Item {item} not found in weapons dictionary.");
            return;
        }
        items[item] = quality;
    }
    public void AddWeaponQuantity(Item item, int quality)
    {
        if (!weapons.ContainsKey(item))
        {
            Debug.LogError($"Weapon {item} not found in weapons dictionary.");
            return;
        }

        weapons[item] += quality;
    }
    public void RemoveWeaponQuantity(Item item, int quality)
    {
        if (!weapons.ContainsKey(item))
        {
            Debug.LogError($"Weapon {item} not found in weapons dictionary.");
            return;
        }
        if(weapons[item] < quality)
        {
            Debug.Log($"Weapon {item} is not enough to use");
            return;
        }
        weapons[item] = Mathf.Max(weapons[item] - quality, 0);
    }


    public void PlaceBomb()
    {
        NotifyObservers(PlayerAction.PlaceBomb, 1);
    }

    public void PlusBomb()
    {
        NotifyObservers(PlayerAction.PlusBomb, 1);
    }


    public void HandleHurt(int damage)
    {
        if (hasShield)
        {
             Debug.Log("Tao co khien roi");
             return;
        }
        
        HP = Mathf.Max(HP-damage, 0);
        PlayerMovement.Instance.Flickering();

        NotifyObservers(PlayerAction.Hurt, damage);
    }

    public void HandleHeal(int bonusHP)
    {
        if (items[Item.Heal] <= 0) return;

        HP = Mathf.Min(HP + bonusHP, maxHP);
        items[Item.Heal]--;
        Debug.Log($"Heal: {bonusHP}");

        NotifyObservers(PlayerAction.Heal, bonusHP);
    }


    public void HandlingSpeedUp()
    {
        if (items[Item.SpeedIncrease] <= 0)
        {
            this.GetComponent<PlayerMovement>().ChangeSpeed(speedDefault);
            speedRealTime = speedDefault;

            return;
        }

        items[Item.SpeedIncrease] -= Time.deltaTime;
        if (speedRealTime == speedDefault)
        {
            speedRealTime = speedDefault + speedAdded;
            GetComponent<PlayerMovement>().ChangeSpeed(speedRealTime);

            NotifyObservers(PlayerAction.SpeedUp, 0);
        }


    }
    public void HandlingShield()
    {
        if (items[Item.Shield] <= 0)
        {
            hasShield = false;
            return;
        }

        items[Item.Shield] -= Time.deltaTime;
        if (!hasShield)
        {
            hasShield = true;

            NotifyObservers(PlayerAction.Shield, 0);
        }
    }
    public void HandlingBlastRadius()
    {
        if(items[Item.SuperBlastRadius] <= 0)
        {
            GetComponent<BombController>().RadiusChange(radiusDefault);

            radiusRealtime = radiusDefault;
            return;
        }

        items[Item.SuperBlastRadius] -= Time.deltaTime;
        if (radiusRealtime == radiusDefault)
        {
            radiusRealtime = radiusRealtime + bombRadiusAdded;
            GetComponent<BombController>().RadiusChange(radiusRealtime);

            NotifyObservers(PlayerAction.BlastRadius, bombRadiusAdded);
        }
    }
   

    public void HandlingSpinningAxe()
    {
        if (items[Item.SpinningAxe] <= 0)
        {
            if (spinningAxe.activeSelf)
            {
                spinningAxe.SetActive(false);
            }

            return;
        }

        items[Item.SpinningAxe] -= Time.deltaTime;

        if (!spinningAxe.activeSelf)
        {
            spinningAxe.SetActive(true);
        }
    }

    public void HandlingExcalibur()
    {
        if (weapons[Item.Excalibur] <= 0)
        {
            if (excalibur.activeSelf)
            {
                excalibur.SetActive(false);
                excaliburIcon.SetActive(false);
            }
            return;
        }

        if (!excalibur.activeSelf)
        {
            excalibur.SetActive(true);
            excaliburIcon.SetActive(true);
        }
    }
    public void HandlingDarkExcalibur()
    {
        if (weapons[Item.DarkExcalibur] <= 0)
        {
            if (darkExcalibur.activeSelf)
            {
                darkExcalibur.SetActive(false);
                darkExcaliburIcon.SetActive(false);
            }
            return;
        }
        if (!darkExcalibur.activeSelf)
        {
            darkExcalibur.SetActive(true);
            darkExcaliburIcon.SetActive(true);
        }
    }

    protected void UpdateIsUsingWeapon()
    {
        foreach (var weapon in weapons)
        {
            if(weapon.Value > 0)
            {
                isUsingWeapon = true;
                return;
            }
        }
        isUsingWeapon = false;
    }


    public void SetUpForItem(Item itemType)
    {
        if (itemType == Item.Excalibur)
        {
            weapons[Item.DarkExcalibur] = 0;
        }
        else if (itemType == Item.DarkExcalibur)
        {
            weapons[Item.Excalibur] = 0;
        }
        Debug.Log("Pickup" + itemType.ToString());
    }
}
