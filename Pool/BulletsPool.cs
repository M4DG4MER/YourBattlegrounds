using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class BulletsPool : MonoBehaviour
    {
        static public BulletsPool Pool { get; private set; }




        public BulletController BulletPrefab;

        public int MaxBullets = 1000;
        public List<BulletController> Bullets;
        private int index;

        private void Awake()
        {
            Pool = this;

            index = 0;
            for (int i = 0; i < MaxBullets; i++)
            {
                BulletController bullet = Instantiate<BulletController>(BulletPrefab);
                Bullets.Add(bullet);
                bullet.gameObject.SetActive(false);
            }
        }



        public BulletController GetBullet()
        {
            Debug.Log("Giving bullet: " + index);
            BulletController bullet = Bullets[index];
            index++;
            if (index >= Bullets.Count)
                index = 0;
            return bullet;
        }






    }
}
