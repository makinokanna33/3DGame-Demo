using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class CameraManager : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 center;
        public Vector3 limitXY;
        private Coroutine shake;

        private void Awake()
        {
            GameManager.Instance.cameraManager = this;
        }

        void Start()
        {
            center = this.transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            if (Input.GetMouseButton(1))
            {
                var x = Input.GetAxis("Mouse X") * 15 * Time.deltaTime;
                var z = Input.GetAxis("Mouse Y") * 15 * Time.deltaTime;

                var eulerAngles = Camera.main.transform.localEulerAngles;

                eulerAngles.x = 0;

                var ro = Quaternion.Euler(eulerAngles);
                Camera.main.transform.Translate(ro * new Vector3(-x, 0, -z), Space.World);
            }

            var maxX = center.x + limitXY.x;
            var minX = center.x - limitXY.x;

            var minZ = center.z - limitXY.z;
            var maxZ = center.z + limitXY.z;
            var pos = this.transform.position;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            this.transform.position = pos;
        }

        // 镜头晃动
        public void Shake(float duration, float power = 1)
        {
            if (shake != null) StopCoroutine(shake);

            shake = StartCoroutine(ShakeCoroutine(duration, power));
        }

        /// <summary>
        /// 晃动镜头 晃动力量衰减
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator ShakeCoroutine(float duration, float power = 1)
        {
            var startTime = Time.time;

            var y = this.transform.position.y;
            var orgPos = this.transform.position;
            var effectPos = this.transform.position;
            var floatValue = 0;

            while (true)
            {
                var t = (Time.time - startTime) / duration;
                //晃动力量衰减
                effectPos.y = y + Mathf.Sin(floatValue) * (1 - t) * power;
                floatValue += 1;
                this.transform.position = effectPos;

                if (t >= 1)
                {
                    this.transform.position = orgPos;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

