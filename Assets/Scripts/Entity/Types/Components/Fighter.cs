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
    public int Defense { get { return defense; } }
    public int Power { get { return power; } }
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
        if (GetComponent<Player>())
        {
            //Debug.Log($"You died!");
            UIManager.instance.AddMessage($"You died!", "#ff0000");
        }
        else
        {
            //Debug.Log($"{name} is dead!");
            UIManager.instance.AddMessage($"{name} is dead!", "#ffa500");
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.DeadSprite;
        spriteRenderer.color = new Color(191, 0, 0, 1);
        spriteRenderer.sortingOrder = 0;

        name = $"Remains of {name}";
        GetComponent<Actor>().BlocksMovement = false;
        GetComponent<Actor>().IsAlive = false;
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
}
