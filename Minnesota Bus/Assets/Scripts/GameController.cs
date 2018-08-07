using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private CardStack dealersCardStack;
    private CardStack playersCardStack;
    private CardStackView dealersCardStackView;
    private CardStackView playersCardStackView;
    private int round;
    private int cardNo;
    private GameObject resultText;
    private bool? playersChoice;  // is made null at the end of each round, ∴ needs to be a nullable bool
    private CardModel.Suits? playersFinalChoice;  // is made null prior to the final round, ∴ needs to be a nullable bool
    private bool waiting;

    public int Round
    {
        get
        {
            return round;
        }
        set
        {
            Debug.Log("setting round(" + round + ") to " + value + " and cardno to " + (value - 1));
            round = value;
            cardNo = value - 1;
        }
    }
    

    public GameObject dealerObject;
    public GameObject playerObject;

    public GameObject[] rounds;
    public GameObject resultTextParent;

    private void Start()
    {
        SetupGame();
    }

    public void ResetGame()
    {
        Debug.Log("restarting...");
        StopAllCoroutines();
        // set any result text to inactive:
        resultText.SetActive(false);
        //resultTextParent.SetActive(false);

        playersChoice = null;
        playersFinalChoice = null;

        // set all buttons to inactive:
        //rounds[cardNo].SetActive(false);
        foreach (GameObject round in rounds)
        {
            round.SetActive(false);
        }

        // removes all elements of the views & all elements of the cardStack models:
        playersCardStackView.Clear();
        dealersCardStackView.Clear();
        
        SetupGame();
    }

    private void SetupGame()
    {
        playersCardStack = playerObject.GetComponent<CardStack>();
        dealersCardStack = dealerObject.GetComponent<CardStack>();
        playersCardStackView = playerObject.GetComponent<CardStackView>();
        dealersCardStackView = dealerObject.GetComponent<CardStackView>();

        waiting = false;
        Round = 1;  // this setter also means that cardNo == round - 1;

        for (int i = 0; i < playersCardStack.CardsCapacity; i++) 
            playersCardStack.Push(dealersCardStack.Pop());  // the push & pop method should indicate to the cardStacks that they should ShowCards() // TODO Investigate

        // called here in case ResetCards() // TODO Raises NullReferenceExcpetion errors!
        playersCardStackView.ShowCards();
        dealersCardStackView.ShowCards();

        //Debug.Log("player.GetCards: " + player.GetCards());  // TODO this is not how you interact with IEnumerable return types
    }

    private void Update()
    {
        // only update when we know we're not waiting for the result text to finish
        if (!waiting) // TODO this does not seem like the most elegant solution, investigate alternatives
        {
            if (!rounds[cardNo].activeInHierarchy && playersChoice == null && playersFinalChoice == null) // if buttons not active AND player has not chosen
            {
                Debug.Log("ActivateButtons:: Round " + Round + ", buttons now activating!");

                ActivateButtons();
            }
            else if (playersChoice != null || playersFinalChoice != null) // i.e. if PlayersChoice() has been hit
            {
                Debug.Log("Switch-Case:: Round " + Round + ": playersChoice: " + playersChoice + ", playersFinalChoice: " + playersFinalChoice);

                bool? roundJudgement = null;

                switch (Round)
                {
                    case 1:
                        CardModel.Suits cardsSuit = playersCardStack.cards[cardNo].suit;
                        roundJudgement = (cardsSuit == CardModel.Suits.Hearts || cardsSuit == CardModel.Suits.Diamonds);
                        
                        break;
                    case 2:
                        roundJudgement = (playersCardStack.cards[cardNo].rank > playersCardStack.cards[cardNo - 1].rank);

                        break;
                    case 3:
                        CardModel.Ranks currCard = playersCardStack.cards[cardNo].rank;
                        CardModel.Ranks lowCard;
                        CardModel.Ranks highCard;

                        if (playersCardStack.cards[cardNo - 1].rank > playersCardStack.cards[cardNo - 2].rank) // the last card is larger than the card before it
                        {
                            lowCard = playersCardStack.cards[cardNo - 2].rank;
                            highCard = playersCardStack.cards[cardNo - 1].rank;
                        }
                        else // the card before last is either larger than or equal to the last card
                        {
                            lowCard = playersCardStack.cards[cardNo - 1].rank;
                            highCard = playersCardStack.cards[cardNo - 2].rank;
                        }

                        int highCardValue = Array.IndexOf(Enum.GetValues(highCard.GetType()), highCard);
                        int lowCardValue = Array.IndexOf(Enum.GetValues(lowCard.GetType()), lowCard);
                        int currCardValue = Array.IndexOf(Enum.GetValues(currCard.GetType()), currCard);

                        roundJudgement = Enumerable.Range(lowCardValue, highCardValue).Contains(currCardValue);  // TODO Is inclusive or outside? Ask Ander.
                        Debug.Log(highCardValue + " >= " + currCardValue + " >= " + lowCardValue + "? " + roundJudgement);

                        break;
                    case 4:
                        roundJudgement = (playersFinalChoice == playersCardStack.cards[cardNo].suit);
                        break;
                    default:
                        Debug.Log("ERROR: round not 1-4");
                        break;
                }

                if (roundJudgement != null) // then the calculations were successfully completed & roundJudgement was assigned, and now let's see the result
                {
                    RoundResult((bool)roundJudgement);
                }
                else
                    Debug.Log("GameController::Update: ERROR roundJudgement not assigned by end of switch-case!");
            }
        }
    }

    private void ActivateButtons()
    {
        // Hide the previous round's buttons // this is now done when the GetPlayersChoice is hit
        //if (cardNo > 0)
        //    rounds[cardNo - 1].SetActive(false);

        //var fetchedCardsList = playersCardStackView.fetchedCards.Values.ToList();
        //print("fetchedCardsList[cardNo]: " + fetchedCardsList[cardNo]);

        //Vector2 cardPos = new Vector2(player.cards[cardNo].position.x, player.cards[cardNo].position.y);
        if (playersCardStackView.fetchedCards.Count > 0) // ie if there are elements in fetchedCards
        {
            //Debug.Log("Round " + Round + ": ");  // will print out "Round 1" etc

            CardModel currentCard = playersCardStackView.fetchedCards.ElementAt(cardNo).Value.Card.GetComponent<CardModel>();
            print("cardNo: " + cardNo + " = " + currentCard.Name);

            //Vector2 cardPos = new Vector2(currentCard.transform.position.x, currentCard.transform.position.y);

            // TODO: this should be moved to ButtonController, with a function call here instead
            /*int buttonCount = rounds[cardNo].transform.childCount;  // gets the total number of buttons for that round
            int buttonNo = 0;

            foreach (Transform button in rounds[cardNo].transform)
            {
                Debug.Log("Button: " + button);  // prints out what button in that round is currently being used

                float buttonScale = button.localScale.y;
                float buttonXPos = cardPos.x;  // FIX this is where the issue arises!
                float buttonYPos = (cardPos.y - buttonScale) + (buttonNo * buttonScale);

                button.position += new Vector3(buttonXPos, buttonYPos);
                //button.

                buttonNo++;
            }*/

        }
        else // DEBUG:
        {
            print("ERROR: " + cardNo + " not found in fetchedCards");
            foreach (var element in playersCardStackView.fetchedCards)
            {
                print("fetchedCards.element.key : " + element.Key);
            }
            //print("fetchedCards: " + cardStackView.fetchedCards);
        }
        
        rounds[cardNo].SetActive(true);  // show buttons for that round
    }

    // hit for rounds 1-3, where the player's choice can only be 1 of 2 options
    public void GetPlayersChoice(bool _playersChoice)
    {
        playersChoice = _playersChoice;
        Debug.Log("The player's choice was: " + playersChoice);

        rounds[cardNo].SetActive(false);  // hides the buttons, as the player has now made their choice!
    }
    
    // hit for rounds 4, where the player's choice is 1 of 4 ranks
    public void GetPlayersChoice(int _playersFinalChoice)
    {
        int noOfSuits = Enum.GetNames(typeof(CardModel.Suits)).Length;  //( == 4)
        playersFinalChoice = (CardModel.Suits)(_playersFinalChoice % noOfSuits);
           
        Debug.Log("The player's choice was: " + playersFinalChoice);

        rounds[cardNo].SetActive(false);  // hides the buttons, as the player has now made their choice!
    }

    private void /*IEnumerator*/ RoundResult(bool cardBool) // TODO come up with a better var name for this (should not need to use Hungarian Notation)
    {
        Debug.Log("GameController::RoundResult: round: " + round + ", player's choice: " + playersChoice + ", cardBool: " + cardBool + "  ∴: " + (playersChoice == cardBool));

        bool playerIsCorrect = (playersChoice == cardBool || (round > 3) && cardBool);

        if (playerIsCorrect) // Player is correct
        {
            //string drinks;
            //if (Round < 2) // use singular
            //    drinks = "DRINK!";
            //else // use plural
            //    drinks = "DRINKS!";

            //resultText = resultTextParent.transform.Find("Give!").gameObject;
            //resultText.GetComponent<TextMeshProUGUI>().text = "GIVE OUT " + Round + " " + drinks;

            resultText = resultTextParent.transform.Find("Correct!").gameObject;
        }
        else // Player is incorrect
        {
            resultText = resultTextParent.transform.Find("Drink!").gameObject;
        }

        //bool waitingForResults = true;
        StartCoroutine(WaitForResults());  // TODO this is where the issue is arising!

        //while (waitingForResults)
        //    yield return new WaitForSeconds(0.1f);

        if (!playerIsCorrect)
        {
            Debug.Log("RoundResult:: Your answer was wrong, so we're going to reset");
            ResetGame();
        }
    }

    private IEnumerator WaitForResults()
    {
        waiting = true;

        /*float waitDuration =*/ ShowThisRoundsCard();

        // bool waitingOnResultText = true;
        StartCoroutine(ShowResultText(1f));  // as ShowThisRoundsCard returns the wait duration of the card flip

        while (waiting) //waitingOnResultText
            yield return new WaitForSeconds(0.1f);  // waitDuration * 1.5

        Round++;
        playersChoice = null;  // TODO i feel like there's a better place for this to go? maybe if setupRound was actually for setting up the round??
    }

    private float ShowThisRoundsCard()
    {
        CardModel currentCard = playersCardStackView.fetchedCards.ElementAt(cardNo).Value.Card.GetComponent<CardModel>();
            //view.fetchedCards.ElementAt(cardNo).Value.IsFaceUp = true;
            //view.Toggle(player.cards[cardNo], true);

        //cardView.WaitTime *= round;

        // Toggle and show that round's card:
        currentCard.ShowFace = true;  // will, in turn, enact the coroutine Flip

        return currentCard.WaitTime;
    }

    private IEnumerator ShowResultText(float waitTime)
    {
        yield return new WaitForSeconds(waitTime / 2);  // wait, so that the result does not show before
        resultText.SetActive(true);
        yield return new WaitForSeconds(waitTime);  // not (waitTime/2) as once the card is fully revealed, we want the text to remain onscreen for a bit longer
        resultText.SetActive(false);

        waiting = false;
    }
}
