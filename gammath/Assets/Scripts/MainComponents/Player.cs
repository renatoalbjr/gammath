using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private string username;
    [SerializeField] private Rank rank;
    [SerializeField] private Sprite profilePic;
    [SerializeField] private List<Card> deckData;
    [SerializeField] private Deck deck;

    [SerializeField] private TMPro.TMP_Text usernameText;
    [SerializeField] private TMPro.TMP_Text rankText;
    [SerializeField] private Image profileImage;

    public void Start(){
        usernameText.text = username;
        rankText.text = rank.Description();
        profileImage.sprite = profilePic;

        SpawnCards(deckData, deck);
    }

    private void SpawnCards(List<Card> deckData, Deck deck)
    {
        foreach (var item in deckData)
        {
            Card c = Instantiate(item);
            c.BelongsTo(this); //Hours to find out that this line was changing item instead of c
            deck.PlaceAtTop(c.transform);
        }
    }
}

public enum Rank
{
    Pre,
    Fundamental,
    Medio,
    Graduado,
    Mestre,
    Doutor,
    PosDoutor
}