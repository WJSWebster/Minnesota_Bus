using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    List<int> cards;
    
    void Awake()
    {
        if (cards == null)
            cards = new List<int>(); // only instantiate if 
        else
            cards.Clear();

        for (int i = 0; i < 52; i++)
        {
            cards.Add(i);
        }


        Shuffle();
    }

    public void Shuffle()
    {
        int length = cards.Count;

        System.Random r = new System.Random();
        int target;
        int temp;

        for(int i = 0; i < length; i++)
        {
            target = i + r.Next(length - i); ;

            temp = cards[target];
            cards[target] = cards[i];
            cards[i] = temp;
        }
    }

    public IEnumerable<int> GetCards()
    {
        foreach (int i in cards)
        {
            yield return i;
        }
        //return cards;
    }
}
