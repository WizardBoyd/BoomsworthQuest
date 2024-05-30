using System.Collections.Generic;
using System;
using UnityEngine;

/*
 changes:
  22.03.18
     CancelCondition {SetStartValue, SetEndValue, AsIs}
  29.03.18
     vector3 and vector3move
  19.10.18
    Cancel
     if (gO == null || tweenObjects==null) return;
   12.11.18
    add isCanceled for UpdateCallback (if object with tween destroyed but updated)
    old:
        case STT.FloatTween:
                        if(!isCanceled) currValue = (timeLeft) ? endValue : startValue + EaseFunc(currTime, dValue, tweenTime);
                        if (UpdateCallBack != null) UpdateCallBack(currValue);
                        break;
    new:
        case STT.FloatTween:
                        if (!isCanceled)
                        {
                            currValue = (timeLeft) ? endValue : startValue + EaseFunc(currTime, dValue, tweenTime);
                            if (UpdateCallBack != null) UpdateCallBack(currValue);
                        }
                        break;
	07.02.19
		add
			ForceCancel(gameobject)
  21.02.19
    improve cancel
 20.04.19 fixed 
    tween rebuild
27.06.19
  -comment debug
21.11.2019
  replace  gO.AddComponent<DontDestroyObj>();
  with DontDestroyOnLoad(gO);
20.03.2019
    add forcecancelall
     */
namespace Mkey
{
    public class SimpleTween : MonoBehaviour
    {
        public enum CancelCondition {SetStartValue, SetEndValue, AsIs}
        static List<SimpleTweenObject> tweenObjects;
        static SimpleTween Instance;
        static float PI = (float)Math.PI;
        static float PId2 = PI / 2.0f;
        private int length;
        private int freeInd = -1;

        static void Create()
        {
            if (Instance != null) return;
            GameObject gO = new GameObject();
            gO.name = "SimpleTween";
            SimpleTween sT = gO.AddComponent<SimpleTween>();
            Instance = sT;
            tweenObjects = new List<SimpleTweenObject>();
            DontDestroyOnLoad(gO);
        }

        /// <summary>
        /// Tween value start to end 
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static SimpleTweenObject Value(GameObject gO, float start, float end, float time)
        {
            Create();
            SimpleTweenObject sTO = CreateTweenObject(gO, start, end, time);
            return sTO;
        }

        /// <summary>
        /// Move gameobject transform from start to end position
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static SimpleTweenObject Value(GameObject gO, Vector3 startPosition, Vector3 endPosition, float time)
        {
            Create();
            SimpleTweenObject sTO = CreateTweenObject(gO, startPosition, endPosition, time, false);
            return sTO;
        }

        /// <summary>
        /// Move gameobject transform from start to end position
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static SimpleTweenObject Move(GameObject gO, Vector3 startPosition, Vector3 endPosition, float time)
        {
            Create();
            SimpleTweenObject sTO = CreateTweenObject(gO, startPosition, endPosition, time, true);
            return sTO;
        }

        /// <summary>
        /// Cancel all tweens for gameObject
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="OnComplete"></param>
        public static void Cancel(GameObject gO, bool OnComplete)
        {
            Cancel(gO, OnComplete, CancelCondition.AsIs);
        }

        /// <summary>
        /// Cancel all tweens for gameObject
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="OnComplete"></param>
        public static void Cancel(GameObject gO, bool OnComplete, float val)
        {
            if (gO == null || tweenObjects == null) return;
            List<SimpleTweenObject> sTOL = new List<SimpleTweenObject>();
            int length = tweenObjects.Count;
            SimpleTweenObject sTO;
            for (int i = 0; i < length; i++)
            {
                sTO = tweenObjects[i];
                if (sTO.gameObject == gO) sTOL.Add(sTO);
            }
            length = sTOL.Count;
            for (int i = 0; i < length; i++)
            {
                sTOL[i].Cancel(OnComplete, val);
            }
        }

