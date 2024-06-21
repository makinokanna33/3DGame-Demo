using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public enum SkillTarget
    {
        Friendly,
        Enemy
    }

    public enum ActiveSkillType
    {
        Attack,
        RestoreHealth
    }

    // 主动技能结点
    public abstract class ActiveSkillAction
    {
        public abstract void BeforeReleaseSkill(MyCharacterController from, List<MyCharacterController> to);

        public abstract void ReleaseSkill(MyCharacterController from, List<MyCharacterController> to);
    }

    // 主动技能配置
    public class ActiveSkillConfig
    {
        // 以人物为中心的范围
        public int releaseRange = 1;

        // 作用范围
        public int actionRange = 1;

        public SkillTarget skillTarget;

        public ActiveSkillAction activeSkillAction;

        public int cdConfig;
        public int cd;
            
        public ActiveSkillType activeSkillType;

        public bool CanSelect(MyCharacterController from, MyCharacterController to)
        {
            if (skillTarget == SkillTarget.Friendly)
            {
                return from.myCharacterData.camp == to.myCharacterData.camp;
            }
            else
            {
                return from.myCharacterData.camp != to.myCharacterData.camp;
            }
        }
    }

    public class RestoreHealth : ActiveSkillAction
    {
        public override void BeforeReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            // 展示特效
            GameManager.Instance.effectManager.ShowMagicCircleSimpleGreen(from);
        }

        public override void ReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            var addHp = Mathf.FloorToInt((float)from.myCharacterData.currentAtk * 1.5f);
            foreach (var player in to)
            {
                // 恢复生命值
                player.RestoreHealth(addHp);
                // 展示特效
                GameManager.Instance.effectManager.ShowRestoreHealth(player);
            }
        }
    }


    public class RestoreHealthEffectBig : RestoreHealth
    {
        public override void ReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            base.ReleaseSkill(from, to);

            // 展示大特效
            GameManager.Instance.effectManager.ShowRestoreHealthBig(from);
        }
    }

    public class FireSkill : ActiveSkillAction
    {
        public override void BeforeReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            GameManager.Instance.effectManager.playEffect = true;
        }

        public override void ReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            var damage = Mathf.FloorToInt((float)from.myCharacterData.currentAtk * 1.5f);
            GameManager.Instance.StartCoroutine(ShowTime(damage, to));
        }

        IEnumerator ShowTime(int damage, List<MyCharacterController> to)
        {
            // 镜头震动
            GameManager.Instance.cameraManager.Shake(0.5f, 0.3f);

            foreach (MyCharacterController player in to)
            {
                GameManager.Instance.StartCoroutine(ShowTime1(damage, player));
                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(1.3f);

            // 特效播放完才可以操作
            GameManager.Instance.effectManager.playEffect = false;
        }


        IEnumerator ShowTime1(int damage, MyCharacterController to)
        {
            GameManager.Instance.effectManager.ShowFireFall(to, 1.5f);
            yield return new WaitForSeconds(1.3f);
            GameManager.Instance.effectManager.ShowMysticExplosionOrange(to.transform.position);

            to.DamageBySkill(damage);

            GameManager.Instance.cameraManager.Shake(1, 1);
        }
    }

    public class BigFireSkill : ActiveSkillAction
    {
        public override void BeforeReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            GameManager.Instance.effectManager.playEffect = true;
        }

        public override void ReleaseSkill(MyCharacterController from, List<MyCharacterController> to)
        {
            var damage = Mathf.FloorToInt((float)from.myCharacterData.currentAtk * 0.75f);
            GameManager.Instance.StartCoroutine(ShowTime(damage, to));
        }

        IEnumerator ShowTime(int damage, List<MyCharacterController> to)
        {
            // 镜头震动
            GameManager.Instance.cameraManager.Shake(0.5f, 0.3f);

            foreach (MyCharacterController player in to)
            {
                GameManager.Instance.StartCoroutine(ShowTime1(damage, player));
                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(1.3f);

            // 特效播放完才可以操作
            GameManager.Instance.effectManager.playEffect = false;
        }


        IEnumerator ShowTime1(int damage, MyCharacterController to)
        {
            GameManager.Instance.effectManager.ShowFireFall(to, 1.5f);
            yield return new WaitForSeconds(1.3f);
            GameManager.Instance.effectManager.ShowMysticExplosionOrange(to.transform.position);

            to.DamageBySkill(damage);

            GameManager.Instance.cameraManager.Shake(1, 1);
        }
    }

}

