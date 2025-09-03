using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景移动控制。用于控制某个物体（如摄像机、手模型等）在多个预设位置间的切换
namespace QFramework.Example
{
    public class SceneMove : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] Transform camera;
        [SerializeField] Transform[] targetPositions;

        //立即移动到指定位置
        public void TransferImmediately(int index)
        {
            StopAllCoroutines();//停止所有协程
            Transform target = targetPositions[index];//获取目标位置
            camera.position = target.position;//移动到目标位置
            camera.rotation = target.rotation;//旋转到目标方向
            camera.localScale = target.localScale;//缩放到目标大小
        }

        //平滑移动到指定位置
        public void TransferLerp(int index)
        {
            StopAllCoroutines();
            Transform target = targetPositions[index];
            StartCoroutine(LerpCoroutine(target));
        }

        private IEnumerator LerpCoroutine(Transform target)
        {
            float duration = 1f;
            float elapsed = 0f;
            Vector3 initialPosition = camera.localPosition;
            Quaternion initialRotation = camera.localRotation;
            Vector3 initialScale = camera.localScale;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                camera.localPosition = Vector3.Lerp(initialPosition, target.localPosition, t);
                camera.localRotation = Quaternion.Lerp(initialRotation, target.localRotation, t);
                camera.localScale = Vector3.Lerp(initialScale, target.localScale, t);
                yield return null;
            }
            camera.localScale = target.localScale;
            camera.localPosition = target.localPosition;
            camera.localRotation = target.localRotation;
        }
    }
}

