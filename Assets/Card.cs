using System;
using UnityEngine.Networking;

public class Card
{

    public enum Suit
    {
        Diamonds,
        Clubs,
        Hearts,
        Spades
    };

    public enum Value
    {
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
        Joker
    };

    public enum GameTurnState
    {
        ShufflingDeck,
        EmptyDeck,
        DealingCards,
        PlayingPlayerHand,
        TakeTwo,
        TakeThree,
    };

    public class CardMessage : MessageBase
    {
        public NetworkInstanceId playerId;
        public CardId cardId;
    }

    public const short CardMsgId = 1000;

}
