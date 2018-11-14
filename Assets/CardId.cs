using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct CardId
{
    public CardId(Card.Suit s, Card.Value v, bool h)
    {
        suit = s;
        value = v;
        hidden = h;
    }

    public Card.Suit suit;
    public Card.Value value;
    public bool hidden;

    public override string ToString()
    {
        return suit.ToString() + ":" + value.ToString();
    }
}


