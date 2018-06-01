using System;
using UnityEngine;

public class CardModel : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    private int index; // the number that will be used to find the appropriate card in the faces array
    public int Index
    {
        get
        {
            return this.index;
        }
        set
        {
            this.index = value;
            UpdateVars();
        }
    }

    public Sprite[] faces;
    //public Sprite face; // == null in CardStackViews
    public Sprite back;

    //public Vector3 position;

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
    public CardModel(int _index = 0/*, Sprite _face = null, Sprite _back = null*/) // currently, the only place this is used is in CardStack:CreateDeck()
    {
        Index = _index;

        //face = _face;
        //back = _back;

        //spriteRenderer = GetComponent<SpriteRenderer>();


        // Debug: this is a test to get the faces from the cardPrefab, which already has a faces List of sprites & cardBack sprite
        /*GameObject cardCopy = Instantiate(cardPrefab);
        CardModel cardModel = cardCopy.GetComponent<CardModel>();

        faces = cardModel.faces;
        Debug.Log("[CONSTRUCTOR]: is faces[] null? = " + (faces == null && faces.Length == 0));
        cardBack = cardModel.cardBack;*/
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // GetComponent only works if attached to a GameObject (i.e. not instantiated)
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
        spriteRenderer = GetComponent<SpriteRenderer>(); // GetComponent only works if attached to a GameObject (i.e. not instantiated)

        if (showFace)
        {
            /*Debug.Log("[ToggleFace]: index = " + index);
            Debug.Log("[ToggleFace]: faces[] is null? = " + (faces == null && faces.Length == 0));*/

            if(faces != null || spriteRenderer.sprite != faces[Index])
                spriteRenderer.sprite = faces[Index];
            //else the sprite is already this face

            //spriteRenderer.sprite = face;
        }
        else
            spriteRenderer.sprite = back;
    }
}
