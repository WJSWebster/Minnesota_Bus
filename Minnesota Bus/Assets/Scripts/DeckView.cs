using UnityEngine;

[RequireComponent(typeof(Deck))]
public class DeckView : MonoBehaviour 
{
    #region Variables
    private Deck deck;

    public Vector3 start;
    public float cardOffset;
    public GameObject cardPrefab;
    #endregion

    #region Methods
    void Start()
    {
        deck = GetComponent<Deck>();

        ShowCards();
    }

    void ShowCards()
    {
        int cardNo = 0;

        foreach (int i in deck.GetCards()) // TODO why not just make cardCount i?
        {
            float spread = cardOffset * cardNo;
            Debug.Log("float spread(" + spread + ") = cardOffset(" + cardOffset + ") * cardNo(" + cardNo);
       
            GameObject cardCopy = (GameObject)Instantiate(cardPrefab); // instantiate takes in a GameObject and copies it
            Vector3 temp = start + new Vector3(spread, 0f); // only altering the X axis (therefore delta of Y & Z axis = 0f)
            cardCopy.transform.position = temp;

            CardModel cardModel = cardCopy.GetComponent<CardModel>();
            cardModel.cardIndex = i;
            cardModel.ToggleFace(true);

            cardNo++;
        }
    }
    #endregion
}
