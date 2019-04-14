using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class InventaryGroupViewer : MonoBehaviour
    {
        internal InventaryViewer inventary_viewer;
        internal InventaryGroup thisGroup;
        internal Transform mtransform;


        public string SlotType;

        List<InventaryItemViewer> itemViewers = new List<InventaryItemViewer>();

        public InventaryItemViewer ItemViewerPrefab;


        public void Initialize(InventaryViewer inventary_viewer, InventaryGroup group)
        {
            this.inventary_viewer = inventary_viewer;
            this.mtransform = this.transform;
            this.thisGroup = group;
            itemViewers = new List<InventaryItemViewer>(GetComponentsInChildren<InventaryItemViewer>());

            while (itemViewers.Count > 0)
            {
                InventaryItemViewer viewer = itemViewers[0];
                Destroy(viewer.gameObject);
                itemViewers.RemoveAt(0);
            }
            itemViewers.Clear();


            for (int i = 0; i< thisGroup.MaxCapacity; i++)
            {
                InventaryItemViewer itemViewer = Instantiate<InventaryItemViewer>(ItemViewerPrefab, this.mtransform);
                itemViewer.Initialize(this);
                itemViewers.Add(itemViewer);
                if (thisGroup.items != null && i < thisGroup.items.Count)
                {
                    itemViewer.SetItem(thisGroup.items[i]);
                }
            }
            thisGroup.AddedItem += new ItemEvent(AddItem);
            thisGroup.RemovedItem += new ItemEvent(RemoveItem);

        }



        public void AddItem(ItemController itm)
        {
            foreach (InventaryItemViewer viewer in itemViewers)
            {
                if(viewer.invItem == null)
                {
                    viewer.SetItem(itm);
                    break;
                }
            }
        }          

        public void RemoveItem(ItemController itm)
        {
            for (int i = 0; i < itemViewers.Count; i++)
            {
                if (itemViewers[i].invItem == itm)
                {
                    itemViewers[i].SetItem(null);
                    itemViewers[i].mtransform.SetSiblingIndex(itemViewers.Count);
                    itemViewers.Add(itemViewers[i]);
                    itemViewers.RemoveAt(i);
                    break;
                }
            }
        }


        public void ShowItems(Collider[] items)
        {
            for (int i = 0; i < itemViewers.Count; i++)
            {
                if (i < items.Length)
                {
                    ItemController it = items[i].GetComponent<ItemController>();
                    itemViewers[i].SetItem(it);
                }
                else
                {
                    itemViewers[i].SetItem(null);
                }
            }
        }
    }
}
