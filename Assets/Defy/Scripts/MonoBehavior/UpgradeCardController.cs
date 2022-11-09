using System;
using Defy.Component;
using Defy.System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Defy.MonoBehavior
{
    public class UpgradeCardController : MonoBehaviour
    {
        [SerializeField] private TMP_Text UpgradeText;
        [SerializeField] private TMP_Text WeaponNameText;
        [SerializeField] private Button _Button;
        public Action ButtonCallback;
        private WeaponCardInfo _Info;

        private void Start()
        {
            //set button callback
            var changeWeaponSys = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ChangeWeaponSystem>();
            _Button.onClick.AddListener(()=>
            {
                changeWeaponSys.ChangeWeaponAndUpgrade(_Info);
                changeWeaponSys.State = ChangeWeaponSystem.ChangeWeaponState.No;
                ButtonCallback.Invoke();
                Time.timeScale = 1;
            });
        }

        public void SetCard(WeaponCardInfo info)
        {
            //update weapon card info
            _Info = info;
            WeaponNameText.text = info.FullName;
            UpgradeText.text = info.UpgradeDesc;
            
        }
    }

    public struct WeaponCardInfo
    {
        public int Id;
        public int Level;
        public string FullName;
        public string UpgradeDesc;
    }
}