using System.Collections.Generic;
using UnityEngine;

namespace Defy.MonoBehavior
{
    public class UpgradePanelController : MonoBehaviour
    {
        public List<UpgradeCardController> Cards;

        private void Start()
        {
            Cards.ForEach(i=>i.ButtonCallback=HideAllCards);
        }

        public void ShowUpgrade(WeaponCardInfo[] infos)
        {
            Time.timeScale = 0;
            Cards.ForEach(i=>i.gameObject.SetActive(false));
            for (int i = 0; i < infos.Length; i++)
            {
                Cards[i].gameObject.SetActive(true);
                Cards[i].SetCard(infos[i]);
            }
        }

        public void HideAllCards()
        {
            Cards.ForEach(i=>i.gameObject.SetActive(false));
        }

    }
}