using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour
{
    [SerializeField] private MMFeedbacks moveFeedbacks;
    [SerializeField] private MMFeedbacks consumeFeedbacks;
    public virtual bool Activate(Actor consumer) => false;
    public virtual bool Cast(Actor consumer, Actor target) => false;
    public virtual bool Cast(Actor consumer, List<Actor> targets) => false;
    
    public MMFeedbacks MoveFeedbacks { get { return moveFeedbacks; } }
    public MMFeedbacks ConsumeFeedbacks { get { return consumeFeedbacks; } }

    public void Consume(Actor consumer)
    {
        gameObject.SetActive(true);
        if (consumer.GetComponent<Inventory>().SelectedConsumable == this)
        {
            consumer.GetComponent<Inventory>().SelectedConsumable = null;
        }
        gameObject.transform.SetParent(null, true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        consumer.Inventory.Items.Remove(GetComponent<Item>());
        GameManager.instance.RemoveEntity(GetComponent<Item>());
        StartCoroutine(waitToDestroy(0.5f));
    }

    private IEnumerator waitToDestroy(float time)
    {
        consumeFeedbacks?.PlayFeedbacks();
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
