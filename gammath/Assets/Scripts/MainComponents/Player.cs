using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Player : MonoBehaviour
{
    internal string username;
    internal Rank rank;
    internal Sprite profilePic;

    public void Start(){
        rank = Rank.Pre;
        Debug.Log(rank.Description());
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