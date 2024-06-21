using PureMVC.Interfaces;
using PureMVC.Patterns.Command;

namespace MyApplication
{
    public class EnterSceneCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            var sceneArgs = notification.Body as SceneArgs;

            if (sceneArgs == null)
            {
                LoggerManager.Instance.LogCommandArgsError("EnterSceneCommand", "SceneArgs");
                return;
            }

            switch (sceneArgs.sceneBuildIndex)
            {
                case 1:
                    // 发送通知
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_StartView)); // 开启开始界面面板
                    break;
                case 2:
                    // 发送通知               
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_MainView)); // 开启主界面
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_ShopBuyTipView)); // 开启购买确认面板
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_ToolTipView)); // 开启提示面板
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_SellItemTipView)); // 开启出售面板
                    break;
                case 3:
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_MapView)); // 开启地图面板
                    break;
                case 4:
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_BattleView));
                    break;
                case 5:
                    SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_BattleView));
                    break;
                default:
                    break;
            }
        }
    }
}
