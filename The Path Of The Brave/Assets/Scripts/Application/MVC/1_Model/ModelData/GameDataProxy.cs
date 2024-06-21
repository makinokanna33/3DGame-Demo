using System.Xml;
using FrameWork.Base;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class GameDataProxy : Proxy
    {
        public new const string NAME = "GameDataProxy";

        public GameData MyData { get { return Data as GameData; } }

        public GameDataProxy():base(NAME)
        {
            // 在构造函数中初始化一个数据进行关联
            GameData gameData = new GameData();
            Data = gameData;

            // 读取 View 配置表
            XmlDocument doc = new XmlDocument();
#if UNITY_EDITOR
            doc.Load(AppConst.ConfigDir + "/ViewConfiguration.xml");
#else
        TextAsset xmlText = ABManager.Instance.LoadRes<TextAsset>(AppConst.AB_Config, AppConst.AB_ViewConfiguration);
        doc.LoadXml(xmlText.text);
#endif
            gameData.ViewConfigList = doc.SelectSingleNode("ViewConfiguration").ChildNodes;
        }

        public void GetViewConfig(string viewName, out string mediatorName)
        {
            GameData gameData = MyData;

            mediatorName = "";

            foreach (XmlElement xe in gameData.ViewConfigList)
            {
                if (viewName == xe.GetAttribute("viewName"))
                {
                    mediatorName = xe.GetAttribute("mediatorName");
                    return;
                }
            }

            LoggerManager.Instance.LogError("未在 ViewConfiguration 配置文件中找到 " + viewName);
        }

        //public Dictionary<string, ItemBase> GetItemDic()
        //{
        //    return MyData.ItemDic;
        //}

        //public int GetInventoryMaxCount()
        //{
        //    return MyData.InventoryMaxCount;
        //}

        //public int GetPropertyMaxCount()
        //{
        //    return MyData.PropertyMaxCount;
        //}

    }
}
