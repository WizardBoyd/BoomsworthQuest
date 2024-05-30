using System.Collections.Generic;
using System;
/*
 20.12.18
    avoid zero sequence error
    old
     public void Start()
        {
            breakSeq = false;
            IsComplete = false;
            fullComplete = new Action(() => { IsComplete = true; });
            CreateCB();
            callBackL[callBackL.Count - 1]();
        }

    new 
     public void Start()
        {
            breakSeq = false;
            IsComplete = false;
            fullComplete = new Action(() => { IsComplete = true; });
            if (seqL.Count == 0)
            {
                if (fullComplete != null) fullComplete();
                if (complCallBack != null) complCallBack();
                return;
            }
            CreateCB();
            callBackL[callBackL.Count - 1]();
        }

16.04.2019 paralleltween
old

    public void Start(Action completeAction)
        {
            for (int i = 0; i < seqL.Count; i++)
            {
                seqL[i](() => { ended++; if (ended == count) { completeAction?.Invoke(); } });
            }

        }

    new

public void Start(Action completeAction)
        {
            if (seqL.Count > 0)
            {
                for (int i = 0; i < seqL.Count; i++)
                {
                    seqL[i](() => { ended++; if (ended == count) { completeAction?.Invoke(); } });
                }
            }
            else
            {
                completeAction?.Invoke();
            }
        }

14.11.2019
    fix Break ()
    fix StartCycled()

 */
namespace Mkey
{
    public class TweenSeq
    {

        List<Action<Action>> seqL;
        List<Action> callBackL;

        Action fullComplete;
        Action complCallBack;
        bool breakSeq = false;

        public bool IsComplete
        {
            get;
            private set;
        }

        public void Start()
        {
            breakSeq = false;
            IsComplete = false;
            fullComplete = new Action(() => { IsComplete = true; });
            if (seqL.Count == 0)
            {
                fullComplete?.Invoke();
                complCallBack?.Invoke();
                return;
            }
            CreateCB();
            if (breakSeq) return;
            callBackL[callBackL.Count - 1]?.Invoke();
        }

        public void StartCycle()
        {
            CreateCB();
            fullComplete = new Action(() => { if(!breakSeq) callBackL[callBackL.Count - 1]?.Invoke(); });
            if (!breakSeq) callBackL[callBackL.Count - 1]?.Invoke();
        }

        public TweenSeq()
        {
            IsComplete = false;
            seqL = new List<Action<Action>>();
            callBackL = new List<Action>();
        }

        public void Add(Action<Action> tweenAction)
        {
            seqL.Add(tweenAction); 
        }

        public void Remove(Action<Action> tweenAction)
        {
            int ind = seqL.IndexOf(tweenAction);
            if (ind != -1)
            {
                seqL.RemoveAt(ind);
            }

        }

        public void Clear()
        {
            seqL.Clear();
            callBackL.Clear();
        }

        void CreateCB()
        {
            if (breakSeq) return;
            callBackL.Add(() =>
            {
                if (!breakSeq)
                    seqL[seqL.Count - 1](() =>
                    {
                        if (!breakSeq) fullComplete?.Invoke();
                        if (!breakSeq) complCallBack?.Invoke();
                    });
            });
            for (int i = 1; i < seqL.Count; i++)
            {
                if (breakSeq) return;
                Action cb = callBackL[i - 1];
                int counter = seqL.Count - 1 - i;
                callBackL.Add(() =>
                {
                    if (!breakSeq)
                        seqL[counter](() =>
                        {
                            if (!breakSeq) cb?.Invoke();
                        });
                });
            }
        }


        /// <summary>
        /// Set bevore start
        /// </summary>
        /// <param name="complCallBack"></param>
        public void OnComplete(Action complCallBack)//??
        {
            this.complCallBack = complCallBack;
        }

        public void Break()
        {
            // Debug.Log("break");
            breakSeq = true;
            callBackL.Clear();
            seqL.Clear();
            IsComplete = true;
        }
    }


    public class ParallelTween
    {
        List<Action<Action>> seqL;
        int count = 0;
        int ended = 0;

        public ParallelTween()
        {
            seqL = new List<Action<Action>>();
            ended = 0;
        }

        public void Add(Action<Action> tA)
        {
            seqL.Add(tA);
            count++;
        }

        public void Start(Action completeAction)
        {
            if (seqL.Count > 0)
            {
                for (int i = 0; i < seqL.Count; i++)
                {
                    seqL[i](() => { ended++; if (ended == count) { completeAction?.Invoke(); } });
                }
            }
            else
            {
                completeAction?.Invoke();
            }
        }
    }


    public class TweenSeqGruop
    {
        List<TweenSeq> tSqL;
        List<Action> callBackL;

        Action fullComplete;
        Action complCallBack;
        bool isComplete;

        public TweenSeqGruop()
        {
            tSqL = new List<TweenSeq>();
        }

        public void Add(TweenSeq tS)
        {
            tSqL.Add(tS);
        }

        public void Start()
        {
            if (tSqL.Count > 0)
            {
                CreateCB();
                tSqL[0].Start();
            }
        }

        void CreateCB()
        {
            callBackL = new List<Action>();

            if (tSqL.Count >= 2)
            {
                for (int i = 0; i < tSqL.Count - 1; i++)
                {
                    int n = i; // very important
                    tSqL[n].OnComplete(() => { tSqL[n + 1].Start(); });
                }
            }

            tSqL[tSqL.Count - 1].OnComplete(() => { if (fullComplete != null) fullComplete(); if (complCallBack != null) complCallBack(); });

        }

        public void OnComplete(Action complCallBack)
        {
            this.complCallBack = complCallBack;
        }

    }
}

