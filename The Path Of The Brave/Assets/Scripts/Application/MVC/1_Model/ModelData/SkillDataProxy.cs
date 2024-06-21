using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class SkillDataProxy : Proxy
    {
        public new const string NAME = "SkillDataProxy";

        private SkillsSOData skillsSoData;
        public SkillDataProxy() : base(NAME)
        {
            // 读取 Skills 配置
            // 获取所有的 ScriptableObject 资源
#if UNITY_EDITOR
            skillsSoData = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillsSOData>("Assets/ABRes/Configuration/SkillsConfiguration.asset");
#else
            skillsSoData = ABManager.Instance.LoadRes<SkillsSOData>(AppConst.AB_Config, AppConst.AB_SkillsConfiguration);
#endif
        }
    }
}