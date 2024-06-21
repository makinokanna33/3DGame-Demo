using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyApplication
{
    public class BattleViewMediator : Mediator
    {
        public new static string NAME = "BattleViewMediator";

        public BattleView MyViewComponent
        {
            get { return ViewComponent as BattleView; }
        }

        public BattleViewMediator() : base(NAME)
        {

        }

        public override void SetView(object obj)
        {
            BattleView view = (obj as GameObject).GetComponent<BattleView>();
            ViewComponent = view;

            MyViewComponent.gameObject.SetActive(false);

            MyViewComponent.btnWait.onClick.AddListener(() => { GameManager.Instance.battleManager.Wait(); });

            MyViewComponent.skillUIs[0].btnSkill.onClick.AddListener(() =>
            {
                GameManager.Instance.battleManager.UseSkill(0);
            });

            MyViewComponent.skillUIs[1].btnSkill.onClick.AddListener(() =>
            {
                GameManager.Instance.battleManager.UseSkill(1);
            });

            MyViewComponent.skillUIs[2].btnSkill.onClick.AddListener(() =>
            {
                GameManager.Instance.battleManager.UseSkill(2);
            });

            MyViewComponent.btnSelectTargetCancel.onClick.AddListener(() =>
            {
                GameManager.Instance.battleManager.CancelSkillSelectTarget();
            });

            MyViewComponent.btnSkillConfirmSure.onClick.AddListener(ConfirmClick);

            MyViewComponent.btnSkillConfirmCancel.onClick.AddListener(() =>
            {
                GameManager.Instance.battleManager.CancelSkillConfirm();
            }); 
            
            MyViewComponent.awardSure.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(3);
            }); 
            
            MyViewComponent.loseSure.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(3);
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
            MyViewComponent.midTip.SetActive(false);
            MyViewComponent.startTip.SetActive(false);
            MyViewComponent.playerTip.SetActive(false);
            MyViewComponent.enemyTip.SetActive(false);

            MyViewComponent.actionPanel.SetActive(false);
            MyViewComponent.skillSelectTarget.SetActive(false);
            MyViewComponent.skillConfirm.SetActive(false);

            // 激活面板
            MyViewComponent.gameObject.SetActive(true);
        }

        public void ShowHudDamage(int damage, Vector3 worldPos)
        {
            MyViewComponent.ShowHudDamage(damage, worldPos);
        }

        public void ShowHudRestore(int num, Vector3 worldPos)
        {
            MyViewComponent.ShowHudRestore(num, worldPos);
        }

        // 重写监听通知的方法
        public override string[] ListNotificationInterests()
        {
            // 返回需要监听通知的字符串
            return new string[]
            {
                AppConst.C_BattleReadyEnd,
                AppConst.C_RoundBegin,
                AppConst.C_EnemyReady,
                AppConst.C_ShowHudDamage,
                AppConst.C_ShowHudRestore,
                AppConst.C_UpdateActionPanel,
                AppConst.C_ShowSkillSelectTarget,
                AppConst.C_ShowSkillConfirm,
                AppConst.C_ShowLevelChallengeWin,
                AppConst.C_LevelChallengeLose,
            };
        }

        // 重写处理通知的方法
        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_BattleReadyEnd: // 准备阶段结束，播放游戏正式开始动画
                    BattleReadyEnd(notification);
                    break;
                case AppConst.C_RoundBegin: // 回合开始，动画播放，动画播放结束正式进入玩家回合
                    int roundNum = (int)notification.Body;
                    MyViewComponent.transform.SetAsLastSibling();
                    MyViewComponent.midTip.SetActive(true);
                    MyViewComponent.textRoundNum.text = "第" + roundNum + "回合";
                    MyViewComponent.playerTip.SetActive(true);
                    break;
                case AppConst.C_EnemyReady: // 敌人回合，动画播放，动画播放结束正式进入敌人回合
                    MyViewComponent.transform.SetAsLastSibling();
                    MyViewComponent.midTip.SetActive(true);
                    MyViewComponent.enemyTip.SetActive(true);
                    break;
                case AppConst.C_ShowHudDamage:
                    ShowHudDamageArgs showHudDamageArgs = notification.Body as ShowHudDamageArgs;
                    ShowHudDamage(showHudDamageArgs.damage, showHudDamageArgs.worldPos);
                    break;
                case AppConst.C_ShowHudRestore:
                    ShowHudDamageArgs damageArgs = notification.Body as ShowHudDamageArgs;
                    ShowHudRestore(damageArgs.damage, damageArgs.worldPos);
                    break;
                case AppConst.C_UpdateActionPanel:
                    var myCharacterController = notification.Body as MyCharacterController;
                    if (myCharacterController == null)
                    {
                        MyViewComponent.actionPanel.SetActive(false);
                    }
                    else
                    {
                        UpdateActionPanel(myCharacterController);
                    }

                    break;
                case AppConst.C_ShowSkillSelectTarget:
                    var targetShow = (bool)notification.Body;
                    ShowSkillSelectTarget(targetShow);
                    break;
                case AppConst.C_ShowSkillConfirm:
                    var confirmShow = (bool)notification.Body;
                    ShowSkillConfirm(confirmShow);
                    break;
                case AppConst.C_ShowLevelChallengeWin:
                    var winArgs = notification.Body as BattleWinArgs;
                    ShowAwardPanel(winArgs);
                    break;
                case AppConst.C_LevelChallengeLose:
                    MyViewComponent.failedContainer.gameObject.SetActive(true);
                    break; 
            }
        }

        private void BattleReadyEnd(INotification notification)
        {
            // 开启面板
            ShowPanel();

            // 更新 UI
            MyViewComponent.InitHpImage();

            // 展示动效
            MyViewComponent.transform.SetAsLastSibling();
            MyViewComponent.midTip.SetActive(true);
            MyViewComponent.startTip.SetActive(true);
        }

        private void UpdateActionPanel(MyCharacterController data)
        {
            MyViewComponent.actionPanel.SetActive(true);

            foreach (var item in MyViewComponent.skillUIs)
            {
                item.imgSkill.gameObject.SetActive(false);
                item.btnSkill.enabled = false;
                item.imgSkillCD.gameObject.SetActive(false);
            }

            for (int i = 0; i < data.myCharacterData.skillDataList.Count; i++)
            {
                // 设置图片
                MyViewComponent.skillUIs[i].imgSkill.sprite = data.myCharacterData.skillDataList[i].configData.icon;
                MyViewComponent.skillUIs[i].imgSkill.gameObject.SetActive(true);

                // 主动技能可以进行点击
                if (data.myCharacterData.skillDataList[i].configData.type == SkillType.Active)
                {
                    MyViewComponent.skillUIs[i].btnSkill.enabled = true;
                    int currentCD = data.skill[i].activeSkillConfig.cd;
                    MyViewComponent.skillUIs[i].imgSkillCD.gameObject.SetActive(currentCD > 0);
                    MyViewComponent.skillUIs[i].textSkillCD.text = currentCD.ToString();
                }
            }
        }

        private void ShowSkillSelectTarget(bool show)
        {
            MyViewComponent.skillSelectTarget.SetActive(show);
            MyViewComponent.actionPanel.SetActive(!show);
        }

        private void ShowSkillConfirm(bool show)
        {
            MyViewComponent.skillConfirm.SetActive(show);
            MyViewComponent.skillSelectTarget.SetActive(!show);
        }

        private void ConfirmClick()
        {
            GameManager.Instance.battleManager.ConfirmClick();
            MyViewComponent.skillConfirm.SetActive(false);
        }

        private void ShowAwardPanel(BattleWinArgs configData)
        {
            MyViewComponent.ShowAwardPanel(configData);
        }
    }
}
