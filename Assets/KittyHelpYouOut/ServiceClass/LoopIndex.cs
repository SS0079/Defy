using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{ 
    public class LoopIndex
    {
        public LoopIndex(int mCount)
        {
            count = mCount;
            indexCache = 0;
        }

        private int count;
        private int indexCache;
        public int Index { get => indexCache % count; }
        public void Next()
        {
            indexCache++;
        }
        public void Previous()
        {
            indexCache--;
        }
    }

    public class LoopIndex<T>
    {
        public LoopIndex(List<T> mList)
        {
            list = mList;
        }
        private List<T> list;
        private int indexCache;
        public int Index
        {
            get
            {
                if (list.Count>0)
                {
                    int i;
                    try
                    {
                        i = indexCache % list.Count;
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError("List 长度为零");
                        throw;
                    }
                    return i;
                }
                else
                {
                    indexCache = 0;
                    return 0;
                }
            }
        }
        public void Next()
        {
            indexCache++;
        }
        public void Previous()
        {
            indexCache--;
        }
    }
//#if UNITY_EDITOR
//    public class TestTryCacheClass
//    {
//        public void TestTryCache()
//        {
//            List<int> sampleList = new List<int>();
//            LoopIndex<int> sampleLoopIndex = new LoopIndex<int>(sampleList);
//            sampleList.Add(1);
//            Debug.Log(sampleLoopIndex.Index);
//            sampleList.Add(1);
//            sampleLoopIndex.Next();
//            Debug.Log(sampleLoopIndex.Index);
//            sampleList.Add(1);
//            sampleLoopIndex.Next();
//            Debug.Log(sampleLoopIndex.Index);
//        }
//    }
//#endif
}


