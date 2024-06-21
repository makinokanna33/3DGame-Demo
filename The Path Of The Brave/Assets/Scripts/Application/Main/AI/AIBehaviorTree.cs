using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace MyApplication
{
    public class AIBehaviorTree : MonoBehaviour
    {
        public MyCharacterController player;
        private BehaviorNode rootAdvancedAttack;

        public Action behaviorEnd;

        // Start is called before the first frame update
        void Start()
        {
            player = GetComponent<MyCharacterController>();

            rootAdvancedAttack = CreateBehaviorAdvancedAttack();

            player.aiBehaviorTree = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnDestroy()
        {
            behaviorEnd = null;
        }

        [ContextMenu("AI行动")]
        public void ExecuteBehaviorAdvanced()
        {
            player.AISelect();
            GameFacade.Instance.SendNotification(AppConst.C_UpdateActionPanel);
            StartCoroutine(CExecuteBehaviorAdvanced(rootAdvancedAttack));
        }

        IEnumerator CExecuteBehaviorAdvanced(BehaviorNode n)
        {
            Debug.Log("AI开始行动");
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(n.Start());
            Debug.Log("行为树流程结束");

            while (player.State != CharacterState.Wait) yield return null;
            while (GameManager.Instance.effectManager.playEffect) yield return null;

            if (behaviorEnd != null) 
                behaviorEnd.Invoke();
        }

        BehaviorNode CreateBehaviorAdvancedAttack()
        {
            var selector = new Selector(); // 根节点

            var sequence1 = new Sequence(); // 第一个行为（判断技能范围内是否有敌人，若有进行技能释放）
            var conditionDamageSkill = new InDamageSkillRange();
            var useSkillDamage = new UseSkillAdvanced();
            sequence1.nodes.Add(conditionDamageSkill);
            sequence1.nodes.Add(useSkillDamage);

            var sequence2 = new Sequence(); // 第三个行为结点（向敌人移动，然后进行第一个行为）
            var move = new MoveToTargetAdvanced();
            sequence2.nodes.Add(move);
            sequence2.nodes.Add(sequence1);

            var useAttack = new UseAttack(); // 第二、四个行为结点（攻击）
            useAttack.player = this.player;

            var sequence3 = new Sequence(); // 第四个行为结点（释放辅助技能）

            var conditionAuxiliarySkill = new InAuxiliarySkillRange();
            var useAuxiliarySkill = new UseSkillAdvanced();
            sequence3.nodes.Add(conditionAuxiliarySkill);
            sequence3.nodes.Add(useAuxiliarySkill);

            var wait = new Wait(); // 第五个行为结点（待机）

            selector.nodes.Add(sequence1);
            selector.nodes.Add(useAttack);
            selector.nodes.Add(sequence2);
            selector.nodes.Add(useAttack);
            selector.nodes.Add(sequence3);
            selector.nodes.Add(wait);

            //参数配置
            conditionAuxiliarySkill.player = player;
            useAuxiliarySkill.behaviorType = BehaviorType.Auxiliary;
            useAuxiliarySkill.inActiveSkillRangeAdvanced = conditionAuxiliarySkill;
            wait.player = this.player;
            move.player = this.player;
            move.behaviorType = BehaviorType.Attack;
            conditionDamageSkill.player = player;
            useSkillDamage.inActiveSkillRangeAdvanced = conditionDamageSkill;
            useSkillDamage.behaviorType = BehaviorType.Attack;

            return selector;
        }
    }
}
