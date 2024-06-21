using System.Text;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "ItemBaseConfiguration", menuName = "Configuration/ItemBaseConfiguration")]
    public class ItemBaseSOData : ScriptableObject
    {
        public ItemConfigData itemConfigData;
    }
}
