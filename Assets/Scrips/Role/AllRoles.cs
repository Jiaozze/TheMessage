using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRoles
{
    public static RoleBase GetRole(role role, int i)
    {
        //return new Role_DuanMuJing(i); //test

        switch (role)
        {
            case role.JinShengHuo:
                return new Role_JinShengHuo(i);
            case role.MaoBuBa:
                return new Role_MaoBuBa(i);
            case role.ShaoXiu:
                return new Role_ShaoXiu(i);
            case role.DuanMuJing:
                return new Role_DuanMuJing(i);
            default:
                return new Role_Unknown(i);
        }
    }
}
