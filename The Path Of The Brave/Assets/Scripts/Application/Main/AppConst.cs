// ReSharper disable InconsistentNaming
namespace MyApplication
{
    public static class AppConst
    {
        // 目录
        public static readonly string JSONDir = UnityEngine.Application.dataPath + @"\PlayerData";
        public static readonly string LevelDir = UnityEngine.Application.dataPath + @"\PlayerData\LevelData";
        public static readonly string ConfigDir = UnityEngine.Application.dataPath + @"\ABRes\Configuration";
        public static readonly string SaveDir = UnityEngine.Application.dataPath + @"\SaveData";

        // 命名空间
        public const string NameSpace = "MyApplication";

        // AB包
        public const string AB_UIPanel = "uipanel";
        public const string AB_Config = "config";
        public const string AB_Prefabs = "prefabs";

        public const string AB_ItemFillerView = "ItemFillerView";
        public const string AB_ShopBuyView = "ShopBuyView";
        public const string AB_CharacterPictureView = "CharacterPictureView";
        public const string AB_HpImageGreen = "HpImageGreen";
        public const string AB_HpImageRed = "HpImageRed";
        public const string AB_HudText = "HudText";
        public const string AB_HudTextGreen = "HudTextGreen";
        public const string AB_MagicCircleSimpleGreen = "MagicCircleSimpleGreen";
        public const string AB_HealBig = "HealBig";
        public const string AB_HealingWindZone = "HealingWindZone";
        public const string AB_RocketMissileFire = "RocketMissileFire";
        public const string AB_MysticExplosionOrange = "MysticExplosionOrange";

        public const string AB_CharactersConfiguration = "CharactersConfiguration";
        public const string AB_LevelConfiguration = "LevelConfiguration";
        public const string AB_ItemsBaseConfiguration = "ItemsBaseConfiguration";
        public const string AB_ShopBuyConfiguration = "ShopBuyConfiguration";
        public const string AB_SkillsConfiguration = "SkillsConfiguration";
        public const string AB_ViewConfiguration = "ViewConfiguration";
        public const string AB_SelectLevelsConfiguration = "SelectLevelsConfiguration";
        public const string AB_StarAwardsConfiguration = "StarAwardsConfiguration";
        //public const string AB_ItemConfiguration = "ItemConfiguration";
        //public const string AB_HeroConfiguration = "HeroConfiguration";
        //public const string AB_PfCharacterBattle = "PfCharacterBattle";
        
        //public const string AB_SkillsConfiguration = "SkillsConfiguration";

        // 动画名
        //public const string A_PlayerAttack_1 = "SlashMelee2H";
        //public const string A_PlayerRelease_1 = "Cast1H";
        //public const string A_DragonSea = "Dragon sea";

        // Model
        // public const string M_GameData = "M_GameData";
        //public const string M_RoundModel = "M_RoundModel";

        // View
        public const string V_StartView = "V_StartView";
        public const string V_MainView = "V_MainView";
        public const string V_BattleReadyView = "V_BattleReadyView";
        public const string V_MainSettingView = "V_MainSettingView";
        public const string V_InventoryView = "V_InventoryView";
        public const string V_ShopView = "V_ShopView";
        public const string V_ToolTipView = "V_ToolTipView";
        public const string V_ShopBuyTipView = "V_ShopBuyTipView";
        public const string V_SellItemTipView = "V_SellItemTipView";
        public const string V_CharacterView = "V_CharacterView";
        public const string V_RecruitingView = "V_RecruitingView";
        public const string V_MapView = "V_MapView";
        public const string V_BattleView = "V_BattleView";

        //public const string V_SelectView = "V_SelectView";
        //public const string V_ToolTipView = "V_ToolTipView";
        //public const string V_ThrowItemView = "V_ThrowItemView";
        //public const string V_DelItemTipView = "V_DelItemTipView";
        //public const string V_PropertyView = "V_PropertyView";
        //public const string V_BattleView = "V_BattleView";

        // Controller
        public const string C_StartUp = "C_StartUp";

        public const string C_EnterScene = "C_EnterScene";                  //SceneArgs
        public const string C_ExitScene = "C_ExitScene";                    //SceneArgs

        public const string C_LoadData = "C_LoadData";
        public const string C_SaveData = "C_SaveData";

        public const string C_BattleReadyBegin = "C_BattleReadyBegin";
        public const string C_BattleReadyEnd = "C_BattleReadyEnd";

        public const string C_OnEndDragPictureFrameView = "C_OnEndDragPictureFrameView";

        public const string C_UpdatePlayerInfo = "C_UpdatePlayerInfo";

