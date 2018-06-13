using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour
{
    private CardStack deck;
    private int lastCount; // INVESTIGATE what does this do?

    public Dictionary<int, CardView> fetchedCards; // a list of only the card's indexes
    public Vector3 startPos;
    public float cardOffset;
    public GameObject cardPrefab;

    public float posNoise;
    public float rotNoise; // TODO make this actually used (see below)

    // Event Handler
    private void deck_CardRemoved(object sender, CardEventArgs e)
    {
        // this if statement should be redundant as both are in-sync, but just for added security (and because i dont trust my code)
        if (fetchedCards.ContainsKey(e.Card.Index))
        {
            //Destroy(fetchedCards[e.Card.Index].Card); // destroys the actual value
            fetchedCards.Remove(e.Card.Index); // removes it from the fetchedCards dictionary
        }
    }

    private void deck_CardAdded(object sender, CardEventArgs e)
    {
        //AddCard(position, e.Card, deck.CardsCount); // TODO there is no position argument passed so cannot work in this current implementation

        // this if statement should be redundant as both are in-sync, but just for added security (and because i dont trust my code)
        //if (!fetchedCards.ContainsKey(e.Card.Index))
        //{
        //    fetchedCards.Add(e.Card.Index, e.Card);

        //}
    }

    public void Start()
    {
        fetchedCards = new Dictionary<int, CardView>();
        deck = GetComponent<CardStack>();

        ShowCards();
        lastCount = deck.CardsCount;

        deck.CardRemoved += deck_CardRemoved;
        deck.CardAdded += deck_CardAdded;
    }

    private void Update()
    {
        if (lastCount != deck.CardsCount) // if something has changed since last update, else dont bother
        {
            lastCount = deck.CardsCount;
            ShowCards();
        }
        // ShowCards();
    }


    public void Toggle(CardModel card, bool isFaceUp) // TODO to be called to from GameController
    {
        fetchedCards[card.Index].IsFaceUp = isFaceUp; // TODO what is the difference between this and card.showFace
        Debug.Log("YOU SHOULD NOT BE HERE.");
    }

    public void Clear()
    {
        deck.Reset(); // TODO investigate is Reset() being referenced in multiple different locations?

        foreach (CardView view in fetchedCards.Values)
        {
            Destroy(view.Card); // destroys the GameObject itself (not just reassign the dictionarys values)
        }

        fetchedCards.Clear();
    }


    private float NextFloat(float range) // specifcally to make either a plus or minus noise value
    {
        System.Random r = new System.Random((int)DateTime.Now.Ticks); // has to be even more random (using number of ticks as seed) due to this function being being called to within the same loops
        return (float)(r.NextDouble() * (-range - range) + range);
    }

    public void ShowCards()
    {
        if(deck.HasCards) // checks that deck list is populated before continueing
        {
            int cardNo = 0;
            Vector3 position;

            foreach(CardModel i in deck.GetCards())
            {
                
                if (deck.IsDealersHand)
                {

                    // TODO: make all of these statements 2 nested foreach of (position, rotation) & (x, y), i.e.:
                        //foreach (char j in /*transform.postion & rotation*/) { }
                    float[] axis = new float[2]; // an array of two float elements
                    for(int j = 0; j < axis.Length; j++)
                        axis[j] = NextFloat(posNoise);

                    position = startPos + new Vector3(axis[0], axis[1]);
                    //cardCopy.transform.rotation = new Vector3Quatern(r.Next(+rotNoise, -rotNoise), r.Next(-rotNoise, +rotNoise));
                }
                else // if is player's hand
                {
                    float spread = cardOffset * cardNo;
                    //Debug.Log("float spread(" + spread + ") = cardOffset(" + cardOffset + ") * cardNo(" + cardNo);

                    position = startPos + new Vector3(spread, 0f); // only altering the X axis (∴ delta of Y & Z axis = 0f)

                    //cardCopy.transform.position = startPos + new Vector3(cardOffset * cardNo, -1f) // the blackjack way of displaying player's hand

                    //DEBUG:
                    Debug.Log("Should Card " + i.rank + " of " + i.suit + " be showingFace? " + i.ShowFace);
                }

                //i.position = position;
                //deck_CardAdded(i, cardNo);
                AddCard(position, i, cardNo);

                cardNo++;
            }
        }
    }

    void AddCard(Vector3 newPosition, CardModel card, int positionalIndex)
    {
        if (fetchedCards.ContainsKey(card.Index)) // If this card is already in fetchedCards
        {
            if (!card.ShowFace)
            {
                CardModel model = fetchedCards[card.Index].Card.GetComponent<CardModel>(); // what is this doing here?!? Do we still need to do this now that cards is a list of CardModels rather than ints?
                //model.ToggleFace(fetchedCards[card.Index].IsFaceUp); // TODO look into changing to !deck.IsDealersHand OR card.ShowFace
                model.ToggleFace(model.ShowFace); // DEBUG
                print(model.rank + " of " + model.suit + " should now be showing it's face? " + model.ShowFace);

                GameObject cardCopyOG = (GameObject)Instantiate(cardPrefab);
                CardModel cardModel = cardCopyOG.GetComponent<CardModel>(); // what is this doing here?!? Do we still need to do this now that cards is a list of CardModels rather than ints?
                cardModel.Index = card.Index;
                //cardModel.ShowFace = true;
                //cardModel.ToggleFace(card.ShowFace); // TODO change to !deck.IsDealersHand OR card.ShowFace
                cardModel.ToggleFace(true); // DEBUG this doesnt do anything

            }
            // TODO make an else statement for if card.ShowFace == true (is this possible? it should be, right?)
            
            return;
        }
        // else:

        {
            GameObject cardCopy = (GameObject)Instantiate(cardPrefab); // instantiate takes in a GameObject and copies it
            cardCopy.transform.position = newPosition;

            CardModel cardModel = cardCopy.GetComponent<CardModel>(); // what is this doing here?!? Do we still need to do this now that cards is a list of CardModels rather than ints?
            cardModel.Index = card.Index;
            //cardModel.ToggleFace(card.ShowFace); // TODO change to !deck.IsDealersHand OR card.ShowFace
            cardModel.ToggleFace(true); // DEBUG

            SpriteRenderer sp = cardCopy.GetComponent<SpriteRenderer>();
            sp.sortingOrder = 51 - positionalIndex; // reverses the order in which cards are drawn

            fetchedCards.Add(card.Index, new CardView(cardCopy));

            if (cardModel.faces != null)
                card.faces = cardModel.faces; // TODO this is a very temporary fix for all cardModel objects not having a faces Sprite[] for some reason? INVESTIGATE
        }
    }
}
