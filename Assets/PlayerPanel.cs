using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPanel : MonoBehaviour
{

	public CardSlot[] cardSlots;
	public int nextSlot = 0;
	public Image cardPanelImage;

	private void Awake()
	{
		cardSlots = GetComponentsInChildren<CardSlot>();
		ClearCards();
	}

	void Start()
	{
		//var top = GameObject.FindGameObjectWithTag("tabletop");
		//transform.SetParent(top.transform, true);
	}

	public void ColorImage(Color c)
	{
		cardPanelImage.color = c;
	}

	public void ShowCard(int slotId)
	{
		var cardSlot = cardSlots[slotId];
		cardSlot.cardId.hidden = false;
		cardSlot.GetComponent<Image>().sprite = CardManager.singleton.GetCardSprite(cardSlot.cardId);

	}

	public void AddCard(CardId cardId)
	{
		var cardSlot = cardSlots[nextSlot];
		nextSlot += 1;
		var findCard = cardId;

		cardSlot.GetComponent<Image>().sprite = CardManager.singleton.GetCardSprite(findCard);
		cardSlot.cardId = cardId;
		cardSlot.gameObject.SetActive(true);


	}

	public void ClearCards()
	{
		foreach (var card in cardSlots)
		{
			card.gameObject.SetActive(false);
			card.cardId = new CardId();
		}
		nextSlot = 0;
	}


}
