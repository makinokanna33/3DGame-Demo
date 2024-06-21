using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class StartViewMediator : Mediator
    {
        public new static string NAME = "StartViewMediator";

        public StartViewMediator():base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            var startView = (obj as GameObject).GetComponent<StartView>();
            ViewComponent = startView;

            // 为UI组件添加一些监听事件
            startView.btnStart.onClick.AddListener(() =>
            {
                // 读取资源数据
                SendNotification(AppConst.C_LoadData);
                // 进入主界面
                SceneManager.LoadScene(2);
            });
            startView.btnExit.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_SaveData);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        public void ClearPanel()
        {
            ViewComponent = null;
        }

        // 重写监听通知的方法
        public override string[] ListNotificationInterests()
        {
            // 返回需要监听通知的字符串
            return new string[]
            {

            };
        }

        // 重写处理通知的方法
        public override void HandleNotification(INotification notification)
        {
            //switch(notification.Name)
            //{

            //}
        }
    }
}
