using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KittyHelpYouOut
{
    /// <summary>
    /// 猫猫定时器！准时帮你把关掉的灯打开（或者反过来）。开关值初始true，在被设置为false之后，经过预设时间自动恢复为true。不基于协程，拯救性能癌
    /// </summary>
    public class KittyTimer
    {
        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="defaultState">默认开关，到时间猫猫会把开关拨到这个状态</param>
        public KittyTimer(bool defaultState)
        {
            Switch = defaultState;
            DefaultSwitch = defaultState;
        }
        private bool Switch,DefaultSwitch,_Running;
        public bool SwitchValue => Switch;
        public bool Running => _Running;
        private float RemainTime=2;
        public void StartTimer(float maxTime)
        {
            RemainTime = maxTime;
            Switch = !DefaultSwitch;
            _Running = true;
        }
        /// <summary>
        /// 在update或fixUpdate中调用，按帧减少剩余时间，返回本次Tick后的开关状态
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public bool Tick(float timeSpan)
        {
            RemainTime -= timeSpan;
            if (RemainTime<=0)
            {
                Switch = DefaultSwitch;
                _Running = false;
            }
            return Switch;
        }

        public void Reset()
        {
            Switch = DefaultSwitch;
            _Running = false;
        }


    }
}