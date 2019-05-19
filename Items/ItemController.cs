using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales
{
    public class ItemController : MonoBehaviourPun
    {
        protected PhotonView viewer;
        public ItemStats Stats;
        internal Transform mTransform;
        protected Rigidbody rg;

        protected Collider collider;
        protected MeshRenderer[] meshes;

        internal CharController owner;

        private void OnEnable()
        {
            mTransform = this.transform;
            rg = GetComponent<Rigidbody>();
            collider = GetComponent<SphereCollider>();
            meshes = GetComponentsInChildren<MeshRenderer>();
            viewer = GetComponent<PhotonView>();
            Initialize();
        }

        public virtual void Show()
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.enabled = true;
            }
        }
        public virtual void Hide()
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.enabled = false;
            }
        }

        protected virtual void Initialize()
        {

        }

        public virtual void Use(CharController character)
        {
            if (Stats.ShowOnUse)
            {
                this.Show();
            }
        }


        public virtual void Take(CharController character, Transform slotPosition, bool selected)
        {
            if (slotPosition != null)
            {
                mTransform.parent = slotPosition;
            }
            mTransform.localPosition = Vector3.zero;
            mTransform.localRotation = Quaternion.identity;
            rg.detectCollisions = false;
            rg.isKinematic = true;
            collider.enabled = false;
            owner = character;

            this.Hide();
            if (!Stats.HideOnTake && selected == true)
            {
                this.Show();
            }
        }

        public virtual void Drop(CharController character, Transform itemTransform, bool selected)
        {
            mTransform.parent = null;
            rg.detectCollisions = true;
            rg.isKinematic = false;
            collider.enabled = true;

            mTransform.position = itemTransform.position;
            
            Vector3 posRot = mTransform.rotation.eulerAngles;

            Quaternion finalRot = Quaternion.Euler(
                (Stats.baseRotation.x != 0 ? Stats.baseRotation.x : posRot.x),
                (Stats.baseRotation.y != 0 ? Stats.baseRotation.y : posRot.y),
                (Stats.baseRotation.z != 0 ? Stats.baseRotation.z : posRot.z)
                );

            mTransform.rotation = finalRot;
            character = null;

            if (Stats.HideOnTake || selected == false)
            {
                this.Show();
            }
        }

        public virtual bool MustBeSync()
        {
            return (meshes.Length > 0 && meshes[0].enabled);
        }

    }
}
