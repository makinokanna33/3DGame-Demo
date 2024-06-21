using PureMVC.Patterns.Facade;

namespace MyApplication
{
    public class GameFacade : Facade
    {
        // 单例设计模式
        public static GameFacade Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameFacade();
                return instance as GameFacade;
            }
        }

        /// <summary>
        /// 初始化控制层，让命令与通知绑定
        /// </summary>
        protected override void InitializeController()
        {
            base.InitializeController();

            RegisterCommand(AppConst.C_StartUp, () => new StartUpCommand());

            RegisterCommand(AppConst.C_EnterScene, ()=> new EnterSceneCommand());
            RegisterCommand(AppConst.C_ExitScene, ()=> new ExitSceneCommand());

            RegisterCommand(AppConst.C_LoadData, ()=> new LoadDataCommand());
            RegisterCommand(AppConst.C_SaveData, ()=> new SaveDataCommand());

            RegisterCommand(AppConst.C_ShowPanel, ()=> new ShowPanelCommand());

            RegisterCommand(AppConst.C_BattleReadyBegin, ()=> new BattleReadyBeginCommand());
            RegisterCommand(AppConst.C_BattleReadyEnd, ()=> new BattleReadyEndCommand());

            RegisterCommand(AppConst.C_UpdateInventory, ()=> { return new UpdateInventoryCommand(); });

            RegisterCommand(AppConst.C_UpdateShop, ()=> { return new UpdateShopCommand(); });
            RegisterCommand(AppConst.C_ShopBuySure, ()=> { return new ShopBuySureCommand(); });
            RegisterCommand(AppConst.C_ShopSellSure, ()=> { return new ShopSellSureCommand(); });

            RegisterCommand(AppConst.C_CharacterItemLvUp, ()=> { return new CharacterItemLvUpCommand(); });
            RegisterCommand(AppConst.C_EquipmentJewelry, ()=> { return new EquipmentJewelryCommand(); });
            RegisterCommand(AppConst.C_CharacterSkillLock, ()=> { return new CharacterItemSkillLockCommand(); });

            RegisterCommand(AppConst.C_GetStarAward, ()=> { return new GetStarAwardCommand(); });

            RegisterCommand(AppConst.C_ChangeBattleState, ()=> { return new ChangeBattleStateCommand(); });
            RegisterCommand(AppConst.C_LevelChallengeWin, ()=> { return new GetLevelChallengeWinCommand(); });
            //RegisterCommand(AppConst.E_UpdateProperty, ()=> { return new UpdatePropertyCommand(); });

            //RegisterCommand(AppConst.E_DropOnInventoryGrid, ()=> { return new DropOnInventoryGridCommand(); });
            //RegisterCommand(AppConst.E_DropOnInventoryItem, ()=> { return new DropOnInventoryItemCommand(); });
            //RegisterCommand(AppConst.E_DropOnPropertyEquipPanel, ()=> { return new DropOnPropertyEquipCommand(); });

            //RegisterCommand(AppConst.E_DelItem, ()=> { return new DelItemCommand(); });

            //RegisterCommand(AppConst.E_UseEquip, ()=> { return new UseEquipCommand(); });

            //RegisterCommand(AppConst.E_AttackComplete, ()=> { return new AttackCompleteCommand(); });
            //RegisterCommand(AppConst.E_EnemyAttack, ()=> { return new EnemyAttackCommand(); });

            //RegisterCommand(AppConst.E_ChangeBattleState, ()=> { return new ChangeBattleStateCommand(); });
        }

        /// <summary>
        /// 启动函数
        /// </summary>
        public void StartUp()
        {
            // 发送事件，启动框架
            SendNotification(AppConst.C_StartUp);
        }
    }
}