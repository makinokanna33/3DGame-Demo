using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShowTipArgs
    {
        public Vector2 position;         // 格子位置
        public ItemData itemData;        // 物品类

        public ShowTipArgs(Vector2 position, ItemData itemData)
        {
            this.position = position;
            this.itemData = itemData;
        }
    }
}