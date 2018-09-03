using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour
{
    private CardStack deck;  // all cards, on the table (either dealer's or player's)
    private int lastCount;  // This represents the previous count of how many cards in deck - TODO is this still required?

    public Dictionary<int, CardView> fetchedCards;  // a list of only the card's indexes
    public Vector3 startPos;
    public float cardOffset;
    public GameObject cardPrefab;

    public float posNoise;
    public float rotNoise;

    // TODO REMOVE:

    //// Event Handler
    //private void deck_CardRemoved(object sender, CardEventArgs e)
    //{
    //    // this if statement should be redundant as both are in-sync, but just for added security (and because i dont trust my code)
    //    if (fetchedCards.ContainsKey(e.Card.Index))
    //    {
    //        //Destroy(fetchedCards[e.Card.Index].Card);  // destroys the actual value
    //        fetchedCards.Remove(e.Card.Index);  // removes it from the fetchedCards dictionary
    //    }
    //}

    //private void deck_CardAdded(object sender, CardEventArgs e)
    //{
    //    //AddCard(position, e.Card, deck.CardsCount);  // TODO there is no position argument passed so cannot work in this current implementation

    //    // this if statement should be redundant as both are in-sync, but just for added security (and because i dont trust my code)
    //    //if (!fetchedCards.ContainsKey(e.Card.Index))
    //    //{
    //    //    fetchedCards.Add(e.Card.Index, e.Card);

    //    //}
    //}

    public void Start()
    {
        fetchedCards = new Dictionary<int, CardView>();
        deck = GetComponent<CardStack>();

        ShowCards();
        lastCount = deck.CardsCount;

        //TODO: REMOVE
            //deck.CardRemoved += deck_CardRemoved;
            //deck.CardAdded += deck_CardAdded;
    }

    private void Update()
    {
        if (lastCount != deck.CardsCount) // if no. of cards has changed since last update, else dont bother // TODO also need to check if a cards ShowFace has changed
        {
            lastCount = deck.CardsCount;
            ShowCards();
        }
        // else, the number of cards has not changed, so do not re-draw // todo make less primative
    }

    public void Toggle(CardModel card, bool isFaceUp) // TODO to be called to from GameController // TODO surely this is useless now? REMOVE
    {
        fetchedCards[card.Index].IsFaceUp = isFaceUp;  // TODO what is the difference between this and card.showFace
        //card.ToggleFace();  // How many times does this need to be called??
    }

    public void Clear()
    {
        deck.Setup();  // TODO investigate is Reset() being referenced in multiple different locations?

        foreach (CardView view in fetchedCards.Values)
        {
            Destroy(view.Card);  // destroys the GameObject itself (not just reassign the dictionarys values)
        }

        fetchedCards.Clear();
    }


    private float NextFloat(float range) // specifcally to make either a plus or minus noise value
    {
        System.Random r = new System.Random((int)DateTime.Now.Ticks);  // has to be even more random (using number of ticks as seed) due to this function being being called to within the same loops
        return (float)(r.NextDouble() * (-range - range) + range);
    }

    public void ShowCards()
    {
        if(deck != null && deck.HasCards) // checks that deck list is populated before continueing
        {
            int cardNo = 0;
            Vector3 position = new Vector3(0, 0, 0);

            foreach(CardModel card in deck.GetCards())
            {
                if (fetchedCards.ContainsKey(card.Index)) // If this card is already in fetchedCards - i.e. are old cards:
                {
                    CardModel model = fetchedCards[card.Index].Card.GetComponent<CardModel>();  // what is this doing here?!? Do we still need to do this now that cards is a list of CardModels rather than ints?
                    model.ToggleFace();
                }
                else // is a new card and is yet to be added to fetchedCards
                {
                    // generating position noise:
                    float[] axis = new float[2];  // an array of two float elements
                    for (int i = 0; i < axis.Length; i++)
                        axis[i] = NextFloat(posNoise);

                    Vector3 randPos = new Vector2(axis[0], axis[1]);

                    // generating rotation noise:
                    float rotation = NextFloat(rotNoise);

                    if (deck.IsDealersHand)
                    {
                        position = startPos + randPos;
                    }
                    else // if is player's hand
                    {
                        float spread = cardOffset * cardNo;
                        Vector3 spreadPos = new Vector2(spread, 0f);  // only altering the X axis (∴ delta of Y & Z axis = 0f)

                        position = startPos + spreadPos + randPos;
                    }

                    AddCard(card, position, rotation, cardNo);
                }

                cardNo++;
            }

            Debug.Log("-------------------------------------------------");  // to indicate that this round's cards drawing is done
        }
    }

    void AddCard(CardModel card, Vector3 newPosition, float newRotation, int positionalIndex)
    {
        GameObject cardCopy = Instantiate(cardPrefab);  // shouldnt this only be used if not already in fetched cards?? // instantiate takes in a GameObject and copies it
        CardModel cardModel = cardCopy.GetComponent<CardModel>();  // what is this doing here?!? Do we still need to do this now that cards is a list of CardModels rather than ints?

        cardModel.Index = card.Index;
        //cardModel.ToggleFace();
        
        // if not previously drawn, the card's need a new random position & rotation:
        cardCopy.transform.position = newPosition;
        cardCopy.transform.rotation = Quaternion.Euler(0, 0, newRotation);

        SpriteRenderer sp = cardCopy.GetComponent<SpriteRenderer>();
        sp.sortingOrder = 51 - positionalIndex;  // reverses the order in which cards are drawn

        fetchedCards.Add(card.Index, new CardView(cardCopy));
        //deck_CardAdded(card, cardNo);  // already added in the method call?
    }
}
