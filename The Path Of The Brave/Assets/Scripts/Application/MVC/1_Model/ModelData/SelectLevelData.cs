using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [System.Serializable]
    public class SelectLevelJsonData
    {
        public int id; // ID
        public bool isLock;
        public int startNum;
    }

    [System.Serializable]
    public class SelectLevelConfigData
    {
        public int id;
        public string levelName;
        public Sprite icon;
        [TextArea] public string description;
        public int playerAddExp;
        public int characterAddExp;
        public int addGoldNum;
        public List<ItemBaseSOData> trophyItem;
        public List<int> trophyNum;

        public List<int> challengeId;
        [TextArea] public List<string> challengeInfo;
    }
}

