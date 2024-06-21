using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class MainViewMediator : Mediator
    {
        public new static string NAME = "MainViewMediator";
        public MainView MyViewComponent { get { return ViewComponent as MainView; } }
        public MainViewMediator():base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            MainView mainView = (obj as GameObject).GetComponent<MainView>();
            ViewComponent = mainView;

            // 为UI组件添加一些监听事件
            mainView.btnRecruiting.onClick.AddListener(() =>
            {
                // 显示招募面板
                SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_RecruitingView));
            });

            mainView.btnItem.onClick.AddListener(() =>
            {
                // 显示背包面板
                SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_InventoryView));
            });

            mainView.btnTeam.onClick.AddListener(() =>
            {
                // 显示角色面板
                SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_CharacterView));
            });

            mainView.btnShop.onClick.AddListener(() =>
            {
                // 显示商店面板
                SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_ShopView));
            });

            mainView.btnGameStart.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(3);
            });

            mainView.btnSet.onClick.AddListener(() =>
            {
                // 显示设置面板面板
                SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_MainSettingView));
            });

            ShowPlayerInfo();
        }

        public void ShowPlayerInfo()
        {
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            int currentLevel = saveDataProxy.MyData.currentLevel;
            int currentExp = saveDataProxy.MyData.currentExp;
            int nextExp = saveDataProxy.GetNextLvExp(currentLevel);
            int goldNum = saveDataProxy.GetGoldNum();
            int crystalNum = saveDataProxy.GetCrystalNum();

            MyViewComponent.textLv.text = currentLevel.ToString();
            if(nextExp == 0)
            {
                MyViewComponent.textExp.text = "MAX";
                MyViewComponent.imgExp.fillAmount = 1;
            }
            else
            {
                MyViewComponent.textExp.text = currentExp + "/" + nextExp;
                MyViewComponent.imgExp.fillAmount = currentExp / (float)nextExp;
            }

            MyViewComponent.textGoldNum.text = goldNum.ToString();
            MyViewComponent.textCrystalNum.text = crystalNum.ToString();
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
                AppConst.C_UpdatePlayerInfo,
            };
        }

        // 重写处理通知的方法
        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_UpdatePlayerInfo:
                    ShowPlayerInfo();
                    break;
            }
        }
    }
}
