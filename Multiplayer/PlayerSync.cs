using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace tutoriales.multiplayer
{
    public class PlayerSync : MonoBehaviour, IPunObservable
    {
        PhotonView view;
        CharController CharController;

        private void Awake()
        {
            CharController = GetComponent<CharController>();
            view = GetComponent<PhotonView>();
        }




        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CharController.rotY);
                stream.SendNext(CharController.tr.rotation.eulerAngles.y);
            }
            else if (stream.IsReading)
            {
                CharController.rotY = (float)stream.ReceiveNext();
                float y = (float)stream.ReceiveNext();
                CharController.tr.rotation = Quaternion.Euler(0,y,0);
            }


        }

    }
}
