using System.Reflection;
using FrameWork.Base;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class ShowPanelCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            UIPanelArgs uiPanelArgs = notification.Body as UIPanelArgs;

            string viewName = uiPanelArgs.panelName;

            // 获取视图名对应的中介者和类名
            GameDataProxy gameDataProxy = Facade.RetrieveProxy(GameDataProxy.NAME) as GameDataProxy;
            gameDataProxy.GetViewConfig(viewName, out string mediatorName);

            // 如果没有mediator就实例化一个
            if (!Facade.HasMediator(mediatorName))
            {
                // 利用反射进行解耦，避免重复修改该类
                string instance_name = AppConst.NameSpace + "." + mediatorName;
                IMediator mediator =
                    Assembly.Load("Assembly-CSharp").CreateInstance(instance_name) as IMediator;
                Facade.RegisterMediator(mediator);
            }   

            // 获取 mediator
            IMediator myMediator = Facade.RetrieveMediator(mediatorName);

            // 实例化面板
            // 如果 ViewComponent 为空才进行面板的实例化
            if (myMediator.ViewComponent == null)
            {
                // 加载UI资源
#if UNITY_EDITOR
                GameObject obj = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/UIPanel/" + viewName + ".prefab"));
#else
            GameObject obj = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_UIPanel, viewName);
#endif
                obj.transform.SetParent(GameObject.Find("Canvas").transform, false);

                // 将预设体上的 view 脚本与 mediator 相关联
                myMediator.SetView(obj);
            }
            else
            {
                // 直接显示面板
                myMediator.ShowPanel();
            }
        }
    }
}
