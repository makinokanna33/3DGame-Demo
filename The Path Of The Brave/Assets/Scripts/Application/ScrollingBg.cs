using UnityEngine;

namespace MyApplication
{
    public class ScrollingBg : MonoBehaviour
    {
        [SerializeField] private float scrollingSpeed = 0;
        [SerializeField] private float bound = 0;

        private void Update()
        {
            transform.Translate(-1 * scrollingSpeed * Time.deltaTime, 0, 0);

            if (transform.position.x <= -bound)
                transform.position = new Vector2(0, 0);
        }
    }
}
