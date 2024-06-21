using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.EventSystems;
using System;

namespace MyApplication
{
    public class InventoryViewMediator : Mediator
    {
        public static new string NAME = "InventoryViewMediator";

        public InventoryView MyViewComponent { get { return ViewComponent as InventoryView; } }
        
        //private GameDataProxy m_gameDataProxy;
        private ItemType m_NowTabType = ItemType.Equip;
        public ItemType NowTabType { get => m_NowTabType; }

        public InventoryViewMediator() : base(NAME)
        {
            // ItemViewMediator 的注册在每个需要 ItemView 的代理中都要进行判断注册
            if (!Facade.HasMediator(ItemFillerViewMediator.NAME))
                Facade.RegisterMediator(new ItemFillerViewMediator());
            
            //m_gameDataProxy = m_gameDataProxy = GameFacade.Instance.RetrieveProxy(GameDataProxy.NAME) as GameDataProxy;
        }
        public override void SetView(object obj)
        {
            InventoryView inventoryView = (obj as GameObject).GetComponent<InventoryView>();
            ViewComponent = inventoryView;

            // 激活面板
            OnEnable();

            inventoryView.btnClose.onClick.AddListener(() =>
            {
                inventoryView.gameObject.SetActive(false);
            });

            inventoryView.togEquip.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    m_NowTabType = ItemType.Equip;
                    SendNotification(AppConst.C_UpdateInventory);
                }

            });

            inventoryView.togMaterials.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    m_NowTabType = ItemType.Material;
                    SendNotification(AppConst.C_UpdateInventory);
                }
            });

            //// 为每个格子添加物品放置事件
            //for (int i = 0; i < MyViewComponent.itemGrids.Length; i++)
            //{
            //    EventTrigger trigger = MyViewComponent.itemGrids[i].GetComponent<EventTrigger>();

            //    int tmpIndex = i;

            //    EventTrigger.Entry entry = new EventTrigger.Entry() { eventID = EventTriggerType.Drop };
            //    entry.callback.AddListener((data) =>
            //    {
            //        // 只有左键拖拽而来的物品才进行检测
            //        if ((data as PointerEventData).toggle == PointerEventData.InputButton.Left)
            //        {
            //            IDropView dropView = (data as PointerEventData).pointerDrag.GetComponent<IDropView>();

            //            // 鼠标抓个空物体过来也能触发 Drop 事件，就离谱，这里做一个判断
            //            if (dropView == null)
            //                return;

            //            SendNotification(Consts.E_DropOnInventoryGrid, new DropOnGridArgs(tmpIndex, dropView));
            //        }
            //    });
            //    trigger.triggers.Add(entry);
            //}
        }

        public override void ShowPanel()
        {
            // 面板未激活时才进行激活显示
            if (!MyViewComponent.gameObject.activeSelf)
                OnEnable();
        }

        private void OnEnable()
        {
            // 激活面板
            MyViewComponent.gameObject.SetActive(true);

            // 更新UI视图
            SendNotification(AppConst.C_UpdateInventory);
        }

        public void ClearPanel()
        {
            ViewComponent = null;
            m_NowTabType = ItemType.Equip;
        }

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                AppConst.C_ShowInventoryItem,
                AppConst.C_HideInventoryItem,
            //Consts.E_BeginDragProperty,
            //Consts.E_PointerDownProperty,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_HideInventoryItem:
                    HideItem();
                    break;
                case AppConst.C_ShowInventoryItem:
                    ShowItemArgs showItemArgs = notification.Body as ShowItemArgs;
                    ShowItem(showItemArgs.gridIndex, showItemArgs.itemData);
                    break;
                    //case Consts.E_BeginDragProperty:
                    //    m_NowTabType = TabType.Equip;
                    //    OnEnable(); // 更新UI视图
                    //    break;
                    //case Consts.E_PointerDownProperty:
                    //    m_NowTabType = TabType.Equip;
                    //    OnEnable(); // 更新UI视图
                    //    break;
                    //default:
                    //    break;
            }
        }

        private void HideItem()
        {
            ItemFillerViewMediator itemViewMediator = Facade.RetrieveMediator(ItemFillerViewMediator.NAME) as ItemFillerViewMediator;
            itemViewMediator.HideView(this);
        }

        private void ShowItem(int gridIndex, ItemData itemData)
        {
            // 如果格子位置越界 或 格子下已有物品，进行报错提示
            if (gridIndex > MyViewComponent.itemGrids.Length - 1 || MyViewComponent.itemGrids[gridIndex].transform.Find("ItemFillerView") != null)
            {
                LoggerManager.Instance.LogError("背包栏物品数量越界或格子位置重复，请检查玩家数据是否有误！");
            }
            else
            {
                ItemFillerViewMediator itemFillerViewMediator = Facade.RetrieveMediator(ItemFillerViewMediator.NAME) as ItemFillerViewMediator;
                itemFillerViewMediator.CreateView(itemData, MyViewComponent.itemGrids[gridIndex], gridIndex);
            }
        }
    }
}


