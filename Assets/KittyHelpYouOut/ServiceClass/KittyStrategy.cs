using System;
using System.Collections.Generic;
using UnityEngine;

namespace KittyHelpYouOut
{
    public interface IStrategy{}
    public abstract class KittyStrategy<T>: IStrategy
    {
        public abstract void Execute(T signal);
    }
    /// <summary>
    /// 当当当当，猫猫策略大机器！
    /// 基于参数类型-策略类型管理的策略系统
    /// </summary>
    public class KittyStrategyMachine
    {
        private static KittyStrategyMachine _Instance;
        public static KittyStrategyMachine Instance
        {
            get
            {
                _Instance ??= new KittyStrategyMachine();
                return _Instance;
            }
        }
        //non repeat Strategy instance cache, think some new way
        private Dictionary<Type, Dictionary<Type, IStrategy>> SignalType_StrategyCacheDic = new Dictionary<Type, Dictionary<Type, IStrategy>>();
        /// <summary>
        /// 注销策略
        /// </summary>
        /// <typeparam name="TSignal">参数类型</typeparam>
        /// <typeparam name="TStrategy">策略类型</typeparam>
        public void UnRegister<TSignal,TStrategy>()
        {
            //if SignalType_StrategyCacheDic contains this signal Type 
            if (SignalType_StrategyCacheDic.TryGetValue(typeof(TSignal), out Dictionary<Type, IStrategy> resultCache))
            {
                if (resultCache.ContainsKey(typeof(TStrategy)))
                {
                    resultCache.Remove(typeof(TStrategy));
                }
                else
                {
                    Debug.LogWarning($"Cannot remove Strategy, {typeof(TStrategy)} not found (=⊝ᆽ⊝=)");
                }
            }
            else
            {
                Debug.LogWarning($"Cannot remove Strategy, {typeof(TSignal)} not found (=⊝ᆽ⊝=)");
            }
        }
        /// <summary>
        /// 注册策略
        /// </summary>
        /// <param name="instance">策略类实例</param>
        /// <typeparam name="TSignal">信号类型</typeparam>
        public void Register<TSignal>( KittyStrategy<TSignal> instance )
        {
            //if SignalType_StrategyCacheDic contains this signal Type 
            if (SignalType_StrategyCacheDic.TryGetValue(typeof(TSignal),out Dictionary<Type,IStrategy> resultCache))
            {
                if (!resultCache.ContainsKey(instance.GetType()))
                {
                    resultCache.Add(instance.GetType(),instance);
                }
                else
                {
                    Debug.LogWarning($"Cannot add strategy, {instance.GetType()} already exist (=⊝ᆽ⊝=)");
                }
            }
            else
            {
                Dictionary<Type, IStrategy> newStrategyCache = new Dictionary<Type, IStrategy>();
                newStrategyCache.Add(instance.GetType(),instance);
                SignalType_StrategyCacheDic.Add(typeof(TSignal),newStrategyCache);
            }
        }
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="signal">信号实例</param>
        /// <typeparam name="TSignal">信号类型</typeparam>
        public void Execute<TSignal>(TSignal signal)
        {
            //if SignalType_StrategyCacheDic contains this signal Type 
            if (SignalType_StrategyCacheDic.TryGetValue(typeof(TSignal),out Dictionary<Type,IStrategy> resultCache))
            {
                if (resultCache!=null&&resultCache.Count>0)
                {
                    foreach (var keyValuePair in resultCache)
                    {
                        (keyValuePair.Value as KittyStrategy<TSignal>).Execute(signal);
                    }
                }
                else
                {
                    Debug.LogWarning($"Cannot Execute strategy, {signal.GetType()} have no strategy registered (=⊝ᆽ⊝=)");
                }
            }
        }
    }
    /// <summary>
    /// 当当当当，猫猫策略箱
    /// 接受一个参数的策略管理类,不涉及类型转换所以性能稍微高一些，而且本身是对象所以管理更灵活。
    /// </summary>
    /// <typeparam name="TSignal"></typeparam>
    public class KittyStrategyBox<TSignal>
    {
        private Dictionary<Type, KittyStrategy<TSignal>> StrategyDic = new Dictionary<Type, KittyStrategy<TSignal>>();
        /// <summary>
        /// 注册策略
        /// </summary>
        /// <param name="instance">策略类</param>
        /// <typeparam name="TStrategy">策略类型</typeparam>
        public void Register<TStrategy>(TStrategy instance) where TStrategy: IStrategy
        {
            if (!StrategyDic.ContainsKey(typeof(TStrategy)))
            {
                StrategyDic.Add(typeof(TStrategy),instance as KittyStrategy<TSignal>);
            }
            else
            {
                Debug.LogWarning($"Cannot add strategy, {typeof(TStrategy)} already exist (=⊝ᆽ⊝=)");
            }
        }
        /// <summary>
        /// 注销策略
        /// </summary>
        /// <typeparam name="TStrategy">策略类型</typeparam>
        public void UnRegister<TStrategy>()
        {
            if (StrategyDic.ContainsKey(typeof(TStrategy)))
            {
                StrategyDic.Remove(typeof(TStrategy));
            }
            else
            {
                Debug.LogWarning($"Cannot remove Strategy, {typeof(TStrategy)} not found (=⊝ᆽ⊝=)");
            }
        }
        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="signal">信号实例</param>
        public void Execute(TSignal signal)
        {
            if (StrategyDic!=null && StrategyDic.Count>0)
            {
                foreach (var strategy in StrategyDic)
                {
                    strategy.Value.Execute(signal);
                }
            }
            else
            {
                Debug.LogWarning($"Cannot execute Strategy, Dictionary of {typeof(TSignal)} is null or empty (=⊝ᆽ⊝=)");
            }
        }
    }
}