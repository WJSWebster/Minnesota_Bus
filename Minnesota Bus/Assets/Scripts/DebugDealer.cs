using System;
using UnityEngine;

public class DebugDealer : MonoBehaviour
{
    private int round;
    private int cardNo;
    public CardStack dealer;
    public CardStack player;
    //public CardStackView stackView; // There's currently no uses of this?? INVESTIGATE
    public GameObject busMeButton;
    //Round 1
    private bool isChoiceRed;
    private bool isCardRed;
    //Round 2
    private bool isChoiceHigher;
    private bool isCardHigher;
    //Round 3
    private bool isChoiceIn;
    private bool isCardIn;
    //Round 4?
        //public enum Suits
        //{
        //    Hearts,
        //    Diamonds,
        //    Clubs,
        //    Spades
        //};
        //public Suits suit;
    public GameObject[] rounds;
    public CardStackView cardStackView;

    public void HitMe() // TODO: this should actually be done 4 times when player CardStack is first made
    { // Think of this method as the player's cursor, for now:

        //Debug.Log("player.Count = " + player.GetCards().Count);

        round = player.CardsCount;
        cardNo = round - 1;

        print("player.CardsCount: " + player.CardsCount);
        print("player.CardsCapacity: " + player.CardsCount);

        if (player.CardsCount < player.CardsCapacity) // because we have already declared that the player cards list cannot have more than 4 cards in it
        {
            player.Push(dealer.Pop()); // takes a card from the dealer's list, and places it at the front of the player's

            if (player.CardsCount == player.CardsCapacity)
                busMeButton.SetActive(true);
        }
        else
            Debug.Log("Cannot Hit, Player has max number of cards in his deck!");

        RoundSetup();
    }

    private void RoundSetup()
    {
        round = player.CardsCount;
        cardNo = round - 1;

        ActivateButtons();

        switch (round)
        {
            case 1:
                //round1.SetActive(true);
                CardModel.Suits cardsSuit = player.cards[cardNo].suit;
                //print("player's card suit == " + player.cards[cardNo].suit);
                isCardRed = (cardsSuit == CardModel.Suits.Hearts || cardsSuit == CardModel.Suits.Diamonds);
                print("isCardRed? " + isCardRed);
                break;
            case 2:
                //round1.SetActive(false);
                print("Round 2: cardNo = " + cardNo + " (should be more than 0)");
                player.cards[cardNo - 1].ToggleFace(true);

                //round2.SetActive(true);
                break;
            case 3:
                //round2.SetActive(false);
                player.cards[cardNo - 1].ToggleFace(true);

                //round3.SetActive(true);
                break;
            case 4:
                //round3.SetActive(false);
                player.cards[cardNo - 1].ToggleFace(true);

                //round4.SetActive(true);
                break;
            default:
                //round1.SetActive(false);
                //round2.SetActive(false);
                //round3.SetActive(false);
                //round4.SetActive(false);
                player.cards[cardNo - 1].ToggleFace(true);

                break;
        }
    }

    private void ActivateButtons()
    {
        // Hide the previous round's buttons
        if (cardNo > 0)
            rounds[cardNo - 1].SetActive(false);

        //Vector2 cardPos = new Vector2(player.cards[cardNo].position.x, player.cards[cardNo].position.y);
        if (cardStackView.fetchedCards.ContainsKey(cardNo))
        {
            GameObject cardView = cardStackView.fetchedCards[cardNo];

            Vector2 cardPos = new Vector2(cardView.transform.position.x, cardView.transform.position.y);

            int buttonCount = rounds[cardNo].transform.childCount;
            int buttonNo = 0;

            Debug.Log(rounds[cardNo] + ": "); // will print out "Round 1" etc

            foreach (Transform button in rounds[cardNo].transform)
            {
                Debug.Log("Transform: " + button); // prints out what button in that round is currently being used

                float buttonScale = button.localScale.y;
                float buttonXPos = cardPos.x; // FIX this is where the issue arises!
                float buttonYPos = (cardPos.y - buttonScale) + (buttonNo * buttonScale);

                button.position += new Vector3(buttonXPos, buttonYPos);
                //button.

                buttonNo++;
            }

        }
        else
        {
            print("ERROR: " + cardNo + " not found in fetchedCards");
            foreach (var element in cardStackView.fetchedCards)
            {
                print("fetchedCards.element.key : " + element.Key);
            }
            //print("fetchedCards: " + cardStackView.fetchedCards);
        }
        
        //for (int i = 0; i < buttonCount; i++)
        //rounds[cardNo].transform.GetChild(i).transform.position.y = player.cards[cardNo].transform.position.y * i;


        
        rounds[cardNo].SetActive(true); // show buttons for that round
    }

