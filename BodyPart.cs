using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class BodyPart : MonoBehaviour
    {
        [HideInInspector]
        public CharController character;

        public string BodyName;
        public float Multiplier;
        public float LastDamage;
        public bool debug;

        public ClothItem ArmorItem;


        public void TakeHit(float damage)
        {
            LastDamage = damage * Multiplier;
            this.character.TakeDamage(LastDamage);
            if (ArmorItem != null)
            {
                ArmorItem.TakeDamage(LastDamage);
            }
            
            Debug.Log(damage + " * " + Multiplier + " = " + LastDamage);
        }

//comentario de prueba

    }
}
