using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class ItemSpawner : MonoBehaviour
    {
        public bool Drop = false;

        public List<ItemController> itemsPrefabs;

        public int MinQuantity;
        public int MaxQuantity;

        public float Diameter;


        private void OnEnable()
        {
            if (!Drop)
                ThrowItems();
        }

        public void ThrowItems()
        {
            if (!Photon.Pun.PhotonNetwork.IsMasterClient && Photon.Pun.PhotonNetwork.CurrentRoom != null)
                return;

            Transform tr = this.transform;
            Transform parent = tr.parent;

            int numItems = Random.Range(MinQuantity, MaxQuantity);

            for (int i = 0; i < numItems; i++)
            {
                int r = Random.Range(0, itemsPrefabs.Count);

                ItemController randomItemPrefab = itemsPrefabs[r];

                Vector3 randomVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                float randomDiameter = Random.Range(0f, Diameter);
                randomVector *= randomDiameter;
                randomVector += tr.position;
                
                ItemController item = Instantiate<ItemController>(randomItemPrefab,  parent);
                Vector3 euler = item.mTransform.eulerAngles;
                Quaternion rotation = Quaternion.Euler(new Vector3(euler.x, Random.Range(0, 360), euler.z));

                Photon.Pun.PhotonNetwork.Instantiate("items/" + randomItemPrefab.name, randomVector, rotation);
            }

        }

    }
}
