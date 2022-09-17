using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectSelect : MonoBehaviour
{
    public Toggle tgLeft;
    public Toggle tgUp;
    public Toggle tgRight;

    private void OnEnable()
    {
        if (!GameManager.Singleton.players.ContainsKey(GameManager.SelfPlayerId))
        {
            return;
        }
        RoleBase role = GameManager.Singleton.players[GameManager.SelfPlayerId].role;
        if (role == null)
        {
            return;
        }

        if (role is Role_LaoBie)
        {
            Role_LaoBie role_LaoBie = role as Role_LaoBie;
            if(GameManager.Singleton.cardsHand.ContainsKey(GameManager.Singleton.SelectCardId))
            {
                role_LaoBie.direction = GameManager.Singleton.cardsHand[GameManager.Singleton.SelectCardId].direction;
            }
            switch (role_LaoBie.direction)
            {
                case DirectionEnum.Left:
                    tgLeft.isOn = true;
                    break;
                case DirectionEnum.Up:
                    tgUp.isOn = true;
                    break;
                case DirectionEnum.Right:
                    tgRight.isOn = true;
                    break;
            }
        }
    }

    public void OnToggleLeft(bool isOn)
    {
        if (isOn)
        {
            SetDirect(DirectionEnum.Left);
        }
    }

    public void OnToggleUp(bool isOn)
    {
        if (isOn)
        {
            SetDirect(DirectionEnum.Up);
        }
    }

    public void OnToggleRight(bool isOn)
    {
        if (isOn)
        {
            SetDirect(DirectionEnum.Right);
        }
    }

    private void SetDirect(DirectionEnum direction)
    {
        if (!GameManager.Singleton.players.ContainsKey(GameManager.SelfPlayerId))
        {
            return;
        }
        RoleBase role = GameManager.Singleton.players[GameManager.SelfPlayerId].role;
        if (role == null)
        {
            return;
        }

        if (role is Role_LaoBie)
        {
            Role_LaoBie role_LaoBie = role as Role_LaoBie;
            role_LaoBie.direction = direction;
        }
    }
}