        public const string C_UpdateCharacterPictureView = "C_UpdateCharacterPictureView";
        public const string C_UpdateCharacterInfo = "C_UpdateCharacterInfo";
        public const string C_ResetUpdateCharacterInfo = "C_ResetUpdateCharacterInfo";
        public const string C_CharacterItemLvUp = "C_CharacterItemLvUp";
        public const string C_EquipmentJewelry = "C_EquipmentJewelry";
        public const string C_CharacterSkillLock = "C_CharacterSkillLock";

        public const string C_UpdateInventory = "C_UpdateInventory";
        public const string C_ShowInventoryItem = "C_ShowInventoryItem";
        public const string C_HideInventoryItem = "C_HideInventoryItem";

        public const string C_UpdateShop = "C_UpdateShop";
        public const string C_ShowShopBuy = "C_ShowShopBuy";
        public const string C_HideShopBuy = "C_HideShopBuy";

        public const string C_ShowShopBuyTip = "C_ShowShopBuyTip";
        public const string C_ShowShopSellTip = "C_ShowShopSellTip";
        public const string C_ShopBuySure = "C_ShopBuySure";
        public const string C_ShopSellSure = "C_ShopSellSure";

        public const string C_UpdateRecruiting = "C_UpdateRecruiting";

        public const string C_UpdateSelectLevelInfo = "C_UpdateSelectLevelInfo";
        public const string C_UpdateStarStatus = "C_UpdateStarStatus";
        public const string C_ShowAwardPanel = "C_ShowAwardPanel";
        public const string C_ShowLevelChallengeWin = "C_ShowLevelChallengeWin";

        public const string C_GetStarAward = "C_GetStarAward";

        public const string C_RoundBegin = "C_RoundBegin";
        public const string C_EnemyReady = "C_EnemyReady";
        public const string C_ChangeBattleState = "C_ChangeBattleState";
        public const string C_ShowHudDamage = "C_ShowHudDamage";
        public const string C_ShowHudRestore = "C_ShowHudRestore";
        public const string C_UpdateActionPanel = "C_UpdateActionPanel";
        public const string C_ShowSkillSelectTarget = "C_ShowSkillSelectTarget";
        public const string C_ShowSkillConfirm = "C_ShowSkillConfirm";

        public const string C_LevelChallengeWin = "C_LevelChallengeWin";
        public const string C_LevelChallengeLose = "C_LevelChallengeLose";

        //public const string C_DropOnSellPanel = "C_DropOnSellPanel";

        //public const string E_UpdatePropertyInfo = "E_UpdatePropertyInfo";  // PropertyInfoArgs

        //public const string E_BeginDragInventory = "E_BeginDragInventory";
        //public const string E_EndDragInventory = "E_EndDragInventory";
        //public const string E_BeginDragProperty = "E_BeginDragProperty";
        //public const string E_EndDragProperty = "E_EndDragProperty";

        //public const string E_DropOnInventoryItem = "E_DropOnInventoryItem";            // DropOnViewArgs
        //public const string E_DropOnInventoryGrid = "E_DropOnInventoryGrid";            // DropOnGridArgs
        //public const string E_DropOnThrowPanel = "E_DropOnThrowPanel";
        //public const string E_DropOnPropertyEquipPanel = "E_DropOnPropertyEquipPanel";  // DropOnViewArgs

        //public const string E_DelItem = "E_DelItem";                                //DelItemArgs

        public const string C_ShowPanel = "C_ShowPanel";                            //UIPanelArgs
        public const string C_ShowTip = "C_ShowTip";                                //ShowTipArgs
        //public const string E_ShowInventoryItem = "E_ShowInventoryItem";            //ShowItemArgs
        //public const string E_ShowPropertyItem = "E_ShowPropertyItem";              //ShowItemArgs
        //public const string E_ShowPropertyImgFrame = "E_ShowPropertyImgFrame";      //ShowPropertyImgFrameArgs

        public const string C_HideTip = "C_HideTip";
        //public const string E_HideInventoryItem = "E_HideInventoryItem";
        //public const string E_HidePropertyItem = "E_HidePropertyItem";
        //public const string E_HidePropertyImgFrame = "E_HidePropertyImgFrame";

        //public const string E_UseEquip = "E_UseEquip";                      // UseEquipArgs

        //public const string E_PointerDownProperty = "E_PointerDownProperty";
        //public const string E_PointerDownInventory = "E_PointerDownInventory";

        //public const string E_StartBattle = "E_StartBattle";                // UnitDataSOArgs
        //public const string E_StartBegin = "E_StartBegin";
        //public const string E_EnemyReady = "E_EnemyReady";

        //public const string E_ChangeBattleState = "E_ChangeBattleState";

        //public const string E_AttackComplete = "E_AttackComplete";
        //public const string E_EnemyAttack = "E_EnemyAttack";

        //public const string E_UpdateHPHUD = "E_UpdateHPHUD";                // UpdateUnitHUD
        //public const string E_UpdateMPHUD = "E_UpdateMPHUD";                // UpdateUnitHUD
    }
}