using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class StartView : MonoBehaviour
    {
        [Header("UI组件")]
        public Image logo;
    
        public Button btnStart;
        public Button btnSetting;
        public Button btnAbout;
        public Button btnExit;

        public Text textStart;
        public Text textSetting;
        public Text textAbout;
        public Text textExit;
    }
}
