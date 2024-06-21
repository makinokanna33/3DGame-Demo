using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [System.Serializable]
    public class StarAwardConfigData
    {
        public int id;
        public int playerAddExp;
        public int characterAddExp;
        public int addGoldNum;
        public List<ItemBaseSOData> trophyItem;
        public List<int> trophyNum;
    }
}

