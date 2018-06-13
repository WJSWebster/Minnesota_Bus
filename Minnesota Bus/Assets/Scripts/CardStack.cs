using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    public List<CardModel> cards; // temporary added security by providing list size TODO evaluate usefulness
    // TODO cards needs to be made private for security purposes
    
    public bool IsDealersHand
    {
        get { return gameObject.name == "Dealer"; } // pulls the name of the GameObject that this component is attached to
    }

    public bool HasCards
    {
        get { return cards != null && cards.Count > 0; } // the first condition means that the second condition is never checked if the first is false (thefore, this condition does not cause OutOfBounds)
    }

    public event CardEventHandler CardRemoved;
    public event CardEventHandler CardAdded;

    public int CardsCount
    {
        get
        {
            if (cards == null)
                return 0;
            else
                return cards.Count;
        }
    }

    public int CardsCapacity
    {
        get
        {
            return cards.Capacity;
        }
    }

    void Awake()
    {
        if (IsDealersHand)
        {
            cards = new List<CardModel>(52);
            //cards.Clear(); // TODO is this necessary??
            CreateDeck(); // at beginning of the game, the dealer has all the cards in their hand
            Shuffle();
        }
        else // owner == "Player"
        {
            cards = new List<CardModel>(4); // player begins with no cards and ∴ should not have a deck created or shuffled
            //cards.Clear(); //TODO is this necessary?
            //CreateDeck();
        }
    }

    void CreateDeck()
    {
        for (int i = 0; i < cards.Capacity; i++) // cannot use magic numbers now that our list can be one of two sizes
        {
            CardModel temp = new CardModel(i, false); // tODO only false because assumed that this is the dealer's hand (as this method is only ever called here - when creating the dealer's hand of all the cards initially)

            cards.Add(temp); // add this temporary card to the 'cards' List of CardModel objects
        }
    }

    public void Shuffle()
    {
        int length = CardsCount;
        
        System.Random rand = new System.Random();
        CardModel temp;
        int target;

        for (int i = 0; i < length; i++)
        {
            target = i + rand.Next(length - i);

            temp = cards[target];
            cards[target] = cards[i];
            cards[i] = temp;
        }
    }

    public IEnumerable<CardModel> GetCards()
    {
        foreach (CardModel i in cards)
        {
            yield return i;
        }
        //return cards;
    }

    public CardModel Pop(int index = 0)
    {
        CardModel temp = cards[index];
        cards.RemoveAt(index);

        if (CardRemoved != null) // so long as there is atleast one subscriber to the CardRemoved event
            CardRemoved(this, new CardEventArgs(temp)); // telling any subscribers to the event that this card has been removed

        return temp;
    }

    public void Push(CardModel card)
    {
        cards.Add(card);

        if (CardAdded != null)
        {
            CardAdded(this, new CardEventArgs(card));
        }
    }
}
