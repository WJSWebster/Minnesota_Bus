using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour
{
    private CardStack deck;
    private int lastCount; // INVESTIGATE what does this do?

    public Dictionary<int, GameObject> fetchedCards; // a list of only the card's indexes
    public Vector3 startPos;
    public float cardOffset;
    public GameObject cardPrefab;

    public float posNoise;
    public float rotNoise; // TODO make this actually used (see below)

    public void Start()
    {
        fetchedCards = new Dictionary<int, GameObject>();
        deck = GetComponent<CardStack>();

        ShowCards();
        lastCount = deck.CardsCount;

        deck.CardRemoved += deck_CardRemoved;
    }

    // Event Handler
    private void deck_CardRemoved(object sender, CardRemovedEventArgs e)
    {
        // this if statement should be redundanta as both are in-sync, but just for added security (and because i dont trust my code)
        if (fetchedCards.ContainsKey(e.CardIndex))
        {
            Destroy(fetchedCards[e.CardIndex]); // destroys the actual value
            fetchedCards.Remove(e.CardIndex); // removes it from the fetchedCards dictionary
        }
    }

    private void Update()
    {
        if(lastCount != deck.CardsCount)
        {
            lastCount = deck.CardsCount;
            ShowCards();
        }
    }

    float NextFloat(float range) // specifcally to make either a plus or minus noise value
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
                }

                //i.position = position;  // TODO find out how to grab .transform.position from Debug Dealer

                Sprite[] tempFaces = AddCard(position, i.Index, cardNo);
                if (tempFaces != null) // TODO this is a very temporary fix for all cardModel objects not having a faces Sprite[] for some reason? INVESTIGATE
                    i.faces = tempFaces;

                cardNo++;
            }
        }
    }

    Sprite[] AddCard(Vector3 position, int cardIndex, int positionalIndex)
    {
        if (fetchedCards.ContainsKey(cardIndex)) // If this card is already in fetchedCards
            return null;

        GameObject cardCopy = (GameObject)Instantiate(cardPrefab); // instantiate takes in a GameObject and copies it
        cardCopy.transform.position = position; 

        CardModel cardModel = cardCopy.GetComponent<CardModel>(); // what is this doing here?!? Do we still need to do this now that cards is a list of CardModels rather than ints?
        cardModel.Index = cardIndex;
        cardModel.ToggleFace(true); // TODO change to !deck.IsDealersHand

        SpriteRenderer sp = cardCopy.GetComponent<SpriteRenderer>();
        sp.sortingOrder = 51 - positionalIndex; // reverses the order in which cards are drawn

        fetchedCards.Add(cardIndex, cardCopy);

        return cardModel.faces;
    }
}
