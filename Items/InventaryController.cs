using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class InventaryController : MonoBehaviour
    {
        public ItemViewer ItemViewer;
        public InventaryViewer inventaryViewer;

        public List<InventaryGroup> inventaryGroups;

        private Dictionary<string, InventaryGroup> mappedInventary = new Dictionary<string, InventaryGroup>();

        public ClothViewer[] clothViewers;
        private Dictionary<string, ClothViewer> mappedCloths = new Dictionary<string, ClothViewer>();
        
        public CharController character;

        public void Initiliaze(bool player, CharController charplayer)
        {
            character = charplayer;


            MapInventaryGroups();
            MapClothViewers();
            if (player)
            {
                ItemViewer = FindObjectOfType<ItemViewer>();
                inventaryViewer = FindObjectOfType<InventaryViewer>();
                inventaryViewer.Initialize(this);
            }
            else
            {
                ItemViewer = null;
            }
        }


        private void MapInventaryGroups()
        {
            mappedInventary.Clear();
            foreach (InventaryGroup g in inventaryGroups)
            {
                if (!mappedInventary.ContainsKey(g.SlotType))
                {
                    mappedInventary.Add(g.SlotType, g);
                }
            }
        }
        private void MapClothViewers()
        {
            clothViewers = GetComponentsInChildren<ClothViewer>();
            foreach (ClothViewer c in clothViewers)
            {
                if (!mappedCloths.ContainsKey(c.ClothName))
                {
                    mappedCloths.Add(c.ClothName, c);
                }
            }
        }
        public void ShowCloth(string name, ClothItem item)
        {
            if (mappedCloths.ContainsKey(name))
                mappedCloths[name].Show(item);
        }
        public void HideCloth(string name, ClothItem item)
        {
            if (mappedCloths.ContainsKey(name))
                mappedCloths[name].Hide(item);
        }






        public bool AddItem(CharController character, ItemController item)
        {
            if (mappedInventary.ContainsKey(item.Stats.SlotType))
            {
                bool result = mappedInventary[item.Stats.SlotType].AddItem(character, item);
                return result;
            }
            return false;
        }


        public ItemController GetSelectedAt(string group)
        {
            InventaryGroup invgroup = GetGroup(group);
            if (invgroup == null)
                return null;
            return invgroup.GetSelected();
        }


        public InventaryGroup GetGroup(string group)
        {
            if (!mappedInventary.ContainsKey(group))
                return null;
            return mappedInventary[group];
        }


        public bool RemoveItem(ItemController item)
        {
            if (mappedInventary.ContainsKey(item.Stats.SlotType))
            {
                return mappedInventary[item.Stats.SlotType].RemoveItem(item);
            }
            return false;
        }
    }

    [System.Serializable]
    public class InventaryGroup
    {
        public string SlotType;
        public Transform RealPosition;
        public int MaxCapacity = 3;
        public bool ReplaceSelectedOnMax = true;


        public ItemEvent AddedItem;
        public ItemEvent RemovedItem;

        private int selIndex;
        public int SelectedIndex
        {
            set { selIndex = value; }
            get { return selIndex; }
        }

        public List<ItemController> items;
        public ItemController GetSelected()
        {
            return GetItemAt(selIndex);
        }

        public bool AddItem(CharController character, ItemController item)
        {
            if (Stack(item))
            {
                return true;
            }


            if (items.Count >= MaxCapacity)
            {
                if (ReplaceSelectedOnMax)
                {
                    DropItem(character, selIndex, item.mTransform);
                    items[selIndex] = item;
                    item.Take(character, RealPosition, true);

                    if (AddedItem != null)
                        AddedItem(item);

                    return true;
 
                }
                return false;
            }


            items.Add(item);
            item.Take(character, RealPosition, items.Count == 1);

            if (AddedItem != null)
                AddedItem(item);

            return true;
        }




        public bool Stack(ItemController item)
        {
            if (item is ConsumableItem)
            {
                ConsumableItem consumable = (item as ConsumableItem);

                ItemController itContrl = GetItemByName(consumable.Stats.ItemName);

                if (itContrl != null && itContrl is ConsumableItem)
                {
                    ConsumableItem alreadyInInventary = (itContrl as ConsumableItem);

                    int max = alreadyInInventary.getConsumableStats().MaxUnits;

                    if (max == alreadyInInventary.Units)
                    {
                        return true;
                    }

                    int totalUnits = alreadyInInventary.Units + consumable.Units;
                    int difference = (totalUnits - max);

                    alreadyInInventary.Units = (difference > 0 ? max : totalUnits);
                    consumable.Units = (difference > 0 ? difference : 0);

                    if (consumable.Units <= 0)
                    {
                        ConsumableItem.Destroy(consumable.gameObject);
                        return true;
                    }
                }
            }
            return false;
        }
        


        public bool DropItem(CharController character, int index, Transform dropTransform)
        {
            ItemController ic = GetItemAt(index);
            if (ic != null)
            {
                ic.Drop(character, dropTransform, index == selIndex);

                if (RemovedItem != null)
                    RemovedItem(ic);

                return true;
            }
            return false;
        }

        public bool DropItem(ItemController item)
        {
            int i = GetIndex(item);
            return this.DropItem(item.owner, i, item.mTransform);
        }




        public ItemController GetItemAt(int index)
        {
            if (items.Count == 0)
                return null;
            if (index < 0)
                return null;
            if (index < items.Count)
                return items[index];
            return null;
        }

        public int GetIndex(ItemController itm)
        {
            return items.IndexOf(itm);
        }

        public void Select(ItemController itm)
        {
            int indx = GetIndex(itm);
            Select(indx);
        }

        public void Select(int indx)
        {
            if (indx != SelectedIndex)
            {
                ItemController item = GetSelected();
                if (item != null)
                {
                    item.Hide();
                }
                SelectedIndex = indx;

                item = GetSelected();
                if (item != null)
                {
                    item.Show();
                }
            }

        }


        public bool RemoveItem(ItemController item)
        {
            if (items.Remove(item))
            {
                if (RemovedItem != null)
                    RemovedItem(item);
                return true;
            }
            return false;
        }

        

        public ItemController GetItemByName(string name)
        {
            foreach(ItemController it in items)
            {
                if (it.Stats.ItemName == name)
                {
                    return it;
                }
            }
            return null;
        }



    }


    public delegate void ItemEvent(ItemController item);
    

}
