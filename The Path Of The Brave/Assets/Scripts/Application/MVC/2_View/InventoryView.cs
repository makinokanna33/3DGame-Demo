using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class InventoryView : MonoBehaviour
    {
        [Header("UI组件")]
        public Button btnClose;

        public Toggle togEquip;
        public Toggle togMaterials;

        //public Text textEquip;
        //public Text textConsumable;
        //public Text textMaterials;

        [Header("格子数组")]
        public GameObject[] itemGrids;

        //public void Init(GameData gameData, TabType tabType)
        //{
        //    List<InventoryTabTypeSO> tabTypeSOs = gameData.InventoryTabTypeSOs;
        //    foreach (InventoryTabTypeSO SO in tabTypeSOs)
        //    {
        //        switch (SO.TabType)
        //        {
        //            case TabType.Equip:
        //                textEquip.text = SO.TabName;
        //                break;
        //            case TabType.Consumable:
        //                textConsumable.text = SO.TabName;
        //                break;
        //            case TabType.Material:
        //                textMaterials.text = SO.TabName;
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    switch (tabType)
        //    {
        //        case TabType.Equip:
        //            togEquip.isOn = true;
        //            togConsumable.isOn = false;
        //            togMaterials.isOn = false;
        //            break;
        //        case TabType.Consumable:
        //            togEquip.isOn = false;
        //            togConsumable.isOn = true;
        //            togMaterials.isOn = false;
        //            break;
        //        case TabType.Material:
        //            togEquip.isOn = false;
        //            togConsumable.isOn = false;
        //            togMaterials.isOn = true;
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}

