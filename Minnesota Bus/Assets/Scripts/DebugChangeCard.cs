using UnityEngine;

public class DebugChangeCard : MonoBehaviour {

    private CardFlipper flipper;
    private CardModel cardModel;
    private int cardIndex = 0;

    public GameObject card;

    void Awake()
    {
        cardModel = card.GetComponent<CardModel>();
        flipper = card.GetComponent<CardFlipper>();
    }

    public void Hit()
    {
        if (cardIndex >= cardModel.faces.Length) // final card face transitioning to the card back
        {
            cardIndex = 0;
            //cardModel.ToggleFace(false);

            // note: cardModel.faces.Length - 1 == 51
            flipper.FlipCard(cardModel.faces[cardModel.faces.Length - 1], cardModel.cardBack, -1);
        }
        else
        {
            if (cardIndex > 0)
                flipper.FlipCard(cardModel.faces[cardIndex - 1], cardModel.faces[cardIndex], cardIndex);
            else // card back transitioning to first card face in array
                flipper.FlipCard(cardModel.cardBack, cardModel.faces[cardIndex], cardIndex);

            //cardModel.cardIndex = cardIndex;
            //cardModel.ToggleFace(true);
            cardIndex++;
        }        
    }
}
