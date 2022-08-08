using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase
{
    public int playerId = -1;
    public int cardNum = 0;
    public List<CardFS> testCards = new List<CardFS>();

    public bool IsSelf()
    {
        return playerId == 0;
    }
}
