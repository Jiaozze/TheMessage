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
            case role.LiXing:
                return new Role_LiXing(i);
            case role.AFuLuoLa:
                return new Role_AFuLuoLa(i);
            case role.WuZhiGuo:
                return new Role_WuZhiGuo(i);
            case role.ZhangYiTing:
                return new Role_ZhangYiTing(i);
            case role.XiaoJiu:
                return new Role_XiaoJiu(i);
            case role.BaiCangLang:
                return new Role_BaiCangLang(i);
            case role.SpGuXiaoMeng:
                return new Role_GuXiaoMengSP(i);
            case role.SpLiNingYu:
                return new Role_LiNingYuSP(i);
            case role.XuanQingZi:
                return new Role_XuanQingZi(i);
            case role.WangTianXiang:
                return new Role_WangTianXiang(i);
            case role.LianYuan:
                return new Role_LianYuan(i);
            case role.GuiJiao:
                return new Role_GuiJiao(i);
            case role.BaiXiaoNian:
                return new Role_BaiXiaoNian(i);
            case role.PeiLing:
                return new Role_PeiLing(i);
            case role.ShangYu:
                return new Role_ShangYu(i);
            case role.ChengXiaoDie:
                return new Role_ChengXiaoDie(i);
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
