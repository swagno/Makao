using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System.Linq;

public class ToDrawSlot : MonoBehaviour
{

    public CardId cardId;
    public Deck deck;

    //zasady
    public void zasady()
    {
        var topCard = deck.usedCards.Last();
        
        if (topCard.value == Card.Value.Two)
        {

        }

        if (topCard.value == Card.Value.Three)
        {

        }

        if (topCard.value == Card.Value.Three)
        {

        }
    }
   
    
}
