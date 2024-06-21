using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using MyApplication;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class ItemDataProxy : Proxy
    {
        public new const string NAME = "ItemDataProxy";

        private ItemsBaseSOData itemsBaseSoData;
        public ItemDataProxy() : base(NAME)
        {
            // 读取 ItemsBase 配置
            // 获取所有的 ScriptableObject 资源
#if UNITY_EDITOR
            itemsBaseSoData = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemsBaseSOData>("Assets/ABRes/Configuration/ItemsBaseConfiguration.asset");
#else
            itemsBaseSoData = ABManager.Instance.LoadRes<ItemsBaseSOData>(AppConst.AB_Config, AppConst.AB_ItemsBaseConfiguration);
#endif
        }

        public ItemData CreateItemData(int id, int level, int count, int ownerId = -1)
        {
            ItemData itemData;
            foreach (var item in itemsBaseSoData.configDataList)
            {
                if(item.itemConfigData.id == id)
                {
                    itemData = new ItemData(item.itemConfigData, level, count, ownerId);
                    return itemData;
                }
            }
            return null;
        }

    }
}


