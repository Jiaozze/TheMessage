// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: common.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from common.proto</summary>
public static partial class CommonReflection {

  #region Descriptor
  /// <summary>File descriptor for common.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static CommonReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cgxjb21tb24ucHJvdG8ioQEKBGNhcmQSDwoHY2FyZF9pZBgBIAEoDRIaCgpj",
          "YXJkX2NvbG9yGAIgAygOMgYuY29sb3ISHAoIY2FyZF9kaXIYAyABKA4yCi5k",
          "aXJlY3Rpb24SHQoJY2FyZF90eXBlGAQgASgOMgouY2FyZF90eXBlEh0KDXdo",
          "b19kcmF3X2NhcmQYBSADKA4yBi5jb2xvchIQCghjYW5fbG9jaxgGIAEoCCqy",
          "BAoEcm9sZRILCgd1bmtub3duEAASDgoKd3VfemhpX2d1bxABEhIKDmNoZW5n",
          "X3hpYW9fZGllEAISDQoJbGlhbl95dWFuEAMSDQoJbWFvX2J1X2JhEAQSEQoN",
          "emhhbmdfeWlfdGluZxAFEhEKDWJhaV9jYW5nX2xhbmcQBhIXChNmZWlfeXVh",
          "bl9sb25nX2NodWFuEAcSDAoIcGVpX2xpbmcQCBIQCgxodWFuZ19qaV9yZW4Q",
          "CRITCg93YW5nX3RpYW5feGlhbmcQChILCgdsaV94aW5nEAsSDAoId2FuZ19r",
          "dWkQDBIPCgthX2Z1X2x1b19sYRANEgsKB2hhbl9tZWkQDhISCg56aGVuZ193",
          "ZW5feGlhbhAPEhAKDHh1YW5fcWluZ196aRAQEgwKCGd1aV9qaWFvEBESDAoI",
          "c2hhb194aXUQEhIRCg1qaW5fc2hlbmdfaHVvEBMSEAoMZ3VfeGlhb19tZW5n",
          "EBQSDwoLYmFpX2ZlaV9mZWkQFRIQCgxkdWFuX211X2ppbmcQFhIPCgt3YW5n",
          "X2Z1X2d1aRAXEgsKB2xhb19oYW4QGBIRCg1iYWlfeGlhb19uaWFuEBkSCwoH",
          "bGFvX2JpZRAaEgwKCHhpYW9faml1EBsSDgoKbGlfbmluZ195dRAcEhAKDGJh",
          "aV9rdW5fc2hhbhAdEgwKCHNoYW5nX3l1EB4SFAoPc3BfZ3VfeGlhb19tZW5n",
          "EPwHEhIKDXNwX2xpX25pbmdfeXUQhAgqcQoFcGhhc2USDgoKRHJhd19QaGFz",
          "ZRAAEg4KCk1haW5fUGhhc2UQARIUChBTZW5kX1N0YXJ0X1BoYXNlEAISDgoK",
          "U2VuZF9QaGFzZRADEg8KC0ZpZ2h0X1BoYXNlEAQSEQoNUmVjZWl2ZV9QaGFz",
          "ZRAFKpkBCgljYXJkX3R5cGUSDgoKQ2hlbmdfUWluZxAAEgsKB1NoaV9UYW4Q",
          "ARIKCgZXZWlfQmkQAhIKCgZMaV9Zb3UQAxINCglQaW5nX0hlbmcQBBIJCgVQ",
          "b19ZaRAFEgsKB0ppZV9IdW8QBhIMCghEaWFvX0JhbxAHEgoKBld1X0RhbxAI",
          "EhYKEkZlbmdfWXVuX0JpYW5fSHVhbhAJKjoKBWNvbG9yEgkKBUJsYWNrEAAS",
          "BwoDUmVkEAESCAoEQmx1ZRACEhMKD0hhc19Ob19JZGVudGl0eRADKk8KC3Nl",
          "Y3JldF90YXNrEgoKBktpbGxlchAAEgsKB1N0ZWFsZXIQARINCglDb2xsZWN0",
          "b3IQAhILCgdNdXRhdG9yEAMSCwoHUGlvbmVlchAEKigKCWRpcmVjdGlvbhIG",
          "CgJVcBAAEggKBExlZnQQARIJCgVSaWdodBACQhYKFGNvbS5mZW5nc2hlbmcu",
          "cHJvdG9zYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(new[] {typeof(global::role), typeof(global::phase), typeof(global::card_type), typeof(global::color), typeof(global::secret_task), typeof(global::direction), }, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::card), global::card.Parser, new[]{ "CardId", "CardColor", "CardDir", "CardType", "WhoDrawCard", "CanLock" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Enums
public enum role {
  /// <summary>
  /// 未知角色
  /// </summary>
  [pbr::OriginalName("unknown")] Unknown = 0,
  /// <summary>
  /// 吴志国
  /// </summary>
  [pbr::OriginalName("wu_zhi_guo")] WuZhiGuo = 1,
  /// <summary>
  /// 程小蝶
  /// </summary>
  [pbr::OriginalName("cheng_xiao_die")] ChengXiaoDie = 2,
  /// <summary>
  /// 连鸢
  /// </summary>
  [pbr::OriginalName("lian_yuan")] LianYuan = 3,
  /// <summary>
  /// 毛不拔
  /// </summary>
  [pbr::OriginalName("mao_bu_ba")] MaoBuBa = 4,
  /// <summary>
  /// 张一挺
  /// </summary>
  [pbr::OriginalName("zhang_yi_ting")] ZhangYiTing = 5,
  /// <summary>
  /// 白沧浪
  /// </summary>
  [pbr::OriginalName("bai_cang_lang")] BaiCangLang = 6,
  /// <summary>
  /// 肥圆龙川
  /// </summary>
  [pbr::OriginalName("fei_yuan_long_chuan")] FeiYuanLongChuan = 7,
  /// <summary>
  /// 裴玲
  /// </summary>
  [pbr::OriginalName("pei_ling")] PeiLing = 8,
  /// <summary>
  /// 黄济仁
  /// </summary>
  [pbr::OriginalName("huang_ji_ren")] HuangJiRen = 9,
  /// <summary>
  /// 王田香
  /// </summary>
  [pbr::OriginalName("wang_tian_xiang")] WangTianXiang = 10,
  /// <summary>
  /// 李醒
  /// </summary>
  [pbr::OriginalName("li_xing")] LiXing = 11,
  /// <summary>
  /// 王魁
  /// </summary>
  [pbr::OriginalName("wang_kui")] WangKui = 12,
  /// <summary>
  /// 阿芙罗拉
  /// </summary>
  [pbr::OriginalName("a_fu_luo_la")] AFuLuoLa = 13,
  /// <summary>
  /// 韩梅
  /// </summary>
  [pbr::OriginalName("han_mei")] HanMei = 14,
  /// <summary>
  /// 鄭文先
  /// </summary>
  [pbr::OriginalName("zheng_wen_xian")] ZhengWenXian = 15,
  /// <summary>
  /// 玄青子
  /// </summary>
  [pbr::OriginalName("xuan_qing_zi")] XuanQingZi = 16,
  /// <summary>
  /// 鬼脚
  /// </summary>
  [pbr::OriginalName("gui_jiao")] GuiJiao = 17,
  /// <summary>
  /// 邵秀
  /// </summary>
  [pbr::OriginalName("shao_xiu")] ShaoXiu = 18,
  /// <summary>
  /// 金生火
  /// </summary>
  [pbr::OriginalName("jin_sheng_huo")] JinShengHuo = 19,
  /// <summary>
  /// 顾小梦
  /// </summary>
  [pbr::OriginalName("gu_xiao_meng")] GuXiaoMeng = 20,
  /// <summary>
  /// 白菲菲
  /// </summary>
  [pbr::OriginalName("bai_fei_fei")] BaiFeiFei = 21,
  /// <summary>
  /// 端木静
  /// </summary>
  [pbr::OriginalName("duan_mu_jing")] DuanMuJing = 22,
  /// <summary>
  /// 王富贵
  /// </summary>
  [pbr::OriginalName("wang_fu_gui")] WangFuGui = 23,
  /// <summary>
  /// 老汉
  /// </summary>
  [pbr::OriginalName("lao_han")] LaoHan = 24,
  /// <summary>
  /// 白小年
  /// </summary>
  [pbr::OriginalName("bai_xiao_nian")] BaiXiaoNian = 25,
  /// <summary>
  /// 老鳖
  /// </summary>
  [pbr::OriginalName("lao_bie")] LaoBie = 26,
  /// <summary>
  /// 小九
  /// </summary>
  [pbr::OriginalName("xiao_jiu")] XiaoJiu = 27,
  /// <summary>
  /// 李宁玉
  /// </summary>
  [pbr::OriginalName("li_ning_yu")] LiNingYu = 28,
  /// <summary>
  /// 白昆山
  /// </summary>
  [pbr::OriginalName("bai_kun_shan")] BaiKunShan = 29,
  /// <summary>
  /// 商玉
  /// </summary>
  [pbr::OriginalName("shang_yu")] ShangYu = 30,
  /// <summary>
  /// SP顾小梦
  /// </summary>
  [pbr::OriginalName("sp_gu_xiao_meng")] SpGuXiaoMeng = 1020,
  /// <summary>
  /// SP李宁玉
  /// </summary>
  [pbr::OriginalName("sp_li_ning_yu")] SpLiNingYu = 1028,
}

public enum phase {
  /// <summary>
  /// 摸牌阶段
  /// </summary>
  [pbr::OriginalName("Draw_Phase")] DrawPhase = 0,
  /// <summary>
  /// 出牌阶段
  /// </summary>
  [pbr::OriginalName("Main_Phase")] MainPhase = 1,
  /// <summary>
  /// 情报传递阶段开始时
  /// </summary>
  [pbr::OriginalName("Send_Start_Phase")] SendStartPhase = 2,
  /// <summary>
  /// 传递阶段
  /// </summary>
  [pbr::OriginalName("Send_Phase")] SendPhase = 3,
  /// <summary>
  /// 争夺阶段
  /// </summary>
  [pbr::OriginalName("Fight_Phase")] FightPhase = 4,
  /// <summary>
  /// 接收阶段
  /// </summary>
  [pbr::OriginalName("Receive_Phase")] ReceivePhase = 5,
}

public enum card_type {
  /// <summary>
  /// 澄清
  /// </summary>
  [pbr::OriginalName("Cheng_Qing")] ChengQing = 0,
  /// <summary>
  /// 试探
  /// </summary>
  [pbr::OriginalName("Shi_Tan")] ShiTan = 1,
  /// <summary>
  /// 威逼
  /// </summary>
  [pbr::OriginalName("Wei_Bi")] WeiBi = 2,
  /// <summary>
  /// 利诱
  /// </summary>
  [pbr::OriginalName("Li_You")] LiYou = 3,
  /// <summary>
  /// 平衡
  /// </summary>
  [pbr::OriginalName("Ping_Heng")] PingHeng = 4,
  /// <summary>
  /// 破译
  /// </summary>
  [pbr::OriginalName("Po_Yi")] PoYi = 5,
  /// <summary>
  /// 截获
  /// </summary>
  [pbr::OriginalName("Jie_Huo")] JieHuo = 6,
  /// <summary>
  /// 调包
  /// </summary>
  [pbr::OriginalName("Diao_Bao")] DiaoBao = 7,
  /// <summary>
  /// 误导
  /// </summary>
  [pbr::OriginalName("Wu_Dao")] WuDao = 8,
  [pbr::OriginalName("Feng_Yun_Bian_Huan")] FengYunBianHuan = 9,
}

public enum color {
  /// <summary>
  /// 对于身份，则是绿色（神秘人）；对于卡牌，则是黑色
  /// </summary>
  [pbr::OriginalName("Black")] Black = 0,
  /// <summary>
  /// 红色
  /// </summary>
  [pbr::OriginalName("Red")] Red = 1,
  /// <summary>
  /// 蓝色
  /// </summary>
  [pbr::OriginalName("Blue")] Blue = 2,
  /// <summary>
  /// 失去身份
  /// </summary>
  [pbr::OriginalName("Has_No_Identity")] HasNoIdentity = 3,
}

/// <summary>
/// 神秘人任务
/// </summary>
public enum secret_task {
  /// <summary>
  /// 你的回合中，一名红色和蓝色情报合计不少于2张的人死亡
  /// </summary>
  [pbr::OriginalName("Killer")] Killer = 0,
  /// <summary>
  /// 你的回合中，有人宣胜，则你代替他胜利
  /// </summary>
  [pbr::OriginalName("Stealer")] Stealer = 1,
  /// <summary>
  /// 你获得3张红色情报或者3张蓝色情报
  /// </summary>
  [pbr::OriginalName("Collector")] Collector = 2,
  /// <summary>
  /// 当一名角色收集了三张红色情报或三张蓝色情报后，若其没有宣告胜利，则你胜利
  /// </summary>
  [pbr::OriginalName("Mutator")] Mutator = 3,
  /// <summary>
  /// 你死亡时，已收集了至少一张红色情报或蓝色情报
  /// </summary>
  [pbr::OriginalName("Pioneer")] Pioneer = 4,
}

public enum direction {
  /// <summary>
  /// 向上
  /// </summary>
  [pbr::OriginalName("Up")] Up = 0,
  /// <summary>
  /// 向左
  /// </summary>
  [pbr::OriginalName("Left")] Left = 1,
  /// <summary>
  /// 向右
  /// </summary>
  [pbr::OriginalName("Right")] Right = 2,
}

#endregion

#region Messages
/// <summary>
/// 卡牌的结构体
/// </summary>
public sealed partial class card : pb::IMessage<card> {
  private static readonly pb::MessageParser<card> _parser = new pb::MessageParser<card>(() => new card());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<card> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::CommonReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public card() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public card(card other) : this() {
    cardId_ = other.cardId_;
    cardColor_ = other.cardColor_.Clone();
    cardDir_ = other.cardDir_;
    cardType_ = other.cardType_;
    whoDrawCard_ = other.whoDrawCard_.Clone();
    canLock_ = other.canLock_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public card Clone() {
    return new card(this);
  }

  /// <summary>Field number for the "card_id" field.</summary>
  public const int CardIdFieldNumber = 1;
  private uint cardId_;
  /// <summary>
  /// 卡牌ID
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public uint CardId {
    get { return cardId_; }
    set {
      cardId_ = value;
    }
  }

  /// <summary>Field number for the "card_color" field.</summary>
  public const int CardColorFieldNumber = 2;
  private static readonly pb::FieldCodec<global::color> _repeated_cardColor_codec
      = pb::FieldCodec.ForEnum(18, x => (int) x, x => (global::color) x);
  private readonly pbc::RepeatedField<global::color> cardColor_ = new pbc::RepeatedField<global::color>();
  /// <summary>
  /// 卡牌颜色（因为可能有双色卡，所以用了repeated）
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<global::color> CardColor {
    get { return cardColor_; }
  }

  /// <summary>Field number for the "card_dir" field.</summary>
  public const int CardDirFieldNumber = 3;
  private global::direction cardDir_ = global::direction.Up;
  /// <summary>
  /// 卡牌上的箭头方向
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::direction CardDir {
    get { return cardDir_; }
    set {
      cardDir_ = value;
    }
  }

  /// <summary>Field number for the "card_type" field.</summary>
  public const int CardTypeFieldNumber = 4;
  private global::card_type cardType_ = global::card_type.ChengQing;
  /// <summary>
  /// 卡牌类型
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::card_type CardType {
    get { return cardType_; }
    set {
      cardType_ = value;
    }
  }

  /// <summary>Field number for the "who_draw_card" field.</summary>
  public const int WhoDrawCardFieldNumber = 5;
  private static readonly pb::FieldCodec<global::color> _repeated_whoDrawCard_codec
      = pb::FieldCodec.ForEnum(42, x => (int) x, x => (global::color) x);
  private readonly pbc::RepeatedField<global::color> whoDrawCard_ = new pbc::RepeatedField<global::color>();
  /// <summary>
  /// （试探卡）哪个身份的人摸1张牌（那么另外的身份一定是弃1张牌）
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<global::color> WhoDrawCard {
    get { return whoDrawCard_; }
  }

  /// <summary>Field number for the "can_lock" field.</summary>
  public const int CanLockFieldNumber = 6;
  private bool canLock_;
  /// <summary>
  /// 是否有锁定标记
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool CanLock {
    get { return canLock_; }
    set {
      canLock_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as card);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(card other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (CardId != other.CardId) return false;
    if(!cardColor_.Equals(other.cardColor_)) return false;
    if (CardDir != other.CardDir) return false;
    if (CardType != other.CardType) return false;
    if(!whoDrawCard_.Equals(other.whoDrawCard_)) return false;
    if (CanLock != other.CanLock) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (CardId != 0) hash ^= CardId.GetHashCode();
    hash ^= cardColor_.GetHashCode();
    if (CardDir != global::direction.Up) hash ^= CardDir.GetHashCode();
    if (CardType != global::card_type.ChengQing) hash ^= CardType.GetHashCode();
    hash ^= whoDrawCard_.GetHashCode();
    if (CanLock != false) hash ^= CanLock.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (CardId != 0) {
      output.WriteRawTag(8);
      output.WriteUInt32(CardId);
    }
    cardColor_.WriteTo(output, _repeated_cardColor_codec);
    if (CardDir != global::direction.Up) {
      output.WriteRawTag(24);
      output.WriteEnum((int) CardDir);
    }
    if (CardType != global::card_type.ChengQing) {
      output.WriteRawTag(32);
      output.WriteEnum((int) CardType);
    }
    whoDrawCard_.WriteTo(output, _repeated_whoDrawCard_codec);
    if (CanLock != false) {
      output.WriteRawTag(48);
      output.WriteBool(CanLock);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (CardId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeUInt32Size(CardId);
    }
    size += cardColor_.CalculateSize(_repeated_cardColor_codec);
    if (CardDir != global::direction.Up) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) CardDir);
    }
    if (CardType != global::card_type.ChengQing) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) CardType);
    }
    size += whoDrawCard_.CalculateSize(_repeated_whoDrawCard_codec);
    if (CanLock != false) {
      size += 1 + 1;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(card other) {
    if (other == null) {
      return;
    }
    if (other.CardId != 0) {
      CardId = other.CardId;
    }
    cardColor_.Add(other.cardColor_);
    if (other.CardDir != global::direction.Up) {
      CardDir = other.CardDir;
    }
    if (other.CardType != global::card_type.ChengQing) {
      CardType = other.CardType;
    }
    whoDrawCard_.Add(other.whoDrawCard_);
    if (other.CanLock != false) {
      CanLock = other.CanLock;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          CardId = input.ReadUInt32();
          break;
        }
        case 18:
        case 16: {
          cardColor_.AddEntriesFrom(input, _repeated_cardColor_codec);
          break;
        }
        case 24: {
          CardDir = (global::direction) input.ReadEnum();
          break;
        }
        case 32: {
          CardType = (global::card_type) input.ReadEnum();
          break;
        }
        case 42:
        case 40: {
          whoDrawCard_.AddEntriesFrom(input, _repeated_whoDrawCard_codec);
          break;
        }
        case 48: {
          CanLock = input.ReadBool();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
