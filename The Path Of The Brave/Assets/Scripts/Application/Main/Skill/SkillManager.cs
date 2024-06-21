using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using UnityEngine;

namespace MyApplication
{
    public enum ExecuteTiming
    {
        FightingStart,
        FightingEnd,
        CDDown,
    }

    public class SkillState
    {
        public ExecuteTiming timing;
        public List<SkillActionNode> actionNodes = new List<SkillActionNode>();

        public void ExecuteAll(MyCharacterController from, MyCharacterController to)
        {
            foreach (SkillActionNode item in actionNodes)
            {
                item.Execute(from, to);
            }
        }

        public void CompleteAll(MyCharacterController from, MyCharacterController to)
        {
            foreach (SkillActionNode item in actionNodes)
            {
                item.Complete(from, to);
            }
        }
    }

    public class Skill
    {
        public string name;
        public ActiveSkillConfig activeSkillConfig;
        public List<SkillState> skillAction = new List<SkillState>();

        // 添加状态
        public SkillState AddState()
        {
            var n = new SkillState();
            skillAction.Add(n);
            return n;
        }

        public List<SkillState> FindNodes(ExecuteTiming executeTiming)
        {
            return skillAction.FindAll(s => s.timing == executeTiming);
        }
    }

    public class SkillManager : SingletonManager<SkillManager>
    {
        public void InitPlayerSkill(MyCharacterController player)
        {
            for (int i = 0; i < player.myCharacterData.skillDataList.Count; i++)
            {
                var id = player.myCharacterData.skillDataList[i].configData.id;
                player.skill[i] = GetSkillByConfig(id, player.myCharacterData.skillDataList[i].configData.name);
            }
        }

        private Skill GetSkillByConfig(int id, string skillName)
        {
            var skill = new Skill();
            if (id == 0)
            {
                skill.name = skillName;

                // 定义战斗开始状态
                SkillState startState = skill.AddState();
                startState.timing = ExecuteTiming.FightingStart;

                // 为技能的拥有者增加防御的百分比 7%
                var addNode = new AddProperty(PropertyKey.Def, AddProperty.ApplyTarget.Owner,
                    AddProperty.AddType.AddPercentage, 0.07f);
                startState.actionNodes.Add(addNode);

                // 定义战斗结束状态
                var overState = skill.AddState();
                overState.timing = ExecuteTiming.FightingEnd;
                overState.actionNodes.Add(addNode);
            }
            else if(id == 1)
            {
                // 克敌先机
                // 遭受近战攻击【对战中】发动【先攻】
                // 【先攻】：先手攻击对方
                skill.name = skillName;

                // 定义战斗开始状态
                SkillState startState = skill.AddState();
                startState.timing = ExecuteTiming.FightingStart;

                // 触发条件->遭受【近战攻击】
                var condition = new AttackedByMeleeCondition();

                // 先攻节点
                var fastAttackNode = new FastAttack();

                // 成功后执行 执行执行先攻
                condition.successTodoNodes.Add(fastAttackNode);

                startState.actionNodes.Add(condition);

                // 战斗结束时 移除状态
                SkillState endState = skill.AddState();
                endState.timing = ExecuteTiming.FightingEnd;
                endState.actionNodes.Add(condition);

                // CD 减少时，更新技能状态
                var cdDownState = skill.AddState();
                cdDownState.timing = ExecuteTiming.CDDown;

                // CD 冷却减少节点
                var cdDownNode = new CDDown();
                cdDownNode.cdDownNode = fastAttackNode;
                cdDownState.actionNodes.Add(cdDownNode);
            }
            else if (id == 3)
            {
                // 小火球
                skill.activeSkillConfig = new ActiveSkillConfig
                {
                    releaseRange = 3,
                    actionRange = 1,
                    activeSkillAction = new FireSkill(),
                    cdConfig = 2,
                    skillTarget = SkillTarget.Enemy,
                    activeSkillType = ActiveSkillType.Attack
                };
            }
            else if(id == 4)
            {
                // 大火球
                skill.activeSkillConfig = new ActiveSkillConfig
                {
                    releaseRange = 3,
                    actionRange = 3,
                    activeSkillAction = new BigFireSkill(),
                    cdConfig = 2,
                    skillTarget = SkillTarget.Enemy,
                    activeSkillType = ActiveSkillType.Attack
                };
            }
            else if(id == 5)
            {
                // 气愈之术 单体治疗
                skill.activeSkillConfig = new ActiveSkillConfig
                {
                    releaseRange = 3,
                    actionRange = 1,
                    activeSkillAction = new RestoreHealth(),
                    cdConfig = 0,
                    skillTarget = SkillTarget.Friendly,
                    activeSkillType = ActiveSkillType.RestoreHealth
                };
            }
            else if(id == 6)
            {
                // 神氛化法 群体治疗
                skill.activeSkillConfig = new ActiveSkillConfig
                {
                    releaseRange = 3,
                    actionRange = 3,
                    activeSkillAction = new RestoreHealthEffectBig(),
                    cdConfig = 2,
                    skillTarget = SkillTarget.Friendly,
                    activeSkillType = ActiveSkillType.RestoreHealth
                };
            }

            return skill;
        }

        public void ReleaseSkill(Skill usingSkill, MyCharacterController from, List<MyCharacterController> filterPlayers)
        {
            usingSkill.activeSkillConfig.activeSkillAction.ReleaseSkill(from, filterPlayers);

            // 技能进入 CD
            usingSkill.activeSkillConfig.cd = usingSkill.activeSkillConfig.cdConfig;
        }

        public void BattleStart(MyCharacterController from, MyCharacterController to)
        {
            // 战斗前双方执行被动技能的节点逻辑
            var states1 = GetSkillState(from, ExecuteTiming.FightingStart);
            foreach (SkillState state in states1)
            {
                state.ExecuteAll(from, to);
            }

            // 考虑对方也存在被动技能
            var states2 = GetSkillState(to, ExecuteTiming.FightingStart);
            foreach (SkillState state in states2)
            {
                state.ExecuteAll(to, from);
            }
        }

        public void BattleEnd(MyCharacterController from, MyCharacterController to)
        {
            // 攻击者被动执行完毕
            var states1 = GetSkillState(from, ExecuteTiming.FightingEnd);
            foreach (SkillState state in states1)
            {
                state.CompleteAll(from, to);
            }

            // 被攻击者被动执行完毕
            var states2 = GetSkillState(to, ExecuteTiming.FightingEnd);
            foreach (SkillState state in states2)
            {
                state.CompleteAll(to, from);
            }
        }

        public List<SkillState> GetSkillState(MyCharacterController player, ExecuteTiming executeTiming)
        {
            // 查找开始战斗执行时机的技能状态
            List<SkillState> n = new List<SkillState>();
            foreach (Skill skill in player.skill)
            {
                if (skill != null)
                {
                    n.AddRange(skill.FindNodes(executeTiming));
                }
            }
            return n;
        }
    }
}

