using System;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    private int index; // the number that will be used to find the appropriate card in the faces array
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
    //public Sprite face; // == null in CardStackViews
    public Sprite back;
    // DEBUG: TODO make this back to private (only currently used in inspector)
    /*private*/ public bool showFace; // the number that will be used to find the appropriate card in the faces array
    public bool ShowFace
    {
        get
        {
            return showFace;
        }
        set
        {
            if(value != showFace)
            {
                showFace = value;

                if (value)
                {
                    //CardFlipper.FlipCard(faces[index], back, index);
                }
                else
                {
                    //CardFlipper.FlipCard(back, faces[index], index);
                }

                ToggleFace(value);
            }
        }
    }

    private SpriteRenderer spriteRenderer; // why is this here instead of just GetComponent<>() when actually used?
    public GameObject cardPrefab;
    public enum Ranks
    {
        Ace,// = 1, // TODO try commenting out these assignments?
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

    // Constructor
    public CardModel(int _index = 0, bool _showFace = false) // currently, the only place this is used is in CardStack:CreateDeck()
    {
        Index = _index;

        //ShowFace = _showFace; // just for the time being, any cards in the player's hand will be made to show their face eventually...

        //face = _face;
        //back = _back;
    }

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>(); // GetComponent only works if attached to a GameObject (i.e. not instantiated)
        UpdateVars();
    }

    public void UpdateVars()
    {
        int noOfRanks = Enum.GetNames(typeof(Ranks)).Length; //( == 13)

        rank = (Ranks)(Index % noOfRanks);
        //Debug.Log("[UPDATEVARS]: rank == " + rank);

        suit = (Suits)((int)(Math.Floor((double)(Index / noOfRanks))));
        //Debug.Log("[UPDATEVARS]: suit == " + suit);
    }

    public void ToggleFace(bool showFace)
    {
        //showFace = ShowFace;

        spriteRenderer = GetComponent<SpriteRenderer>(); // GetComponent only works if attached to a GameObject (i.e. not instantiated)

        if (showFace)
        {
            if (faces != null || spriteRenderer.sprite != faces[Index])
                spriteRenderer.sprite = faces[Index];
            else
                Debug.Log("WARNING[ToggleFace]: This card's sprite is already it's face!");
        }
        else
            spriteRenderer.sprite = back;
    }
}