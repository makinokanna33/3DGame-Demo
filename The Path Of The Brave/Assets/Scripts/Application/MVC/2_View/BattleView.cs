using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    [System.Serializable]
    public class BattleSkillUI
    {
        public Button btnSkill;
        public Image imgSkill;
        public Image imgSkillCD;
        public Text textSkillCD;
    }

    public class BattleView : MonoBehaviour
    {
        public GameObject midTip;
        public GameObject startTip;
        public GameObject playerTip;
        public GameObject enemyTip;
        public GameObject awardContainer;
        public GameObject failedContainer;
        public GameObject actionPanel;
        public GameObject skillConfirm;
        public GameObject skillSelectTarget;

        public Text textRoundNum;
        public Image[] winStar;
        public Text[] winInfo;

        public BattleSkillUI[] skillUIs;

        public Button btnWait;

        public Button btnSelectTargetCancel;

        public Button btnSkillConfirmSure;
        public Button btnSkillConfirmCancel;

        public Text textAwardPlayerAddExp;
        public Text textAwardCharacterAddExp;
        public Text textAwardGoldAdd;
        public ItemFillerView[] awardItems;
        public Button awardSure;
        public Button loseSure;

        private CanvasScaler canvasScaler;
        private bool updateHpImage;

        void Start()
        {
            if (canvasScaler == null)
                canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
        }

        private void LateUpdate()
        {
            // 每帧更新血条位置
            if (updateHpImage)
            {
                foreach (var player in GameManager.Instance.battleManager.characters)
                {
                    var screenPos = GetScreenPos(Camera.main, player.transform.position + new Vector3(0, 5, 0));
                    player.hpTransform.position = screenPos;
                }
            }
        }

        public void InitHpImage()
        {
            foreach (var player in GameManager.Instance.battleManager.characters)
            {
                Transform hpTransform = null;
                if (player.myCharacterData.camp == CharacterCamp.Player)
                {
                    // 加载UI资源
#if UNITY_EDITOR
                    hpTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/HpImageGreen.prefab")).transform;
#else
                    hpTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_HpImageGreen).transform;
#endif
                    hpTransform.SetParent(GameObject.Find("Canvas").transform, false);
                }
                else
                {
                    // 加载UI资源
#if UNITY_EDITOR
                    hpTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/HpImageRed.prefab")).transform;
#else
                    hpTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_HpImageRed).transform;
#endif
                    hpTransform.SetParent(GameObject.Find("Canvas").transform, false);
                }

                var screenPos = GetScreenPos(Camera.main, player.transform.position + new Vector3(0, 5, 0));
                hpTransform.position = screenPos;
                player.hpTransform = hpTransform;
                player.hpImage = hpTransform.Find("hp_front").GetComponent<Image>();

                player.viewHp = (int)player.myCharacterData.currentHp;
                UpdateHp(player);
            }

            updateHpImage = true;
        }

        public void UpdateHp(MyCharacterController player)
        {
            player.hpImage.fillAmount = (float)player.viewHp / player.myCharacterData.currentHp;
        }


        public void ShowHudDamage(int damage, Vector3 worldPos)
        {
            Transform hubTransform;
#if UNITY_EDITOR
            hubTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/HudText.prefab")).transform;
#else
            hubTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_HudText).transform;
#endif
            hubTransform.SetParent(GameObject.Find("Canvas").transform, false);

            var screenPos = GetScreenPos(Camera.main, worldPos);
            hubTransform.position = screenPos;


            StartCoroutine(FloatUI(hubTransform.gameObject));

            var text = hubTransform.Find("Text").GetComponent<Text>();
            text.text = damage.ToString();
        }

        public void ShowHudRestore(int num, Vector3 worldPos)
        {
            Transform hubTransform;
#if UNITY_EDITOR
            hubTransform = Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABRes/Prefabs/HudTextGreen.prefab")).transform;
#else
            hubTransform = ABManager.Instance.LoadRes<GameObject>(AppConst.AB_Prefabs, AppConst.AB_HudTextGreen).transform;
#endif
            hubTransform.SetParent(GameObject.Find("Canvas").transform, false);

            var screenPos = GetScreenPos(Camera.main, worldPos);
            hubTransform.position = screenPos;

            StartCoroutine(FloatUI(hubTransform.gameObject));

            var text = hubTransform.Find("Text").GetComponent<Text>();
            text.text = "+" + num;
        }

        IEnumerator FloatUI(GameObject go)
        {
            //1.2秒 升高180 米
            var duration = 1.2f;
            var startTime = Time.time;

            var startPos = go.transform.position;
            var y_offset = 180;
            float t1 = 0;
            while (t1 < 1)
            {
                t1 = (Time.time - startTime) / duration;

                if (t1 >= 1f) t1 = 1;

                yield return new WaitForEndOfFrame();

                var y = Mathf.Lerp(0, y_offset, t1);

                go.transform.position = startPos + new Vector3(0, y, 0);
            }
        }

        // 将精灵的世界坐标转换成屏幕坐标
        public Vector3 GetScreenPos(Camera cam, Vector3 worldPos)
        {
            if(canvasScaler == null)
                canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();

            var resolutionX = canvasScaler.referenceResolution.x;
            var resolutionY = canvasScaler.referenceResolution.y;
            var offset = (Screen.width / canvasScaler.referenceResolution.x) * (1 - canvasScaler.matchWidthOrHeight) +
                         (Screen.height / canvasScaler.referenceResolution.y) * canvasScaler.matchWidthOrHeight;
            var screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
            return new Vector3(screenPos.x, screenPos.y, 0);
        }

        public void ShowAwardPanel(BattleWinArgs args)
        {
            for (var index = 0; index < winStar.Length; index++)
            {
                winStar[index].gameObject.SetActive(!args.isComplete[index]);
                winInfo[index].text = args.challengeInfo[index];
            }

            var configData = args.dataProxy.GetSelectLevelData(args.awardId);
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