        /// <summary>
        /// Cancel all tweens for gameObject
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="OnComplete"></param>
        public static void Cancel(GameObject gO, bool OnComplete, Vector3 val)
        {
            if (gO == null || tweenObjects == null) return;
            List<SimpleTweenObject> sTOL = new List<SimpleTweenObject>();
            int length = tweenObjects.Count;
            SimpleTweenObject sTO;
            for (int i = 0; i < length; i++)
            {
                sTO = tweenObjects[i];
                if (sTO.gameObject == gO) sTOL.Add(sTO);
            }
            length = sTOL.Count;
            for (int i = 0; i < length; i++)
            {
                sTOL[i].Cancel(OnComplete, val);
            }
        }

        /// <summary>
        /// Cancel all tweens for gameObject
        /// </summary>
        /// <param name="gO"></param>
        /// <param name="OnComplete"></param>
        public static void Cancel(GameObject gO, bool OnComplete, CancelCondition cancelCondition)
        {
            if (gO == null || tweenObjects==null) return;
            List<SimpleTweenObject> sTOL = new List<SimpleTweenObject>();
            int length = tweenObjects.Count;
            SimpleTweenObject sTO;
            for (int i = 0; i < length; i++)
            {
                sTO = tweenObjects[i];
                if (sTO.gameObject == gO) sTOL.Add(sTO);
            }
            length = sTOL.Count;
            for (int i = 0; i < length; i++)
            {
                sTOL[i].Cancel(OnComplete, cancelCondition);
            }
        }

        /// <summary>
        /// Cancel tween id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OnComplete"></param>
        public static void Cancel(int id, bool OnComplete)
        {
            Cancel(id, OnComplete, CancelCondition.AsIs);
        }

        /// <summary>
        /// Cancel tween id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OnComplete"></param>
        public static void Cancel(int id, bool OnComplete, CancelCondition cancelCondition)
        {
            if (tweenObjects == null) return;
            SimpleTweenObject sTO;
            int length = tweenObjects.Count;
            for (int i = 0; i < length; i++)
            {
                sTO = tweenObjects[i];
                if (sTO.ID == id) sTO.Cancel(OnComplete, cancelCondition);
            }
        }

        /// <summary>
        /// Cancel all tweens
        /// </summary>
        public static void ForceCancelAll()
        {
            if (tweenObjects == null) return;
            SimpleTweenObject sTO;
            int length = tweenObjects.Count;
            for (int i = 0; i < length; i++)
            {
                sTO = tweenObjects[i];
                sTO.Cancel(false);
            }
        }

        /// <summary>
        /// Cancel all tweens for gameObject, without to start complete callbacks
        /// </summary>
        /// <param name="gO"></param>
        public static void ForceCancel(GameObject gO)
        {
            Cancel(gO, false);
        }

        void Update()
        {
            length = tweenObjects.Count;
            if (length == 0) return;
            freeInd = -1;
            for (int i = 0; i < length; i++)
            {
                tweenObjects[i].Update(Time.deltaTime);
                if (tweenObjects[i].IsFree) freeInd=i;
            }

            if (freeInd > -1 && length > 50) tweenObjects.RemoveAt(freeInd);
            //Debug.Log("tween length: " + length);
        }

        private static void Remove(int id)
        {
            int pos = -1;
            int length = tweenObjects.Count;
            for (int i = 0; i < length; i++)
            {
                if (tweenObjects[i].ID == id) pos = i;
            }
            if (pos != -1) tweenObjects.RemoveAt(pos);
        }

        private static SimpleTweenObject CreateTweenObject(GameObject gO, float start, float end, float time)
        {
            SimpleTweenObject sTO;
            int length = tweenObjects.Count;// Debug.Log("tw length: " + length);
            for (int i = 0; i < length; i++)
            {
                if (tweenObjects[i].IsFree)
                {
                    sTO = tweenObjects[i];
                    sTO.Rebuild(gO, start, end, time);
                    return sTO;
                }
            }
            sTO = new SimpleTweenObject(gO, start, end, time);
            tweenObjects.Add(sTO);
            return sTO;
        }

