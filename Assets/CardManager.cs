using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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
 
    [SyncVar]
    public Card.GameTurnState turnState;
    public PlayerPanel[] playerPanels = new PlayerPanel[2];
    public Button buttonDobierz;

    private void Awake()
    {
        singleton = this;
    }

    public override void OnStartClient()
    {
        cardSprites = Resources.LoadAll<Sprite>("cards");
        ClientHandleState(turnState, turnState.ToString());
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

    public void AddPlayer(Player player)
    {
        players.Add(player);

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

    public void ClientDisableAllButtons()
    {
        buttonDobierz.interactable = false;

    }

    [Client]
    void ClientState_PlayHands()
    {
        // these will only be activated when it is the player's turn
        /*buttonStay.gameObject.SetActive(true);
		buttonHitMe.gameObject.SetActive(true);
		buttonDoubleDown.gameObject.SetActive(true);
		buttonSplit.gameObject.SetActive(true);*/
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
        buttonDobierz.interactable = true;
    }
    //GAMEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
    [Client]
    public void ClientState_StartDeck()
    {
        deck.ReturnAllCards();
        deck.Shuffle();

        ServerNextState("DealingCards");
    }

    [Client]
    void ClientState_EmptyDeck()
    {
        deck.Empty();
    }

    [Client]
    void ClientState_DealingCards()
    {
    }
 

    // ------------------------ Server State Functions -------------------------------
    [Server]
    public void ServerState_StartDeck()
    {
        ServerEnterGameState(Card.GameTurnState.ShufflingDeck, "Shuffling");
        deck.ReturnAllCards();
        deck.Shuffle();
        ServerClearHands();

        ServerNextState("Dealing Cards");
    }

    [Server]
    void ServerState_DealingCards()
    {
        ServerEnterGameState(Card.GameTurnState.DealingCards, "Dealing cards");
        int j;
        for (j = 0; j < 6; j++)
        {
            foreach (var player in players)
            {
                ServerDealCard(player, false);
            }
        };

        ServerNextState("ServerState_PlayHands");
    }
    
    [Server]
    void ServerState_PlayHands()
    {
        ServerEnterGameState(Card.GameTurnState.PlayingPlayerHand, "Playing hands");

        // currentPlayerIndex will be incremented in ServerNextPlayer
        currentPlayerIndex = -1;
        ServerNextPlayer();
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
            currentTurnPlayer.RpcYourTurn(false);
        }
        currentPlayerIndex += 1;
        while (currentPlayerIndex < players.Count)
        {
            currentTurnPlayer = players[currentPlayerIndex];
            if (currentTurnPlayer != null)
            {
            }
            currentPlayerIndex += 1;
        }

        if (currentPlayerIndex >= players.Count)
        {
            currentTurnPlayer.RpcYourTurn(false);

            // all players have played their hands
            ServerNextState("ServerState_PlayDealerHand");
        }
    }


    ////// Client UI hooks ////////

    public void UiDobierz()
    {
        localPlayer.CmdDobierz();
    }

}


