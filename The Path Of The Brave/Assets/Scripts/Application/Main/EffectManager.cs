using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using UnityEngine;

namespace MyApplication
{
    public class EffectManager : Singleton<EffectManager>
    {
        public bool playEffect; // 是否正在播放特效

        public void ShowMagicCircleSimpleGreen(MyCharacterController player)
        {
            Transform effectTransform;
#if UNITY_EDITOR
            effectTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/MagicCircleSimpleGreen.prefab")).transform;
#else
            effectTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_MagicCircleSimpleGreen).transform;
#endif
            effectTransform.localPosition = player.transform.localPosition;
            Object.Destroy(effectTransform.gameObject, 6.0f);
        }

        public void ShowRestoreHealth(MyCharacterController player)
        {
            Transform effectTransform;
#if UNITY_EDITOR
            effectTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/HealBig.prefab")).transform;
#else
            effectTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_HealBig).transform;
#endif
            effectTransform.localPosition = player.transform.localPosition;
            Object.Destroy(effectTransform.gameObject, 2.0f);
        }

        public void ShowRestoreHealthBig(MyCharacterController from)
        {
            Transform effectTransform;
#if UNITY_EDITOR
            effectTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/HealingWindZone.prefab")).transform;
#else
            effectTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_HealingWindZone).transform;
#endif
            effectTransform.localPosition = from.transform.localPosition;
            Object.Destroy(effectTransform.gameObject, 5.0f);

        }

        public void ShowFireFall(MyCharacterController player, float duration)
        {
            Transform effectTransform;
#if UNITY_EDITOR
            effectTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/RocketMissileFire.prefab")).transform;
#else
            effectTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_RocketMissileFire).transform;
#endif

            int range = Random.Range(0, 10);

            range = range < 5 ? -1 : 1;

            var dir = GameManager.Instance.cameraManager.transform.position - player.transform.position;
            dir.Normalize();

            var qua = Quaternion.Euler(0, range * 45, 0);

            // 在摄像机的后方 5 点和 7 点方向出现特效
            var startPos = Vector3.up * 3 + player.transform.position + qua * (dir * 25);

            var fireDir = player.transform.position - startPos;
            fireDir.Normalize();

            var endPos = player.transform.position + fireDir * 2;

            Object.Destroy(effectTransform.gameObject, 4.0f);

            player.StartCoroutine(FireFall(effectTransform, duration, startPos, endPos));
        }

        IEnumerator FireFall(Transform trs, float duration, Vector3 startPos, Vector3 endPos)
        {
            var startTime = Time.time;

            while (true)
            {
                var t = (Time.time - startTime) / duration;
                trs.position = Vector3.Lerp(startPos, endPos, t);
                if (t >= 1)
                {
                    trs.position = endPos;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void ShowMysticExplosionOrange(Vector3 worldPos)
        {
            Transform effectTransform;
#if UNITY_EDITOR
            effectTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/MysticExplosionOrange.prefab")).transform;
#else
            effectTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_RocketMissileFire).transform;
#endif
            effectTransform.position = worldPos;
            Object.Destroy(effectTransform.gameObject, 4.0f);
        }
    }
}