        private static SimpleTweenObject CreateTweenObject(GameObject gO, Vector3 startPosition, Vector3 endPosition, float time, bool move)
        {
            SimpleTweenObject sTO;
            int length = tweenObjects.Count;// Debug.Log("tw length: " + length);
            for (int i = 0; i < length; i++)
            {
                if (tweenObjects[i].IsFree)
                {
                    sTO = tweenObjects[i];
                    sTO.Rebuild(gO, startPosition, endPosition, time, move);
                    return sTO;
                }
            }
            sTO = new SimpleTweenObject(gO, startPosition, endPosition, time, move);
            tweenObjects.Add(sTO);
            return sTO;
        }

        public class SimpleTweenObject
        {
            enum STT {FloatTween, Vector3Tween, Vector3TweenMove}
            static int lastID = 1;
            public GameObject gameObject;
            public int ID
            {
                get; private set;
            }
            private EaseAnim easeAnim;
            public bool IsFree
            {
                get; private set;
            }
            public bool IsCycled
            {
                get; private set;
            }

            private float startValue;
            private float endValue;
            private float dValue;
            private float currValue;

            private Vector3 startPosition;
            private Vector3 endPosition;
            private Vector3 dPosition;
            private Vector3 currPosition;

            private float tweenTime;
            private float currTime = 0.0f;
            private float delay = 0.0f;
            private Action completeCallBack;
            private Action<float> UpdateCallBack;
            private Action<Vector3> UpdateCallBackV3;
            private Func<float, float, float, float> EaseFunc;
            private float minValue;
            private float maxValue;
            private STT stt;
            private bool timeLeft = false;

            public SimpleTweenObject(GameObject gameObject, float startValue, float endValue, float tweenTime)
            {
                this.gameObject = gameObject;
                this.tweenTime = tweenTime;
                this.startValue = startValue;
                this.endValue = endValue;
                lastID++;
                ID = lastID;
                maxValue = (endValue >= startValue) ? endValue : startValue;
                minValue = (endValue <= startValue) ? endValue : startValue;
                currValue = startValue;
                dValue = endValue - startValue;
                EaseFunc = EaseLinear;
                stt = STT.FloatTween;
                IsFree = false;
                IsCycled = false;
                UpdateCallBack = null;
            }

            public SimpleTweenObject(GameObject gameObject, Vector3 startPosition, Vector3 endPosition, float tweenTime, bool move)
            {
                this.gameObject = gameObject;
                this.tweenTime = tweenTime;

                this.startPosition = startPosition;
                this.endPosition = endPosition;
                currPosition = startPosition;
                dPosition = endPosition - startPosition;

                lastID++;
                ID = lastID;

                EaseFunc = EaseLinear;
                stt =(!move)? STT.Vector3Tween : STT.Vector3TweenMove;
                IsFree = false;
                IsCycled = false;
                UpdateCallBackV3 = null;
            }

