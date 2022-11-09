/*using System;
using KittyHelpYouOut;
using Obj701.Model.Player;
using QFramework;

namespace Obj701
{
    public abstract class AbstractBuff: ICanGetModel
    {
        private PlayerModel _PM;
        protected PlayerModel PM
        {
            get
            {
                _PM ??= this.GetModel<PlayerModel>();
                return _PM;
            }
        }
        public enum BuffStackType
        {
            Stack,
            Refresh
        }

        public BuffStackType StackType;//set
        public int MaxStack;//set
        protected int Stack=1;
        public float MaxTimeSpan;//set
        protected float RemainTime=Single.PositiveInfinity;
        public float CycleTime=0;//set
        protected float LastCycleTime;
        public abstract void BuffStart();
        public abstract void BuffEnd();
        public abstract void BuffCycle();
        public float Tick(float Δt)
        {
            if (RemainTime>MaxTimeSpan)
            {
                RemainTime = MaxTimeSpan;
                BuffStart();
                LastCycleTime = 10 * MaxTimeSpan;
            }
            RemainTime -= Δt;
            if (CycleTime!=0 && LastCycleTime-RemainTime>=CycleTime)
            {
                LastCycleTime = RemainTime;
                for (int i = 0; i < Stack; i++)
                {
                    BuffCycle();
                }
            }
            if (RemainTime<=0)
            {
                for (int i = 0; i < Stack; i++)
                {
                    BuffEnd();
                }
            }
            return RemainTime;
        }

        public void TryStack()
        {
            switch (StackType)
            {
                case BuffStackType.Stack:
                    if (Stack<MaxStack)
                    {
                        Stack++;
                        RemainTime = MaxTimeSpan*10;
                    }
                    break;
                case BuffStackType.Refresh:
                    RemainTime = MaxTimeSpan;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public IArchitecture GetArchitecture()
        {
            return Obj701Architecture.Interface;
        }
    }
}*/