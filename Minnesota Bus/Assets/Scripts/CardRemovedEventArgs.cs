using System;

public class CardRemovedEventArgs : EventArgs
{
    public int CardIndex { get; private set; }

    // Constructor
    public CardRemovedEventArgs(int cardIndex) // cardIndex is not currently an index, but a cardModel
    {
        CardIndex = cardIndex;
    }
}