            public SimpleTweenObject SetEase(EaseAnim ease)
            {
                easeAnim = ease;
                switch (ease)
                {
                    case EaseAnim.EaseInSine:
                        EaseFunc = EaseInSine;
                        break;
                    case EaseAnim.EaseOutSine:
                        EaseFunc = EaseOutSine;
                        break;
                    case EaseAnim.EaseInOutSine:
                        EaseFunc = EaseInOutSine;
                        break;
                    case EaseAnim.EaseInQuad:
                        EaseFunc = EaseInQuad;
                        break;
                    case EaseAnim.EaseOutQuad:
                        EaseFunc = EaseOutQuad;
                        break;
                    case EaseAnim.EaseInOutQuad:
                        EaseFunc = EaseInOutQuad;
                        break;
                    case EaseAnim.EaseInCubic:
                        EaseFunc = EaseInCubic;
                        break;
                    case EaseAnim.EaseOutCubic:
                        EaseFunc = EaseOutCubic;
                        break;
                    case EaseAnim.EaseInOutCubic:
                        EaseFunc = EaseInOutCubic;
                        break;
                    case EaseAnim.EaseInQuart:
                        EaseFunc = EaseInQuart;
                        break;
                    case EaseAnim.EaseOutQuart:
                        EaseFunc = EaseOutQuart;
                        break;
                    case EaseAnim.EaseInOutQuart:
                        EaseFunc = EaseInOutQuart;
                        break;
                    case EaseAnim.EaseInQuint:
                        EaseFunc = EaseInQuint;
                        break;
                    case EaseAnim.EaseOutQuint:
                        EaseFunc = EaseOutQuint;
                        break;
                    case EaseAnim.EaseInOutQuint:
                        EaseFunc = EaseInOutQuint;
                        break;
                    case EaseAnim.EaseInExpo:
                        EaseFunc = EaseInExpo;
                        break;
                    case EaseAnim.EaseOutExpo:
                        EaseFunc = EaseOutExpo;
                        break;
                    case EaseAnim.EaseInOutExpo:
                        EaseFunc = EaseInOutExpo;
                        break;
                    case EaseAnim.EaseInCirc:
                        EaseFunc = EaseInCirc;
                        break;
                    case EaseAnim.EaseOutCirc:
                        EaseFunc = EaseOutCirc;
                        break;
                    case EaseAnim.EaseInOutCirc:
                        EaseFunc = EaseInOutCirc;
                        break;
                    case EaseAnim.EaseInBack:
                        EaseFunc = EaseInBack;
                        break;
                    case EaseAnim.EaseOutBack:
                        EaseFunc = EaseOutBack;
                        break;
                    case EaseAnim.EaseInOutBack:
                        EaseFunc = EaseInOutBack;
                        break;
                    case EaseAnim.EaseInElastic:
                        EaseFunc = EaseInElastic;
                        break;
                    case EaseAnim.EaseOutElastic:
                        EaseFunc = EaseOutElastic;
                        break;
                    case EaseAnim.EaseInOutElastic:
                        EaseFunc = EaseInOutElastic;
                        break;
                    case EaseAnim.EaseInBounce:
                        EaseFunc = EaseInBounce;
                        break;
                    case EaseAnim.EaseOutBounce:
                        EaseFunc = EaseOutBounce;
                        break;
                    case EaseAnim.EaseInOutBounce:
                        EaseFunc = EaseInOutBounce;
                        break;
                    default:
                        EaseFunc = EaseLinear;
                        break;
                }
                return this;
            }

            public SimpleTweenObject SetDelay(float delay)
            {
                this.delay = delay;
                return this;
            }

            public SimpleTweenObject SetOnUpdate(Action<float> callBack)
            {
                UpdateCallBack = callBack;
                return this;
            }

            public SimpleTweenObject SetOnUpdate(Action<Vector3> callBack)
            {
                UpdateCallBackV3 = callBack;
                return this;
            }

            public SimpleTweenObject AddCompleteCallBack(Action callBack)
            {
               if(callBack!=null) completeCallBack += callBack;
                return this;
            }

            public SimpleTweenObject SetCycled()
            {
                IsCycled = true;
                return this;
            }
 
            public void Update(float deltaTime)
            {
                if (IsFree) return;
               // Debug.Log(ID + " : " + gameObject );
                if (delay > 0)
                {
                    delay -= deltaTime;
                    return;
                }

                currTime += deltaTime;
                timeLeft = (currTime >= tweenTime);

                UpdateValue();

                if (timeLeft)
                {
                     completeCallBack?.Invoke();
                    if (!IsCycled) IsFree = true;
                    else
                    {
                        Restart();
                    }
                }
            }

            private void UpdateValue()
            {
                switch (stt)
                {
                    case STT.FloatTween:
                        currValue = (timeLeft) ? endValue : startValue + EaseFunc(currTime, dValue, tweenTime);
                        UpdateCallBack?.Invoke(currValue);
                        break;
                    case STT.Vector3Tween:
                        currPosition = (timeLeft) ? endPosition : startPosition + new Vector3(EaseFunc(currTime, dPosition.x, tweenTime), EaseFunc(currTime, dPosition.y, tweenTime), EaseFunc(currTime, dPosition.z, tweenTime));
                        UpdateCallBackV3?.Invoke(currPosition);
                        break;
                    case STT.Vector3TweenMove:
                        currPosition = (timeLeft) ? endPosition : startPosition + new Vector3(EaseFunc(currTime, dPosition.x, tweenTime), EaseFunc(currTime, dPosition.y, tweenTime), EaseFunc(currTime, dPosition.z, tweenTime));
                        if (gameObject) gameObject.transform.position = currPosition;
                        UpdateCallBackV3?.Invoke(currPosition);
                        break;
                }
            }

