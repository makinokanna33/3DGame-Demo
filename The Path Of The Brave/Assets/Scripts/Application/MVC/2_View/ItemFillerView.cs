using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyApplication
{
    public class ItemFillerView : MonoBehaviour
    {
        [Header("UI组件")]
        public Image imgItem;
        public Text textNum;

        [Header("组件")]
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;

        [HideInInspector]public ItemData itemData;
        [HideInInspector]public GameObject parentObj;
        [HideInInspector]public int gridIndex;
        
        // 获取 Canvas
        private Canvas m_Canvas;
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
        public string Text
        {
            get
            {
                return textNum.text;
            }
            set
            {
                if (int.Parse(value) <= 1)
                {
                    // 数量为1，则隐藏数量文本
                    textNum.gameObject.SetActive(false);
                    // 做个数据保护，传入的数量如果小于1，则自动修正为1
                    textNum.text = "1";
                }
                else
                {
                    textNum.gameObject.SetActive(true);
                    textNum.text = value;
                }
            }
        }

        /// <summary>
        /// 更新UI面板
        /// </summary>
        public void UpdateInfo(ItemData itemData)
        {
            this.itemData = itemData;

            this.Text = this.itemData.count.ToString();

            this.imgItem.sprite = this.itemData.configData.icon;

            rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        public void AddDragEvent()
        {
            // 获取 EventTrigger 组件
            EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();

            // 添加事件        
            AddEvent(eventTrigger, EventTriggerType.BeginDrag, OnBeginDrag);        // 开始拖拽     
            AddEvent(eventTrigger, EventTriggerType.Drag, OnDrag);                  // 拖拽 
            AddEvent(eventTrigger, EventTriggerType.EndDrag, OnEndDrag);            // 拖拽结束
        }

        private void AddEvent(EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> call)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry() { eventID = triggerType };
            entry.callback.AddListener(call);
            eventTrigger.triggers.Add(entry);
        }

        public void OnPointerEnter(BaseEventData data)
        {
            // 发送事件通知，展示装备提示面板
            GameFacade.Instance.SendNotification(AppConst.C_ShowTip, new ShowTipArgs(gameObject.transform.position, itemData));
        }
        public void OnPointerExit(BaseEventData data)
        {
            // 关闭装备提示面板展示
            GameFacade.Instance.SendNotification(AppConst.C_HideTip);
        }

        public void OnBeginDrag(BaseEventData data)
        {
            // 只有左键才可拖拽
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                // 取消阻止射线检测
                canvasGroup.blocksRaycasts = false;

                parentObj = transform.parent.gameObject;

                // 改变父物体让物品UI显示在最上层
                transform.SetParent(Canvas.transform);
                transform.SetAsLastSibling();
            }

        }
        public void OnDrag(BaseEventData data)
        {
            // 只有左键才可拖拽
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                PointerEventData eventData = data as PointerEventData;

                // 由于 canvas 画布大小不一定为1，所以需要用 delta / canvas.scaleFactor
                rectTransform.anchoredPosition += eventData.delta / Canvas.scaleFactor;
            }

        }
        public void OnEndDrag(BaseEventData data)
        {
            // 只有左键才可拖拽
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                // 开启阻止射线检测
                canvasGroup.blocksRaycasts = true;

                GameFacade.Instance.SendNotification(AppConst.C_UpdateInventory);
            }

        }
    }
}