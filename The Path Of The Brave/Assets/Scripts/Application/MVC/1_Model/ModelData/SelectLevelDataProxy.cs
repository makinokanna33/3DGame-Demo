using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class SelectLevelDataProxy : Proxy
    {
        public new const string NAME = "SelectLevelDataProxy";


        private SelectLevelsSOData selectLevelsSo;

        public SelectLevelDataProxy() : base(NAME)
        {
            // 读取 SelectLevel 配置
            // 获取所有的 ScriptableObject 资源
#if UNITY_EDITOR
            selectLevelsSo = UnityEditor.AssetDatabase.LoadAssetAtPath<SelectLevelsSOData>("Assets/ABRes/Configuration/SelectLevelsConfiguration.asset");
#else
            selectLevelsSo = ABManager.Instance.LoadRes<SelectLevelsSOData>(AppConst.AB_Config, AppConst.AB_SelectLevelsConfiguration);
#endif
        }

        public SelectLevelConfigData GetSelectLevelData(int id)
        {
            foreach (var item in selectLevelsSo.selectLevelSo)
            {
                if (item.configData.id == id)
                {
                    return item.configData;
                }
            }

            return null;
        }
    }
}

