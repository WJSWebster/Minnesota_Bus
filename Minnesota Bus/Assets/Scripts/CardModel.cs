using System;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    private int index;  // the number that will be used to find the appropriate card in the faces array
    private bool showFace;  // the number that will be used to find the appropriate card in the faces array
    private SpriteRenderer spriteRenderer;  // why is this here instead of just GetComponent<>() when actually used?
    private CardFlipper cardFlipper;

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
            UpdateVars();
        }
    }
    public Sprite[] faces;
    public Sprite back;

    public bool ShowFace
    {
        get
        {
            return showFace;
        }

        set
        {
            if(value != showFace) // if the value has changed
            {
                showFace = value;

                if (value) // if changing ShowFace from false to true:
                {
                    StartCoroutine(cardFlipper.Flip(back, faces[index], WaitTime));
                }
                else // if changing ShowFace from true to false:
                {
                    StartCoroutine(cardFlipper.Flip(faces[index], back, WaitTime));
                }

                //ToggleFace();
            }
            else
            {
                Debug.Log(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + Name + "'s ShowFace value is already " + value);
            }
        }
    }

    //public GameObject cardPrefab;  // TODO remove
    public enum Ranks
    {
        Ace,// = 1,
        Two,// = 2,
        Three,// = 3,
        Four,// = 4,
        Five,// = 5,
        Six,// = 6,
        Seven,// = 7,
        Eight,// = 8,
        Nine,// = 9,
        Ten,// = 10,
        Jack,// = 11,
        Queen,// = 12,
        King,// = 13
    };
    public Ranks rank;

    public enum Suits
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    };
    public Suits suit;

    // a simple getter method, mostly used for debug logs
    public string Name
    {
        get
        {
            return rank + " of " + suit;
        }
    }

    public float WaitTime { get; set; }

    // Constructor
    public CardModel(int _index = 0) // currently, the only place this is used is in CardStack:CreateDeck()
    {
        Index = _index;  // in turn, results in a call to UpdateVars()
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // GetComponent only works if attached to a GameObject (i.e. not instantiated)
        cardFlipper = GetComponent<CardFlipper>();
        WaitTime = 1f;

        UpdateVars();
    }

    public void UpdateVars()
    {
        int noOfRanks = Enum.GetNames(typeof(Ranks)).Length;  //( == 13)

        rank = (Ranks)(Index % noOfRanks);
        suit = (Suits)((int)(Math.Floor((double)(Index / noOfRanks))));
    }
    

    public void /*IEnumerator*/ ToggleFace()
    {
        //yield return new WaitForSeconds(1f);

        if (ShowFace)
        {
            if (faces != null || spriteRenderer.sprite != faces[Index])
            {
                spriteRenderer.sprite = faces[Index];
                Debug.Log("CardModel::ToggleFace: " + Name + " just changed ShowFace to: " + ShowFace);
            }
            else
            {
                //Debug.Log("WARNING[ToggleFace]: " + Name + "'s sprite is already it's face!");
            }
        }
        else
        {
            if (back != null || spriteRenderer.sprite != back)
            {
                spriteRenderer.sprite = back;
                //Debug.Log("CardModel::ToggleFace: " + Name + " just changed ShowFace to: " + ShowFace + " ...why?");
            }
            else
            {
                //Debug.Log("WARNING[ToggleFace]: " + Name + "'s sprite is already it's back!");
            }
        }
    }
}