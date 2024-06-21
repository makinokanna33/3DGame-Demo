using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShowItemArgs
    {
        public int gridIndex;
        public ItemData itemData;

        public ShowItemArgs(int gridIndex, ItemData itemData)
        {
            this.gridIndex = gridIndex;
            this.itemData = itemData;
        }
    }

}
