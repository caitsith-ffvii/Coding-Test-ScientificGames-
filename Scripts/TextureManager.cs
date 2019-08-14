using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    [SerializeField] private Texture[] spades   = new Texture[13];
    [SerializeField] private Texture[] hearts   = new Texture[13];
    [SerializeField] private Texture[] diamonds = new Texture[13];
    [SerializeField] private Texture[] clubs    = new Texture[13];
    public static Texture[][] all;

    private void Awake()
    {
        all = new Texture[][] { clubs, diamonds, hearts, spades };
    }
}
