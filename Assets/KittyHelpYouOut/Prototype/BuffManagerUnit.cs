/*using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obj701
{
    public class BuffManagerUnit : MonoBehaviour
    {
        private List<AbstractBuff> BuffList = new List<AbstractBuff>();
        private AbstractBuff AddBuffCache;
        
        public void AddBuff<T>(T buff) where T: AbstractBuff
        {
            AddBuffCache = BuffList.Find(x => x.GetType() == typeof(T));
            if (AddBuffCache!=null)//means already have this buff
            {
                AddBuffCache.TryStack();
            }
            else//means not having this buff yet
            {
                BuffList.Add(buff);
            }
        }

        private void Update()
        {
            for (int i = 0; i < BuffList.Count; i++)
            {
                if (BuffList[i].Tick(Time.deltaTime)<=0)
                {
                    BuffList.RemoveAt(i);
                }
            }
        }
    }
}*/