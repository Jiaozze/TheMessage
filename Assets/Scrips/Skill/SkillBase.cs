using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase
{
    public abstract string name { get; }
    public abstract bool canUser { get; }
    public int playerId;
    public virtual void PrepareUse()
    {
        GameManager.Singleton.SelectCardId = -1;
        GameManager.Singleton.SelectPlayerId = -1;
    }

    public virtual bool CheckTriger()
    {
        return false;
    }

    public virtual void Use()
    {

    }

    public virtual void OnCardSelect(int cardId)
    {

    }

    public virtual void OnPlayerSelect(int PlayerId)
    {

    }

    public virtual void OnMessageSelect(int playerId, int cardId)
    {

    }

    public virtual void Cancel()
    {

    }

    public virtual void OnUse()
    {

    }
}
