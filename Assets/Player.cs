using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{

	public PlayerPanel playerPanel;


	[SyncVar] 
	public int playerId;

	public List<CardId> handCards = new List<CardId>();

    public override void OnStartServer()
    {
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
	}

	[Server]
	public void ServerAddCard(CardId newCard)
	{
		handCards.Add(newCard);
		RpcAddCard(newCard);
	}

	[ClientRpc]
	void RpcAddCard(CardId newCard)
	{
		if (!isServer)
		{
			// this was already done for host player
			handCards.Add(newCard);
		}

		playerPanel.AddCard(newCard);
	}


	public void MsgAddCard(CardId cardId)
	{
		handCards.Add(cardId);
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


	[Server]
	public void ServerLose(int amount)
	{
		// money was already subtracted
		RpcLose(amount);
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
			c = Color.green;

		playerPanel.GetComponent<PlayerPanel>().ColorImage(c);

		if (isYourTurn && isLocalPlayer)
		{
			CardManager.singleton.EnablePlayHandButtons();
		}
		else
		{
			CardManager.singleton.ClientDisableAllButtons();
		}
	}


	////////// Commands /////////

	[Command]
	public void CmdDobierz()
	{
		if (CardManager.singleton.turnState != Card.GameTurnState.PlayingPlayerHand)
		{
			Debug.LogError("cannot hitme now");
			return;
		}
		if (CardManager.singleton.currentTurnPlayer != this)
		{
			Debug.LogError("not your turn");
			return;
		}

		Debug.Log("CmdDobierz");

		ServerAddCard(CardManager.singleton.GetRandomCard());
	}



}
