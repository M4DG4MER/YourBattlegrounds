using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace tutoriales
{
    public class camerazone : MonoBehaviour
    {



        public PostProcessingProfile inzone;
        public PostProcessingProfile outofzone;


        public PostProcessingBehaviour behave;

        private void OnEnable()
        {
            behave = GetComponent<PostProcessingBehaviour>();
        }



        private void OnTriggerEnter(Collider other)
        {
            Area a = Area.AREA();
            if (a != null && other.gameObject == a.Cylinder.gameObject)
            {
                behave.profile = inzone;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            Area a = Area.AREA();
            if (a != null && other.gameObject == a.Cylinder.gameObject)
            {
                behave.profile = outofzone;
            }
        }


    }
}
