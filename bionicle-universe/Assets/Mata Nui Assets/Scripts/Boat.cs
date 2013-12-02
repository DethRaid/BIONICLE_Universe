using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[AddComponentMenu("Physics/Bouyancy")]
public class Boat : MonoBehaviour 
{
    public float bouyancyFactor = 1.0f;
    public float pointsDist;
    public float waterHeight;
    public List<float> bouyancies;
    public List<Vector3> bouyancyCenters;
    public Mesh mesh;
    public Vector3 bouyancyCenter;
    public Vector3[] verts;     //transformed, clamped data
    public Vector3[] vertexes;  //untransformed, original data
    public int[] triangles;

    void Awake() {
        bouyancies = new List<float>();
        bouyancyCenters = new List<Vector3>();
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertexes = mesh.vertices;
        verts = new Vector3[vertexes.Length];
        triangles = mesh.triangles;
        Matrix4x4 curTransform;
        Matrix4x4 inverse;
        //for each height (divisions of 0.1 units)
        for( float f = 0; f < waterHeight; f += 0.1f ) {
            //for each of 72 x-rotations (divisions of 5 degrees)
            for( int xRot = 0; xRot < 360; xRot += 4 ) {
                //for each of 72 z-rotations (divisions of 5 degrees)
                for( int zRot = 0; zRot < 360; zRot += 5 ) {
                    curTransform = Matrix4x4.TRS( new Vector3( 0, f, 0 ), 
                        Quaternion.Euler( new Vector3( xRot, 0, zRot ) ), 
                        Vector3.one );
                    inverse = curTransform.inverse;
                    for( int i = 0; i < vertexes.Length; i++ ) {
                        verts[i] = curTransform.MultiplyPoint( vertexes[i] );
                        if( verts[i].y > waterHeight ) {
                            verts[i].y = waterHeight;
                        }
                    }
                    bouyancies.Add( VolumeOfMesh() * bouyancyFactor );
                    mesh.vertices = verts;
                    mesh.RecalculateBounds();
                    bouyancyCenters.Add( mesh.bounds.center );
                    //transform each vertex by the current matrix
                    //clamp the vertex's z-vaule to the water height
                    //determine the submerged volume
                    //volume * bouyancyFactor = bouyantForce
                    //determine center of submerged mesh (bounds)
                    //save this
                }
            }
        }
        mesh.vertices = vertexes;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        using( StreamWriter file = new StreamWriter( "BoatBouyancies.budf" ) ) {
            for( int i = 0; i < bouyancies.Count; i++ ) {
                file.WriteLine( "b" + bouyancies[i] + " c" + bouyancyCenters[i] );
            }
        }
    }

    public float SignedVolumeOfTriangle( Vector3 p1, Vector3 p2, Vector3 p3 ) {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    public float VolumeOfMesh() {
        float volume = 0;

        for( int i = 0; i < triangles.Length; i += 3 ) {
            Vector3 p1 = verts[triangles[i + 0]];
            Vector3 p2 = verts[triangles[i + 1]];
            Vector3 p3 = verts[triangles[i + 2]];
            volume += SignedVolumeOfTriangle( p1, p2, p3 );
        }

        return Mathf.Abs( volume );
    }
}

