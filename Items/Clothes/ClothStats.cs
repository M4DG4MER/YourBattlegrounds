using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    [CreateAssetMenu(fileName ="clothStats", menuName = "tutoriales/cloth stats")]
    public class ClothStats : ItemStats
    {

        public string ItemViewerName;

        public float ArmorIncrement;
        public string[] ProtectBodyParts;


    }
}