            private float EaseLinear(float t, float c, float d)
            {
                return c * t / d;
            }

            #region 1) quad
            private float EaseInQuad(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                return c * t * t;
            }

            private float EaseOutQuad(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                return c * t * (2.0f - t);
            }

            private float EaseInOutQuad(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                if (t < 0.5f) return c * 2.0f * t * t;
                return c * (t * (4 - 2.0f * t) - 1.0f);
            }
            #endregion quad

            #region 2) sine
            private float EaseInSine(float t, float c, float d)
            {
                if (d == 0) return 1;
                return c * (1.0f - Mathf.Cos(t / d * PId2));
            }

            private float EaseOutSine(float t, float c, float d)
            {
                if (d == 0) return 1;
                return c * Mathf.Sin(t / d * PId2);
            }

            private float EaseInOutSine(float t, float c, float d)
            {
                if (d == 0) return 1;
                return c * (1.0f - Mathf.Cos(t / d * PI)) * 0.5f;
            }
            #endregion sine

            #region 3) cubic
            private float EaseInCubic(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                return c * t * t * t;
            }

            private float EaseOutCubic(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                t = t - 1.0f;
                return c * (t * t * t + 1.0f);
            }

            private float EaseInOutCubic(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                if (t < 0.5f) return c * 4.0f * t * t * t;
                t = t - 1.0f;
                return c * (4.0f * t * t * t + 1.0f);
            }
            #endregion cubic

            #region 4) quart
            private float EaseInQuart(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                t = t * t;
                return c * t * t;
            }

            private float EaseOutQuart(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                t = t - 1.0f;
                t = t * t;
                return c * (1.0f - t * t);
            }

            private float EaseInOutQuart(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float t2 = t * t;
                if (t < 0.5f) return c * (8.0f * t2 * t2);
                t = t - 1.0f;
                t2 = t * t;
                return c * (1.0f - 8.0f * (t2 * t2));
            }
            #endregion quart

            #region 5) quint
            private float EaseInQuint(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float t2 = t * t;
                return c * t2 * t2 * t;
            }

            private float EaseOutQuint(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                t = t - 1.0f;
                float t2 = t * t;
                return c * (1.0f + t2 * t2 * t);
            }

            private float EaseInOutQuint(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float t2 = t * t;
                if (t < 0.5f) return c * (16.0f * t2 * t2 * t);
                t = t - 1.0f;
                t2 = t * t;
                return c * (1.0f + 16.0f * t2 * t2 * t);
            }
            #endregion quint

            #region 6) bounce
            private float EaseInBounce(float t, float c, float d)
            {
                return (c - EaseOutBounce(d - t, c, d));
            }

            private float EaseOutBounce(float t, float c, float d)
            {
                if (d == 0) return 1;
                float y;
                float t1;
                float k275 = 1f / 2.75f;
                t = t / d;
                if (t < k275)
                {
                    y = (7.5625f * t * t);
                }
                else if (t < (2f * k275))
                {
                    t1 = t - 1.5f * k275;
                    y = (7.5625f * t1 * t1 + 0.75f);
                }
                else if (t < (2.5f * k275))
                {
                    t1 = t - 2.25f * k275;
                    y = 7.5625f * t1 * t1 + 0.9375f;
                }
                else
                {
                    t1 = t - 2.625f * k275;
                    y = 7.5625f * t1 * t1 + 0.984375f;
                }
                return c * y;
            }

            private float EaseInOutBounce(float t, float c, float d)
            {
                if (t < 0.5f * d)
                {
                    return (0.5f * EaseInBounce(t * 2.0f, c, d));
                }
                else
                {
                    return (0.5f * EaseOutBounce(t * 2.0f - d, c, d) + c * 0.5f);
                }
            }
            #endregion bounce

