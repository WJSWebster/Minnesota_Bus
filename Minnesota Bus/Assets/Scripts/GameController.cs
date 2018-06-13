using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int round;
    private int cardNo;
    private GameObject resultText;
    private bool? playersChoice; // is made null at the end of each round, ∴ needs to be a nullable bool

    public int Round
    {
        get
        {
            return round;
        }
        set
        {
            round = value;
            cardNo = value - 1;
        }
    }
    public CardStack dealer;
    public CardStack player;
    public GameObject[] rounds;
    public GameObject resultTextParent;
    private bool waiting;

    private void Start()
    {
        SetupGame();
    }

    public void ResetGame()
    {
        // set any result text to inactive:
        //resultText.SetActive(false);
        resultTextParent.SetActive(false);

        // set all buttons to inactive:
        //rounds[cardNo].SetActive(false);
        foreach (GameObject round in rounds)
        {
            round.SetActive(false);
        }

        // removes all elements of the views & all elements of the cardStack models:
        player.GetComponent<CardStackView>().Clear();
        dealer.GetComponent<CardStackView>().Clear();
        
        SetupGame();
    }

    private void SetupGame()
    {
        waiting = false;
        Round = 1; // this setter also means that cardNo == round - 1;

        print("player.CardsCapacity: " + player.CardsCapacity); // doesnt work here?!

        for (int i = 0; i < player.CardsCapacity; i++) 
            player.Push(dealer.Pop()); // the push & pop method should indicate to the cardStacks that they should ShowCards() // TODO Investigate

        // called here in case ResetCards()
        player.GetComponent<CardStackView>().ShowCards();
        dealer.GetComponent<CardStackView>().ShowCards();
        //RoundSetup();
    }

    //private void RoundSetup()
    private void Update()
    {
        // only update when we know we're not waiting for the result text to finish
        if (!waiting) // TODO this does not seem like the most elegant solution, investigate alternatives
        {
            if (!rounds[cardNo].activeInHierarchy && playersChoice == null) // if buttons not active AND player has not chosen
                ActivateButtons();
            else if (playersChoice != null) // i.e. if PlayersChoice() has been hit
            {
                switch (Round)
                {
                    case 1:
                        CardModel.Suits cardsSuit = player.cards[cardNo].suit;
                        bool isCardRed = (cardsSuit == CardModel.Suits.Hearts || cardsSuit == CardModel.Suits.Diamonds);
                        RoundResult(isCardRed); // TODO move this out of the switch-case statement

                        break;
                    case 2:
                        bool isCardHigher = (player.cards[cardNo].rank > player.cards[cardNo - 1].rank);
                        RoundResult(isCardHigher);

                        break;
                    case 3:
                        CardModel.Ranks currCard = player.cards[cardNo].rank;
                        CardModel.Ranks lowCard;
                        CardModel.Ranks highCard;

                        if (player.cards[cardNo - 1].rank > player.cards[cardNo - 2].rank) // the last card is larger than the card before it
                        {
                            lowCard = player.cards[cardNo - 2].rank;
                            highCard = player.cards[cardNo - 1].rank;
                        }
                        else // the card before last is either larger than or equal to the last card
                        {
                            lowCard = player.cards[cardNo - 1].rank;
                            highCard = player.cards[cardNo - 2].rank;
                        }

                        int highCardValue = Array.IndexOf(Enum.GetValues(highCard.GetType()), highCard);
                        int lowCardValue = Array.IndexOf(Enum.GetValues(lowCard.GetType()), lowCard);
                        int currCardValue = Array.IndexOf(Enum.GetValues(currCard.GetType()), currCard);

                        bool isCardInside = Enumerable.Range(lowCardValue, highCardValue).Contains(currCardValue); // TODO Is inclusive or outside? Ask Ander.
                        print(highCardValue + " >= " + currCardValue + " >= " + lowCardValue + "? " + isCardInside);
                        RoundResult(isCardInside);

                        break;
                    case 4:


                        break;
                    default:
                        Debug.Log("ERROR: round not 1-4");

                        break;
                }
            }
        }
    }

    private void ActivateButtons()
    {
        CardStackView view = player.GetComponent<CardStackView>();

        // Hide the previous round's buttons // this is now done when the GetPlayersChoice is hit
        //if (cardNo > 0)
        //    rounds[cardNo - 1].SetActive(false);

        //var fetchedCardsList = cardStackView.fetchedCards.Values.ToList();
        //print("fetchedCardsList[cardNo]: " + fetchedCardsList[cardNo]);

        //Vector2 cardPos = new Vector2(player.cards[cardNo].position.x, player.cards[cardNo].position.y);
        if (view.fetchedCards.Count > 0) // ie if there are elements in fetchedCards // for some reason this always fails TODO investigate
        {
            Debug.Log(rounds[cardNo] + ": "); // will print out "Round 1" etc

            CardModel cardView = view.fetchedCards.ElementAt(cardNo).Value.Card.GetComponent<CardModel>();
            //Debug.Log(" ");
            //print("cardNo: " + cardNo + " = " + cardView);
            //Debug.Log(" ");

            Vector2 cardPos = new Vector2(cardView.transform.position.x, cardView.transform.position.y);

            int buttonCount = rounds[cardNo].transform.childCount; // gets the total number of buttons for that round
            int buttonNo = 0;

            /*foreach (Transform button in rounds[cardNo].transform)
            {
                Debug.Log("Button: " + button); // prints out what button in that round is currently being used

                float buttonScale = button.localScale.y;
                float buttonXPos = cardPos.x; // FIX this is where the issue arises!
                float buttonYPos = (cardPos.y - buttonScale) + (buttonNo * buttonScale);

                button.position += new Vector3(buttonXPos, buttonYPos);
                //button.

                buttonNo++;
            }*/

        }
        else // DEBUG:
        {
            print("ERROR: " + cardNo + " not found in fetchedCards");
            foreach (var element in view.fetchedCards)
            {
                print("fetchedCards.element.key : " + element.Key);
            }
            //print("fetchedCards: " + cardStackView.fetchedCards);
        }
        
        rounds[cardNo].SetActive(true); // show buttons for that round
    }

    // hit for rounds 1-3, where the player's choice can only be 1 of 2 options
    public void GetPlayersChoice(bool _playersChoice)
    {
        playersChoice = _playersChoice;
        Debug.Log("The player's choice was: " + playersChoice);

        rounds[cardNo].SetActive(false); // hides the buttons, as the player has now made their choice!
    }

    private void /*IEnumerator*/ RoundResult(bool cardBool) // TODO come up with a better var name for this (should not need to use Hungarian Notation)
    {
        ShowThisRoundsCard();

        string drinks;
        if (Round < 2) // use singular
            drinks = "DRINK!";
        else // use plural
            drinks = "DRINKS!";

        if (playersChoice == cardBool) // Player is correct
        {
            resultText = resultTextParent.transform.Find("Give!").gameObject;
            resultText.GetComponent<TextMeshProUGUI>().text = "GIVE OUT " + Round + " " + drinks;
        }
        else // Player is incorrect
        {
            resultText = resultTextParent.transform.Find("Drink!").gameObject;

            // TODO if gameMode = "Gauntlett"
            // waitforseconds(X);
            // Restart();
            // return;
        }

        ShowResultText(true);
        //TODO make the ShowResult method work this way, i.e. using a WaitForSeconds(X) rather than needing a button press

        StartCoroutine(Wait(1.5f));
    }

    // VERY VERY DEBUG ATM, TODO CLEAN THIS UP
    private void ShowThisRoundsCard()
    {
        // cut and pasted over from GetPlayersChoice
        //cardStackView.fetchedCards.ElementAt(cardNo).Value.IsFaceUp = true;
        //cardStackView.ShowCards();

        player.cards[cardNo].ShowFace = true;

        // Toggle and show that round's card:
        CardStackView view = player.GetComponent<CardStackView>();
        view.fetchedCards.ElementAt(cardNo).Value.IsFaceUp = true;
        view.Toggle(player.cards[cardNo], true);

        //cardStackView.ShowCards();
        view.ShowCards();
    }

    /*private*/
    IEnumerator Wait(float waitTime)
    {
        waiting = true;

        yield return new WaitForSeconds(waitTime);

        waiting = false;

        ShowResultText(false);
    }

    public void ShowResultText(bool isShow)
    {
        resultText.SetActive(isShow);

        if (!isShow) // i.e. if ShowResult method was hit by the Done! button
        {
            Round++;
            //rounds[cardNo].SetActive(false);
            playersChoice = null; // TODO i feel like there's a better place for this to go? maybe if setupRound was actually for setting up the round??
        }
    }
}
