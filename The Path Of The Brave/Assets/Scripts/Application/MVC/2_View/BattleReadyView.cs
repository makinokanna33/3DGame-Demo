using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class BattleReadyView : MonoBehaviour
    {
        public Button btnStart;

        public GameObject content;

        private void Start()
        {
            btnStart.interactable = false;
        }
    }

}
