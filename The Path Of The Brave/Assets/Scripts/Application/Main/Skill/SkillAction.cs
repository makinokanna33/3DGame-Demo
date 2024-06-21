using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 技能结点
namespace MyApplication
{
    public interface ICDDownNode
    {
        void CdAdd(int value);
    }

    public abstract class SkillActionNode
    {
        public abstract void Execute(MyCharacterController from, MyCharacterController to);


        public abstract void Complete(MyCharacterController from, MyCharacterController to);
    }

    // 拥有条件限制的技能结点
    public abstract class SkillActionConditionNode : SkillActionNode
    {
        public abstract bool Condition(MyCharacterController from, MyCharacterController to);
    }

    // 有 CD 限制的技能结点
    public class CDDown : SkillActionNode
    {
        public ICDDownNode cdDownNode;
        public override void Execute(MyCharacterController from, MyCharacterController to)
        {
            cdDownNode.CdAdd(-1);
        }

        public override void Complete(MyCharacterController from, MyCharacterController to)
        {

        }
    }

    // 遭受近战攻击
    public class AttackedByMeleeCondition : SkillActionConditionNode
    {
        public List<SkillActionNode> successTodoNodes = new List<SkillActionNode>();

        bool success = false;

        public override bool Condition(MyCharacterController from, MyCharacterController to)
        {
            // form 表达自己, to 代表对手
            // 遭受近战攻击 = 对手的攻击距离为 1
            return to.myCharacterData.configData.maxAtkRange == 1;

        }

        public override void Execute(MyCharacterController from, MyCharacterController to)
        {
            success = Condition(from, to);

            if (success)
            {
                foreach (var item in successTodoNodes)
                {
                    item.Execute(from, to);
                }
            }
        }

        public override void Complete(MyCharacterController from, MyCharacterController to)
        {
            if (success)
            {
                foreach (var item in successTodoNodes)
                {
                    item.Complete(from, to);
                }
            }
            success = false;
        }
    }

    // 增加属性结点
    public class AddProperty : SkillActionNode
    {
        public enum AddType
        {
            AddValue, 
            AddPercentage
        }

        public enum ApplyTarget
        {
            Owner, 
            Other
        }

        public AddType addType; // 数值类型
        public PropertyKey propertyKey; // 修改的属性类型
        public ApplyTarget applyTarget; // 作用目标
        public float value; // 修改的数值

        private int tempValue;
        public AddProperty(PropertyKey key, ApplyTarget target, AddType addType, float value)
        {
            
            this.propertyKey = key;
            this.applyTarget = target;
            this.addType = addType;
            this.value = value;
        }
        public override void Execute(MyCharacterController from, MyCharacterController to)
        {
            MyCharacterController target = null;
            target = applyTarget == ApplyTarget.Owner ? @from : to;

            switch (propertyKey)
            {
                case PropertyKey.Hp:
                    if (addType == AddType.AddValue)
                    {
                        tempValue = (int)value;
                    }
                    else if(addType == AddType.AddPercentage)
                    {
                        tempValue = Mathf.FloorToInt((float)target.myCharacterData.maxHp * value);
                    }
                    target.myCharacterData.currentHp += tempValue;
                    break;
                case PropertyKey.Atk:
                    if (addType == AddType.AddValue)
                    {
                        tempValue = (int)value;
                    }
                    else if (addType == AddType.AddPercentage)
                    {
                        tempValue = Mathf.FloorToInt((float)target.myCharacterData.maxAtk * value);
                    }
                    target.myCharacterData.currentAtk += tempValue;
                    break;
                case PropertyKey.Def:
                    if (addType == AddType.AddValue)
                    {
                        tempValue = (int)value;
                    }
                    else if (addType == AddType.AddPercentage)
                    {
                        tempValue = Mathf.FloorToInt((float)target.myCharacterData.maxDef * value);
                    }
                    target.myCharacterData.currentDef += tempValue;
                    break;
            }
        }

        public override void Complete(MyCharacterController from, MyCharacterController to)
        {
            MyCharacterController target = null;
            target = applyTarget == ApplyTarget.Owner ? @from : to;

            switch (propertyKey)
            {
                case PropertyKey.Hp:
                    target.myCharacterData.currentHp -= tempValue;
                    break;
                case PropertyKey.Atk:
                    target.myCharacterData.currentAtk -= tempValue;
                    break;
                case PropertyKey.Def:
                    target.myCharacterData.currentDef -= tempValue;
                    break;
            }
        }
    }

    // 先攻节点
    public class FastAttack : SkillActionNode, ICDDownNode
    {
        public int count = 1;

        public void CdAdd(int value)
        {
            count = 1;
        }
        public override void Execute(MyCharacterController from, MyCharacterController to)
        {
            from.isFastAttack = true;
        }

        public override void Complete(MyCharacterController from, MyCharacterController to)
        {
            from.isFastAttack = false;
        }
    }
}

