using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    /// <summary>
    /// Fills the deck with the proper cards
    /// </summary>
    public static List<Card> PopulateCards(Card prefab, Transform deck_transform)
    {
        List<Card> deck = new List<Card>();
        //Fill the deck with one of each card value between 1 - 13
        for (int i = 0; i < 4; i++)
        {
            //and each suit as well
            for (int j = 0; j < 13; j++)
            {
                Card card = Instantiate(prefab, deck_transform);//copy the card prefab
                card.transform.localPosition = Vector3.zero;
                card.transform.localEulerAngles = Vector3.zero;

                card.suit = (CardSuit)i; //Change card suit
                card.cardValue = j + 1; //Change card value

                card.ChangeTexture(TextureManager.all[i][j]);
                card.name = card.cardValue.ToString() + " of " + card.suit; // rename card

                card.CheckProbability(ref deck);
            }
        }
        return deck;
    }
}