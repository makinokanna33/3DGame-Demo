using FrameWork.Base;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ItemFillerViewMediator : Mediator
    {
        public static new string NAME = "ItemFillerViewMediator";

        // 获取 Canvas
        private Canvas m_Canvas;

        // 存储 ItemFillerView
        private List<ItemFillerView> itemFillerViewList;
        public Canvas Canvas
        {
            get
            {
                if (m_Canvas == null)
                    m_Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                return m_Canvas;
            }
            set
            {
                m_Canvas = value;
            }
        }

        public ItemFillerViewMediator() : base(NAME)
        {
            Canvas = null;
            itemFillerViewList = new List<ItemFillerView>();
        }


        public void CreateView(ItemData itemData, GameObject parentObj, int gridIndex)
        {
            ItemFillerView itemFillerView;
#if UNITY_EDITOR
            itemFillerView = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemFillerView>("Assets/ABRes/Prefabs/ItemFillerView.prefab"));
#else
            itemFillerView = Object.Instantiate(ABManager.Instance.LoadRes<ItemFillerView>(AppConst.AB_Prefabs, AppConst.AB_ItemFillerView));
#endif
            // 更新UI信息
            itemFillerView.UpdateInfo(itemData);
            itemFillerView.gridIndex = gridIndex;

            // 设置父物体
            itemFillerView.gameObject.transform.SetParent(parentObj.transform, false);

            // 添加拖拽事件
            itemFillerView.AddDragEvent();

            // 添加到列表中进行管理
            itemFillerViewList.Add(itemFillerView);
        }

        public void HideView(Mediator mediator)
        {
            for (int i = itemFillerViewList.Count - 1; i >= 0; i--)
            {
                // 销毁游戏物体
                Object.Destroy(itemFillerViewList[i].gameObject);
                // 从列表中移除
                itemFillerViewList.RemoveAt(i);
            }
        }

        public override void SetView(object obj)
        {
            base.SetView(obj);
        }

        public void ClearPanel()
        {
            itemFillerViewList.Clear();
        }
        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
            //Consts.E_BeginDragInventory,
            //Consts.E_EndDragInventory,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            //switch (notification.Name)
            {
                //case Consts.E_BeginDragInventory:
                //    IsDrag = true;
                //    break;
                //case Consts.E_EndDragInventory:
                //    IsDrag = false;
                //    break;
                //default:
                //    break;
            }
        }


    }
}