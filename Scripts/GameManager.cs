using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Space]
    [Tooltip("Prefab of the card")] public Card card_prefab;
    [Space]
    [Tooltip("Visual representation of the deck")]
    [SerializeField] private List<Card> deck;
    [Space]
    [Tooltip("Speed for drawing the cards")] public float drawSpeed = 1;
    public Transform card1Pos, card2Pos, winnerPos, loserPos;
    public Text drawButton;

    private const float distanceFromCamera = 17.5f;
    private bool running = false;

    private void Start()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .2f, distanceFromCamera));//Ensures Deck position is always bottom left corner.
        card1Pos.position = Camera.main.ViewportToWorldPoint(new Vector3(.2f, .5f, distanceFromCamera)); //Ensures card positions will always be center.
        card2Pos.position = Camera.main.ViewportToWorldPoint(new Vector3(.8f, .5f, distanceFromCamera));//And 80% to the right.
        winnerPos.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, 1.5f, distanceFromCamera));//And 80% to the right.
        loserPos.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, -1.5f, distanceFromCamera));//And 80% to the right.

        if (card_prefab == null)
		{
			card_prefab = Resources.Load<Card>("Card");
		}
        Assert.IsNotNull(card1Pos, "You have to have a place for the first card to go. Please set card1Pos");
        Assert.IsNotNull(card2Pos, "You have to have a place for the second card to go. Please set card2Pos");
        deck = Deck.PopulateCards(card_prefab, transform);
    }

    private void Update()
    {
        drawButton.text = deck.Count > 0 ? "Draw" : "Shuffle";
    }
    //Attach to a button on screen to start game
    public void Begin()
    {
        if (!running) //Prevent overloading screen while animation runs
        {
            StartCoroutine(DrawTwoCards());
        }
    }

    /// <summary>
    /// Repopulates the deck once the deck is empty.
    /// </summary>
    List<Card> RepopulateDeck()
    {
        Card[] cards1 = GetComponentsInChildren<Card>(true); //Avoids searching through EVERY child in case something other than a card is childed.

        List<Card> cards = new List<Card>();
        for (int i = 0; i < cards1.Length; i++)
        {
            Card card = cards1[i];

            card.gameObject.SetActive(true); //Reactivates the cards, prevents Garbage (rather than destroy and create)
            card.ReDraw();

            //Add cards to the probability deck
            card.CheckProbability(ref cards);
        }
        return cards;
    }

    /// <summary>
    /// Coroutine to run when you want to draw two cards at random.
    /// </summary>
    IEnumerator DrawTwoCards()
    {
        if (deck.Count > 0) //As long as there are cards in the deck
        {
            running = true;
            int cardOne, cardTwo;
            cardOne = Random.Range(0, deck.Count);
            cardTwo = Random.Range(0, deck.Count);

            while (deck[cardTwo] == deck[cardOne])
            {
                cardTwo = Random.Range(0, deck.Count);
                yield return null;
            }

            Card firstCard = deck[cardOne],
                 secondCard = deck[cardTwo];

            DrawCards(firstCard, secondCard);
            DeckMaintanence(firstCard, secondCard);

            yield return new WaitUntil(() => firstCard.Drawn && secondCard.Drawn);

            StartCoroutine(firstCard.RotateCard(firstCard.transform.rotation, firstCard.transform.rotation * new Quaternion(0,0,1,0), .5f));
            StartCoroutine(secondCard.RotateCard(secondCard.transform.rotation, secondCard.transform.rotation * new Quaternion(0,0,-1,0), .5f));

            yield return new WaitWhile(() => firstCard.Rotating && secondCard.Rotating);
            yield return new WaitForSeconds(1);

            CompareCards(firstCard, secondCard);
        }
        else //Rebuild the deck
        {
            deck = RepopulateDeck();
        }
    }

    void CompareCards(Card firstCard, Card secondCard)
    {
        Card winner, loser;

        //Check values first

        if (firstCard.cardValue == secondCard.cardValue)
        {
            //Check the suits
            winner = (int)firstCard.suit > (int)secondCard.suit ? firstCard : secondCard;
            loser = (int)firstCard.suit > (int)secondCard.suit ? secondCard : firstCard;
        }

        else
        {
            winner = firstCard.cardValue > secondCard.cardValue ? firstCard : secondCard;
            loser = firstCard.cardValue > secondCard.cardValue ? secondCard : firstCard;
        }

        //Spin based on side of the board
        if (winner == firstCard)
        {
            StartCoroutine(WinningCard(winner, -1));
        }
        else
        {
            StartCoroutine(WinningCard(winner));
        }

        StartCoroutine(LoserCard(loser));
    }


    /// <summary>
    /// Starts the coroutines for drawing the cards
    /// </summary>
    void DrawCards(Card firstCard, Card secondCard)
    {
        StartCoroutine(firstCard.TranslateCard(firstCard.transform.position, card1Pos.position, drawSpeed));
        StartCoroutine(firstCard.RotateCard(firstCard.transform.rotation, card1Pos.rotation, drawSpeed));

        StartCoroutine(secondCard.TranslateCard(secondCard.transform.position, card2Pos.position, drawSpeed));
        StartCoroutine(secondCard.RotateCard(secondCard.transform.rotation, card2Pos.rotation, drawSpeed));
    }

    /// <summary>
    /// Removes cards from the probability deck
    /// </summary>
    void DeckMaintanence(Card one, Card two)
    {
        RemoveCard(one);
        RemoveCard(two);
    }

    /// <summary>
    /// Remove a card from the probability deck
    /// </summary>
    void RemoveCard(Card card)
    {
        deck?.RemoveAll(t => (t.cardValue == card.cardValue) && (t.suit == card.suit));
    }

    /// <summary>
    /// Rotates the winning card and sends it upwards.
    /// </summary>
    IEnumerator WinningCard(Card winner, int direction = 1)
    {
        StartCoroutine(winner.RotateCard(.7f, direction));
        yield return new WaitWhile(() => winner.Rotating);

        StartCoroutine(winner.TranslateCard(winner.transform.position, winnerPos.position, 1));
        yield return new WaitWhile(() => winner.Moving);

        winner.gameObject.SetActive(false);//Don't Destroy the cards, avoids creating garbage
        running = false;//Allows you to press the draw button again
    }

    /// <summary>
    /// Sends the losing card to the bottom
    /// </summary>
    IEnumerator LoserCard(Card loser)
    {
        StartCoroutine(loser.TranslateCard(loser.transform.position, loserPos.position, 1));
        yield return new WaitWhile(() => loser.Moving);

        loser.gameObject.SetActive(false);//Don't Destroy the cards, avoids creating garbage
    }
}