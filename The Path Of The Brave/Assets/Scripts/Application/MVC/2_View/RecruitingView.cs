using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace MyApplication
{
    [System.Serializable]
    public class RecruitingCharacterData
    {
        public CharacterSOData character;
        public Image picture;
        public Text textName;
        public Text textQuality;
        
        public Text textProb;

        public string quality;
        public Color color;
        public int prob;

        public void Init()
        {
            picture.sprite = character.configData.sprite;
            textName.text = character.configData.name;
            textQuality.text = quality;
            textQuality.color = color;
            textProb.text = prob.ToString() + "%";
            textProb.color = color;
        }
    }

    [System.Serializable]
    public class RecruitingView : MonoBehaviour
    {
        public Button btnClose;
        public Button btnStart;
        public Text textNum;

        public GameObject recruitingResult;
        public Text textInfo;
        public Image picture;
        public Button btnSure;


        public ItemBaseSOData itemBase;
        public List<RecruitingCharacterData> recruitingCharacterDataList;

        void Start()
        {
            recruitingResult.SetActive(false);

            foreach (var item in recruitingCharacterDataList)
            {
                item.Init();
            }
        }

        public void UpdateTextNum(int num)
        {
            if (num <= 0)
            {
                textNum.text = "x0";
                textNum.color = Color.red;
                btnStart.interactable = false;
            }
            else
            {
                textNum.text = "x" + num;
                textNum.color = Color.white;
                btnStart.interactable = true;
            }
        }
    }
}

