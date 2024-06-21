using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using MyApplication;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class StarAwardDataProxy : Proxy
    {
        public new const string NAME = "StarAwardDataProxy";

        private StarAwardsSOData starAwardsSoData;
        public StarAwardDataProxy() : base(NAME)
        {
            // 读取 StarAwards 配置
            // 获取所有的 ScriptableObject 资源
#if UNITY_EDITOR
            starAwardsSoData = UnityEditor.AssetDatabase.LoadAssetAtPath<StarAwardsSOData>("Assets/ABRes/Configuration/StarAwardsConfiguration.asset");
#else
            starAwardsSoData = ABManager.Instance.LoadRes<StarAwardsSOData>(AppConst.AB_Config, AppConst.AB_StarAwardsConfiguration);
#endif
        }

        public StarAwardConfigData GetConfigData(int id)
        {
            foreach (var starAwardSoData in starAwardsSoData.soDataList)
            {
                if (starAwardSoData.configData.id == id)
                {
                    return starAwardSoData.configData;
                }
            }

            return null;
        }
    }
}

