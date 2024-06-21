using System;
using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyApplication
{
    public class RecruitingViewMediator : Mediator
    {
        public new static string NAME = "RecruitingViewMediator";

        public RecruitingView MyViewComponent
        {
            get { return ViewComponent as RecruitingView; }
        }

        private List<int> probList = new List<int>();

        public RecruitingViewMediator() : base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            RecruitingView view = (obj as GameObject).GetComponent<RecruitingView>();
            ViewComponent = view;

            OnEnable();
            InitProbList();

            view.btnClose.onClick.AddListener(() =>
            {
                view.gameObject.SetActive(false);
            }); 

            view.btnStart.onClick.AddListener(() =>
            {
                BeginRecruiting();
            });

            view.btnSure.onClick.AddListener(() =>
            {
                view.recruitingResult.SetActive(false);
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

            // 初始化面板
            InitTextNum();
        }

        private void InitTextNum()
        {
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            int num = saveDataProxy.GetItemNum(MyViewComponent.itemBase.itemConfigData.id,
                MyViewComponent.itemBase.itemConfigData.tabType);
            MyViewComponent.UpdateTextNum(num);
        }

        // 初始化概率列表
        private void InitProbList()
        {
            probList.Clear();

            foreach (var recruiting in MyViewComponent.recruitingCharacterDataList)
            {
                for (int i = 0; i < recruiting.prob; i++)
                {
                    probList.Add(recruiting.character.configData.id);
                }
            }
        }

        // 招募角色
        private void BeginRecruiting()
        {
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            // 消耗一张召唤符
            saveDataProxy.DelItem(MyViewComponent.itemBase.itemConfigData.tabType,
                MyViewComponent.itemBase.itemConfigData.id, 1);

            // 随机抽取角色
            int resultId = probList[Random.Range(0, probList.Count)];
            var characterData = characterDataProxy.FindCharacterDataSo(resultId);

            // 判断角色是否已拥有
            bool flag = false;
            foreach (var item in characterDataProxy.playerDataList)
            {
                // 玩家已重复拥有该角色
                if (item.configData.id == resultId)
                {
                    flag = true;

                    // 为玩家新增 400 金币
                    saveDataProxy.AddGoldNum(400);

                    ShowRecruitingResult(string.Format("角色[{0}]已拥有，已兑换为400金币！", characterData.configData.name),
                        characterData.configData.sprite);
                }
            }

            // 若玩家未拥有该角色，则为玩家添加该角色
            if (!flag)
            {
                // 为玩家新增角色
                saveDataProxy.AddCharacter(resultId);

                // 更新角色数据
                characterDataProxy.ClearCharacterData(CharacterCamp.Player);
                foreach (var item in saveDataProxy.MyData.characterJsonDataList)
                {
                    characterDataProxy.InitCharacterData(item.id, item.level, item.currentExp, item.capLevel, item.armorLevel, item.skillUnLock, CharacterCamp.Player);
                }

                ShowRecruitingResult("恭喜获得新角色：" + characterData.configData.name, characterData.configData.sprite);
            }

            // 更新招募 UI
            SendNotification(AppConst.C_UpdateRecruiting);

            // 更新玩家信息 UI
            SendNotification(AppConst.C_UpdatePlayerInfo);

            // 更新背包 UI
            SendNotification(AppConst.C_UpdateInventory);

            // 更新商店 UI
            SendNotification(AppConst.C_UpdateShop);

            // 更新队伍 UI
            SendNotification(AppConst.C_UpdateCharacterPictureView);
        }

        // 展示招募结果
        private void ShowRecruitingResult(string textInfo, Sprite sprite)
        {
            MyViewComponent.recruitingResult.SetActive(true);
            MyViewComponent.textInfo.text = textInfo;
            MyViewComponent.picture.sprite = sprite;
        }

        // 重写监听通知的方法
        public override string[] ListNotificationInterests()
        {
            // 返回需要监听通知的字符串
            return new string[]
            {
                AppConst.C_UpdateRecruiting
            };
        }

        // 重写处理通知的方法
        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_UpdateRecruiting:
                    InitTextNum();
                    break;
            }
    }
    }
}
