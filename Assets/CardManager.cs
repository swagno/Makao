using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class CardManager : NetworkBehaviour
{
    public static CardManager singleton;
    public Sprite[] cardSprites;
    public Deck deck;
    public List<Player> players = new List<Player>();
    public Player currentTurnPlayer;
    public int currentPlayerIndex;
    public Player localPlayer;
    public Text infoText;
    public int sizePlayers;
    public PlayerPanel cardsToPlay;
 
    [SyncVar]
    public Card.GameTurnState turnState;
    public PlayerPanel[] playerPanels = new PlayerPanel[2];

    public Button buttonConfirm;
    public Button buttonGetCard;


    private bool nextHandWaiting;
    private void Awake()
    {
        singleton = this;
    }

    public override void OnStartClient()
    {
        cardSprites = Resources.LoadAll<Sprite>("cards");
        //ClientHandleState(turnState, turnState.ToString()); dd

    }

    public Sprite GetCardSprite(Card.Suit suit, Card.Value value)
    {
        int suiteOffset = (int)suit * 14;
        int cardOffset = (int)value;
        int offset = suiteOffset + cardOffset;
        return cardSprites[offset];


    }

    public Sprite GetCardSprite(CardId cardId)
    {
        return GetCardSprite(cardId.suit, cardId.value);

    }

    public CardId GetRandomCard()
    {
        return deck.GetTopCard();
    }

    public CardId RemoveCard()
    {
        return deck.GetTopCard();
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        if ( players.Count < 2)
        {
            Debug.Log(players.Count + " . Czekaj na kolejnego gracza");
            //System.Threading.Thread.Sleep(5000);
            ServerNextState("ServerState_StartDeck");
            return;
        }
        Debug.Log("Marcin " + players.Count);
        Debug.Log("Mamy dwóch graczy");


        // add cards for other existing players
        foreach (var other in players)
        {
            if (other == player)
                continue;

            foreach (var card in other.handCards)
            {
                var msg = new Card.CardMessage();
                msg.playerId = other.netId;
                msg.cardId = card;

                player.connectionToClient.Send(Card.CardMsgId, msg);
            }
        }
    }


    // enters the state immediately
    [Server]
    void ServerEnterGameState(Card.GameTurnState newState, string message)
    {
        Debug.Log("Server Enter state:" + newState);
        turnState = newState;
        RpcGameState(newState, message);
    }

    // will transition to a new state via the function after a delay
    [Server]
    void ServerNextState(string funcName)
    {
        Debug.Log("Server next state:" + funcName);
        Invoke(funcName, 1.0f);
    }

    [ClientRpc]
    private void RpcGameState(Card.GameTurnState newState, string message)
    {
        ClientHandleState(newState, message);
    }

 
    
    // ------------------------ Client State Functions -------------------------------
    [Client]
    void ClientHandleState(Card.GameTurnState newState, string message)
    {
        turnState = newState;
        string msg = "Client TurnState:" + newState + " : " + message;
        infoText.text = message;
        Debug.Log(msg);
        //ClientDisableAllButtons();

        switch (newState)
        {
            case Card.GameTurnState.ShufflingDeck:
                {
                    ClientState_StartDeck();
                    break;
                }

            case Card.GameTurnState.DealingCards:
                {
                    ClientState_DealingCards();
                    break;
                }

            case Card.GameTurnState.PlayingPlayerHand:
                {
                    ClientState_PlayHands();
                    break;
                }


        }
    }

    [Client]
    public void EnablePlayHandButtons()
    {
        buttonGetCard.interactable = true;
        buttonConfirm.interactable = true;

    }

    [Client]
    public void ClientDisableAllButtons()
    {
        buttonConfirm.interactable = false;
        buttonGetCard.interactable = false;
        
        
    }

 

    //GAMEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
    [Client]
    public void ClientState_StartDeck()
    {
            deck.ReturnAllCards();
           // deck.Shuffle();
    }

    /*[Client]
    void ClientState_EmptyDeck()
    {
        deck.Empty();
    }*/

    [Client]
    void ClientState_DealingCards()
    {
        //Debug.Log("serverState powinien dilować");
    }

    [Client]
    void ClientState_PlayHands()
    {
        //Debug.Log("teraz się moge ruszać");
    }


    // ------------------------ Server State Functions -------------------------------
    [Server]
    public void ServerState_StartDeck()
    {
            ServerEnterGameState(Card.GameTurnState.ShufflingDeck, "Shuffling");
            Debug.Log("serverState zwraca karty");
            deck.ReturnAllCards();
            Debug.Log("serverState szufluje");
            deck.Shuffle();
            ServerClearHands();
            Debug.Log("serverState czysci karty");
            ServerNextState("ServerState_DealingCards");
    }

    [Server]
    void ServerState_DealingCards()
    {
        ServerEnterGameState(Card.GameTurnState.DealingCards, "Dealing cards");

        foreach (var player in players) 
        {
            for (int j = 0; j < 5; j++)
            {
                ServerDealCard(player, false);
            }
        }
        Debug.Log("serverState teraz rozdaje karty");

        ServerNextState("ServerState_PlayHands");
    }
    
    [Server]
    void ServerState_PlayHands()
    {
        ServerEnterGameState(Card.GameTurnState.PlayingPlayerHand, "Playing hands");
        if (isLocalPlayer)
        {
            Debug.Log("local player");
        }
        // currentPlayerIndex will be incremented in ServerNextPlayer
        currentPlayerIndex = -1;
        Debug.Log("tura gracza " + currentPlayerIndex);
        Debug.Log("ilosc graczy " + players.Count);

        //players[0].playerPanel.CreateInstances();
        // players[1].playerPanel.CreateInstances();
        ServerNextPlayer();
        
        Debug.Log("jest po serwer next player");

    }

    // ---- server player actions -----


    [Server]
    void ServerDealCard(Player player, bool hideCard)
    {
        
        var newCard = deck.GetTopCard();
        newCard.hidden = hideCard;
        player.ServerAddCard(newCard);
    }

    [Server]
    void ServerClearHands()
    {
        foreach (var player in players)
        {
            player.ServerClearCards();
        }
        currentTurnPlayer = null;
    }

    [Server]
    public void ServerNextPlayer()
    {


         if (currentTurnPlayer != null)
         {
             Debug.Log("servernextplayer1 "+ currentPlayerIndex);
             currentTurnPlayer.RpcYourTurn(false);
             if( currentTurnPlayer.handCards.Count == 0)
             {
                 Debug.Log("wygrał gracz " + currentPlayerIndex);
             }
         }
         else
         {
             players[1].RpcYourTurn(false);
         }
         Debug.Log("servernextplayer12 " + currentPlayerIndex);
         currentPlayerIndex += 1;
         currentTurnPlayer = players[currentPlayerIndex % (players.Count)];
         Debug.Log("servernextplayer13 " + currentPlayerIndex % (players.Count));
         currentTurnPlayer.RpcYourTurn(true);

         /*while (currentPlayerIndex < players.Count)
         {
             Debug.Log("servernextplayer2 " + currentPlayerIndex%(players.Count));
             currentTurnPlayer = players[currentPlayerIndex % (players.Count)];
             if (currentTurnPlayer != null)
             {
                 currentTurnPlayer.RpcYourTurn(true);
                 Debug.Log("tura pierwsza");
             }
              currentPlayerIndex += 1;
         }*/

    }

    //ZASADY
    public bool IsMoveValid(CardId topCard, List<CardSlot> pickedCards)
    {
        if (pickedCards.Count == 2 || pickedCards.Count == 0)
        {
            Debug.Log("nie mozna wyrzucic");
            return false;
        }

        var firstValue = pickedCards[0].cardId.value;
        if (pickedCards.Any(card => card.cardId.value != firstValue))
        {
            Debug.Log("nie mozna wyrzucic różne wartości");
            return false;
        }

        if (topCard.value == pickedCards[0].cardId.value || topCard.suit == pickedCards[0].cardId.suit)
        {
            return true;
        }
        return false;
    }

    ////// Client UI hooks ////////

    /*public void UiConfirm()
    {
        localPlayer.CmdPlaceBet();
    }*/

    public void UiGetCard()
    {
        localPlayer.CmdGetCard();
    }

    public void UiConfirm()
    {
        localPlayer.CmdConfirm();
    }

    /*public void Uicard1()
    {
        localPlayer.Cmdcard1();
    }
    */




}


