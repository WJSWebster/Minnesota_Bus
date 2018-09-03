using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private CardStack dealersCardStack;
    private CardStack playersCardStack;
    private CardStackView dealersCardStackView;
    private CardStackView playersCardStackView;
    private int round;
    private int cardNo;
    private int noOfDrinks = 0;
    private GameObject resultText;
    private bool? playersChoice;  // is made null at the end of each round, ∴ needs to be a nullable bool
    private CardModel.Suits? playersFinalChoice;  // is made null prior to the final round, ∴ needs to be a nullable bool
    private bool waiting;
    private AudioManager audioManager;

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
    public GameObject noOfDrinksGameObject;
    public int NoOfDrinks
    {
        get
        {
            return noOfDrinks;
        }

        set
        {
            if (value != noOfDrinks)
            {
                noOfDrinks = value;
                noOfDrinksGameObject.GetComponent<TextMeshProUGUI>().text = "Drinks:\n" + noOfDrinks;
            }
        }
    }
    // TODO: Temp - needs to be moved into a ButtonController along with all the button position calculations
    public GameObject mainCamera;

    private void Start()
    {
        SetupGame();
    }

    public void ResetGame()
    {
        audioManager.Play("Shuffle" + GenRandInt(1, 3));
        //StopAllCoroutines();

        // set any result text to inactive:
        //resultText.SetActive(false);
        //resultTextParent.SetActive(false);
        //DeactivateText();

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

    // Only used for picking 1 of 3 shuffle sound effects
    private int GenRandInt(int min, int max)
    {
        System.Random rand = new System.Random((int)DateTime.Now.Ticks);
        int randNo = rand.Next(min, max + 1);

        //Debug.Log("random number generated from (" + min + " - " + max + "): " + randNo);
        return randNo;
    }

    private void DeactivateText()
    {
        resultText = null;

        resultTextParent.transform.Find("Give!").gameObject.SetActive(false);
        resultTextParent.transform.Find("Correct!").gameObject.SetActive(false);
        resultTextParent.transform.Find("Drink!").gameObject.SetActive(false);
    }

    private void SetupGame()
    {
        audioManager = FindObjectOfType<AudioManager>();

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
            if (Round < 5 && !rounds[cardNo].activeInHierarchy && playersChoice == null && playersFinalChoice == null) // if buttons not active AND player has not chosen
            {
                ActivateButtons();
            }
            else if (playersChoice != null || playersFinalChoice != null) // i.e. if PlayersChoice() has been hit
            {
                bool? roundJudgement = null;

                switch (Round)
                {
                    case 1:
                        CardModel.Suits cardsSuit = playersCardStack.GetCards().ElementAt(cardNo).suit;
                        roundJudgement = (cardsSuit == CardModel.Suits.Hearts || cardsSuit == CardModel.Suits.Diamonds);
                        
                        break;
                    case 2:
                        roundJudgement = (playersCardStack.GetCards().ElementAt(cardNo).rank > playersCardStack.GetCards().ElementAt(cardNo - 1).rank);

                        break;
                    case 3:
                        CardModel.Ranks currCard = playersCardStack.GetCards().ElementAt(cardNo).rank;
                        CardModel.Ranks lowCard;
                        CardModel.Ranks highCard;

                        if (playersCardStack.GetCards().ElementAt(cardNo - 1).rank > playersCardStack.GetCards().ElementAt(cardNo - 2).rank) // the last card is larger than the card before it
                        {
                            lowCard = playersCardStack.GetCards().ElementAt(cardNo - 2).rank;
                            highCard = playersCardStack.GetCards().ElementAt(cardNo - 1).rank;
                        }
                        else // the card before last is either larger than or equal to the last card
                        {
                            lowCard = playersCardStack.GetCards().ElementAt(cardNo - 1).rank;
                            highCard = playersCardStack.GetCards().ElementAt(cardNo - 2).rank;
                        }

                        int highCardValue = Array.IndexOf(Enum.GetValues(highCard.GetType()), highCard);
                        int lowCardValue = Array.IndexOf(Enum.GetValues(lowCard.GetType()), lowCard);
                        int currCardValue = Array.IndexOf(Enum.GetValues(currCard.GetType()), currCard);

                        roundJudgement = Enumerable.Range(lowCardValue, highCardValue).Contains(currCardValue);  // TODO Is inclusive or outside? Ask Ander.
                        Debug.Log(highCardValue + " >= " + currCardValue + " >= " + lowCardValue + "? " + roundJudgement);

                        break;
                    case 4:
                        roundJudgement = (playersFinalChoice == playersCardStack.GetCards().ElementAt(cardNo).suit);

                        break;
                    case 5:
                        Debug.Log("Congrats, you passed The Gauntlet!");
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

                        break;
                    default:
                        Debug.Log("ERROR: round not 1-5");
                        break;
                }

                if (roundJudgement != null) // then the calculations were successfully completed & roundJudgement was assigned, and now let's see the result
                {
                    StartCoroutine(CalculateResult((bool)roundJudgement));
                }
                else
                    Debug.Log("GameController::Update: ERROR roundJudgement not assigned by end of switch-case!");
            }
        }
    }

    private void ActivateButtons()
    {
        // TODO: this should be moved to ButtonController, with a function call here instead

        if (playersCardStackView.fetchedCards.Count > 0) // ie if there are elements in fetchedCards
        {
            CardModel currentCard = playersCardStackView.fetchedCards.ElementAt(cardNo).Value.Card.GetComponent<CardModel>();
            Debug.Log("cardNo: " + cardNo + " = " + currentCard.Name);

            float cardYGap = 30f;
            float cardXGap = cardYGap / 2;

            //Vector2 cardPos = new Vector2(currentCard.transform.position.x, currentCard.transform.position.y);
            Vector2 cardPos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(currentCard.transform.position);

            int buttonCount = rounds[cardNo].transform.childCount;  // gets the total number of buttons for that round
            int buttonNo = 0;

            foreach (Transform button in rounds[cardNo].transform)
            {
                Debug.Log("Button: " + button);  // prints out what button in that round is currently being used
                float buttonHeight = button.GetComponent<RectTransform>().sizeDelta.y + cardYGap;
                float buttonXPos = cardPos.x;
                float buttonYPos = cardPos.y;

                switch (buttonCount)
                {
                    case 2:
                        //buttonXPos = cardPos.x;
                        buttonYPos = (cardPos.y + (buttonHeight / 2)) - (buttonNo * buttonHeight);

                        break;
                    case 4:
                        if (buttonNo == 0 || buttonNo == buttonCount - 1)
                        {
                            int heightMultiplier = (buttonNo == 0) ? buttonNo : 1;
                            buttonYPos = (cardPos.y + (buttonHeight / 2)) - (heightMultiplier * buttonHeight);
                        }
                        else // buttonYPos = cardPos.y
                        {
                            float buttonWidth = button.GetComponent<RectTransform>().sizeDelta.x + cardXGap;

                            buttonXPos = (cardPos.x + (buttonWidth / 2)) - ((buttonNo - 1) * buttonWidth);
                        }

                        break;
                    default:
                        Debug.LogWarning("Unexpected No of Buttons: " + buttonCount);

                        break;
                }
                
                button.position = new Vector3(buttonXPos, buttonYPos);

                buttonNo++;
            }

        }
        else // DEBUG:
        {
            Debug.Log("ERROR: " + cardNo + " not found in fetchedCards");
            foreach (var element in playersCardStackView.fetchedCards)
            {
                Debug.Log("fetchedCards.element.key : " + element.Key);
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

    private /*void */IEnumerator CalculateResult(bool cardBool) // TODO come up with a better var name for this (should not need to use Hungarian Notation)
    {
        Debug.Log("GameController::RoundResult: round: " + round + ", player's choice: " + playersChoice + ", cardBool: " + cardBool + "  ∴: " + (playersChoice == cardBool));
        DeactivateText(); // TODO should this be here?

        // if is round 4, the isPlayerCorrect is cardBool arg, else if round == [1 - 3], playerIsCorrect is if the player's choice == cardbool: 
        bool playerIsCorrect = round > 3 ? cardBool : playersChoice == cardBool;

        if (playerIsCorrect) // Player is correct
        {
            // TODO for multiplayer games:
            //string drinks;
            //if (Round < 2) // use singular
            //    drinks = "DRINK!";
            //else // use plural
            //    drinks = "DRINKS!";

            //resultText = resultTextParent.transform.Find("Give!").gameObject;
            //resultText.GetComponent<TextMeshProUGUI>().text = "GIVE OUT " + Round + " " + drinks;

            resultText = resultTextParent.transform.Find("Correct!").gameObject;
            if(round > 3)
                resultText.GetComponent<TextMeshProUGUI>().text = "CONGRATS, YOU\nPASSED THE\nGAUNTLET!";
        }
        else // Player is incorrect
        {
            resultText = resultTextParent.transform.Find("Drink!").gameObject;
        }

        DisplayResult(!playerIsCorrect);  // TODO this is where the issue is arising!
        StartCoroutine(WaitForDisplayResults());

        Round++;
        playersChoice = null;  // TODO i feel like there's a better place for this to go? maybe if setupRound was actually for setting up the round??
        

        if (!playerIsCorrect)
        {
            //NoOfDrinks++;

            while (waiting)
                yield return new WaitForSeconds(0.1f);
            Debug.Log("RoundResult:: Your answer was wrong, so we're going to reset");
            ResetGame();
        }
    }

    private void DisplayResult(bool wasPlayerWrong)
    {
        waiting = true;

        float waitDuration = ShowThisRoundsCard();

        // bool waitingOnResultText = true;
        StartCoroutine(ShowResultText(waitDuration, wasPlayerWrong));  // as ShowThisRoundsCard returns the wait duration of the card flip

        //while (waiting) //waitingOnResultText
        //    yield return new WaitForSeconds(0.1f);  // waitDuration * 1.5
    }

    private float ShowThisRoundsCard()
    {
        CardModel currentCard = playersCardStackView.fetchedCards.ElementAt(cardNo).Value.Card.GetComponent<CardModel>();
            //view.fetchedCards.ElementAt(cardNo).Value.IsFaceUp = true;
            //view.Toggle(player.cards[cardNo], true);

        //cardView.WaitTime *= round;

        // Toggle and show that round's card:
        currentCard.ShowFace = true;  // will, in turn, enact the coroutine Flip

        // generate random number for card flip sound effect
        int randNumber = GenRandInt(1, 3); // todo: or however many card flip sound effects there are

        // play card flip sound:
        audioManager.Play("Flip" + randNumber);

        return currentCard.WaitTime;
    }

    private IEnumerator ShowResultText(float waitTime, bool playerWasWrong)
    {
        yield return new WaitForSeconds(waitTime / 2);  // wait, so that the result does not show before
        resultText.SetActive(true);

        if (playerWasWrong)
        {
            audioManager.Play("Drink");
            NoOfDrinks++;
        }
        else if (round > 3) // and player was correct
        {
            waitTime *= 3;
        }

        yield return new WaitForSeconds(waitTime);  // not (waitTime/2) as once the card is fully revealed, we want the text to remain onscreen for a bit longer
        DeactivateText();

        waiting = false;
    }

    private IEnumerator WaitForDisplayResults()
    {
        while (waiting)
            yield return new WaitForSeconds(0.1f);
    }
}
