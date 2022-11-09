using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Defy.MonoBehavior
{
    public class AmmoPanelController : MonoBehaviour
    {
        [SerializeField]private List<AmmoBarController> AmmoBars;
        [SerializeField] private TMP_Text WeaponName;

        public void SetAmmo(int cur, int max)
        {
            int barCount = Mathf.Clamp(cur / 100+1,1,AmmoBars.Count);
            int remainderAmmo = cur % 100;
            for (int i = 0; i < AmmoBars.Count; i++)
            {
                //check activity
                if (i<barCount)
                {
                    AmmoBars[i].gameObject.SetActive(true);
                    if (max>=100)
                    {
                        //if max ammo is more than 100
                        AmmoBars[i].SetMaxAmmo(100);
                        if (i<barCount-1)
                        {
                            //this is a bar that remain full at moment
                            AmmoBars[i].SetCurAmmo(100);
                        }
                        else
                        {
                            //this is a bar that current been consuming
                            AmmoBars[i].SetCurAmmo(cur-100*i);
                        }
                    }
                    else
                    {
                        // ammo is less than 100
                        AmmoBars[i].SetMaxAmmo(max);
                        AmmoBars[i].SetCurAmmo(cur);
                    }
                }
                else
                {
                    AmmoBars[i].gameObject.SetActive(false);
                }
            }
        }
        

        public void SetWeaponName(string name)
        {
            WeaponName.text = name;
        }

    }
}