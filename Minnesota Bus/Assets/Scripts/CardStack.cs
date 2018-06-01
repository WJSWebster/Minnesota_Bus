using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    private readonly Sprite[] cardFaces;
    private readonly Sprite cardBack;

    public List<CardModel> cards; // temporary added security by providing list size TODO evaluate usefulness
    // TODO cards needs to be made private for security purposes

    public GameObject cardPrefab; // DEBUG: is this the best way to pull in all of the class member assignments of the card? (e.g. the faces array & cardBack)

    public bool IsDealersHand
    {
        get { return gameObject.name == "Dealer"; } // pulls the name of the GameObject that this component is attached to
    }

    public bool HasCards
    {
        get { return cards != null && cards.Count > 0; } // the first condition means that the second condition is never checked if the first is false (thefore, this condition does not cause OutOfBounds)
    }

    public event CardRemovedEventHandler CardRemoved;

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
            cards.Clear();
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
            //GameObject cardCopy = (GameObject)Instantiate(cardPrefab); // instantiate takes in a GameObject and copies it
            //CardModel cardModel = cardCopy.GetComponent<CardModel>();
            //Debug.Log(cardModel + " cardBack == null?: " + (cardModel.cardBack == null));

            CardModel temp = new CardModel(i);
            //Debug.Log(temp + " cardBack == null?: " + (temp.cardBack == null));

            // add this temporary card to the 'cards' List of CardModel objects
            cards.Add(temp);
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
            CardRemoved(this, new CardRemovedEventArgs(temp.Index)); // telling any subscribers to the event that this card has been removed

        return temp;
    }

    public void Push(CardModel card)
    {
        cards.Add(card);
    }
}