    public void GetPlayersChoice(bool redHighIn)
    {
        isChoiceRed = redHighIn;
        Debug.Log("The player's choice was: " + redHighIn);
    }


    //TODO implement!
    //public void BusMe() // TODO: have button OnClick this behaviour instead once properly implemented
    //{
    //    int round = player.CardsCount; // effectively gets the round number it currently is (by getting how many cards are currently in player's deck)

    //    switch (round)
    //    {
    //        case 0: // ROUND 1: 
    //            bool isChoiceRed = true;// TODO get player's choice as value for this
    //            bool isCardRed = (player.cards[round].suit <= 1); // only cards with an index of 25 or less are Red!
    //            // TODO is this the correct use of the Suit enum? is there not a better way to be using this?

    //            if (isChoiceRed == isCardRed)
    //                Debug.Log("Correct! Hand out " + (round + 1) + " drink(s)!");
    //                // TODO replace with on screen text, similiar to the "You Win!" in Roll-a-Ball
    //            else
    //                Debug.Log("Wrong! DDDRRRRIIIINNKKK "+ (round + 1) + " drink(s)!!!");
    //                // TODO " "
    //            break;

    //        case 1:
    //            bool isChoiceHigher = ;// TODO get player's choice as value for this
    //            bool isCardHigher = (player.cards[round].rank > player.cards[round - 1].rank);

    //            if (isChoiceHigher == isCardHigher)
    //                Debug.Log("Correct! Hand out " + (round + 1) + " drink(s)!");
    //                // TODO replace with on screen text, similiar to the "You Win!" in Roll-a-Ball
    //            else
    //                Debug.Log("Wrong! DDDRRRRIIIINNKKK " + (round + 1) + " drink(s)!!!");
    //                // TODO " "
    //            break;

    //        case 2:
    //            int lowCard;
    //            int highCard;

    //            if (player.cards[round - 1].rank > player.cards[round - 2].rank)
    //            {
    //                lowCard = player.cards[round - 2];
    //                highCard = player.cards[round - 1];
    //            }
    //            else // the previous card 
    //            {
    //                lowCard = player.cards[round - 1];
    //                highCard = player.cards[round - 2];
    //            }


    //            bool isChoiceOutside = ;// TODO get player's choice as value for this
    //            bool isCardOutside = (lowCard.rank > player.cards[round].rank > highCard.rank); // TODO Is inclusive or outside? Ask Ander.
    //            bool isCardOutside = System.Linq.Enumerable.Range();

    //            if (isChoiceHigher == isCardHigher)
    //                Debug.Log("Correct! Hand out " + (round + 1) + " drink(s)!");
    //            // TODO replace with on screen text, similiar to the "You Win!" in Roll-a-Ball
    //            else
    //                Debug.Log("Wrong! DDDRRRRIIIINNKKK " + (round + 1) + " drink(s)!!!");
    //            // TODO " "
    //            break;

    //        case 3:
    //            break;

    //        default:
    //            Debug.Log("ERROR: What round is it?!!");
    //            break;
    //    }
    //}
}
