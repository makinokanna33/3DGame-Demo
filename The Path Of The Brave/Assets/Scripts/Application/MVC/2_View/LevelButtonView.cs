using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class LevelButtonView : MonoBehaviour
    {
        public Button levelButton;

        public GameObject myLock;
        public GameObject stars;
        public GameObject[] StarsBlank;

        public int sceneIndex;

        [HideInInspector] public bool isLock;

        public void UpdateButtonStatus(SelectLevelJsonData data)
        {
            isLock = data.isLock;

            if (isLock)
            {
                levelButton.interactable = false;
                myLock.SetActive(true);
                stars.SetActive(false);
            }
            else
            {
                levelButton.interactable = true;
                myLock.SetActive(false);
                stars.SetActive(true);

                for (int i = 0; i < data.startNum; ++i)
                {
                    StarsBlank[i].SetActive(false);
                }
            }
        }

        public void OnButtonClick()
        {
            GameFacade.Instance.SendNotification(AppConst.C_UpdateSelectLevelInfo, sceneIndex);
        }
}
}

