using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyApplication
{
    public class PictureFrameView : MonoBehaviour
    {
        public Image picture;
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;

        [HideInInspector] public int id;
        [HideInInspector] public bool isEnable = true;
        [HideInInspector] public Transform myFather;
        [HideInInspector] public Canvas canvas;
        [HideInInspector] public CanvasScaler canvasScaler;
        [HideInInspector] public Transform transformAnchor; // 英雄登场的位置，若为 null 则英雄没有登场

        private void Start()
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();
            transformAnchor = null;
        }

        public void Init(int id, Sprite sprite, Transform father)
        {
            this.id = id;
            picture.sprite = sprite;
            this.myFather = father;
            
        }

        public void OnBeginDrag(BaseEventData data)
        {
            // 只有左键才可拖拽
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                // 取消阻止射线检测
                canvasGroup.blocksRaycasts = false;
                transformAnchor = null;

                // 改变父物体让物品UI显示在最上层
                transform.SetParent(canvas.transform);
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
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }

        }
        public void OnEndDrag(BaseEventData data)
        {
            // 只有左键才可拖拽
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                // 开启阻止射线检测
                canvasGroup.blocksRaycasts = true;

                // 生成一条从摄像机发出的射线
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // 用来存储射线打中物体的信息
                RaycastHit hit;
                // 发射射线
                bool result = Physics.Raycast(ray, out hit);

                if(hit.transform.name == "PlayerTransform")
                {
                    //var resolutionX = this.canvasScaler.referenceResolution.x;
                    //var resolutionY = this.canvasScaler.referenceResolution.y;
                    //var offset =
                    //    (Screen.width / this.canvasScaler.referenceResolution.x) *
                    //    (1 - this.canvasScaler.matchWidthOrHeight) +
                    //    (Screen.height / this.canvasScaler.referenceResolution.y) *
                    //    this.canvasScaler.matchWidthOrHeight;
                    var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, hit.transform.position);
                    this.transform.position = new Vector3(screenPos.x, screenPos.y, 0);
                    transformAnchor = hit.transform.parent.transform;
                }
                else
                {
                    transform.SetParent(myFather);
                }

                GameFacade.Instance.SendNotification(AppConst.C_OnEndDragPictureFrameView);
            }

        }
    }
}



