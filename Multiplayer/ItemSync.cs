using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales.multiplayer
{
    public class ItemSync : MonoBehaviour, IPunObservable
    {
        ItemController item;

        PhotonView photonView;


        private void OnEnable()
        {
            photonView = GetComponent<PhotonView>();
            photonView.OwnershipTransfer = OwnershipOption.Request;
            photonView.ObservedComponents.Add(this);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!item.MustBeSync())
                return;

            if (stream.IsWriting)
            {
                Vector3 p = item.mTransform.position;
                Vector3 r = item.mTransform.rotation.eulerAngles;
                stream.SendNext(p);
                stream.SendNext(r);
            }
            else
            {
                Vector3 p = (Vector3)stream.ReceiveNext();
                Vector3 r = (Vector3)stream.ReceiveNext();
                item.mTransform.position = p;
                item.mTransform.rotation = Quaternion.Euler(r);

            }

        }
    }
}
