using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerPanel : MonoBehaviour
{

    public CardSlot[] cardSlots;
    
    public int nextSlot = 0;
    public Image cardSlotImage;
    public Player player;
    public Vector3 startPosition;
    //public Dictionary<CardSlot, bool> cardState = new Dictionary<CardSlot, bool>();
    public List<CardSlot> CardsToCompare = new List<CardSlot>();

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
        cardSlotImage.color = c;
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

    public void ClearCardsInPlayerSlot(List<CardSlot> CardsToRemove)
    {
        foreach (var card in CardsToRemove)
        {
            Debug.Log("usunieta karta " + card.cardId);
            card.gameObject.SetActive(false);
            card.cardId = new CardId();
        }
        //nextSlot = 0;
    }

    public void CreateInstances()
    {
        var temp = cardSlots[0].GetComponent<CardSlot>();
        startPosition = temp.transform.position;
        Debug.Log("startPosition: " + startPosition);


        for (int i = 0; i < cardSlots.Length; ++i)
        {

            int j = i;
            Debug.Log("create button with id" + i);
            //Debug.Log("W slocie znajduje się karta " + cardSlots[i].cardId);
            Button button = cardSlots[i].GetComponent<Button>();
            button.onClick.AddListener(() => { OnButtonClicked(j); });


        }
    }
    public void OnButtonClicked(int i)
    {
        //Debug.Log("clicked button with id" + i);
        //Debug.Log("clicked" + cardSlots[i].cardId);

        Vector3 pos1 = cardSlots[i].transform.position;
        Debug.Log("pozycja aktualna" + pos1);

        if (pos1.y == startPosition.y)
        {
            cardSlots[i].transform.Translate(0, 10, 0);
            //cardState[cardSlots[i]] = true;
            CardsToCompare.Add(cardSlots[i]);
            Debug.Log("Pos wysuwanie" + cardSlots[i].transform.position);
        }
        else if(pos1.y - 10.0 == startPosition.y )
        {
            cardSlots[i].transform.Translate(0, -10, 0);
            //cardState[cardSlots[i]] = false;
            CardsToCompare.Remove(cardSlots[i]);
            Debug.Log("Pos wsuwanie" + cardSlots[i].transform.position);
        }
    }

        
    

    /*public void GetCardStatus()//do usuniecia
    {
        cardState.Clear();
        foreach (var card in cardSlots)
        {
            cardState.Add(card, false);
            //Debug.Log("Dodaje do mapy kartę: " + card.cardId);
        }
    }

    public void printCards()
    {
        foreach (var cardsToPlay in CardsToCompare)
        {
            
        }

        //var lastUsedCards = deck.usedCards[deck.usedCards.Count - 1];


        foreach (var state in cardState)
        {
                if (state.Value == true)
                {
                    Debug.Log("Wartosc karty " + state.Key.cardId + " czy wcisniety " + state.Value);
                }
            
        }
    }*/
}


