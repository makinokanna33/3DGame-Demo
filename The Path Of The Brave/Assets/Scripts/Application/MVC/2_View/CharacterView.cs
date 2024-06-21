using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace MyApplication
{
    public class CharacterView : MonoBehaviour
    {
        [Header("UI组件")] public Button btnClose;
        public ToggleGroup teamToggleGroup;
        public ToggleGroup characterToggleGroup;
        public Toggle toggleProperty;
        public Toggle toggleEquip;
        public Toggle toggleSkill;

        [Header("状态 UI")] public Text textName;
        public Text textLv;
        public Image textExpBar;
        public Text textExp;
        public Text textHpNum;
        public Text textDefNum;
        public Text textAtkNum;
        public Text textMoveRangeNum;
        public Text textMinAtkRangeNum;
        public Text textMaxAtkRangeNum;
        public Image[] imageUnLockSkill;
        public Sprite unLockSkillSprite;

        [Header("及身 UI")] public Image imageCap;
        public Text textCapName;
        public Text textCapInfo;
        public Text textCapProperty;
        public Text textCapUpNum;
        public Button btnCapUp;

        public Image imageArmor;
        public Text textArmorName;
        public Text textArmorInfo;
        public Text textArmorProperty;
        public Text textArmorUpNum;
        public Button btnArmorUp;

        public Image imageJewelry;
        public Text textJewelryName;
        public Text textJewelryInfo;
        public Text textJewelryProperty;

        [Header("技能 UI")] 
        public Image imageSkill1;
        public Text textSkill1Name;
        public Text textSkill1Type;
        public Text textSkill1CD;
        public Text textSkill1Info;
        public Text textSkill1UnlockNum;
        public Button btnSkill1Unlock;
        public Text btnSkill1UnlockText;

        public Image imageSkill2;
        public Text textSkill2Name;
        public Text textSkill2Type;
        public Text textSkill2CD;
        public Text textSkill2Info;
        public Text textSkill2UnlockNum;
        public Button btnSkill2Unlock;
        public Text btnSkill2UnlockText;

        public Image imageSkill3;
        public Text textSkill3Name;
        public Text textSkill3Type;
        public Text textSkill3CD;
        public Text textSkill3Info;
        public Text textSkill3UnlockNum;
        public Button btnSkill3Unlock;
        public Text btnSkill3UnlockText;

        [Header("配置数据")] 
        public ItemBaseSOData upItem;
        public int addUpNum;
        public ItemBaseSOData unlockSkillItem;
        public int unlockSkillNum;

        [Header("其他")] public GameObject teamInfoContent;

        public GameObject propertyContainer;
        public GameObject equipContainer;
        public GameObject skillContainer;
        public GameObject skill1;
        public GameObject skill2;
        public GameObject skill3;
        public GameObject lockTip1;
        public GameObject lockTip2;
        public GameObject lockTip3;

        public void Start()
        {
            imageJewelry.gameObject.SetActive(false);

            // 默认优先显示状态面板
            HideAllContainer();
            propertyContainer.gameObject.SetActive(true);
        }

        public void OnDrop(BaseEventData data)
        {
            // 只有左键拖拽而来的物品才进行检测
            if ((data as PointerEventData).button == PointerEventData.InputButton.Left)
            {
                // 获取当前拖拽而来的物品
                ItemFillerView itemFillerView = (data as PointerEventData).pointerDrag.GetComponent<ItemFillerView>();
                if (itemFillerView == null)
                    return;

                // 发送通知处理拖拽放置事件
                CharacterViewMediator mediator =
                    GameFacade.Instance.RetrieveMediator(CharacterViewMediator.NAME) as CharacterViewMediator;
                GameFacade.Instance.SendNotification(AppConst.C_EquipmentJewelry,
                    new AddItemOwnerArgs(itemFillerView.itemData, itemFillerView.gridIndex,
                        mediator.currentCharacterId));
            }
        }

        public void HideAllContainer()
        {
            propertyContainer.gameObject.SetActive(false);
            equipContainer.gameObject.SetActive(false);
            skillContainer.gameObject.SetActive(false);
        }

        public void InitPropertyContainer(CharacterData characterData, int nextLvExp)
        {
            textName.text = characterData.configData.name;

            textLv.text = "LV:" + characterData.level;
            if (nextLvExp == 0)
            {
                textExp.text = "MAX";
                textExpBar.fillAmount = 1;
            }
            else
            {
                textExp.text = characterData.currentExp + "/" + nextLvExp;
                textExpBar.fillAmount = characterData.currentExp / (float)nextLvExp;
            }

            int equipAddHp = characterData.capItemData.maxHpUp + characterData.armorItemData.maxHpUp +
                             (characterData.jewelryData != null ? characterData.jewelryData.maxHpUp : 0);
            int equipAddDef = characterData.capItemData.maxDefUp + characterData.armorItemData.maxDefUp +
                             (characterData.jewelryData != null ? characterData.jewelryData.maxDefUp : 0);
            int equipAddAtk = characterData.capItemData.maxAtkUp + characterData.armorItemData.maxAtkUp +
                             (characterData.jewelryData != null ? characterData.jewelryData.maxAtkUp : 0);

            textHpNum.text = equipAddHp == 0
                ? characterData.maxHp.ToString()
                : string.Format("{0}(<color=green>+{1}</color>)", characterData.maxHp, equipAddHp);

            textDefNum.text = equipAddDef == 0
                ? characterData.maxDef.ToString()
                : string.Format("{0}(<color=green>+{1}</color>)", characterData.maxDef, equipAddDef);

            textAtkNum.text = equipAddAtk == 0
                ? characterData.maxAtk.ToString()
                : string.Format("{0}(<color=green>+{1}</color>)", characterData.maxAtk, equipAddAtk);

            textMoveRangeNum.text = characterData.configData.moveRange.ToString();
            textMinAtkRangeNum.text = characterData.configData.minAtkRange.ToString();
            textMaxAtkRangeNum.text = characterData.configData.maxAtkRange.ToString();

            // 展示已解锁技能
            int arrayIndex = 0;
            foreach (var t in imageUnLockSkill)
            {
                // 全部重置为默认图片
                t.sprite = unLockSkillSprite;
            }
            for (int i = 0; i < characterData.skillUnLock.Count; i++)
            {
                if (characterData.skillUnLock[i] && arrayIndex < imageUnLockSkill.Length)
                {
                    // 若已解锁展示解锁技能图片
                    imageUnLockSkill[arrayIndex++].sprite = characterData.configData.skillSoList[i].configData.icon;
                }
            }
        }

        public void InitEquipContainer(CharacterData characterData, int upItemNum)
        {
            imageCap.sprite = characterData.capItemData.configData.icon;
            textCapName.text = string.Format("名字：{0}<color=purple>+{1}</color>",
                characterData.capItemData.configData.itemName,
                characterData.capItemData.level);
            textCapInfo.text = string.Format("描述：{0}", characterData.capItemData.configData.description);
            textCapProperty.text = string.Format("属性：生命<color=green>+{0}</color>", characterData.capItemData.maxHpUp);

            // 计算升级所需要的材料数量
            int capUpNum = characterData.capItemData.level * addUpNum;
            if (upItemNum >= capUpNum)
            {
                btnCapUp.interactable = true;
                textCapUpNum.text = string.Format("{0}({1})", capUpNum, upItemNum);
            }
            else
            {
                btnCapUp.interactable = false;
                textCapUpNum.text = string.Format("{0}(<color=red>{1}</color>)", capUpNum, upItemNum);
            }


            imageArmor.sprite = characterData.armorItemData.configData.icon;
            textArmorName.text = string.Format("名字：{0}<color=purple>+{1}</color>",
                characterData.armorItemData.configData.itemName,
                characterData.armorItemData.level);
            textArmorInfo.text = string.Format("描述：{0}", characterData.armorItemData.configData.description);
            textArmorProperty.text =
                string.Format("属性：防御力<color=yellow>+{0}</color>", characterData.armorItemData.maxDefUp);

            // 计算升级所需要的材料数量
            int armorUpNum = characterData.armorItemData.level * addUpNum;
            if (upItemNum >= armorUpNum)
            {
                btnArmorUp.interactable = true;
                textArmorUpNum.text = string.Format("{0}({1})", armorUpNum, upItemNum);
            }
            else
            {
                btnArmorUp.interactable = false;
                textArmorUpNum.text = string.Format("{0}(<color=red>{1}</color>)", armorUpNum, upItemNum);
            }

            if (characterData.jewelryData != null)
            {
                imageJewelry.gameObject.SetActive(true);
                imageJewelry.sprite = characterData.jewelryData.configData.icon;
                textJewelryName.text = string.Format("名字：{0}", characterData.jewelryData.configData.itemName);
                textJewelryInfo.text = string.Format("描述：{0}", characterData.jewelryData.configData.description);
                string jewelryPropertyInfo = "属性：";
                if (characterData.jewelryData.maxHpUp != 0)
                    jewelryPropertyInfo +=
                        string.Format("<color=green>生命值：{0}</color> ", characterData.jewelryData.maxHpUp);
                if (characterData.jewelryData.maxAtkUp != 0)
                    jewelryPropertyInfo +=
                        string.Format("<color=red>攻击力：{0}</color> ", characterData.jewelryData.maxAtkUp);
                if (characterData.jewelryData.maxDefUp != 0)
                    jewelryPropertyInfo +=
                        string.Format("<color=yellow>防御力：{0}</color> ", characterData.jewelryData.maxDefUp);
                textJewelryProperty.text = jewelryPropertyInfo;
            }
            else
            {
                imageJewelry.gameObject.SetActive(false);
                textJewelryName.text = "名字：无";
                textJewelryInfo.text = "描述：无";
                textJewelryProperty.text = "属性：无";
            }
        }

        public void InitSkillContainer(CharacterData characterData, int lockSkillNum)
        {
            skill1.SetActive(false);
            skill2.SetActive(false);
            skill3.SetActive(false);

            int skillCount = characterData.configData.skillSoList.Count;

            if (skillCount >= 1)
            {
                var skillSoData = characterData.configData.skillSoList[0];
                skill1.SetActive(true);
                imageSkill1.sprite = skillSoData.configData.icon;
                textSkill1Name.text = "名字：" + skillSoData.configData.name;
                textSkill1Type.text = "类型：" + (skillSoData.configData.type == SkillType.Active ? "主动" : "被动");
                textSkill1CD.text = "冷却：" + skillSoData.configData.cd.ToString() + "回合";
                textSkill1Info.text = skillSoData.configData.description;

                if (characterData.skillUnLock[0])
                {
                    btnSkill1Unlock.interactable = false;
                    btnSkill1UnlockText.text = "已解锁";
                    lockTip1.SetActive(false);
                }
                else
                {
                    btnSkill1UnlockText.text = "技能解锁";
                    lockTip1.SetActive(true);
                    if (lockSkillNum >= unlockSkillNum)
                    {
                        btnSkill1Unlock.interactable = true;
                        textSkill1UnlockNum.text = string.Format("{0}({1})", unlockSkillNum, lockSkillNum);
                    }
                    else
                    {
                        btnSkill1Unlock.interactable = false;
                        textSkill1UnlockNum.text = string.Format("{0}(<color=red>{1}</color>)", unlockSkillNum, lockSkillNum);
                    }
                }
            }

            if (skillCount >= 2)
            {
                var skillSoData = characterData.configData.skillSoList[1];
                skill2.SetActive(true);
                imageSkill2.sprite = skillSoData.configData.icon;
                textSkill2Name.text = "名字：" + skillSoData.configData.name;
                textSkill2Type.text = "类型：" + (skillSoData.configData.type == SkillType.Active ? "主动" : "被动");
                textSkill2CD.text = "冷却：" + skillSoData.configData.cd.ToString() + "回合";
                textSkill2Info.text = skillSoData.configData.description;

                if (characterData.skillUnLock[1])
                {
                    btnSkill2Unlock.interactable = false;
                    btnSkill2UnlockText.text = "已解锁";
                    lockTip2.SetActive(false);
                }
                else
                {
                    btnSkill2UnlockText.text = "技能解锁";
                    lockTip2.SetActive(true);
                    if (lockSkillNum >= unlockSkillNum)
                    {
                        btnSkill2Unlock.interactable = true;
                        textSkill2UnlockNum.text = string.Format("{0}({1})", unlockSkillNum, lockSkillNum);
                    }
                    else
                    {
                        btnSkill2Unlock.interactable = false;
                        textSkill2UnlockNum.text = string.Format("{0}(<color=red>{1}</color>)", unlockSkillNum, lockSkillNum);
                    }
                }
            }

            if (skillCount >= 3)
            {
                var skillSoData = characterData.configData.skillSoList[2];
                skill3.SetActive(true);
                imageSkill3.sprite = skillSoData.configData.icon;
                textSkill3Name.text = "名字：" + skillSoData.configData.name;
                textSkill3Type.text = "类型：" + (skillSoData.configData.type == SkillType.Active ? "主动" : "被动");
                textSkill3CD.text = "冷却：" + skillSoData.configData.cd.ToString() + "回合";
                textSkill3Info.text = skillSoData.configData.description;

                if (characterData.skillUnLock[2])
                {
                    btnSkill3Unlock.interactable = false;
                    btnSkill3UnlockText.text = "已解锁";
                    lockTip3.SetActive(false);
                }
                else
                {
                    btnSkill3UnlockText.text = "技能解锁";
                    lockTip3.SetActive(true);
                    if (lockSkillNum >= unlockSkillNum)
                    {
                        btnSkill3Unlock.interactable = true;
                        textSkill3UnlockNum.text = string.Format("{0}({1})", unlockSkillNum, lockSkillNum);
                    }
                    else
                    {
                        btnSkill3Unlock.interactable = false;
                        textSkill3UnlockNum.text = string.Format("{0}(<color=red>{1}</color>)", unlockSkillNum, lockSkillNum);
                    }
                }
            }
        }
    }
}