            #region 7) expo
            private float EaseInExpo(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                if (t == 0) return 0;
                else return c * Mathf.Pow(2.0f, 10.0f * (t - 1.0f));
            }

            private float EaseOutExpo(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                if (t == 1.0f) return c;
                else return c * (1.0f - Mathf.Pow(2.0f, -10.0f * t));
            }

            private float EaseInOutExpo(float t, float c, float d)
            {
                if (d == 0) return 1;
                if (t == 0) return 0;
                else if (t == 1) return c;
                else
                {
                    float td2 = t * 2.0f;
                    if (td2 < 1.0f) return c * Mathf.Pow(2.0f, 10.0f * (td2 - 1.0f)) * 0.5f;
                    else
                    {
                        float tm1 = td2 - 1.0f;
                        return c * (2.0f - Mathf.Pow(2.0f, -10.0f * tm1)) * 0.5f;
                    }
                }
            }
            #endregion expo

            #region 8) circ
            private float EaseInCirc(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                return c * (1.0f - Mathf.Sqrt(1.0f - t * t));
            }

            private float EaseOutCirc(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                t = t - 1;
                return c * Mathf.Sqrt(1.0f - t * t);
            }

            private float EaseInOutCirc(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float td2 = t * 2.0f;
                if (td2 < 1.0f)
                {
                    return c * (1.0f - Mathf.Sqrt(1.0f - td2 * td2)) * 0.5f;
                }
                td2 = td2 - 2.0f;
                return c * (Mathf.Sqrt(1.0f - td2 * td2) + 1.0f) * 0.5f;
            }
            #endregion circ

            #region 9) back
            private float EaseInBack(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float s =  1.70158f;
               // Debug.Log("t: " + t + " ;c: "+c +  " ;d: " + d +" ; return: " +  (c * t * t * ((s + 1.0f) * t - s)));
                return c * t * t * ((s + 1.0f) * t - s);
            }

            private float EaseOutBack(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float s =  1.70158f;
                t = t - 1;
                return c * (t * t * ((s + 1.0f) * t + s) + 1.0f);
            }

            private float EaseInOutBack(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float s =   1.70158f;
                float s1 = s * 1.525f;

                float td2 = t * 2.0f;
                if (td2 < 1.0f) return c * 0.5f * (td2 * td2 * ((s1 + 1.0f) * td2 - s1));

                td2 = td2 - 2.0f;
                return c * 0.5f * (td2 * td2 * ((s1 + 1.0f) * td2 + s1) + 2.0f);
            }
            #endregion back

            #region 10) elastic
            private float EaseInElastic(float t, float c, float d)
            {
                if (d == 0) return 1;
                if (t == 0) return 0;
                t /= d;
                return c * ((0.04f - 0.04f / t) * Mathf.Sin(25f * t) + 1f);
            }

            private float EaseOutElastic(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float tt = t - 1f;
                if (tt == 0) return 0;
                return c * (0.04f * t / tt * Mathf.Sin(25f * tt));
            }

            private float EaseInOutElastic(float t, float c, float d)
            {
                if (d == 0) return 1;
                t /= d;
                float tm05 = t - 0.5f;
                if (tm05 == 0) return 0;
                if (tm05 < 0.0f)
                    return c * (0.02f + 0.01f / tm05) * Mathf.Sin(50f * tm05);
                else
                    return c * ((0.02f - 0.01f / tm05) * Mathf.Sin(50f * tm05) + 1f);
            }
            #endregion elastic

            #region cancel
            public void Cancel(bool OnComplete)
            {
                Cancel(OnComplete, CancelCondition.AsIs);
            }

            public void Cancel(bool OnComplete, float val)
            {
                currValue = val;
                startValue = val;
                endValue = val;
                Cancel(OnComplete, CancelCondition.AsIs);
            }

            public void Cancel(bool OnComplete, Vector3 val)
            {
                currPosition = val;
                Cancel(OnComplete, CancelCondition.AsIs);
            }

