using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using PureMVC.Patterns.Facade;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public enum CharacterState : uint
    {
        Idle,
        MoveAttack,
        Wait,
        Skill
    }

    public class MyCharacterController : MonoBehaviour
    {
        public Animator animator;
        public CharacterData myCharacterData;
        private CharacterState characterState;

        public bool isDeath;
        public bool moving;

        public GameObject childObj;

        [HideInInspector] public bool waitStrikingPathSearchEnd = false;
        [HideInInspector] public Int3? goMapPos; // 移动的目标地点
        [HideInInspector] public Path actionRangePath; // 主动技能释放范围

        [HideInInspector] public List<GraphNode> moveRangePath = new List<GraphNode>(); // 移动路径
        [HideInInspector] public List<GraphNode> strikingRange = new List<GraphNode>(); // 攻击范围
        [HideInInspector] public List<GraphNode> standStrikingRange = new List<GraphNode>(); // 原地攻击范围

        private MyCharacterController damageTarget;

        private System.Action moveStopAction;

        private Coroutine moveCoroutine;
        public Transform hpTransform;
        public Image hpImage;
        public int viewHp;
        public Skill[] skill = new Skill[3];

        public bool isFastAttack; // 是否是先攻
        private bool isActiveAttack; // 是否是主动攻击
        private bool isBeatBack; // 是否已经反击过

        private Skill tmpUsingSkill;
        private List<MyCharacterController> tmpFilterPlayers;
        public AIBehaviorTree aiBehaviorTree;

        // 获得该角色所在的地图位置
        public Int3 MapPos
        {
            get
            {
                return AstarPath.active.GetNearest((Vector3)this.transform.position).node.position;
            }
        }

        // 获取该角色所在的地图结点
        public GraphNode MapNode
        {
            get
            {
                return AstarPath.active.GetNearest((Vector3)this.transform.position).node;
            }
        }

        public CharacterState State
        {
            get => characterState;
            set
            {
                characterState = value;
                if (characterState == CharacterState.Wait)
                {
                    StartCoroutine(ChangeSkinnedColor());
                }
                else
                {
                    childObj.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1, 1, 1);
                }
            }
            
        }

        public void RestoreHealth(int addHp)
        {
            myCharacterData.currentHp += addHp;
            if (myCharacterData.currentHp > myCharacterData.maxHp)
                myCharacterData.currentHp = myCharacterData.maxHp;

            viewHp = myCharacterData.currentHp;

            hpImage.fillAmount = (float)viewHp / myCharacterData.maxHp;

            // 展示特效字体
            GameFacade.Instance.SendNotification(AppConst.C_ShowHudRestore,
                new ShowHudDamageArgs(addHp, transform.position + Vector3.up * 4));
        }

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            myCharacterData.startMovePos = this.transform.position;
            State = CharacterState.Idle;
            isDeath = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        [ContextMenu("显示范围")]
        // 显示移动范围和攻击范围
        public void ShowMoveRange()
        {
            waitStrikingPathSearchEnd = true;
            GetMovePath((Path movePath) =>
            {
                moveRangePath = movePath.path; // 移动范围
                strikingRange = new List<GraphNode>(); // 攻击范围

                // 获取其他角色的位置节点
                var otherCharacters = GameManager.Instance.battleManager.GetCharactersMapNode();
                otherCharacters.Remove(MapNode);

                var progress = 0;
                foreach (var node in movePath.path)
                {
                    // 在移动范围预查找攻击范围时，对有人物占位的节点进行忽略
                    if (otherCharacters.Contains(node))
                    {
                        progress += 1;
                        continue;
                    }

                    GetStrikingRange(node, myCharacterData.configData.minAtkRange, myCharacterData.configData.maxAtkRange, 
                        (Path strPath) =>
                        {
                            // 过滤查找到的重复路径
                            foreach (var item in strPath.path)
                            {
                                if (!strikingRange.Contains(item))
                                    strikingRange.Add(item);
                            }

                            progress += 1;

                            // 查找完毕
                            if (progress >= movePath.path.Count)
                            {
                                waitStrikingPathSearchEnd = false;

                                // 显示移动范围
                                GridMeshManager.Instance.ShowPath(movePath.path);

                                // 显示攻击范围（攻击范围的查找包含移动范围，需要过滤掉）
                                strikingRange = strikingRange.FindAll(t => !movePath.path.Contains(t));
                                GridMeshManager.Instance.StrRangePath(strikingRange);
                            }
                        });
                }
            });

            // 存储角色在当前不移动的情况下的攻击范围节点
            standStrikingRange.Clear();

            GetStrikingRange(MapNode, myCharacterData.configData.minAtkRange, myCharacterData.configData.maxAtkRange,
                (path) =>
                {
                    standStrikingRange.AddRange(path.path);
                });
        }

        public void DamageBySkill(int damage)
        {
            // 攻击力 - 防御力 = 造成的伤害
            int dps = damage - myCharacterData.currentDef;
            if (dps < 0)
            {
                dps = 0;
            }

            myCharacterData.currentHp -= dps;
            // 修正血量，避免出现负数
            if (myCharacterData.currentHp < 0)
            {
                myCharacterData.currentHp = 0;
            }

            // 展示特效
            GameFacade.Instance.SendNotification(AppConst.C_ShowHudDamage,
                new ShowHudDamageArgs(damage, transform.position + Vector3.up * 4));

            // 更新 UI
            viewHp = myCharacterData.currentHp;
            hpImage.fillAmount = (float)viewHp / myCharacterData.maxHp;

            if (myCharacterData.currentHp > 0)
            {
                // 受击目标播放动画
                AnimationGettingHit();
            }
            else
            {
                // 播放死亡动画
                Death();
            }
        }

        public bool CanMoveAttack(MyCharacterController hitPlayer)
        {
            return strikingRange.Contains(hitPlayer.MapNode);
        }

        public void MoveAttack(MyCharacterController hitPlayer)
        {
            State = CharacterState.MoveAttack;
            if (moveRangePath == null)
                return;

            damageTarget = hitPlayer;

            moveStopAction -= MoveAnimaStop2Attack;
            moveStopAction += MoveAnimaStop2Attack;


            StartCoroutine(GetMove2EnemyPath(transform.position, hitPlayer.transform.position, myCharacterData.configData.minAtkRange,
                (path) =>
                {
                    OnAbPathComplete(path);
                    goMapPos = hitPlayer.MapPos;
                }));
        }

        /// <summary>
        /// 获取移动路径
        /// </summary>
        public void GetMovePath(System.Action<Path> onPathSearchOkCallBack)
        {
            var moveGScore = (myCharacterData.MoveRange + 1) * 1000 * 3;

            // 获取敌人的结点位置
            List<MyCharacterController> enemies = GameManager.Instance.battleManager.GetEnemy(myCharacterData.camp);
            var enemyNodes = enemies.ToGraphNode();

            // 调用 AStar API 寻找路径结点（过滤敌人结点）
            var searchPath = MoveRangConStantPath.ConstructEnemy(myCharacterData.startMovePos, moveGScore, myCharacterData.CanSwim, enemyNodes,
                (Path path) =>
                {
                    path.path = (path as MoveRangConStantPath).allNodes;
                    onPathSearchOkCallBack.Invoke(path);
                }
            );

            // 异步返回搜索结果
            AstarPath.StartPath(searchPath, true);
        }

        /// <summary>
        /// 获取攻击距离
        /// </summary>
        public void GetStrikingRange(GraphNode node, int minRange, int maxRange, System.Action<Path> OnPathSerchOkCallBack)
        {
            var moveGScore = (maxRange + 1) * 1000 * 3;

            var SerchPath = StrikingRangePath.Construct((Vector3)node.position, moveGScore,
                (Path path) => // path 参数为搜索出来的最大攻击范围
                {
                    // 搜索最小攻击范围
                    this.GetStrikingMinRange(path, node, minRange, OnPathSerchOkCallBack);
                }
            );

            // 异步返回搜索结果
            AstarPath.StartPath(SerchPath, true);
        }
        private void GetStrikingMinRange(Path maxPath, GraphNode node, int minRange, System.Action<Path> OnPathSerchOkCallBack)
        {
            var minLength = ((int)minRange) * 1000 * 3;

            maxPath.path = (maxPath as StrikingRangePath).allNodes;

            var strikingMinPath = StrikingRangePath.Construct((Vector3)node.position, minLength,
            (Path minPath) =>
            {
                minPath.path = (minPath as StrikingRangePath).allNodes;

                // 最大攻击范围减去最小范围，获得真正攻击范围
                var resulePath = new StrikingRangePath();
                resulePath.path = maxPath.path.FindAll(s => !minPath.path.Contains(s));

                OnPathSerchOkCallBack.Invoke(resulePath);
            });

            AstarPath.StartPath(strikingMinPath, true);
        }

        private void MoveAnimaStop2Attack()
        {
            ActiveAttack(damageTarget);
        }

        [ContextMenu("显示射程")]
        void TestShowStrikingRange()
        {
            GetStrikingRange(MapNode, myCharacterData.configData.minAtkRange, myCharacterData.configData.maxAtkRange, 
                (path) =>
                {
                    GridMeshManager.Instance.ShowPathWhite(path.path);
                });
        }
        public void Ready()
        {
            myCharacterData.startMovePos = this.transform.position;
        }
        public IEnumerator GetMove2EnemyPath(Vector3 start, Vector3 end, int minRange, System.Action<ABPath> callBack)
        {
            var minLength = minRange * 1000 * 3;

            // 以敌人为中心生成最小攻击范围
            var unWalkPath = StrikingRangePath.Construct(end, minLength);
            AstarPath.StartPath(unWalkPath, true);
            yield return StartCoroutine(unWalkPath.WaitForPath());

            var otherCharactersMapNode = GameManager.Instance.battleManager.GetCharactersMapNode();
            otherCharactersMapNode.Remove(MapNode);

            // 从可移动路径节点列表中过滤：以敌人为中心生成的最小攻击范围节点、有角色存在的节点
            var searchNode = moveRangePath.FindAll(s => !unWalkPath.allNodes.Contains(s) && 
                                                        !otherCharactersMapNode.Contains(s));

            // 约束规则
            var nnc = new NNCPlayerMove();
            // 符合条件的节点必须满足：在移动范围之内、不能有玩家
            nnc.moveRangePath = searchNode;
            nnc.playersMapNode = otherCharactersMapNode;

            // 返回的结果是在移动路径上距离敌人的坐标
            var nearestNode = AstarPath.active.GetNearest(end, nnc).node;
            Vector3 endPos = (Vector3)nearestNode.position;

            ABPathExt movePath = ABPathExt.ConstructRange(start, endPos, moveRangePath,
                 (Path path) =>
                 {
                     ABPathExt tmpPath = path as ABPathExt;
                     callBack(tmpPath);
                 }
            );

            movePath.canTraverseWater = myCharacterData.CanSwim;

            AstarPath.StartPath(movePath, true);
        }
        // 主动攻击
        public void ActiveAttack(MyCharacterController target)
        {
            // 执行双方拥有的被动能力
            GameManager.Instance.skillManager.BattleStart(this, target);

            // 攻击者与被击者对视
            LookAtTarget(target);
            target.LookAtTarget(this);

            damageTarget = target;

            isActiveAttack = true;

            // 若对手可以触发先攻并且还未触发过
            if (damageTarget.isFastAttack && !damageTarget.isBeatBack)
            {
                // 对手先攻
                // UICtrl.instance.ShowFastAttack(orderList[0]);
                damageTarget.isActiveAttack = false;
                damageTarget.damageTarget = this;
                damageTarget.animator.SetTrigger("attacking");
            }
            else
            {
                animator.SetTrigger("attacking");
            }
        }
        void LookAtTarget(MyCharacterController target)
        {
            this.transform.LookAt(target.transform);
            var localEulerAngles = this.transform.localEulerAngles;

            //只需要Y轴旋转
            localEulerAngles.x = 0;
            localEulerAngles.z = 0;

            this.transform.localEulerAngles = localEulerAngles;
        }

        // 攻击动画事件：造成伤害
        public void AnimationAttack()
        {
            //this.viewHp = (int)this.attribute.hp;

            // 攻击力 - 防御力 = 造成的伤害
            int dps = myCharacterData.currentAtk - damageTarget.myCharacterData.currentDef;
            if(dps < 0)
            {
                dps = 0;
            }

            damageTarget.myCharacterData.currentHp -= dps;
            // 修正血量，避免出现负数
            if(damageTarget.myCharacterData.currentHp < 0)
            {
                damageTarget.myCharacterData.currentHp = 0;
            }

            Debug.Log("攻击了敌人造成了 " + dps + " 点伤害");

            // 展示特效
            GameFacade.Instance.SendNotification(AppConst.C_ShowHudDamage,
                new ShowHudDamageArgs(dps, damageTarget.transform.position + Vector3.up * 4)); 

            // 更新 UI
            damageTarget.viewHp = damageTarget.myCharacterData.currentHp;
            damageTarget.hpImage.fillAmount = (float)damageTarget.viewHp / damageTarget.myCharacterData.maxHp;

            // 受击目标播放动画
            if (damageTarget.myCharacterData.currentHp <= 0)
            {
                damageTarget.Death();
            }
            else
            {
                damageTarget.AnimationGettingHit();
            }
            
        }

        // 攻击动画事件：攻击结束
        public void AnimationAttackEnd()
        {
            if (isActiveAttack) // 主动攻击
            {
                isActiveAttack = false;
                if(!damageTarget.isDeath)
                    StartCoroutine(damageTarget.UnderActiveAttack(this));
                else
                    AttackRoundEnd(this);
            }
            else if (isFastAttack) // 先攻
            {
                isBeatBack = true;
                if (!damageTarget.isDeath)
                    damageTarget.animator.SetTrigger("attacking");
                else
                    AttackRoundEnd(damageTarget);
            }
            else // 被动反击
            {
                AttackRoundEnd(damageTarget);
            }
        }

        // 受击动画事件
        void AnimationGettingHit()
        {
            animator.SetTrigger("gettingHit");
        }

        // 主动技能释放
        public void AnimationSkillAction()
        {
            GameManager.Instance.skillManager.ReleaseSkill(tmpUsingSkill, this, tmpFilterPlayers);
        }

        // 主动技能释放结束
        public void AnimationSkillActionEnd()
        {
            GameManager.Instance.battleManager.ActionEnd();
        }

        // 受到主动攻击
        IEnumerator UnderActiveAttack(MyCharacterController attacker)
        {
            LookAtTarget(attacker);

            // 如果已经没血，进行死亡处理
            if (myCharacterData.currentHp <= 0)
            {
                Death();
                AttackRoundEnd(attacker);
            }
            else if(!isBeatBack) // 若没有反击过，尝试反击
            {
                // 计算攻击距离
                var strikeNode = MapNode;
                var attackerNode = attacker.MapNode;

                var abPath = StrikingRangeABPath.Construct((Vector3)strikeNode.position, (Vector3)attackerNode.position);
                AstarPath.StartPath(abPath);
                yield return StartCoroutine(abPath.WaitForPath());

                var dis = abPath.path.Count - 2;

                if(myCharacterData.configData.maxAtkRange > dis &&
                    myCharacterData.configData.minAtkRange <= dis + 1)
                {
                    damageTarget = attacker;
                    isActiveAttack = false; // 本次攻击是被动反击，对方无法再还击
                    animator.SetTrigger("attacking");
                }
                else
                {
                    AttackRoundEnd(attacker);
                }
            }
            else
            {
                isBeatBack = false;
                AttackRoundEnd(attacker);
            }
        }

        public IEnumerator ChangeSkinnedColor()
        {
            yield return new WaitForSeconds(1f);
            childObj.GetComponent<SkinnedMeshRenderer>().material.color = new Color(111 / 255f, 111 / 255f, 111 / 255f);
        }

        private void AttackRoundEnd(MyCharacterController attacker)
        {
            GameManager.Instance.skillManager.BattleEnd(attacker, this);
            GameManager.Instance.battleManager.AttackRoundEnd(attacker);
        }

        private void OnAbPathComplete(ABPath abPath)
        {
            // 展示移动路径
            GridMeshManager.Instance.ShowPathWhite(abPath.path);
            MoveAnimation(abPath.path.toPos());
        }

        void MoveAnimation(List<Vector3> pos)
        {
            moving = true;
            animator.SetBool("moving", moving);

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(this.MoveUpdate(pos, this.transform));
        }

        public void CancelMove()
        {
            if (State == CharacterState.Wait) return;

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);

            transform.position = myCharacterData.startMovePos;

            moving = false;
            animator.SetBool("moving", moving);

            WorldPos2MapPos();
            
            goMapPos = null;
        }

        void WorldPos2MapPos()
        {
            transform.position = (Vector3)MapPos;
        }

        // 插值移动
        IEnumerator MoveUpdate(List<Vector3> pos, Transform transform)
        {
            var startPosition = transform.position;
            var index = 0;
            int moveSpeed = 7;

            while (true)
            {
                // 每帧执行
                yield return new WaitForEndOfFrame();
                if (index == pos.Count)
                {
                    // 完成
                    break;
                }

                var girdPos = pos[index];
                var finalPosition = new Vector3(girdPos.x, girdPos.y, girdPos.z);
                var curdistance = Vector3.Distance(startPosition, finalPosition);

                var speed = moveSpeed * Time.deltaTime;
                var remainingDistance = curdistance - speed;

                // l:路程, s:速度, t3:两点坐标的插值
                // t1 = l / s
                // t2 = (l - s) / s
                // t3 = 1 - t2 / t1
                var t3 = 1f;
                if (remainingDistance <= 0)
                {
                    remainingDistance = 0;
                    t3 = 1;
                }
                else
                {
                    var t1 = curdistance / speed;
                    var t2 = remainingDistance / speed;
                    t3 = 1 - t2 / t1;
                }

                if (t3 == 1)
                {
                    index += 1;
                }

                var outPos = Vector3.Lerp(startPosition, finalPosition, t3);
                startPosition = outPos;

                var orgQua = transform.rotation;
                transform.LookAt(outPos);

                var newQua = transform.rotation;
                transform.rotation = Quaternion.Lerp(orgQua, newQua, 0.3f);

                transform.position = outPos;
            }

            MoveStop();
        }
        public void Move(GraphNode hitNode)
        {
            if (moveRangePath == null) return;

            // 进行寻路计算
            GetMoveABPathCallback(this.transform.position, (Vector3)hitNode.position, 
                (abPath) =>
                {
                    if (abPath.path.Count == 0)
                    {
                        CancelMove();
                        return;
                    }

                    OnAbPathComplete(abPath);
                    goMapPos = hitNode.position;
                });
        }
        private void MoveStop()
        {
            moving = false;
            animator.SetBool("moving", moving);

            if (moveStopAction != null)
                moveStopAction.Invoke();
        }

        // 获取两个点之间行走路径
        public void GetMoveABPathCallback(Vector3 start, Vector3 end, System.Action<ABPath> callBack)
        {
            Vector3 endPos = (Vector3)AstarPath.active.GetNearest(end, new NNCPlayerMove()).node.position;
            ABPathExt mPath = ABPathExt.ConstructRange(start, endPos, moveRangePath,
                 (Path path) =>
                 {
                     ABPathExt tmpPath = path as ABPathExt;
                     callBack(tmpPath);
                 }
            );
            mPath.canTraverseWater = myCharacterData.CanSwim;
            var startNode = AstarPath.active.GetNearest(start).node;
            mPath.nnConstraint = new NNCMoveAbPath(startNode);
            AstarPath.StartPath(mPath, true);
        }

        // 判断目标是否在原地攻击范围内
        public bool InAttackRange(MyCharacterController hitPlayer)
        {
            return standStrikingRange.Contains(hitPlayer.MapNode);
        }

        public Skill ShowActiveSkillReleaseRange(int skillId)
        {
            var activeSkill = skill[skillId];
            return ShowActiveSkillReleaseRange(activeSkill);
        }

        // 显示主动释放范围
        public Skill ShowActiveSkillReleaseRange(Skill activeSkill)
        {
            GetActiveSkillRange(MapNode, activeSkill.activeSkillConfig.releaseRange, (rangePath) =>
            {
                if (activeSkill.activeSkillConfig.skillTarget == SkillTarget.Enemy)
                    GridMeshManager.Instance.ShowPathRed(rangePath.path);
                else
                    GridMeshManager.Instance.ShowPath(rangePath.path);
            });

            return activeSkill;
        }

        // 显示主动作用范围
        public void ShowActiveSkillActionRange(Skill activeSkill, MyCharacterController target)
        {
            GetActiveSkillRange(target.MapNode, activeSkill.activeSkillConfig.actionRange, (rangePath) =>
            {
                actionRangePath = rangePath;
                if (activeSkill.activeSkillConfig.skillTarget == SkillTarget.Enemy)
                    GridMeshManager.Instance.ShowPathRed(rangePath.path);
                else
                    GridMeshManager.Instance.ShowPath(rangePath.path);
            });
        }

        // 主动技能范围
        public void GetActiveSkillRange(GraphNode node, int range, System.Action<Path> onPathSearchOkCallBack)
        {
            var moveGScore = (int)range * 1000 * 3;

            var searchPath = StrikingRangePath.Construct((Vector3)node.position, moveGScore,
                (Path path) =>
                {
                    path.path = (path as StrikingRangePath).allNodes;
                    onPathSearchOkCallBack.Invoke(path);
                }
            );

            // 异步返回搜索结果
            AstarPath.StartPath(searchPath, true);
        }

        public void ReleaseSkill(Skill usingSkill, MyCharacterController skillTarget)
        {
            State = CharacterState.Skill;

            if (skillTarget != this) 
                LookAtTarget(skillTarget);

            var players = GameManager.Instance.battleManager.characters;

            var filterPlayers = new List<MyCharacterController>();

            filterPlayers.AddRange(players);

            // 筛选符合技能执行条件的对象
            filterPlayers = filterPlayers.FindAll(player => usingSkill.activeSkillConfig.CanSelect(this, player));

            // 筛选在技能作用范围内的对象
            filterPlayers = filterPlayers.FindAll(player => actionRangePath.path.Contains(player.MapNode));

            // 显示起手特效
            usingSkill.activeSkillConfig.activeSkillAction.BeforeReleaseSkill(this, filterPlayers);

            // 动画关键帧触发事件
            tmpUsingSkill = usingSkill;
            tmpFilterPlayers = filterPlayers;

            animator.SetTrigger("usingSkill1");
        }

        public void CdAdd(int value)
        {
            for (var index = 0; index < skill.Length; index++)
            {
                var item = skill[index];
                if (item != null)
                {
                    // 主动技能
                    if (item.activeSkillConfig != null)
                    {
                        item.activeSkillConfig.cd += value;
                        if (item.activeSkillConfig.cd <= 0) item.activeSkillConfig.cd = 0;
                        myCharacterData.skillDataList[index].currentCD = item.activeSkillConfig.cd;
                    }
                    else
                    {
                        // 被动技能, cd值减少
                        var states = GameManager.Instance.skillManager.GetSkillState(this, ExecuteTiming.CDDown);

                        foreach (SkillState skillState in states)
                        {
                            skillState.ExecuteAll(this, null);
                        }
                    }
                }
            }
        }

        public void Death()
        {
            animator.SetTrigger("die");
            isDeath = true;
            GameManager.Instance.battleManager.RemoveDeathCharacter(this);
        }

        /// <summary>
        /// 查询主动技能释放范围区域
        /// </summary>
        /// <param name="node">角色当前位置</param>
        /// <param name="range">技能范围</param>
        /// <returns></returns>
        public StrikingRangePath StartPathActiveSkillRange(GraphNode node, int range)
        {
            var moveGScore = (int)range * 1000 * 3;

            var searchPath = StrikingRangePath.Construct((Vector3)node.position, moveGScore);
            AstarPath.StartPath(searchPath, true);

            return searchPath;
        }

        public void CloseActiveSkillActionRange()
        {
            GridMeshManager.Instance.DespawnAllPath();
        }

        public void MoveAI(GraphNode endNode, List<GraphNode> path)
        {
            moveRangePath = path;
            Move(endNode);
        }

        public void CalStrikingRangePathAI()
        {
            ShowMoveRange();
        }

        public void AISelect()
        {
            myCharacterData.startMovePos = transform.position;
        }

        public void HideMe()
        {
            this.gameObject.SetActive(false);
        }
    }
}

