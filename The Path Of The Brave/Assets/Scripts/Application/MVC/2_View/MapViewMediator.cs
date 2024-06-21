using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class MapViewMediator : Mediator
    {
        public new static string NAME = "MapViewMediator";

        public MapView MyViewComponent
        {
            get { return ViewComponent as MapView; }
        }

        private int currentSceneIndex;

        public MapViewMediator() : base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            MapView view = (obj as GameObject).GetComponent<MapView>();
            ViewComponent = view;

            OnEnable();

            view.btnBack.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(2);
            });

            view.btnStart.onClick.AddListener(() =>
            {
                if (currentSceneIndex == 0)
                {
                    GameManager.Instance.StartCoroutine(LoadScene(4));
                }
                else if(currentSceneIndex == 1)
                {
                    GameManager.Instance.StartCoroutine(LoadScene(5));
                }
            });

            view.btnBox1Lock.onClick.AddListener(() =>
            {
                // 获取配置 1 的奖励
                SendNotification(AppConst.C_GetStarAward, 0);
            });

            view.btnBox2Lock.onClick.AddListener(() =>
            {
                // 获取配置 2 的奖励
                SendNotification(AppConst.C_GetStarAward, 1);
            });
        }

        public void ClearPanel()
        {
            ViewComponent = null;
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

            // 根据玩家数据更新地图按钮状态
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            var dataList = saveDataProxy.MyData.levelJsonDataList;
            for (int i = 0; i < dataList.Count; ++i)
            {
                MyViewComponent.UpdateLevelButtonStatus(i, dataList[i]);
            }

            UpdateSelectLevelInfo(0);
            MyViewComponent.UpdateStarAward(saveDataProxy.GetStarAward(), saveDataProxy.GetHasStarNum());
        }

        // 重写监听通知的方法
        public override string[] ListNotificationInterests()
        {
            // 返回需要监听通知的字符串
            return new string[]
            {
                AppConst.C_UpdateSelectLevelInfo,
                AppConst.C_ShowAwardPanel,
                AppConst.C_UpdateStarStatus
            };
        }

        // 重写处理通知的方法
        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_UpdateSelectLevelInfo:
                    int sceneIndex = (int)notification.Body;
                    UpdateSelectLevelInfo(sceneIndex);
                    break;
                case AppConst.C_ShowAwardPanel:
                    var awardConfigData = notification.Body as StarAwardConfigData;
                    ShowAwardPanel(awardConfigData);
                    break;
                case AppConst.C_UpdateStarStatus:
                    UpdateStarStatus();
                    break;
            }
        }

        private void UpdateSelectLevelInfo(int sceneIndex)
        {
            currentSceneIndex = sceneIndex;

            SelectLevelDataProxy dataProxy = Facade.RetrieveProxy(SelectLevelDataProxy.NAME) as SelectLevelDataProxy;
            MyViewComponent.UpdateLevelInfo(dataProxy.GetSelectLevelData(sceneIndex));
        }

        private void ShowAwardPanel(StarAwardConfigData configData)
        {
            MyViewComponent.ShowAwardPanel(configData);
        }

        private void UpdateStarStatus()
        {
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            MyViewComponent.UpdateStarAward(saveDataProxy.GetStarAward(), saveDataProxy.GetHasStarNum());
        }

        IEnumerator LoadScene(int sceneIndex)
        {
            yield return null;

            MyViewComponent.loading.SetActive(true);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                MyViewComponent.imageLoading.fillAmount = asyncOperation.progress;

                if (asyncOperation.progress >= 0.9f)
                {
                    MyViewComponent.imageLoading.fillAmount = 1;
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
