using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales
{
    public class ClothItem : ItemController
    {

        public float armorLife;

        internal CharController usingChar;


        public ClothStats GetClothStats()
        {
            if (Stats is ClothStats)
                return Stats as ClothStats;

            ClothStats defect = new ClothStats();
            Stats = defect;
            return defect;
        }

        protected override void Initialize()
        {
            base.Initialize();
            ClothStats stats = GetClothStats();
            armorLife = stats.ArmorIncrement;
        }


        public override void Take(CharController character, Transform slotPosition, bool selected)
        {
            base.Take(character, slotPosition, selected);
            Use(character);
            base.Hide();
        }

        public override void Use(CharController character)
        {
            base.Use(character);
            ClothStats stats = GetClothStats();
            this.usingChar = character;
            character.inventary.ShowCloth(stats.ItemViewerName, this);
            character.Consume(armorLife, "armor");
        }

        public override void Drop(CharController character, Transform itemTransform, bool selected)
        {
            base.Drop(character, itemTransform, selected);
            ClothStats stats = GetClothStats();
            this.usingChar = character;
            character.inventary.HideCloth(stats.ItemViewerName, this);
            character.Consume(-armorLife, "armor");
        }


        public void TakeDamage(float lastDamage)
        {
            viewer.RPC("TakeDamage_RPC", Photon.Pun.RpcTarget.AllBuffered, lastDamage);
        }

        [Photon.Pun.PunRPC]
        public void TakeDamage_RPC(float lastDamage)
        {
            this.armorLife -= lastDamage;
            if (this.armorLife < 0)
                armorLife = 0;
        }




    }
}
