using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyApplication
{
    public enum SellState { InputNum, EntrySure }    // 出售的状态，分两种：输入数量、确认删除

    public class SellItemTipViewMediator : Mediator
    {
        public static new string NAME = "SellItemTipViewMediator";

        public SellItemTipView MyViewComponent { get { return ViewComponent as SellItemTipView; } }

        private SellState sellState;

        private ItemData dropItemData; // 记录拖拽来的 itemData
        private int dropItemGridIndex; // 记录拖拽来的格子位置

        public SellItemTipViewMediator() : base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            SellItemTipView sellItemTipView = (obj as GameObject).GetComponent<SellItemTipView>();
            ViewComponent = sellItemTipView;

            // 设置默认状态
            sellState = SellState.InputNum;
            // 激活时初始化面板
            MyViewComponent.Init();

            sellItemTipView.inputField.onValueChanged.AddListener(NumberofCorrection);

            sellItemTipView.btnUp.onClick.AddListener(() => { ChangeNumber(10); });
            sellItemTipView.btnDown.onClick.AddListener(() => { ChangeNumber(-10); });

            sellItemTipView.btnSure.onClick.AddListener(EntrySure);
            sellItemTipView.btnCancel.onClick.AddListener(CancelSure);

            //ShowPanel();
        }

        // 更改数量
        private void ChangeNumber(int num)
        {
            string inputStrNum = MyViewComponent.inputField.text;

            int inputNum = 0;
            // 检测输入是否合法
            if (inputStrNum.Equals(""))
                inputNum = 0;
            else
                inputNum = int.Parse(inputStrNum);

            inputNum += num;

            NumberofCorrection(inputNum.ToString());
        }


        // 数量修正
        private void NumberofCorrection(string strNum)
        {
            if (!strNum.Equals(""))
            {
                int maxNum = dropItemData.count;
                // 超出最大数量则修正为最大数量
                if (int.Parse(strNum) > maxNum)
                {
                    MyViewComponent.inputField.text = maxNum.ToString();
                }
                else if (int.Parse(strNum) < 0)
                {
                    MyViewComponent.inputField.text = "0";
                }
                else
                {
                    MyViewComponent.inputField.text = strNum;
                }
            }
        }

        // 确认
        private void EntrySure()
        {
            // 对于输入状态和二次确认状态有两种处理方式
            if (sellState == SellState.InputNum)
            {
                // 更改当前状态
                sellState = SellState.EntrySure;
                // 更新UI面板
                string inputStrNum = MyViewComponent.inputField.text;
                int inputNum = 0;
                // 检测输入是否合法
                if (inputStrNum.Equals(""))
                    inputNum = 0;
                else
                    inputNum = int.Parse(inputStrNum);

                if (inputNum == 0)
                {
                    // 关闭当前UI面板
                    MyViewComponent.m_Tip.SetActive(false);

                    // 更改背包UI面板
                    SendNotification(AppConst.C_UpdateInventory);
                }
                else
                {
                    MyViewComponent.UpdateInfo(sellState, dropItemData, inputNum);
                }
            }
            else
            {
                // 处理数据信息
                int inputNum = 0;
                string inputStrNum = MyViewComponent.inputField.text;

                // 检测输入是否合法
                if (!dropItemData.configData.isStackUp)
                    inputNum = 1;
                else if (inputStrNum.Equals(""))
                    inputNum = 0;
                else
                    inputNum = int.Parse(inputStrNum);

                // 关闭当前UI面板
                MyViewComponent.m_Tip.SetActive(false);

                SendNotification(AppConst.C_ShopSellSure, new ShopSellArgs(dropItemData, dropItemGridIndex, inputNum));
            }
        }

        internal void ClearPanel()
        {
            ViewComponent = null;
        }

        // 取消
        private void CancelSure()
        {
            // 关闭提示UI面板
            MyViewComponent.m_Tip.SetActive(false);
        }

        //public override void ShowPanel()
        //{
            
        //    // 设置UI显示层级为最高
        //    MyViewComponent.transform.SetAsLastSibling();
        //}

        public void ShowSellItemTipView(ItemData itemData, int num)
        {
            MyViewComponent.UpdateInfo(sellState, itemData, num);
        }
        

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                AppConst.C_ShowShopSellTip
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_ShowShopSellTip:
                    DropOnSellPanel(notification);
                    break;
                default:
                    break;
            }
        }

        public void DropOnSellPanel(INotification notification)
        {
            ShopSellArgs shopSellView = notification.Body as ShopSellArgs;

            dropItemData = shopSellView.itemData;
            dropItemGridIndex = shopSellView.gridIndex;

            // 如果是不可叠加物品，直接进入确认状态
            if (!shopSellView.itemData.configData.isStackUp)
                sellState = SellState.EntrySure;
            else
                sellState = SellState.InputNum;

            MyViewComponent.Init();

            // 更新UI面板
            MyViewComponent.UpdateInfo(sellState, shopSellView.itemData);
        }
    }
}

