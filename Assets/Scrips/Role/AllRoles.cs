﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRoles
{
    public static RoleBase GetRole(role role, int i)
    {
        //return new Role_DuanMuJing(i); //test

        switch (role)
        {
            case role.LiNingYu:
                return new Role_LiNingYu(i);
            case role.GuXiaoMeng:
                return new Role_GuXiaoMeng(i);
            case role.BaiFeiFei:
                return new Role_BaiFeiFei(i);
            case role.LaoHan:
                return new Role_LaoHan(i);
            case role.HanMei:
                return new Role_HanMei(i);
            case role.ZhengWenXian:
                return new Role_ZhengWenXian(i);
            case role.FeiYuanLongChuan:
                return new Role_FeiYuanLongChuan(i);
            case role.WangKui:
                return new Role_WangKui(i);
            case role.LaoBie:
                return new Role_LaoBie(i);
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
