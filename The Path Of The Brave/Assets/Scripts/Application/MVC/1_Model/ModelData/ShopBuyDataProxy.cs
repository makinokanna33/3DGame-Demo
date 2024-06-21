using FrameWork.Base;
using PureMVC.Patterns.Proxy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [SerializeField]
    public class ShopBuyDataProxy : Proxy
    {
        public new const string NAME = "ShopBuyDataProxy";

        public ShopBuySOData soData;

        public ShopBuyDataProxy() : base(NAME)
        {
#if UNITY_EDITOR
            soData = UnityEditor.AssetDatabase.LoadAssetAtPath<ShopBuySOData>("Assets/ABRes/Configuration/ShopBuyConfiguration.asset");
#else
            soData = ABManager.Instance.LoadRes<ShopBuySOData>(AppConst.AB_Config, AppConst.AB_ShopBuyConfiguration);
#endif
        }
    }
}