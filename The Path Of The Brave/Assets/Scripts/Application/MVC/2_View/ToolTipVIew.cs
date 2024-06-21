using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class ToolTipVIew : MonoBehaviour
    {
        [Header("UI组件")]
        public Text textInfo;

        [Header("组件")]
        public RectTransform rectTransform;
        public ContentSizeFitter contentSizeFitter;

        // 更新信息
        // 踩坑踩的最多的一个逻辑，里面有很多需要注意的点
        public void UpdateInfo(Vector2 position, string text)
        {
            // 该UI将显示在最前面
            transform.SetAsLastSibling();

            // 更新文本
            textInfo.text = text;

            // 强制刷新 ContentSizeFitter，否则这一帧得不到正确的宽高
            // 注意这里有一个坑！必须先让面板激活显示，LayoutRebuilder 这段代码才能刷新出来正确的宽高，否则还是会慢一帧
            transform.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            // 获取提示UI面板的宽高
            // rectTransform.rect 方法获取的宽高有问题，不会随屏幕缩放而改变，是固定的，所以判断会有问题
            // rectTransform.GetWorldCorners 方法可以解决这个问题
            Vector3[] vectors = new Vector3[4];
            rectTransform.GetWorldCorners(vectors);
            float width = vectors[2].x - vectors[1].x;
            float height = vectors[1].y - vectors[0].y;

            // 设置最终匹配的锚点
            Vector2 finallPivot;

            if (position.y < height)
                finallPivot.y = 0;
            else
                finallPivot.y = 1;

            if (position.x < width)
                finallPivot.x = 0;
            else
                finallPivot.x = 1;
            rectTransform.pivot = finallPivot;

            // 设置位置
            rectTransform.transform.position = position;
        }
    }
}
