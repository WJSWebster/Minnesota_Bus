using UnityEngine;

// this class contains both the card object and a boolean value for whether or not the face is shown
// TODO why not just move isFaceUp into the CardModel, such as ShowFace?
public class CardView
{
    public GameObject Card { get; private set; }
    public bool IsFaceUp { get; set; } // TODO maybe rename this to something like showFace?

    public CardView(GameObject card)
    {
        Card = card;
        IsFaceUp = false;
    }
}
