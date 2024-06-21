using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyApplication
{
    public class ShopView : MonoBehaviour
    {
        [Header("UI组件")]
        public Button btnClose;

        public Toggle togBuy;
        public Toggle togSell;

        public GameObject buyContainer;
        public GameObject buyContainerContent;

        public GameObject sellContainer;

        public void OnDrop(BaseEventData data)
        {
            // 只有左键拖拽而来的物品才进行检测
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                // 获取当前拖拽而来的物品
                ItemFillerView itemFillerView = (data as PointerEventData).pointerDrag.GetComponent<ItemFillerView>();
                // 鼠标抓个空物体过来也能触发 Drop 事件，就离谱，这里做一个判断
                if (itemFillerView == null)
                    return;

                // 发送通知处理拖拽放置事件
                GameFacade.Instance.SendNotification(AppConst.C_ShowShopSellTip, new ShopSellArgs(itemFillerView.itemData, itemFillerView.gridIndex));
            }

        }

    }

}

