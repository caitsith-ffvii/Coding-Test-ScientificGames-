using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Card(CardSuit suit, int value)
    {
        this.suit = suit;
        cardValue = value;
    }

    /// <summary>
    /// Suit of specified card
    /// </summary>
	public CardSuit suit;

    /// <summary>
    /// Value of card between 1 and 13
    /// </summary>
    public int cardValue;

    /// <summary>
    /// Determines if card has been pulled from the stack.
    /// </summary>
    private bool drawn;
    public bool Drawn { get { return drawn; } }

    /// <summary>
    /// Determines if the card has been flipped over
    /// </summary>
    private bool rotating = false;
    public bool Rotating { get { return rotating; } }

    private bool moving = false;
    public bool Moving { get { return moving; } }

    /// <summary>
    /// Animates the movement of the card to a new position
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="end">Position that the card ends at</param>
    /// <param name="dur">How long it takes to move the card</param>
    /// <returns></returns>
    public IEnumerator TranslateCard(Vector3 start, Vector3 end, float dur)
    {
        moving = true;
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Slerp(start, end, t / dur);
            yield return null;
        }
        drawn = true;
        moving = false;
    }

    /// <summary>
    /// Rotates the card from the start rotation to the end rotation
    /// </summary>
    /// <param name="start">Angle to start at</param>
    /// <param name="end">Angle to end at</param>
    /// <param name="dur">How long it takes to get there</param>
    /// <returns></returns>
    public IEnumerator RotateCard(Quaternion start, Quaternion end, float dur)
    {
        rotating = true;
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(start, end, t / dur);
            yield return null;
        }
        rotating = false;
    }
    
    
    /// <summary>
    /// Rotates the card in a full 360 degrees for the given duration
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator RotateCard(float duration, int numberOfRotations)
    {
        rotating = true;
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + (360f * numberOfRotations);
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % (360f * numberOfRotations);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, -yRotation, transform.eulerAngles.z); //Rotate the same direction as the other rotate animation
            yield return null;
        }
        rotating = false;
    }

    /// <summary>
    /// Change the texture on the card
    /// </summary>
    public void ChangeTexture(Texture texture)
    {
        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    /// <summary>
    /// Sets drawn to false and positions card back in the deck.
    /// </summary>
    public void ReDraw()
    {
        drawn = false;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void CheckProbability(ref List<Card> cards)
    {
        cards.Add(this);
        if (suit == CardSuit.Hearts) //Hearts have 2 chances of being drawn
        {
            cards.Add(this);
        }
        if (suit == CardSuit.Spades && cardValue == 1) //Ace of Spades has 3 chances of being drawn
        {
            cards.Add(this);
            cards.Add(this);
        }
    }
}