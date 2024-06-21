using PureMVC.Patterns.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class MainSettingViewMediator : Mediator
    {
        public new static string NAME = "MainSettingViewMediator";
        public MainSettingView MyViewComponent { get { return ViewComponent as MainSettingView; } }
        public MainSettingViewMediator() : base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            MainSettingView mainSettingView = (obj as GameObject).GetComponent<MainSettingView>();
            ViewComponent = mainSettingView;

            mainSettingView.btnEnterStart.onClick.AddListener(() =>
            {
                // 进入开始界面
                SceneManager.LoadScene(1);
            });

            mainSettingView.btnOut.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_SaveData);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });

            mainSettingView.btnCancel.onClick.AddListener(() =>
            {
                mainSettingView.gameObject.SetActive(false);
            });
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
        }

        public void ClearPanel()
        {
            ViewComponent = null;
        }
    }
}