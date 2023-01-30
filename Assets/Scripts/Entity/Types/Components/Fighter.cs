using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Actor))]
public class Fighter : MonoBehaviour
{
    [SerializeField] private int maxHp, hp, defense, power;
    [SerializeField] private Actor target;
    [SerializeField] private MMFeedbacks meleeFeedbacks;
    [SerializeField] private MMFeedbacks dieFeedbacks;

    public MMFeedbacks MeleeFeedbacks { get { return meleeFeedbacks; } }

    public int Hp
    {
        get => hp;
        set
        {
            hp = Mathf.Max(0, Mathf.Min(value, maxHp));

            if (GetComponent<Player>())
            {
                UIManager.instance.SetHealth(hp, maxHp);
            }

            if (hp <= 0)
                Die();
        }
    }
    public int MaxHp { get { return maxHp; } set { maxHp = value; } }
    public int Defense { get { return defense; } set { defense = value; } }
    public int Power { get { return power; } set { power = value; } }
    public Actor Target { get { return target; } set { target = value; } }

    private void Start()
    {
        if (GetComponent<Player>())
        {
            UIManager.instance.SetHealthMax(maxHp);
            UIManager.instance.SetHealth(hp, maxHp);
        }
    }

    public void Die()
    {
        if (GetComponent<Actor>().IsAlive)
        {
            if (GetComponent<Player>())
            {
                UIManager.instance.AddMessage("You died!", "#ff0000"); //Red
                dieFeedbacks?.PlayFeedbacks();
                UIManager.instance.ToggleRebornMenu();
            }
            else
            {
                GameManager.instance.Actors[0].GetComponent<Level>().AddExperience(GetComponent<Level>().XPGiven);
                UIManager.instance.AddMessage($"{name} is dead!", "#ffa500"); //Light Orange
            }
            GetComponent<Actor>().IsAlive = false;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.DeadSprite;
        spriteRenderer.color = new Color(191, 0, 0, 1);
        spriteRenderer.sortingOrder = 0;

        name = $"Remains of {name}";
        GetComponent<Actor>().BlocksMovement = false;
        if (!GetComponent<Player>())
        {
            GameManager.instance.RemoveActor(this.GetComponent<Actor>());
        }
    }

    public int Heal(int amount)
    {
        if (hp == maxHp)
        {
            return 0;
        }

        int newHpValue = hp + amount;

        if (newHpValue > maxHp)
        {
            newHpValue = maxHp;
        }

        int amountRecovered = newHpValue - hp;
        Hp = newHpValue;
        return amountRecovered;
    }
    public FighterState SaveState() => new FighterState(
        maxHp: maxHp,
        hp: hp,
        defense: defense,
        power: power,
        target: target != null ? target.name : null
    );

    public void LoadState(FighterState state)
    {
        maxHp = state.MaxHp;
        hp = state.Hp;
        defense = state.Defense;
        power = state.Power;
        target = GameManager.instance.Actors.Find(a => a.name == state.Target);
    }
}

//[System.Serializable]
public class FighterState
{
    [SerializeField] private int maxHp, hp, defense, power;
    [SerializeField] private string target;

    public int MaxHp
    {
        get => maxHp; set => maxHp = value;
    }
    public int Hp
    {
        get => hp; set => hp = value;
    }
    public int Defense
    {
        get => defense; set => defense = value;
    }
    public int Power
    {
        get => power; set => power = value;
    }
    public string Target
    {
        get => target; set => target = value;
    }

    public FighterState(int maxHp, int hp, int defense, int power, string target)
    {
        this.maxHp = maxHp;
        this.hp = hp;
        this.defense = defense;
        this.power = power;
        this.target = target;
    }
}