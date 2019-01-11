using System;
using UnityEngine;
using System.Collections.Generic;
[Serializable]
public class Deck : MonoBehaviour
{
    public Player player;
    public List<CardId> readyCards = new List<CardId>();
    public List<CardId> usedCards = new List<CardId>();

    public void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                readyCards.Add(new CardId((Card.Suit)i, (Card.Value)j, false));
            }
        }
        Shuffle();
    }



    public void Shuffle()
    {
        List<CardId> tmp = new List<CardId>();

        int max = readyCards.Count;
        while (max > 0)
        {
            int offset = UnityEngine.Random.Range(0, max);
            tmp.Add(readyCards[offset]);
            readyCards.RemoveAt(offset);
            max -= 1;
        }
        readyCards = tmp;
    }

    public void ReturnAllCards()
    {
        foreach (var cardId in usedCards)
        {
            readyCards.Add(cardId);
        }
        usedCards.Clear();
    }

    public CardId GetTopCard()
    {
        int top = readyCards.Count - 1;

        var cardId = readyCards[top];
        readyCards.RemoveAt(top);
        //usedCards.Add(cardId);

        //Debug.Log("Got top:" + top + " " + cardId);
        return cardId;
    }


    public void RemoveCard()
    {
        
    }
}