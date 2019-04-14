using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class InventaryViewer : MonoBehaviour
    {
        public List<InventaryGroupViewer> viewers = new List<InventaryGroupViewer>();
        Dictionary<string, InventaryGroupViewer> mappedViewers = new Dictionary<string, InventaryGroupViewer>();

        public InventaryGroupViewer SceneViewer;
        public int MaxSceneItems;
        public SceneScrollView Scroll;

        public InventaryController inventary;

        internal void Initialize(InventaryController inventaryController)
        {
            inventary = inventaryController;
            foreach (InventaryGroupViewer v in viewers)
            {
                if (!mappedViewers.ContainsKey(v.SlotType))
                {
                    mappedViewers.Add(v.SlotType, v);
                    InventaryGroup group = inventaryController.GetGroup(v.SlotType);
                    if (group != null)
                    {
                        v.Initialize(this, group);
                    }
                }
            }

            SceneViewer.Initialize(this, new InventaryGroup() { MaxCapacity = MaxSceneItems });
            Scroll.Initialize();
        }
    }
}
