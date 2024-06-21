using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ToolTipViewMediator : Mediator
    {
        public static new string NAME = "ToolTipViewMediator";
        public ToolTipVIew MyViewComponent { get { return ViewComponent as ToolTipVIew; } }
        public ToolTipViewMediator() : base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            ToolTipVIew toolTipVIew = (obj as GameObject).GetComponent<ToolTipVIew>();
            ViewComponent = toolTipVIew;

            // 实例化后先隐藏自己
            toolTipVIew.gameObject.SetActive(false);
        }

        public void ClearPanel()
        {
            ViewComponent = null;
        }

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                AppConst.C_ShowTip,
                AppConst.C_HideTip
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_ShowTip:
                    ShowTip(notification);
                    break;
                case AppConst.C_HideTip:
                    HideTip();
                    break;
                default:
                    break;
            }
        }
        private void ShowTip(INotification notification)
        {
            ItemFillerViewMediator mediator = null;

            if (Facade.HasMediator(ItemFillerViewMediator.NAME))
                mediator = Facade.RetrieveMediator(ItemFillerViewMediator.NAME) as ItemFillerViewMediator;

            // 若物品中介不存在，就不显示Tip面板
            if (mediator == null)
                return;

            ShowTipArgs showTipArgs = notification.Body as ShowTipArgs;
            MyViewComponent.UpdateInfo(showTipArgs.position, showTipArgs.itemData.GetMessage());
        }
        private void HideTip()
        {
            MyViewComponent.gameObject.SetActive(false);
        }
    }
}