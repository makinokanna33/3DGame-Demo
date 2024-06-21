using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyApplication
{
    public enum CharacterViewToggleType
    {
        None,
        ToggleProperty,
        ToggleEquip,
        ToggleSkill
    }
    public class CharacterViewMediator : Mediator
    {
        public new static string NAME = "CharacterViewMediator";

        public CharacterView MyViewComponent
        {
            get { return ViewComponent as CharacterView; }
        }

        public CharacterViewToggleType toggleType;
        public int currentCharacterId;
        public CharacterViewMediator() : base(NAME)
        {
            toggleType = CharacterViewToggleType.None;
            currentCharacterId = -1;
        }

        public override void SetView(object obj)
        {
            CharacterView characterView = (obj as GameObject).GetComponent<CharacterView>();
            ViewComponent = characterView;

            GenerateCharacterPictureView();

            OnEnable();

            characterView.btnClose.onClick.AddListener(() =>
            {
                characterView.gameObject.SetActive(false);
            });

            characterView.toggleProperty.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    if (toggleType != CharacterViewToggleType.ToggleProperty)
                    {
                        toggleType = CharacterViewToggleType.ToggleProperty;
                        SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(currentCharacterId));
                    }
                }
            });

            characterView.toggleEquip.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    if (toggleType != CharacterViewToggleType.ToggleEquip)
                    {
                        toggleType = CharacterViewToggleType.ToggleEquip;
                        SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(currentCharacterId));
                    }
                }
            });

            characterView.toggleSkill.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    if (toggleType != CharacterViewToggleType.ToggleSkill)
                    {
                        toggleType = CharacterViewToggleType.ToggleSkill;
                        SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(currentCharacterId));
                    }
                }
            });

            characterView.toggleSkill.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    if (toggleType != CharacterViewToggleType.ToggleSkill)
                    {
                        toggleType = CharacterViewToggleType.ToggleSkill;
                        SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(currentCharacterId));
                    }
                }
            });

            characterView.btnCapUp.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_CharacterItemLvUp,
                    new CharacterLvUpArgs(currentCharacterId, CharacterItemType.cap, MyViewComponent.upItem, MyViewComponent.addUpNum));
            });

            characterView.btnArmorUp.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_CharacterItemLvUp,
                    new CharacterLvUpArgs(currentCharacterId, CharacterItemType.armor, MyViewComponent.upItem, MyViewComponent.addUpNum));
            });

            characterView.btnSkill1Unlock.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_CharacterSkillLock,
                    new CharacterSkillLockArgs(currentCharacterId, 0, MyViewComponent.unlockSkillItem, MyViewComponent.unlockSkillNum));
            });
            characterView.btnSkill2Unlock.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_CharacterSkillLock,
                    new CharacterSkillLockArgs(currentCharacterId, 1, MyViewComponent.unlockSkillItem, MyViewComponent.unlockSkillNum));
            });
            characterView.btnSkill3Unlock.onClick.AddListener(() =>
            {
                SendNotification(AppConst.C_CharacterSkillLock,
                    new CharacterSkillLockArgs(currentCharacterId, 2, MyViewComponent.unlockSkillItem, MyViewComponent.unlockSkillNum));
            });
        }

        public void ClearPanel()
        {
            ViewComponent = null;
            toggleType = CharacterViewToggleType.ToggleProperty;
            currentCharacterId = -1;
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

            // 更新UI视图
            var childObj = MyViewComponent.teamInfoContent.transform.GetChild(0);
            if (childObj != null)
            {
                toggleType = CharacterViewToggleType.ToggleProperty;
                currentCharacterId = childObj.GetComponent<CharacterPictureView>().id;
                childObj.GetComponent<CharacterPictureView>().toggle.isOn = true;
                MyViewComponent.toggleProperty.isOn = true;
                SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(currentCharacterId));
            }
        }

        // 根据玩家拥有角色数据列表生成角色列表图
        private void GenerateCharacterPictureView()
        {
            // 先清空之前已有的角色列表图
            for (int i = 0; i < MyViewComponent.teamInfoContent.transform.childCount; i++)
            {
                GameManager.Destroy(MyViewComponent.teamInfoContent.transform.GetChild(i).gameObject);
            }

            // 根据数据生成新的角色列表图
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            foreach (var item in characterDataProxy.playerDataList)
            {
                CharacterPictureView obj;
#if UNITY_EDITOR
                obj = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterPictureView>("Assets/ABRes/Prefabs/CharacterPictureView.prefab"));
#else
                obj = Object.Instantiate(ABManager.Instance.LoadRes<CharacterPictureView>(AppConst.AB_Prefabs, AppConst.AB_CharacterPictureView));
#endif
                obj.transform.SetParent(MyViewComponent.teamInfoContent.transform, false);
                obj.GetComponent<CharacterPictureView>().Init(item.configData.id, item.configData.sprite, MyViewComponent.teamToggleGroup);
            }
        }

        private void UpCharacterItemData()
        {
            //if(currentCharacterId == -1)
            //    return;

            //// 根据 id 找到角色数据
            //CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            //var data = characterDataProxy.FindCharacterData(currentCharacterId, CharacterCamp.Player);

            //// 提升一级等级
            //data.capItemData.level += 1;

            //SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            //int upItemNum = saveDataProxy.GetItemNum(MyViewComponent.lockItem.itemConfigData.id, MyViewComponent.lockItem.itemConfigData.tabType);

        }

        // 重写监听通知的方法
        public override string[] ListNotificationInterests()
        {
            // 返回需要监听通知的字符串
            return new string[]
            {
                AppConst.C_UpdateCharacterPictureView,
                AppConst.C_UpdateCharacterInfo,
                AppConst.C_ResetUpdateCharacterInfo,
            };
        }

        // 重写处理通知的方法
        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_UpdateCharacterPictureView:
                    GenerateCharacterPictureView();
                    break;
                case AppConst.C_UpdateCharacterInfo:
                    CharacterArgs characterArgs = notification.Body as CharacterArgs;
                    if(characterArgs != null)
                        currentCharacterId = characterArgs.id;
                    if(currentCharacterId != -1)
                        ShowCharacterInfo(currentCharacterId);
                    break;
                case AppConst.C_ResetUpdateCharacterInfo:
                    CharacterArgs characterArgs1 = notification.Body as CharacterArgs;
                    currentCharacterId = characterArgs1.id;
                    toggleType = CharacterViewToggleType.ToggleProperty;
                    MyViewComponent.toggleProperty.isOn = true;
                    SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(currentCharacterId));
                    break;
            }
        }

        private void ShowCharacterInfo(int characterArgsId)
        {
            HideAllContainer();

            switch (toggleType)
            {
                case CharacterViewToggleType.ToggleProperty:
                    ShowProperty(characterArgsId);
                    break;
                case CharacterViewToggleType.ToggleEquip:
                    ShowEquip(characterArgsId);
                    break;
                case CharacterViewToggleType.ToggleSkill:
                    ShowSkill(characterArgsId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 展示属性面板
        private void ShowProperty(int characterArgsId)
        {
            MyViewComponent.propertyContainer.SetActive(true);

            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;

            // 根据 id 找到角色数据
            var data = characterDataProxy.FindCharacterData(characterArgsId, CharacterCamp.Player);
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            // 更新 UI 面板
            MyViewComponent.InitPropertyContainer(data, saveDataProxy.GetNextLvExp(data.level));
        }

        // 展示装备面板
        private void ShowEquip(int characterArgsId)
        {
            MyViewComponent.equipContainer.SetActive(true);

            // 根据 id 找到角色数据
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            var data = characterDataProxy.FindCharacterData(characterArgsId, CharacterCamp.Player);

            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            int upItemNum = saveDataProxy.GetItemNum(MyViewComponent.upItem.itemConfigData.id, MyViewComponent.upItem.itemConfigData.tabType);
            
            // 更新 UI 面板
            MyViewComponent.InitEquipContainer(data, upItemNum);
        }

        // 展示技能面板
        private void ShowSkill(int characterArgsId)
        {
            MyViewComponent.skillContainer.SetActive(true);

            // 根据 id 找到角色数据
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            var data = characterDataProxy.FindCharacterData(characterArgsId, CharacterCamp.Player);
            
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            int lockSkillNum = saveDataProxy.GetItemNum(MyViewComponent.unlockSkillItem.itemConfigData.id, MyViewComponent.unlockSkillItem.itemConfigData.tabType);

            // 更新 UI 面板
            MyViewComponent.InitSkillContainer(data, lockSkillNum);
        }


        private void HideAllContainer()
        {
            MyViewComponent.HideAllContainer();
        }
    }
}

