using System;
using UnityEngine.Networking;

public class Card
{


    public enum Suit
    {
        None = -1,
        Diamonds,
        Clubs,
        Hearts,
        Spades,
        
    };

    public enum Value
    {   
        None = -1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace,
    };

    public enum GameTurnState
    {
        ShufflingDeck,
        DealingCards,
        PlayingPlayerHand,
    };

    public class CardMessage : MessageBase
    {
        public NetworkInstanceId playerId;
        public CardId cardId;
    }

    public const short CardMsgId = 1000;

}
