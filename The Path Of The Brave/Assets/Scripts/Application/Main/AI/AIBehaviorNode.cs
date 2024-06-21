using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace MyApplication
{
    public enum State
    {
        Running = 0,
        Succeed,
        Fail
    }

    public enum BehaviorType
    {
        None,
        Attack,
        Auxiliary
    }

    /// <summary>
    /// 基类结点
    /// </summary>
    public class BehaviorNode
    {
        public State state = State.Running;

        public virtual IEnumerator Execute()
        {
            yield return 1;
        }

        public virtual IEnumerator Start()
        {
            state = State.Running;

            yield return GameManager.Instance.StartCoroutine(Execute());
        }
    }

    /// <summary>
    /// 条件结点
    /// </summary>
    public class ConditionBehavior : BehaviorNode
    {

    }

    /// <summary>
    /// 行为结点
    /// </summary>
    public class ActionBehavior : BehaviorNode
    {

    }

    /// <summary>
    /// 复合结点
    /// </summary>
    public class Composite : BehaviorNode
    {
        public List<BehaviorNode> nodes = new List<BehaviorNode>();
    }

    /// <summary>
    /// 选择器
    /// 只要子节点有一个返回 true，则停止执行其它子节点，并且 Selector 返回 true
    /// 如果所有子节点都返回 false，则 Selector 返回 false
    /// </summary>
    public class Selector : Composite
    {
        public override IEnumerator Execute()
        {
            foreach (var node in nodes)
            {
                yield return GameManager.Instance.StartCoroutine(node.Start());
                if (node.state == State.Succeed) yield break; // 直接终止函数 = return
            }

            state = State.Fail;
        }
    }

    /// <summary>
    /// 顺序器
    /// 只要有一个子节点返回 false，则停止执行其它子节点，并且 Sequence 返回 false
    /// 如果所有子节点都返回 true，则 Sequence 返回 true
    /// </summary>
    public class Sequence : Composite
    {
        public override IEnumerator Execute()
        {
            foreach (var node in nodes)
            {
                yield return GameManager.Instance.StartCoroutine(node.Start());
                // 只要有一个子节点返回false，则停止执行其它子节点，
                if (node.state == State.Fail)
                {
                    state = State.Fail;
                    yield break;
                }
            }

            // 如果所有子节点都返回 true，则 Sequence 返回 true。
            state = State.Succeed;
        }
    }

    /// <summary>
    /// 数据模型
    /// </summary>
    public class InActiveSkillRangeResult
    {
        public List<MyCharacterController> insidePlayers;
        public Skill resultSkill;
        public StrikingRangePath resultPath;
    }

    /// <summary>
    /// 条件结点：主动伤害技能范围内是否有对象
    /// </summary>
    public class InActiveSkillRangeAdvanced : ConditionBehavior
    {
        public MyCharacterController player;
        public List<InActiveSkillRangeResult> data;

        protected virtual List<MyCharacterController> GetTargetPlayers()
        {
            return null;
        }

        public override IEnumerator Start()
        {
            data = new List<InActiveSkillRangeResult>();
            return base.Start();
        }

        public override IEnumerator Execute()
        {
            GridMeshManager.Instance.DespawnAllPath();

            foreach (Skill skill in player.skill)
            {
                if (skill != null && skill.activeSkillConfig != null && skill.activeSkillConfig.cd == 0)
                {
                    if (!CanSelectSkill(skill)) continue;

                    StrikingRangePath path = player.StartPathActiveSkillRange(player.MapNode, skill.activeSkillConfig.releaseRange);

                    yield return player.StartCoroutine(path.WaitForPath());

                    // 根据技能的配置获取作用对象
                    var players = GetTargetPlayers();

                    // 查找技能射程范围内的玩家
                    var insidePlayers = players.FindAll(resultPlayer => path.allNodes.Contains(resultPlayer.MapNode));

                    if (insidePlayers.Count > 0)
                    {
                        data.Add(new InActiveSkillRangeResult()
                        {
                            insidePlayers = insidePlayers,
                            resultPath = path,
                            resultSkill = skill
                        });
                    }
                }
            }

            // 没有技能符合条件
            if (data.Count > 0)
            {
                Debug.Log("条件结点[主动伤害技能范围内是否有对象]执行完毕，结果成功");
                state = State.Succeed;
            }
            else
            {
                Debug.Log("条件结点[主动伤害技能范围内是否有对象]执行完毕，结果失败");
                state = State.Fail;
                yield break;
            }
        }

        protected virtual bool CanSelectSkill(Skill skill)
        {
            return false;
        }
    }

    public class InDamageSkillRange : InActiveSkillRangeAdvanced
    {
        /// <summary>
        /// 伤害型技能的作用目标为敌人
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        protected override bool CanSelectSkill(Skill skill)
        {
            return skill.activeSkillConfig.skillTarget == SkillTarget.Enemy;
        }

        protected override List<MyCharacterController> GetTargetPlayers()
        {
            return GameManager.Instance.battleManager.GetEnemy(player.myCharacterData.camp);
        }
    }

    public class InAuxiliarySkillRange : InActiveSkillRangeAdvanced
    {        
        /// <summary>
        /// 辅助型技能的作用目标为友军
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        protected override bool CanSelectSkill(Skill skill)
        {
            return skill.activeSkillConfig.skillTarget == SkillTarget.Friendly;
        }
        protected override List<MyCharacterController> GetTargetPlayers()
        {
            return GameManager.Instance.battleManager.GetPlayers(player.myCharacterData.camp);
        }
    }

    /// <summary>
    /// 数据模型
    /// </summary>
    public class UseSkillResult
    {
        public MyCharacterController target;
        public Skill skill;
        public List<MyCharacterController> insidePlayers;
        public StrikingRangePath resultPath;
    }

    /// <summary>
    /// 行为结点：释放技能
    /// </summary>
    public class UseSkillAdvanced : ActionBehavior
    {
        public InActiveSkillRangeAdvanced inActiveSkillRangeAdvanced;
        public BehaviorType behaviorType;
        public override IEnumerator Execute()
        {
            var from = inActiveSkillRangeAdvanced.player;
            from.actionRangePath = null;

            List<UseSkillResult> dataUseSkill = new List<UseSkillResult>();

            // 穷举出节能射程范围内所有敌人，得到所需的数据模型
            foreach (var item in inActiveSkillRangeAdvanced.data)
            {
                var resultSkill = item.resultSkill;

                var players = new List<MyCharacterController>();
                if (resultSkill.activeSkillConfig.skillTarget == SkillTarget.Enemy)
                    players = GameManager.Instance.battleManager.GetEnemy(from.myCharacterData.camp);
                else
                    players = GameManager.Instance.battleManager.GetPlayers(from.myCharacterData.camp);

                foreach (var target in item.insidePlayers)
                {
                    var path = from.StartPathActiveSkillRange(target.MapNode, resultSkill.activeSkillConfig.actionRange);
                    yield return from.StartCoroutine(path.WaitForPath());

                    // 找出所有在主动释放区域内的目标
                    var filterPlayer = players.FindAll(s => path.allNodes.Contains(s.MapNode));

                    // 假如是治疗类型的技能的话,人物血量大于95%则不使用技能
                    // 假如不做判断很会出现满血也会使用技能
                    if (resultSkill.activeSkillConfig.activeSkillType == ActiveSkillType.RestoreHealth)
                    {
                        if (target.myCharacterData.currentHp / (float)target.myCharacterData.maxHp > 0.95f)
                        {
                            continue;
                        }
                    }

                    dataUseSkill.Add(new UseSkillResult()
                    {
                        target = target,
                        insidePlayers = filterPlayer,
                        resultPath = path,
                        skill = resultSkill,
                    });
                }
            }

            if (dataUseSkill.Count == 0)
            {
                Debug.Log("行为结点[释放技能]执行完毕，结果失败");
                state = State.Fail;
                yield break;
            }
            else
            {
                dataUseSkill.Sort(SortSkillResult);
                var data = dataUseSkill[0];
                var to = dataUseSkill[0].target;

                // 为了看清楚 节点的 运行过程
                GridMeshManager.Instance.ShowPathRed(data.resultPath.allNodes);
                yield return new WaitForSeconds(1f);

                GameManager.Instance.battleManager.SkillSelectionTarget(from, to, data.skill, true);

                state = State.Succeed;
                Debug.Log("行为结点[释放技能]执行完毕，结果成功");

                // 等待路径计算完成
                while (from.actionRangePath == null) yield return null;

                //为了看清楚 节点的 运行过程
                yield return new WaitForSeconds(1f);

                GameManager.Instance.battleManager.ConfirmUseSkillAI(from, to);
            }
        }

        private int SortSkillResult(UseSkillResult x, UseSkillResult y)
        {
            var x1 = GetWeight(x);

            var x2 = GetWeight(y);

            if (x1 > x2) return -1;
            if (x1 < x2) return 1;
            return 0;
        }

        private int GetWeight(UseSkillResult x)
        {
            // 技能能打到的人数越多，权重越高
            var playerNumWeight = x.insidePlayers.Count * 10;

            var hpWeight = 0;
            foreach (var player in x.insidePlayers)
            {
                // 血量越少,权重越高
                var p = 1f - player.myCharacterData.currentHp / (float)player.myCharacterData.maxHp;
                var h = Mathf.FloorToInt(p * 10);
                h *= h;
                hpWeight += h;
            }

            return playerNumWeight + hpWeight;
        }
    }
    
    /// <summary>
    /// 数据模型
    /// </summary>
    public class MoveResult
    {
        internal List<GraphNode> abPath;

        public MyCharacterController moveToPlayer;
        internal List<GraphNode> fullAbPath;
    }

    /// <summary>
    /// 行为结点：向目标移动
    /// </summary>
    public class MoveToTargetAdvanced : ActionBehavior
    {
        public MyCharacterController player;
        private Path moveRangePath;
        public BehaviorType behaviorType;
        public List<MoveResult> moveResult = new List<MoveResult>();

        public override IEnumerator Start()
        {
            moveRangePath = null;
            moveResult.Clear();
            return base.Start();
        }

        public override IEnumerator Execute()
        {
            // 路径查找，求出移动范围和最短路径，求出交集部分
            player.GetMovePath(OnMovePathOk);
            while (moveRangePath == null) yield return new WaitForSeconds(0.5f);

            // 移动的目标角色
            var players = new List<MyCharacterController>();

            if (behaviorType == BehaviorType.Attack)
                players = GameManager.Instance.battleManager.GetEnemy(player.myCharacterData.camp);
            else
                players = GameManager.Instance.battleManager.GetPlayers(player.myCharacterData.camp);

            if (players.Count == 0)
            {
                Debug.Log("行为结点[移动]执行完毕，结果失败");
                state = State.Fail;
                yield break;

            }

            foreach (MyCharacterController characterController in players)
            {
                var abPathExt = GetMove2TargetPath(player.transform.position, characterController.transform.position, moveRangePath);
                yield return player.StartCoroutine(abPathExt.WaitForPath());

                var fullAbPath = GetMove2TargetFullPath(player.transform.position, characterController.transform.position);
                yield return player.StartCoroutine(abPathExt.WaitForPath());

                var m = new MoveResult()
                {
                    abPath = abPathExt.path, 
                    moveToPlayer = characterController, 
                    fullAbPath = fullAbPath.path
                };

                moveResult.Add(m);
            }



            moveResult.Sort(SortResult);

            var abPath = moveResult[0].abPath;
            var endNode = abPath[abPath.Count - 1];
            player.MoveAI(endNode, abPath);
            yield return new WaitForSeconds(0.5f);

            while (player.moving) yield return new WaitForSeconds(0.5f);

            state = State.Succeed;
            Debug.Log("行为结点[移动]执行完毕，结果成功");
        }

        private int SortResult(MoveResult x, MoveResult y)
        {
            var x1 = GetWeight(x);
            var x2 = GetWeight(y);


            if (x1 > x2) return -1;
            if (x1 < x2) return 1;
            return 0;
        }

        int GetWeight(MoveResult t)
        {
            // 血量越少,权重越高
            var p = 1f - ((float)t.moveToPlayer.myCharacterData.currentHp / t.moveToPlayer.myCharacterData.maxHp);
            var hpWeight = Mathf.FloorToInt(p * 10);

            // 距离越远权重越低
            var distanceWeight = -((3 * t.fullAbPath.Count));

            return hpWeight + distanceWeight;
        }

        private void OnMovePathOk(Path path)
        {
            moveRangePath = path;
        }

        /// <summary>
        /// 查找在移动范围内起点到终点的移动路径
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="moveRangePath"></param>
        /// <returns></returns>
        public ABPathExt GetMove2TargetPath(Vector3 start, Vector3 end, Path moveRangePath)
        {
            // 获取其他角色的位置节点
            var otherPlayersMapNode = GameManager.Instance.battleManager.GetCharactersMapNode();
            otherPlayersMapNode.Remove(player.MapNode);
            var searchNode = moveRangePath.path.FindAll(s => !otherPlayersMapNode.Contains(s));

            // 约束规则
            var nnc = new NNCPlayerMove();
            // 符合条件的节点必须满足
            // 1 在移动范围之内
            // 2 不能有玩家
            nnc.moveRangePath = searchNode;
            nnc.playersMapNode = otherPlayersMapNode;

            // 返回的结果是在移动路径上距离敌人的坐标
            var node = AstarPath.active.GetNearest(end, nnc).node;
            Vector3 nodePosition = (Vector3)node.position;
            ABPathExt mPath = ABPathExt.ConstructRange(start, nodePosition, null);
            mPath.canTraverseWater = player.myCharacterData.CanSwim;
            AstarPath.StartPath(mPath, true);

            return mPath;
        }

        /// <summary>
        /// 查找起点到终点的移动路径（无视移动范围）
        /// </summary>
        /// <returns></returns>
        public ABPathExt GetMove2TargetFullPath(Vector3 start, Vector3 end)
        {
            // 获取其他角色的位置节点
            var otherPlayersMapNode = GameManager.Instance.battleManager.GetCharactersMapNode();
            otherPlayersMapNode.Remove(player.MapNode);

            // 约束规则
            var nnc = new NNCPlayerMove();
            nnc.playersMapNode = otherPlayersMapNode;
            // 返回的结果是在移动路径上距离敌人的坐标
            var node = AstarPath.active.GetNearest(end, nnc).node;
            Vector3 nodePosition = (Vector3)node.position;
            ABPathExt mPath = ABPathExt.ConstructRange(start, nodePosition, null);
            mPath.canTraverseWater = player.myCharacterData.CanSwim;
            AstarPath.StartPath(mPath, true);

            return mPath;
        }
    }

    /// <summary>
    /// 行为结点：攻击
    /// </summary>
    public class UseAttack : ActionBehavior
    {
        public MyCharacterController player;

        public override IEnumerator Execute()
        {
            // 计算可以攻击的所有节点
            player.CalStrikingRangePathAI();

            while (player.waitStrikingPathSearchEnd) yield return null;

            // 查找在攻击范围内的敌人
            var enemies = GameManager.Instance.battleManager.GetEnemy(player.myCharacterData.camp);
            enemies = enemies.FindAll(e => player.strikingRange.Contains(e.MapNode));

            if (enemies.Count <= 0)
            {
                Debug.Log("行动结点[攻击]执行完毕，结果失败");
                this.state = State.Fail;
                yield break;
            }

            // 优先攻击 范围之内血量最少的敌人
            enemies.Sort(OrderByHp);
            GameManager.Instance.battleManager.AttackSelectAI(player, enemies[0]);
            state = State.Succeed;
        }

        /// <summary>
        /// 升序排序，血量小在前
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int OrderByHp(MyCharacterController x, MyCharacterController y)
        {
            var x1 = x.myCharacterData.currentHp / x.myCharacterData.maxHp;

            var x2 = y.myCharacterData.currentHp / y.myCharacterData.maxHp;

            if (x1 > x2) return 1;
            if (x1 < x2) return -1;
            return 0;
        }
    }

    /// <summary>
    /// 行为结点：待机
    /// </summary>
    public class Wait : ActionBehavior
    {
        public MyCharacterController player;
        public override IEnumerator Execute()
        {
            GameManager.Instance.battleManager.WaitAI(player);
            return base.Execute();
        }
    }

}
