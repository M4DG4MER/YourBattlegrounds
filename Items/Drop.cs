using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class Drop : MonoBehaviour
    {
        public ItemSpawner SpawnerPrefab;
        public float Vspeed = 1f;
        Rigidbody rg;


        private void OnEnable()
        {
            rg = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Area a = Area.AREA();
            if (a != null && other.gameObject != a.Cylinder.gameObject)
            {

                ItemSpawner spawner = Instantiate<ItemSpawner>(SpawnerPrefab);
                spawner.transform.position = this.transform.position;
                spawner.ThrowItems();
                Destroy(this.gameObject);
            }
        }


        private void FixedUpdate()
        {
            rg.velocity = Vector3.up * -Vspeed;
        }

    }
}
