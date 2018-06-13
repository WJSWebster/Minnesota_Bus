using System;

public class CardEventArgs : EventArgs
{
    public CardModel Card { get; private set; }

    // Constructor
    public CardEventArgs(CardModel card) // cardIndex is not currently an index, but a cardModel
    {
        Card = card;
    }
}