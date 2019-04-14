using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class HolesPool : MonoBehaviour
    {
        static public HolesPool Pool { get; private set; }

        internal Transform mtransform;

        public List<MaterialType> Materials = new List<MaterialType>();


        // Use this for initialization
        void Start()
        {
            Pool = this;

            mtransform = this.transform;

            foreach (MaterialType m in Materials)
            {
                m.Initiate(mtransform);
            }
            
        }


        public void Impact(string materialName, Vector3 position, Vector3 normal)
        {
            foreach (MaterialType m in Materials)
            {
                if (m.MaterialName == materialName)
                {
                    m.Impact(position, normal);
                    break;
                }
            }
        }

    }


    [System.Serializable]
    public class MaterialType
    {
        public string MaterialName;
        public MaterialHole HolePrefab;
        private List<MaterialHole> holes = new List<MaterialHole>();

        public int MaxHoles = 10;
        private int index = 0;

        public void Initiate(Transform parent)
        {
            for (int i = 0; i < MaxHoles; i++)
            {
                holes.Add(HolesPool.Instantiate<MaterialHole>(HolePrefab, parent));
            }
        }

        public void Impact(Vector3 pos, Vector3 normal)
        {
            if (index >= MaxHoles)
                index = 0;

            holes[index].Reubicate(pos, normal);
            index++;


        }


    }


}
