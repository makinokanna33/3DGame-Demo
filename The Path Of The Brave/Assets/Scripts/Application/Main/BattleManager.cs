using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyApplication
{
    public enum BattleStageEnum
    {
        None,
        Ready,              // 准备阶段
        RoundBegin,         // 回合开始
        PlayerRound,        // 玩家回合
        EnemyRound,         // 敌人回合
        Busy,               // 忙碌状态
    }

    public class BattleManager : MonoBehaviour
    {
        public int awardId;
        // 回合流程控制
        private int roundNum = 0;
        private BattleStageEnum battleStage = BattleStageEnum.None;

        public MyCharacterController[] characters; // 所有角色
        private MyCharacterController curSelect; // 当前选中的角色
        private List<MyCharacterController> dieCharacterControllers = new List<MyCharacterController>();
        private Bounds mapBounds; // 地图边界
        private bool battling; // 是否在战斗中
        private int lastSkillId;
        private Skill usingSkill;
        private bool showSkillReleaseRange;
        private MyCharacterController skillTarget;

        public BattleStageEnum BattleStage
        {
            get { return battleStage; }
            set
            {
                battleStage = value;
                switch (value)
                {
                    case BattleStageEnum.Ready:
                        BattleReady();
                        break;
                    case BattleStageEnum.RoundBegin:
                        RoundBegin();
                        break;
                    case BattleStageEnum.PlayerRound:
                        PlayerRound();
                        break;
                    case BattleStageEnum.EnemyRound:
                        EnemyRound();
                        break;
                    case BattleStageEnum.Busy:
                        break;
                    case BattleStageEnum.None:
                        break;
                    default:
                        break;
                }
            }
        }

        private void Awake()
        {
            GameManager.Instance.battleManager = this;
            battling = false;
            

            // 游戏开始，进入战斗准备阶段
            BattleStage = BattleStageEnum.Ready;
        }

        // Start is called before the first frame update
        void Start()
        {
            var gridGraph = (AstarPath.active.graphs[0] as GridGraph);
            var size = gridGraph.nodeSize;
            mapBounds = new Bounds(gridGraph.center, new Vector3(gridGraph.width * size, 10, size * gridGraph.depth));
            GridMeshManager.Instance.Reset();
        }

        // Update is called once per frame
        void Update()
        {
            // 以下情况不能进行操作
            if (battleStage != BattleStageEnum.PlayerRound || // 不在玩家回合
                battling == true || // 战斗中
                (curSelect != null && curSelect.moving) ||// 已选角色正在移动中
                (curSelect != null && curSelect.State == CharacterState.Skill) ||// 释放技能时不能操作
                EventSystem.current.IsPointerOverGameObject() // 当鼠标正在点击 UI 时，阻止后续操作
               ) 
                return;

            var hitWorldPoint = MouseRaycast();

            // 角色正在移动攻击中无法进行其他操作
            if (curSelect != null && curSelect.State == CharacterState.MoveAttack)
            {
                return;
            }

            if (hitWorldPoint != null)
            {
                var hitMapNode = AstarPath.active.GetNearest((Vector3)hitWorldPoint).node;
                var hitMapPos = hitMapNode.position;

                // 若在准备释放技能阶段，尝试选择目标并准备技能释放
                if (showSkillReleaseRange)
                {
                    SkillSelectionTarget(curSelect, SelectPlayer(hitMapPos));
                    return;
                }

                // 超出 A 星图边界，不进行任何处理
                if (!mapBounds.Contains((Vector3)hitWorldPoint))
                {
                    if (curSelect != null)
                    {
                        this.CancelSelect();
                    }
                    return;
                }

                // 通过地图坐标获取人物
                var hitPlayer = SelectPlayer(hitMapPos);

                // 当前已经选中的角色为玩家，鼠标点中的角色为敌人且玩家为 Idle 状态，执行攻击逻辑
                if (hitPlayer != null && curSelect != null &&
                    curSelect.myCharacterData.camp == CharacterCamp.Player && 
                    hitPlayer.myCharacterData.camp != curSelect.myCharacterData.camp &&
                    curSelect.State == CharacterState.Idle)
                {
                    // 敌人在当前攻击距离内，直接攻击
                    if (curSelect.InAttackRange(hitPlayer))
                    {
                        Debug.Log("在有效范围，直接攻击敌人");
                        SetBattleState();
                        curSelect.ActiveAttack(hitPlayer);
                        ReleaseSelect();
                        return;
                    }
                    // 敌人在当前攻击距离外且在移动范围内，移动过去攻击
                    else if (curSelect.CanMoveAttack(hitPlayer))
                    {
                        Debug.Log("不在有效范围，跑过去攻击");
                        SetBattleState();
                        curSelect.MoveAttack(hitPlayer);
                        return;
                    }
                }

                // 假如点击的角色非玩家阵容则仅显示移动范围
                if (hitPlayer != null && hitPlayer.myCharacterData.camp != CharacterCamp.Player)
                {
                    if (curSelect != null)
                    {
                        CancelSelect();
                    }

                    hitPlayer.ShowMoveRange();
                    return;
                }

                // 若之前未选中过任何角色
                if (curSelect == null)
                {
                    curSelect = hitPlayer;
                    if (curSelect != null)
                    {
                        // 展示移动范围
                        curSelect.ShowMoveRange();

                        // 若选中的角色为己方阵营且可以行动，进入准备状态
                        if (curSelect.myCharacterData.camp == CharacterCamp.Player
                            && curSelect.State == CharacterState.Idle)
                        {
                            curSelect.Ready();
                            // 展示行动 UI
                            GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel, curSelect);
                        }
                    }
                }
                // 若之前选中过角色
                else
                {
                    // otherSelect 只可能是地图格或己方队友（敌方已在上面的逻辑中过滤掉）
                    var otherSelect = hitPlayer;

                    if (otherSelect == null)
                    {
                        if (curSelect.goMapPos != hitMapPos &&
                            curSelect.State == CharacterState.Idle)
                            curSelect.Move(hitMapNode);
                    }
                    // 如果不是则把当前人物的行动取消，切换人物后再进行范围移动显示
                    else if (otherSelect != curSelect)
                    {
                        UpdateSelect(hitPlayer);
                    }
                }

            }

        }
            
        // 等待人物选择释放范围内的目标
        private void SkillSelectionTarget(MyCharacterController from, MyCharacterController to)
        {
            if (to == null) return;

            skillTarget = to;

            if (to != null && usingSkill.activeSkillConfig.CanSelect(from, to))
            {
                // 显示主动技能作用范围
                from.ShowActiveSkillActionRange(usingSkill, skillTarget);
                // 显示 UI 面板
                GameFacade.Instance.SendNotification(AppConst.C_ShowSkillConfirm, true);
            }
        }

        public void SkillSelectionTarget(MyCharacterController from, MyCharacterController to, Skill usingSkill, bool isAi = false)
        {
            if (to == null) return;
            skillTarget = to;
            this.usingSkill = usingSkill;
            if (to != null && this.usingSkill.activeSkillConfig.CanSelect(from, to))
            {
                // 显示主动技能作用范围
                from.ShowActiveSkillActionRange(this.usingSkill, skillTarget);
                
                // 显示 UI 面板
                if (!isAi)
                    GameFacade.Instance.SendNotification(AppConst.C_ShowSkillConfirm, true);
            }
        }

        private void BattleReady()
        {
            // 打开战斗准备 UI
            GameFacade.Instance.SendNotification(AppConst.C_ShowPanel, new UIPanelArgs(AppConst.V_BattleReadyView));
        }
        
        private void RoundBegin()
        {
            // 回合数++
            roundNum++;

            // 通知视图回合开始
            GameFacade.Instance.SendNotification(AppConst.C_RoundBegin, roundNum);
        }

        private void PlayerRound()
        {
            Debug.Log("————玩家回合————");
        }

        private void EnemyRound()
        {
            Debug.Log("————敌人回合————");
            var idlePlayer = FindIdlePlayer(CharacterCamp.Enemy);

            if (idlePlayer != null)
            {
                UpdateSelect(idlePlayer);
            }
        }
        public void ReleaseSelect()
        {
            GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);

            // 关闭路径
            GridMeshManager.Instance.DespawnAllPath();

            curSelect = null;
        }

        private void SetBattleState()
        {
            GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);
            battling = true;
        }

        // 准备结束，战斗开始
        public void BattleReadyEnd()
        {
            // 初始化所有角色的战斗数据
            characters = GameObject.FindObjectsOfType<MyCharacterController>();
            foreach (var character in characters)
            {
                character.myCharacterData.UpdateCharacterData(); // 初始化角色数据
                character.myCharacterData.InitSkillData(); // 初始化技能数据
                GameManager.Instance.skillManager.InitPlayerSkill(character); // 初始化技能
            }

            //BattleStage = BattleStageEnum.RoundBegin;
        }

        // camp 为自己的阵容，该 API 会返回与自己相对的阵容列表
        public List<MyCharacterController> GetEnemy(CharacterCamp camp)
        {
            List<MyCharacterController> enemy = new List<MyCharacterController>();
            foreach (var item in this.characters)
            {
                if (item.myCharacterData.camp != camp)
                    enemy.Add(item);
            }

            return enemy;
        }
        public List<GraphNode> GetCharactersMapNode()
        {
            List<GraphNode> tmp = new List<GraphNode>();
            foreach (var item in characters)
            {
                tmp.Add(item.MapNode);
            }
            return tmp;
        }
        Vector3? MouseRaycast()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 生成一条从摄像机发出的射线
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // 用来存储射线打中物体的信息
                RaycastHit hit;
                // 发射射线
                bool result = Physics.Raycast(ray, out hit);
                // 如果为true说明打中物体了
                if (result)
                {
                    return hit.point;
                }
            }
            return null;
        }

        public void CancelSelect()
        {
            GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);
            curSelect.CancelMove();

            //关闭路径
            GridMeshManager.Instance.DespawnAllPath();

            curSelect = null;
        }

        private MyCharacterController SelectPlayer(Int3 hitMapPos)
        {
            foreach (MyCharacterController character in characters)
            {
                if (character.MapPos == hitMapPos)
                {
                    return character;
                }
            }
            return null;
        }
        public void ChangeBattleStage(BattleStageEnum stageEnum)
        {
            BattleStage = stageEnum;
        }

        public void UseSkill(int id)
        {
            if (curSelect.moving) return;

            if (curSelect.skill[id].activeSkillConfig.cd > 0)
            {
                Debug.Log("技能还没冷却");
                return;
            }

            lastSkillId = id;
            ShowActiveSkillReleaseRange();
        }

        // 取消技能选择
        public void CancelSkillSelectTarget()
        {
            showSkillReleaseRange = false;

            // 重新显示移动范围
            curSelect.ShowMoveRange();
            curSelect.CloseActiveSkillActionRange();

            // 关闭技能选择目标 UI
            GameFacade.Instance.SendNotification(AppConst.C_ShowSkillSelectTarget, false);
        }

        // 展示技能释放范围
        private void ShowActiveSkillReleaseRange()
        {
            usingSkill = curSelect.ShowActiveSkillReleaseRange(lastSkillId);
            showSkillReleaseRange = true;
            GridMeshManager.Instance.DespawnAllPath();

            // 显示技能选择目标 UI
            GameFacade.Instance.SendNotification(AppConst.C_ShowSkillSelectTarget, true);
        }

        void UpdateSelect(MyCharacterController characterController)
        {
            if (characterController.State == CharacterState.Idle)
            {
                GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel, characterController);
                characterController.Ready();
            }
            else
            {
                GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);
                
            }

            if(characterController.myCharacterData.camp == CharacterCamp.Enemy)
                GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);

            if (curSelect != null)
                curSelect.CancelMove();

            // 关闭路径
            GridMeshManager.Instance.DespawnAllPath();

            if(characterController.myCharacterData.camp != CharacterCamp.Enemy)
                characterController.ShowMoveRange();

            characterController.Ready();
            curSelect = characterController;

            // AI 进行行动
            if (curSelect.myCharacterData.camp == CharacterCamp.Enemy)
                curSelect.aiBehaviorTree.ExecuteBehaviorAdvanced();
        }

        public void RemoveDeathCharacter(MyCharacterController myCharacterController)
        {
            myCharacterController.hpTransform.gameObject.SetActive(false);
            dieCharacterControllers.Add(myCharacterController);
            
            var n = new List<MyCharacterController>();
            n.AddRange(characters);
            n.Remove(myCharacterController);
            characters = n.ToArray();

            CheckBattleIsEnd();
        }

        public void CheckBattleIsEnd()
        {
            var playerList = GetPlayers(CharacterCamp.Player);
            var enemyList = GetPlayers(CharacterCamp.Enemy);
            if (playerList.Count == 0)
            {
                Lose();
            }
            else if (enemyList.Count == 0)
            {
                Win();
            }
        }

        public void Win()
        {
            Debug.Log("游戏结束，挑战成功！");
            SelectLevelDataProxy selectLevelDataProxy = GameFacade.Instance.RetrieveProxy(SelectLevelDataProxy.NAME) as SelectLevelDataProxy;
            var configData = selectLevelDataProxy.GetSelectLevelData(awardId);

            List<bool> isComplete = new List<bool>();
            List<string> challengeInfo = new List<string>();
            for (var index = 0; index < configData.challengeId.Count; index++)
            {
                isComplete.Add(ChallengeCheck(configData.challengeId[index]));
                challengeInfo.Add(configData.challengeInfo[index]);
            }

            GameFacade.Instance.SendNotification(AppConst.C_LevelChallengeWin, new BattleWinArgs(awardId, challengeInfo, isComplete, selectLevelDataProxy));
        }

        public void Lose()
        {
            Debug.Log("游戏结束，挑战失败！");
            GameFacade.Instance.SendNotification(AppConst.C_LevelChallengeLose);
        }

        public bool ChallengeCheck(int id)
        {
            if (id == 0)
            {
                if (roundNum <= 7)
                    return true;
                else
                    return false;
            }
            else if (id == 1)
            {
                foreach (var item in dieCharacterControllers)
                {
                    if (item.myCharacterData.camp == CharacterCamp.Player)
                        return false;
                }

                return true;
            }

            return false;
        }

        public void AttackRoundEnd(MyCharacterController attacker)
        {
            // 进攻过的玩家进入待机状态，无法再操作
            attacker.State = CharacterState.Wait;
            battling = false;
            ReleaseSelect();

            Wait(true);
        }

        // 主动技能释放结束
        public void ActionEnd()
        {
            showSkillReleaseRange = false;
            curSelect.State = CharacterState.Wait;
            curSelect = null;
            battling = false;

            StartCoroutine(EffectEnd(() => { Wait(true); }));
        }

        // 等待特效播放完毕
        IEnumerator EffectEnd(Action func)
        {
            // 等待特效播放完成
            while (GameManager.Instance.effectManager.playEffect)
            {
                yield return new WaitForEndOfFrame();
            }

            if (func != null) func.Invoke();
        }

        public bool Wait(bool attackRoundEnd = false)
        {
            if (curSelect != null && curSelect.moving) return false;

            if (!attackRoundEnd)
            {
                curSelect.State = CharacterState.Wait;
                ReleaseSelect();
            }

            MyCharacterController idlePlayer = null;
            if (BattleStage == BattleStageEnum.PlayerRound)
                idlePlayer = FindIdlePlayer(CharacterCamp.Player);
            else
                idlePlayer = FindIdlePlayer(CharacterCamp.Enemy);

            if (idlePlayer != null)
            {
                UpdateSelect(idlePlayer);
            }
            else
            {
                Debug.Log("回合结束！");
                StartCoroutine(OrderOver());
            }
            return true;
        }

        private MyCharacterController FindIdlePlayer(CharacterCamp camp)
        {
            foreach (MyCharacterController item in characters)
            {
                if (item.myCharacterData.camp == camp && item.State == CharacterState.Idle) return item;
            }

            return null;
        }

        IEnumerator OrderOver()
        {
            // 等待特效播放完毕
            yield return StartCoroutine(EffectEnd(null));
            yield return new WaitForSeconds(2f);

            // 更新回合
            NextOrder();
        }

        private void NextOrder()
        {
            if (BattleStage == BattleStageEnum.PlayerRound)
            {
                // 玩家回合结束，进入敌人准备阶段，准备动画播放完毕将进入敌人回合
                GameFacade.Instance.SendNotification(AppConst.C_EnemyReady);
            }
            else if (BattleStage == BattleStageEnum.EnemyRound)
            {
                // 敌人回合结束，新的回合开始
                BattleStage = BattleStageEnum.RoundBegin;
            }

            // 如果新的回合开始
            if (BattleStage == BattleStageEnum.RoundBegin)
            {
                // 把所有角色设置为可行动
                foreach (MyCharacterController item in characters)
                {
                    item.State = CharacterState.Idle;
                    // 技能CD减少1
                    item.CdAdd(-1);
                }

                // 默认选中一个角色
                var players = GetPlayers(CharacterCamp.Player);
                UpdateSelect(players[0]);
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.battleManager = null;
        }

        public void CancelSkillConfirm()
        {
            // 重新显示技能释放范围
            ShowActiveSkillReleaseRange();

            // 更新 UI
            GameFacade.Instance.SendNotification(AppConst.C_ShowSkillConfirm, false);
        }

        public void ConfirmClick()
        {
            // 释放技能
            curSelect.ReleaseSkill(usingSkill, skillTarget);
            // 关闭行动面板
            GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);
            // 关闭格子
            GridMeshManager.Instance.DespawnAllPath();
        }

        public void ConfirmUseSkillAI(MyCharacterController from, MyCharacterController to)
        {
            this.curSelect = from;
            skillTarget = to;
            this.curSelect.ReleaseSkill(this.usingSkill, to); // AI 进行技能释放
            GridMeshManager.Instance.DespawnAllPath();
        }

        public void WaitAI(MyCharacterController player)
        {
            this.curSelect = player;
            Wait();
        }

        /// <summary>
        /// 获取所有友军
        /// </summary>
        /// <param name="camp"></param>
        /// <returns></returns>
        public List<MyCharacterController> GetPlayers(CharacterCamp camp)
        {
            List<MyCharacterController> players = new List<MyCharacterController>();
            foreach (var item in this.characters)
            {
                if (item.myCharacterData.camp == camp)
                    players.Add(item);
            }

            return players;
        }

        public void AttackSelectAI(MyCharacterController from, MyCharacterController to)
        {
            curSelect = from;
            if (curSelect.InAttackRange(to))
            {
                Debug.Log("在有效范围 直接攻击敌人");
                SetBattleState();
                curSelect.ActiveAttack(to);
                this.ReleaseSelect();
                return;
            }
            //并且在移动范围内，就跑过去攻击
            else if (curSelect.CanMoveAttack(to))
            {
                SetBattleState();
                Debug.Log("不在有效范围 跑过去攻击");
                curSelect.MoveAttack(to);
                return;
            }
        }
    }
}

