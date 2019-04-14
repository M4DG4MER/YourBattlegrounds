using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace tutoriales
{
    public class InventaryItemViewer : MonoBehaviour
    {
        InventaryGroupViewer inventaryGroupViewer;
        Image itemIcon;

        internal ItemController invItem;
        internal Transform mtransform;

        public Sprite defaultSprite;
        public Image countDown;
        internal bool StartCountDown;


        public TextMeshProUGUI Text;


        public void Initialize(InventaryGroupViewer inventaryGroup)
        {
            this.mtransform = this.transform;
            this.inventaryGroupViewer = inventaryGroup;
            itemIcon = GetComponent<Image>();
        }

        public void SetItem(ItemController item)
        {
            this.invItem = item;
            itemIcon.sprite = (item != null ? item.Stats.Icon : defaultSprite);
            Text.text = "";
            countDown.fillAmount = 0;
        }


        private void FixedUpdate()
        {
            if (invItem != null)
            {
                if (invItem is ConsumableItem)
                {
                    ConsumableItem it = (invItem as ConsumableItem);
                    
                    if (Text != null)
                    {
                        Text.text = it.Units + "/" + it.getConsumableStats().MaxUnits;
                    }

                    countDown.fillAmount = it.GetActualUseState();
                } 
                else if (invItem is GunController)
                {
                    GunController it = (invItem as GunController);
                    if (Text != null)
                    {
                        Text.text = it.FireState.mode + "\n" + it.ActualAmmo + "/" + it.getGunStats().maxClip;
                    }

                }
            }

           
        }



        public void OnClickSelect()
        {
            inventaryGroupViewer.thisGroup.Select(invItem);
        }

        public void OnClickDrop()
        {
            if (invItem != null)
            {
                CharController owner = invItem.owner;
                invItem.Drop(owner, owner.tr, false);
                owner.inventary.RemoveItem(invItem);
            }
        }
        

        public void OnClickTake()
        {
            inventaryGroupViewer.inventary_viewer.inventary.character.Take(invItem);
        }

    }
}
