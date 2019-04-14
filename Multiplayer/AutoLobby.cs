using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tutoriales.multiplayer
{
    public class AutoLobby : MonoBehaviourPunCallbacks
    {
        public Button ConnectButton;
        public Button JoinRandomButton;
        public Text Log;
        public Text PlayerCount;
        public int playersCount;

        public byte maxPlayersPerRoom = 4;
        public byte minPlayersPerRoom = 2;
        private bool IsLoading = false;



        public void Connect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.ConnectUsingSettings())
                {
                    Log.text += "\nConnected to Server";
                }
                else
                {
                    Log.text += "\nFailing Connecting to Server";
                }
            }
        }

        public override void OnConnectedToMaster()
        {
            ConnectButton.interactable = false;
            JoinRandomButton.interactable = true;
        }


        public void JoinRandom()
        {
            if (!PhotonNetwork.JoinRandomRoom())
            {
                Log.text += "\nFail Joining room";
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Log.text += "\nNo Rooms to Join, creating one...";

            if (PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions() { MaxPlayers = maxPlayersPerRoom }))
            {
                Log.text += "\nRoom Created";
            }
            else
            {
                Log.text += "\nFail Creating Room";
            }
        }

        public override void OnJoinedRoom()
        {
            Log.text += "\nJoinned";
            JoinRandomButton.interactable = false;
        }


        private void FixedUpdate()
        {
            if (PhotonNetwork.CurrentRoom != null)
                playersCount = PhotonNetwork.CurrentRoom.PlayerCount;

            PlayerCount.text = playersCount + "/" + maxPlayersPerRoom;

            if (!IsLoading && playersCount >= minPlayersPerRoom)
            {
                LoadMap();
            }
            
        }


        private void LoadMap()
        {
            IsLoading = true;
            
            PhotonNetwork.LoadLevel("isla_demo_4");
        }


    }
}