            public void Cancel(bool OnComplete, CancelCondition cancelCondition)
            {
                if (IsFree) return;
                IsFree = true;
                IsCycled = false;

                switch (cancelCondition)
                {
                    case CancelCondition.SetStartValue:
                        currValue = startValue;
                        currPosition = startPosition;
                        break;
                    case CancelCondition.SetEndValue:
                        currValue = endValue;
                        currPosition = endPosition;
                        break;
                }

                switch (stt)
                {
                    case STT.FloatTween:
                        UpdateCallBack?.Invoke(currValue);
                        break;
                    case STT.Vector3Tween:
                        UpdateCallBackV3?.Invoke(currPosition);
                        break;
                    case STT.Vector3TweenMove:
                        if (gameObject) gameObject.transform.position = currPosition;
                        UpdateCallBackV3?.Invoke(currPosition);
                        break;
                }
                if (OnComplete) completeCallBack?.Invoke();
            }
            #endregion cancel

            internal void Rebuild(GameObject gameObject, float startValue, float endValue, float tweenTime)
            {
                this.gameObject = gameObject;
                this.tweenTime = tweenTime;
                this.startValue = startValue;
                this.endValue = endValue;
                lastID++;
                ID = lastID;
                maxValue = (endValue >= startValue) ? endValue : startValue;
                minValue = (endValue <= startValue) ? endValue : startValue;
                currValue = startValue;
                dValue = endValue - startValue;
                EaseFunc = EaseLinear;
                currTime = 0.0f;
                delay = 0.0f;
                stt = STT.FloatTween;
                IsFree = false;
                IsCycled = false;
                completeCallBack = null;
                UpdateCallBackV3 = null;
                UpdateCallBack = null;
              //  Debug.Log("Rebuild " + ID + gameObject);
            }

            internal void Rebuild(GameObject gameObject, Vector3 startPosition, Vector3 endPosition,  float tweenTime, bool move)
            {
                this.gameObject = gameObject;
                this.tweenTime = tweenTime;
                lastID++;
                ID = lastID;

                this.startPosition = startPosition;
                this.endPosition = endPosition;
                currPosition = startPosition;
                dPosition = endPosition - startPosition;

                EaseFunc = EaseLinear;
                currTime = 0.0f;
                delay = 0.0f;
                stt =(move) ? STT.Vector3TweenMove : STT.Vector3Tween;
                IsFree = false;
                IsCycled = false;
                completeCallBack = null;
                UpdateCallBack = null;
                UpdateCallBackV3 = null;
               // Debug.Log("Rebuild " + ID + gameObject);
            }

            internal void Restart()
            {
                currTime = 0.0f;
                delay = 0.0f;
                currPosition = startPosition;
                dPosition = endPosition - startPosition;
                maxValue = (endValue >= startValue) ? endValue : startValue;
                minValue = (endValue <= startValue) ? endValue : startValue;
                currValue = startValue;
                dValue = endValue - startValue;
                IsFree = false;
            }
        }
    }

    public enum EaseAnim
    {
        EaseLinear,
        EaseInSine, EaseOutSine, EaseInOutSine,
        EaseInQuad, EaseOutQuad, EaseInOutQuad,
        EaseInCubic, EaseOutCubic, EaseInOutCubic,
        EaseInQuart, EaseOutQuart, EaseInOutQuart,
        EaseInQuint, EaseOutQuint, EaseInOutQuint,
        EaseInExpo, EaseOutExpo, EaseInOutExpo,
        EaseInCirc, EaseOutCirc, EaseInOutCirc,
        EaseInBack, EaseOutBack, EaseInOutBack,
        EaseInElastic, EaseOutElastic, EaseInOutElastic,
        EaseInBounce, EaseOutBounce, EaseInOutBounce
    }
}
//https://gist.github.com/gre/1650294
//http://gizma.com/easing/
//https://stackoverflow.com/questions/5207301/jquery-easing-functions-without-using-a-plugin
//https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/easing-functions
//http://robertpenner.com/scripts/easing_equations.txt
//http://easings.net/
