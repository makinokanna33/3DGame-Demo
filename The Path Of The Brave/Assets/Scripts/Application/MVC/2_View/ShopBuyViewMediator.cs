using FrameWork.Base;
using PureMVC.Patterns.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShopBuyViewMediator : Mediator
    {
        public static new string NAME = "ShopBuyViewMediator";
        public ShopBuyView MyViewComponent { get { return ViewComponent as ShopBuyView; } }
        
        // 存储 ShopBuyView
        private List<ShopBuyView> shopBuyViewList;
        public ShopBuyViewMediator() : base(NAME)
        {
            shopBuyViewList = new List<ShopBuyView>();
        }
        public override void SetView(object obj)
        {
            base.SetView(obj);
        }

        public void CreateView(ShopBuyData data, GameObject parentObj)
        {
            ShopBuyView shopBuyView;
#if UNITY_EDITOR
            shopBuyView = UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<ShopBuyView>("Assets/ABRes/Prefabs/ShopBuyView.prefab"));
#else
            shopBuyView = UnityEngine.Object.Instantiate(ABManager.Instance.LoadRes<ShopBuyView>(AppConst.AB_Prefabs, AppConst.AB_ShopBuyView));
#endif

            // 更新UI信息
            shopBuyView.UpdateInfo(data);

            // 设置父物体
            shopBuyView.gameObject.transform.SetParent(parentObj.transform, false);

            // 添加到列表中进行管理
            shopBuyViewList.Add(shopBuyView);
        }

        public void HideView()
        {
            for (int i = shopBuyViewList.Count - 1; i >= 0; i--)
            {
                // 销毁游戏物体
                UnityEngine.Object.Destroy(shopBuyViewList[i].gameObject);
                // 从列表中移除
                shopBuyViewList.RemoveAt(i);
            }
        }

        public void ClearPanel()
        {
            shopBuyViewList.Clear();
        }
    }
}

