using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class MoveFollow : MonoBehaviour
    {

        public Transform targetBone; // 拖拽 root 或 DEF-head
        public float fixedY = 1.5f;   // 固定高度
        public float fixedZ = 2.5f;   // 固定前后位置
        public float smoothSpeed = 5f;

        private bool followEnabled = true; // 是否跟随

        void LateUpdate()
        {
            if (targetBone == null || !followEnabled) return;

            // 只用 targetBone 的 x 值，y 和 z 用固定值
            Vector3 desiredPosition = new Vector3(
                targetBone.position.x-2.5f,
                fixedY,
                fixedZ
            );

            // 平滑跟随
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

            // 朝向目标
            transform.LookAt(new Vector3(targetBone.position.x, fixedY, fixedZ));
        }

        /// <summary>
        /// 旋转到人物正面并停止跟随
        /// </summary>
        public void MoveFront(float rotateTime = 1.0f)
        {
            if (targetBone != null)
            {
                followEnabled = false; // 关闭跟随
                StartCoroutine(RotateAroundTarget(rotateTime));
            }
        }

        private IEnumerator RotateAroundTarget(float duration)
        {
            Vector3 startPos = transform.position;
            Vector3 center = targetBone.position;
            center.y = fixedY; // 固定高度
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 从 0° 旋转到 180°
                float angle = Mathf.Lerp(0, 180, t);
                Vector3 offset = Quaternion.Euler(0, angle, 0) * (startPos - center);
                transform.position = center + offset;
                transform.LookAt(center);

                yield return null;
            }
        }
    }   
}
