using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
public class Player : NetworkBehaviour
{

	public PlayerPanel playerPanel;
    public Deck deck;
    bool isObserver = false;

	[SyncVar] 
	public int playerId;

	public List<CardId> handCards = new List<CardId>();

    public override void OnStartClient()
    {
        playerPanel = CardManager.singleton.playerPanels[playerId];
        //Debug.Log("OnStartClient1");
        playerPanel.gameObject.SetActive(true);
        //Debug.Log("onStartClient2");
    }

    public override void OnStartServer()
    {
        //Debug.Log("SERWER DZIAŁA");
        CardManager.singleton.AddPlayer(this);
    }


    public override void OnNetworkDestroy()
	{
		if (playerPanel != null)
		{
			playerPanel.ClearCards();
			playerPanel.gameObject.SetActive(false);
		}
	}


	public override void OnStartLocalPlayer()
	{
		CardManager.singleton.localPlayer = this;
        //Debug.Log("local player "+ playerId);
    }

	[Server]
	public void ServerAddCard(CardId newCard)
	{
		handCards.Add(newCard);
        //Debug.Log("przydzielam karte " + newCard);
        RpcAddCard(newCard);
	}

	[ClientRpc]
	void RpcAddCard(CardId newCard)
	{
		if (!isServer)
		{
			// this was already done for host player
			//handCards.Add(newCard);
		}

		playerPanel.AddCard(newCard);
	}


	public void MsgAddCard(CardId cardId)
	{
		handCards.Add(cardId);
        //Debug.Log("przydzielam karte " + cardId);

        playerPanel.AddCard(cardId);
	}

	[Server]
	public void ServerClearCards()
	{
        handCards.Clear();
		RpcClearCards();
	}

	[ClientRpc]
	private void RpcClearCards()
	{
		if (!isServer)
		{
			// this was already done for host player
			handCards.Clear();
		}
		playerPanel.ClearCards();
	}

	[Client]
	public void ShowCards()
	{
		var card = handCards[0];
		card.hidden = false;
		handCards[0] = card;

		playerPanel.ShowCard(0);
	}


	[ClientRpc]
	void RpcWin(int amount)
	{
		if (isLocalPlayer)
		{
			var winMsg = "You Won " + amount;
			Debug.Log(winMsg);
			CardManager.singleton.infoText.text = winMsg;
		}
	}

	[ClientRpc]
	void RpcLose(int amount)
	{
		if (isLocalPlayer)
		{
			var lostMsg = "You Lost " + amount;
			Debug.Log(lostMsg);
			CardManager.singleton.infoText.text = lostMsg;
		}
	}

	[ClientRpc]
	public void RpcYourTurn(bool isYourTurn)
	{
		// make player who is having current turn green
		Color c = new Color(1, 1, 1, 0.5f);
		if (isYourTurn)
			c = Color.red;

		playerPanel.GetComponent<PlayerPanel>().ColorImage(c);
        Debug.Log("Powinno dac kolorek");
        
       
        if (isYourTurn && isLocalPlayer)
		{
            //playerPanel.GetCardStatus();
            Debug.Log("MOJA TURA");
            if(!isObserver)
            {
                isObserver = true;
                playerPanel.CreateInstances();
            }
            //CardManager.singleton.EnablePlayHandButtons();
            //playerPanel.CreateInstances();
        }
        else
		{
            Debug.Log("TU WYPIERDALA BUGA");

            //CardManager.singleton.ClientDisableAllButtons();
		}


    }


    ////////// Commands /////////

    [Command]
    public void CmdGetCard()
    {
        if (CardManager.singleton.turnState != Card.GameTurnState.PlayingPlayerHand)
        {
            Debug.LogError("nie mozna klikac teraz");
            return;
        }
        if (CardManager.singleton.currentTurnPlayer != this)
        {
            Debug.LogError("nie twoja tura");
            return;
        }
        Debug.Log("Dziala dobieranie chyba");

        ServerAddCard(CardManager.singleton.GetRandomCard());
        CardManager.singleton.ServerNextPlayer();

    }

    public void CmdConfirm()
    {
        if (CardManager.singleton.turnState != Card.GameTurnState.PlayingPlayerHand)
        {
            Debug.LogError("nie mozna klikac teraz");
            return;
        }
        if (CardManager.singleton.currentTurnPlayer != this)
        {
            Debug.LogError("nie twoja tura");
            return;
        }
        //playerPanel.printCards();
        foreach (var i in playerPanel.CardsToCompare)
        {
            Debug.Log("wcisniete karty na liscie " + i.cardId);
        }

        CardManager.singleton.IsMoveValid(deck.usedCards.Last(), playerPanel.CardsToCompare);

        CardManager.singleton.ServerNextPlayer();

    }


    /*public void Cmdcard1()
    {
        if (CardManager.singleton.turnState != Card.GameTurnState.PlayingPlayerHand)
        {
            Debug.LogError("nie mozna klikac teraz");
            return;
        }
        /*if (CardManager.singleton.currentTurnPlayer != this)
        {
            Debug.LogError("nie twoja tura");
            return;
        }
        if(handCards.Count > 1)
        Debug.Log("karta pierwsza" + handCards[1]);
        */
    //ServerAddCard(CardManager.singleton.GetRandomCard());

}




