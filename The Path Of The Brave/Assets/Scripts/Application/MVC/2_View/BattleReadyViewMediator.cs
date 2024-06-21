using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MyApplication
{
    public class BattleReadyViewMediator : Mediator
    {
        public new static string NAME = "BattleReadyViewMediator";
        public BattleReadyView MyViewComponent { get { return ViewComponent as BattleReadyView; } }

        public List<PictureFrameView> pictureFrameViews = new List<PictureFrameView>();

        public BattleReadyViewMediator() : base(NAME)
        {
            pictureFrameViews.Clear();
        }

        public override void SetView(object obj)
        {
            BattleReadyView battleReadyView = (obj as GameObject).GetComponent<BattleReadyView>();
            ViewComponent = battleReadyView;

            // 为UI组件添加一些监听事件
            battleReadyView.btnStart.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_BattleReadyEnd);
                MyViewComponent.gameObject.SetActive(false);
            });

            // 发送通知生成玩家角色列表
            SendNotification(AppConst.C_BattleReadyBegin);
        }

        public void GeneratePictureFrameView(int id, Sprite sprite)
        {
            // 加载UI资源
#if UNITY_EDITOR
            GameObject obj = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/UIPanel/V_PictureFrameView.prefab"));
#else
            GameObject obj = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_UIPanel, "V_PictureFrameView");
#endif
            obj.transform.SetParent(MyViewComponent.content.transform, false);
            obj.GetComponent<PictureFrameView>().Init(id, sprite, MyViewComponent.content.transform);

            pictureFrameViews.Add(obj.GetComponent<PictureFrameView>());
        }

        void ChangeBattleReadyButton()
        {
            foreach (var item in pictureFrameViews)
            {
                // 只要有一位英雄登场就可以开战
                if(item.transformAnchor != null)
                {
                    MyViewComponent.btnStart.interactable = true;
                    return;
                }
            }

            MyViewComponent.btnStart.interactable = false;
        }

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                AppConst.C_OnEndDragPictureFrameView,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_OnEndDragPictureFrameView:
                    ChangeBattleReadyButton();
                    break;
                default:
                    break;
            }
        }

        public void ClearPanel()
        {
            ViewComponent = null;
            pictureFrameViews.Clear();
        }
    }
}

