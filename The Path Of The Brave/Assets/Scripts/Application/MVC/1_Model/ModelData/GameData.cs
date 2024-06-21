using System.Xml;

namespace MyApplication
{
    public class GameData
    {
        #region 字段
        //private int m_InventoryMaxCount = 30;                   // 背包最大容量，后期会动态获取
        //private int m_PropertyMaxCount = 6;                     // 属性栏最大容量，后期会动态获取
        private XmlNodeList viewConfigList;                       // 视图配置表
        //private Dictionary<string, ItemBase> m_ItemDic;         // 物品配置表
        //private List<InventoryTabTypeSO> inventoryTabTypeSOs;   // 背包标签列表
        #endregion

        #region 属性
        public XmlNodeList ViewConfigList { get => viewConfigList; set => viewConfigList = value; }
        //public Dictionary<string, ItemBase> ItemDic { get => m_ItemDic; set => m_ItemDic = value; }
        //public int InventoryMaxCount { get => m_InventoryMaxCount; set => m_InventoryMaxCount = value; }
        //public List<InventoryTabTypeSO> InventoryTabTypeSOs { get => inventoryTabTypeSOs; set => inventoryTabTypeSOs = value; }
        //public int PropertyMaxCount { get => m_PropertyMaxCount; set => m_PropertyMaxCount = value; }
        #endregion

        public GameData()
        {
            ViewConfigList = null;
            //ItemDic = new Dictionary<string, ItemBase>();
            //inventoryTabTypeSOs = new List<InventoryTabTypeSO>();
        }
    }
}
