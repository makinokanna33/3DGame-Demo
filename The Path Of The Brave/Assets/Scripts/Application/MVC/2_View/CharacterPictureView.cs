using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns.Facade;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class CharacterPictureView : MonoBehaviour
    {
        public Image picture;
        public Toggle toggle;

        [HideInInspector] public int id;
        [HideInInspector] public bool isEnable;

        public void Init(int id, Sprite sprite, ToggleGroup toggleGroup)
        {
            this.id = id;
            picture.sprite = sprite;
            picture.color = new Color(picture.color.r, picture.color.g, picture.color.b, 0.5f);

            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        public void OnValueChanged(bool value)
        {
            if (value)
            {
                if (isEnable) // 防止重复点击
                {
                    return;
                }

                isEnable = true;
                picture.color = new Color(picture.color.r, picture.color.g, picture.color.b, 1f);

                GameFacade.Instance.SendNotification(AppConst.C_ResetUpdateCharacterInfo, new CharacterArgs(id));
            }
            else
            {
                isEnable = false;
                picture.color = new Color(picture.color.r, picture.color.g, picture.color.b, 0.5f);
            }
        }
    }
}

