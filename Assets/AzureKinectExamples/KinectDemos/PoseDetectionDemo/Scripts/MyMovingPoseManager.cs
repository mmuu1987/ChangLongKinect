using System;
using com.rfilkov.kinect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rfilkov.components
{


    /// <summary>
    /// MovingPoseManager manages and estimates the user performance in sequence of dynamic poses.
    /// </summary>
    public class MyMovingPoseManager : MonoBehaviour
    {

        /// <summary>
        /// 姿势检测成功事件
        /// </summary>
        public event Action<string> DetectorEvent;

        public Animator animator;

        public MYDynamicPoseDetector myDynamicPoseDetector;

        /// <summary>
        /// 检测时间
        /// </summary>
        public float _checkTime = 1.5f;

        /// <summary>
        /// 在checkTime里检测的次数
        /// </summary>
        public int _checkCount = 3;

        /// <summary>
        /// 检测通过的百分比
        /// </summary>
        public float _CheckPercent = 0.8f;


        /// <summary>
        /// 是否识别成功
        /// </summary>
        private bool _isTrigger = false;


        private Coroutine _checkCoroutine;

        private string _curAnimato;

        public void Init(float checkTime,int checkCount,float checkPercent)
        {
            _checkTime = checkTime;

            _checkCount = checkCount;

            _CheckPercent = checkPercent;

            StopAnimator();
        }

        public void PlayAnimator(string animatorName)
        {

            animator.enabled = true;
            animator.Play(animatorName);

            _curAnimato = animatorName;

            if (_checkCoroutine != null) StopCoroutine(_checkCoroutine);
            _checkCoroutine = StartCoroutine(Check());
        }

        public void StopAnimator()
        {
            if (_checkCoroutine != null) StopCoroutine(_checkCoroutine);
            animator.enabled = false;
        }
        /// <summary>
        /// 开始姿势检测
        /// </summary>
        /// <returns></returns>
        private IEnumerator Check()
        {
            

            int tempCount = 0;
            float tiemTemp = _checkTime / _checkCount;
            while (tempCount< _checkCount)
            {
                yield return  new WaitForSeconds(tiemTemp);

                float percent = myDynamicPoseDetector.GetMatchPercent();


              
                if (percent > _CheckPercent)
                {
                    tempCount++;
                }
                else
                {
                    Debug.Log("识别失败，需要重新识别");

                    _checkCoroutine= StartCoroutine(Check());
                    yield break;
                }

              
                
            }

            if (tempCount >= _checkCount)
            {
                //识别成功
              Debug.Log("识别成功");
              _isTrigger = true;
              if (DetectorEvent != null) DetectorEvent(_curAnimato);
            }
        }
#if UNITY_EDITOR
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
            {
                PlayAnimator("taishou");//PaiZhang
            }

            if (GUI.Button(new Rect(100f, 0f, 100f, 100f), "test"))
            {
                PlayAnimator("PaiZhang");//PaiZhang
            }
        }
#endif
    }
}

