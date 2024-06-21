using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace MyApplication
{
    public class ExitSceneCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            var sceneArgs = notification.Body as SceneArgs;

            if (sceneArgs == null)
            {
                LoggerManager.Instance.LogCommandArgsError("ExitSceneCommand", "SceneArgs");
                return;
            }

            switch (sceneArgs.sceneBuildIndex)
            {
                case 1:
                    if (Facade.HasMediator(StartViewMediator.NAME))
                    {
                        // 获取 mediator
                        StartViewMediator mediator = Facade.RetrieveMediator(StartViewMediator.NAME) as StartViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    break;
                case 2:
                    if (Facade.HasMediator(MainViewMediator.NAME))
                    {
                        // 获取 mediator
                        MainViewMediator mediator = Facade.RetrieveMediator(MainViewMediator.NAME) as MainViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(MainSettingViewMediator.NAME))
                    {
                        // 获取 mediator
                        MainSettingViewMediator mediator = Facade.RetrieveMediator(MainSettingViewMediator.NAME) as MainSettingViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(InventoryViewMediator.NAME))
                    {
                        // 获取 mediator
                        InventoryViewMediator mediator = Facade.RetrieveMediator(InventoryViewMediator.NAME) as InventoryViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(ItemFillerViewMediator.NAME))
                    {
                        // 获取 mediator
                        ItemFillerViewMediator mediator = Facade.RetrieveMediator(ItemFillerViewMediator.NAME) as ItemFillerViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(ToolTipViewMediator.NAME))
                    {
                        // 获取 mediator
                        ToolTipViewMediator mediator = Facade.RetrieveMediator(ToolTipViewMediator.NAME) as ToolTipViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(ShopViewMediator.NAME))
                    {
                        // 获取 mediator
                        ShopViewMediator mediator = Facade.RetrieveMediator(ShopViewMediator.NAME) as ShopViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(ShopBuyViewMediator.NAME))
                    {
                        // 获取 mediator
                        ShopBuyViewMediator mediator = Facade.RetrieveMediator(ShopBuyViewMediator.NAME) as ShopBuyViewMediator;
                        
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(ShopBuyTipViewMediator.NAME))
                    {
                        // 获取 mediator
                        ShopBuyTipViewMediator mediator = Facade.RetrieveMediator(ShopBuyTipViewMediator.NAME) as ShopBuyTipViewMediator;

                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(SellItemTipViewMediator.NAME))
                    {
                        // 获取 mediator
                        SellItemTipViewMediator mediator = Facade.RetrieveMediator(SellItemTipViewMediator.NAME) as SellItemTipViewMediator;

                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(CharacterViewMediator.NAME))
                    {
                        // 获取 mediator
                        CharacterViewMediator mediator = Facade.RetrieveMediator(CharacterViewMediator.NAME) as CharacterViewMediator;

                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(RecruitingViewMediator.NAME))
                    {
                        // 获取 mediator
                        RecruitingViewMediator mediator = Facade.RetrieveMediator(RecruitingViewMediator.NAME) as RecruitingViewMediator;

                        mediator.ClearPanel();
                    }
                    break;
                case 3:
                    if (Facade.HasMediator(MapViewMediator.NAME))
                    {
                        // 获取 mediator
                        MapViewMediator mediator = Facade.RetrieveMediator(MapViewMediator.NAME) as MapViewMediator;

                        mediator.ClearPanel();
                    }
                    break;
                case 4:
                    if (Facade.HasMediator(BattleReadyViewMediator.NAME))
                    {
                        // 获取 mediator
                        BattleReadyViewMediator mediator = Facade.RetrieveMediator(BattleReadyViewMediator.NAME) as BattleReadyViewMediator;

                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(BattleViewMediator.NAME))
                    {
                        // 获取 mediator
                        BattleViewMediator mediator = Facade.RetrieveMediator(BattleViewMediator.NAME) as BattleViewMediator;

                        mediator.ClearPanel();
                    }
                    break;
                case 5:
                    if (Facade.HasMediator(BattleReadyViewMediator.NAME))
                    {
                        // 获取 mediator
                        BattleReadyViewMediator mediator = Facade.RetrieveMediator(BattleReadyViewMediator.NAME) as BattleReadyViewMediator;
                        // 置空 ViewComponent
                        mediator.ClearPanel();
                    }
                    if (Facade.HasMediator(BattleViewMediator.NAME))
                    {
                        // 获取 mediator
                        BattleViewMediator mediator = Facade.RetrieveMediator(BattleViewMediator.NAME) as BattleViewMediator;

                        mediator.ClearPanel();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
