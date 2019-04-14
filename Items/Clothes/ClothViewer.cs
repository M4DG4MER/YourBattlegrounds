using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class ClothViewer : MonoBehaviour
    {
        BodyMaskManager[] maskManagers;
        RagdollController ragdoll;

        SkinnedMeshRenderer mesh;

        List<BodyPart> partsToProtect;

        public string ClothName;
        public bool HideMask;
        public List<string> BodyMasks;

        private void OnEnable()
        {
            mesh = GetComponent<SkinnedMeshRenderer>();
            maskManagers = this.transform.parent.GetComponentsInChildren<BodyMaskManager>();
            ragdoll = this.transform.parent.GetComponent<RagdollController>();
        }


        public void Show(ClothItem item)
        {
            mesh.enabled = true;

            if (HideMask)
            {
                foreach(string bmask  in BodyMasks)
                {
                    foreach (BodyMaskManager manager in maskManagers)
                        if (manager.Hide(bmask))
                            break;
                }
            }

            ClothStats sts = item.GetClothStats();
            partsToProtect = ragdoll.getBodyParts(sts.ProtectBodyParts);

            foreach (BodyPart bp in partsToProtect)
            {
                bp.ArmorItem = item;
            }


        }

        public void Hide(ClothItem item)
        {
            mesh.enabled = false;

            if (HideMask)
            {
                foreach (string bmask in BodyMasks)
                {
                    foreach (BodyMaskManager manager in maskManagers)
                        if (manager.Show(bmask))
                            break;
                }
            }

            ClothStats sts = item.GetClothStats();
            partsToProtect = ragdoll.getBodyParts(sts.ProtectBodyParts);

            foreach (BodyPart bp in partsToProtect)
            {
                bp.ArmorItem = null;
            }
        }



    }
}
