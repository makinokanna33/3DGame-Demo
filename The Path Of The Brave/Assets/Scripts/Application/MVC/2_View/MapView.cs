using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace MyApplication
{
    public class MapView : MonoBehaviour
    {
        public Button btnBack;
        public Button btnStart;

        public LevelButtonView[] levelButtonViews;

        public Text textLevelName;
        public Image imageLevelIcon;
        public Text textLevelInfo;

        public Text textPlayerAddExp;
        public Text textCharacterAddExp;
        public Text textGoldAdd;

        public ItemFillerView[] trophyItems;

        public Image imageStarBar;
        public Button btnBox1Lock;
        public GameObject btnBox1UnLock;
        public Button btnBox2Lock;
        public GameObject btnBox2UnLock;
        public Text textStarNum;

        public Text textAwardPlayerAddExp;
        public Text textAwardCharacterAddExp;
        public Text textAwardGoldAdd;

        public ItemFillerView[] awardItems;
        public GameObject awardContainer;

        public int maxStarNum;

        public GameObject loading;
        public Image imageLoading;

        public void Start()
        {
            awardContainer.SetActive(false);
            loading.SetActive(false);
        }

        public void UpdateLevelButtonStatus(int index, SelectLevelJsonData data)
        {
            if (index >= levelButtonViews.Length)
                return;

            levelButtonViews[index].UpdateButtonStatus(data);
        }

        public void UpdateLevelInfo(SelectLevelConfigData configData)
        {
            textLevelName.text = configData.levelName;
            imageLevelIcon.sprite = configData.icon;
            textLevelInfo.text = configData.description;

            textPlayerAddExp.text = "+" + configData.playerAddExp + "Exp";
            textCharacterAddExp.text = "+" + configData.characterAddExp + "Exp";
            textGoldAdd.text = "+" + configData.addGoldNum + "Gold";

            foreach (var trophyItem in trophyItems)
            {
                trophyItem.gameObject.SetActive(false);
            }

            for (int i = 0; i < configData.trophyItem.Count; ++i)
            {
                trophyItems[i].imgItem.sprite = configData.trophyItem[i].itemConfigData.icon;
                trophyItems[i].Text = configData.trophyNum[i].ToString();
                trophyItems[i].gameObject.SetActive(true);
            }
        }

        public void UpdateStarAward(List<bool> getStarAward, int hasStarNum)
        {
            imageStarBar.fillAmount = hasStarNum / (float)maxStarNum;
            textStarNum.text = hasStarNum + "/" + maxStarNum;

            if (getStarAward.Count >= 1)
            {
                if (getStarAward[0])
                {
                    btnBox1Lock.gameObject.SetActive(false);
                    btnBox1UnLock.SetActive(true);
                }
                else
                {
                    btnBox1Lock.gameObject.SetActive(true);
                    btnBox1UnLock.SetActive(false);
                    btnBox1Lock.interactable = hasStarNum >= 3;
                }
            }

            if (getStarAward.Count >= 2)
            {
                if (getStarAward[1])
                {
                    btnBox2Lock.gameObject.SetActive(false);
                    btnBox2UnLock.SetActive(true);
                }
                else
                {
                    btnBox2Lock.gameObject.SetActive(true);
                    btnBox2UnLock.SetActive(false);
                    btnBox2Lock.interactable = hasStarNum >= 6;
                }
            }
        }

        public void ShowAwardPanel(StarAwardConfigData configData)
        {
            textAwardPlayerAddExp.text = "+" + configData.playerAddExp + "Exp";
            textAwardCharacterAddExp.text = "+" + configData.characterAddExp + "Exp";
            textAwardGoldAdd.text = "+" + configData.addGoldNum + "Gold";

            foreach (var item in awardItems)
            {
                item.gameObject.SetActive(false);
            }

            for (int i = 0; i < configData.trophyItem.Count; ++i)
            {
                awardItems[i].imgItem.sprite = configData.trophyItem[i].itemConfigData.icon;
                awardItems[i].Text = configData.trophyNum[i].ToString();
                awardItems[i].gameObject.SetActive(true);
            }

            awardContainer.SetActive(true);
        }
    }
}

