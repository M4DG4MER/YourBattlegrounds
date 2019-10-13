using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tutoriales.Sounds
{
    public class FootstepSounds : MonoBehaviour
    {
        CharacterController collider;
        AudioSource source;

        public TextureMaterialCollection TextureMaterialCollection;
        public SoundEvent Event;

        private void Awake()
        {
            collider = GetComponentInParent<CharacterController>();
            source = GetComponent<AudioSource>();
        }



        public void FootStep()
        {
            Texture t = GetOnTexture();

            TextureEvent _event = new TextureEvent() { EventType = Event, _Texture = t };
            TextureMaterialCollection.Play(_event, source, float.MaxValue);

            //Debug.Log(t?.name ?? "NULL");
            
        }


        private Texture GetOnTexture()
        {
            RaycastHit hit;
            bool floor = Physics.Raycast(transform.position + Vector3.up * .01f, -Vector3.up, out hit, .2f, ~this.gameObject.layer);

            if (floor && hit.collider != collider)
            {
                if (hit.collider is TerrainCollider)
                {
                    //DO MAP
                    Terrain t = hit.collider.GetComponent<Terrain>();
                    return t.GetTextureAt(hit.point);
                }
                else{

                    MeshRenderer r = hit.collider.GetComponent<MeshRenderer>();

                    if (hit.collider is MeshCollider)
                    {
                        return r.GetMaterialAt(hit.triangleIndex, true)?.mainTexture;
                    }
                    else
                    {
                        return r.material.mainTexture;
                    }
                }
            }
            return null;
        }

    }
}


public static class UnityClassesExtension
{

    public static Material GetMaterialAt(this MeshRenderer r, int i, bool draw = false)
    {
        Transform tr = r.transform;
        MeshFilter f = r.GetComponent<MeshFilter>();
        Mesh m = f.sharedMesh;
        int[] t = m.triangles;
        Vector3[] v = m.vertices;

        var v0 = t[i * 3 + 0];
        var v1 = t[i * 3 + 1];
        var v2 = t[i * 3 + 2];

        if (draw)
        {
            Vector3 p0 = tr.TransformPoint(v[v0]);
            Vector3 p1 = tr.TransformPoint(v[v1]);
            Vector3 p2 = tr.TransformPoint(v[v2]);

            Debug.DrawLine(p0, p1);
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p0);
        }

        for (int x = 0; x < r.materials.Length; x++)
        {
            var mts = m.GetTriangles(x);

            for (int d = 0; d < mts.Length; d += 3)
            {
                if (mts[d] == v0 && mts[d + 1] == v1 && mts[d + 2] == v2)
                {
                    return r.materials[x];
                }
            }
        }

        return null;
    }


    public static Texture GetTextureAt(this Terrain t, Vector3 pos)
    {
        TerrainData td = t.terrainData;
        var alphamapWidth = td.alphamapWidth;
        var alphamapHeight = td.alphamapHeight;
        var mSplatmapData = td.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        var mNumTextures = mSplatmapData.Length / (alphamapWidth * alphamapHeight);

        Vector3 vecRet = new Vector3();
        Vector3 terPosition = t.transform.position;
        vecRet.x = ((pos.x - terPosition.x) / td.size.x) * alphamapWidth;
        vecRet.z = ((pos.z - terPosition.z) / td.size.z) * alphamapHeight;

        int ret = 0;
        float comp = 0f;
        for (int i = 0; i < mNumTextures; i++)
        {
            float textureValue = mSplatmapData[(int)vecRet.z, (int)vecRet.x, i];
            if (comp < textureValue)
            {
                ret = i;
                comp = textureValue;
            }
        }
        
        return td.terrainLayers[ret]?.diffuseTexture;
    }


}

