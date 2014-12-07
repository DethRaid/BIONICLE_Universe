using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Jotun Navmesh/Level Scanner")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class JNavLevelScanner : MonoBehaviour
{
    public float maxAngle, progress;
    public int curError = 0, xSectors, ySectors;
    public List<Mesh> staticMeshes = new List<Mesh>();
    public List<Vector3> worldPositions = new List<Vector3>();
    public List<Vector3> scales = new List<Vector3>();
    public List<Vector3> floorVerts = new List<Vector3>();
    public List<int> floorFaces = new List<int>();

    public List<Vector3> navVerts = new List<Vector3>();
    public List<int> navTris = new List<int>();

    public List<Vector3> edgeVerts = new List<Vector3>();
    public List<JNavEdge> nonWalkableEdges = new List<JNavEdge>();
    public List<Vector3> intersections = new List<Vector3>();

    public GameObject meshBase;
    public Mesh navMesh;
    public Sector[][] sectors;

    public Vector3 toFind;
    public Vector3[] curMeshVerts;
    public int[] curMeshTris;

    public int scanLevel()
    {
        gatherMeshes();
        if (curError == 0)
            findWalkableFaces();
            if (curError == 0)
                assignSectors();
        return curError;
    }

    /*
     * Saves a reference to all meshes associated with a static GameObject to the List staticMeshes
     * This functions sets curError to 0 if at least one mesh is added to staticMeshes. Otherwise, the function
     * sets curError to -1
     */
    public void gatherMeshes()
    {
        int value = -1;
        MeshFilter[] mrs = GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
        for (int i = 0; i < mrs.Length; i++)
            if (mrs[i].gameObject.isStatic)
            {
                value = 0;
                staticMeshes.Add(mrs[i].sharedMesh);
                scales.Add(mrs[i].gameObject.transform.lossyScale);
                worldPositions.Add(mrs[i].gameObject.transform.position);
            }
        curError = value;
    }

    /*
     * Determines which faces have a normal of greater then 45 degrees from the positive y-axis
     * If no faces of ths sort are found, curError is set to -2
     * Otherwise, curError is set to 0
     */
    public void findWalkableFaces()
    {
        Mesh curMesh;
        Vector3 s, t, n;   
        Vector3[] verts;
        float thetaX, thetaZ;
        int[] tris;
        int value = -2;
        for (int i = 0; i < staticMeshes.Count; i++)
        {
            //create a cache of the mesh's vertexes and triangles to save CPU cycles down the road
            curMesh = staticMeshes[i];
            tris = curMesh.triangles;
            verts = curMesh.vertices;
            for (int j = 0; j < curMesh.triangles.Length-2; j += 3)
            {
                s = verts[tris[j]] - verts[tris[j + 1]];
                t = verts[tris[j]] - verts[tris[j + 2]];
                n = Vector3.Cross(s, t);    //determine the normal vector of the plane

                thetaX = Mathf.Atan(n.y / n.x);
                thetaZ = Mathf.Atan(n.y / n.z);
                //if the face normal is less than 45 degrees from the vertical, we should be add it to the array of walkable faces
                if (thetaX < maxAngle && thetaX > -maxAngle &&
                    thetaZ < maxAngle && thetaZ > -maxAngle)      
                {
                    value = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        toFind = verts[tris[j + k]] + Vector3.up;
                        int pos = floorVerts.FindIndex(matchToFind);
                        if (pos == -1)
                        {
                            //scale the vertex by the mesh's scale factor and translate it by the mesh's world position to make sure that all vertexes are in world coordinates
                            toFind.Scale(scales[i]);
                            floorVerts.Add(toFind + worldPositions[i]);
                            floorFaces.Add(floorVerts.Count - 1);
                            print("The vertex array is now " + floorVerts.Count + " vertexes long");
                            print("The latest vertex is number " + floorFaces[floorFaces.Count - 1]);
                        }
                        else
                            floorFaces.Add(pos);
                    }
                }

                //else, the face is unwalkable and should be considered an obstacle
                //We're going to add its edges to an array of edges
                //We know that all the edges in that array aren't connected to a walkable face
                //Later, we'll check if any of those edges intersect a walkable face to determine where scene geometry intersects an obstacle
                else
                {
                    nonWalkableEdges.Add(new JNavEdge(verts[tris[j]], verts[tris[j + 1]], n));
                    nonWalkableEdges.Add(new JNavEdge(verts[tris[j]], verts[tris[j + 2]], n));
                    nonWalkableEdges.Add(new JNavEdge(verts[tris[j+2]], verts[tris[j + 1]], n));
                }
            }
        }
        //calculateIntersects();
        //splitNavmesh();
        for( int i = 0; i < navTris.Count; i++ )
        print(navTris[i]);
        print("There are " + navVerts.Count + " vertexes in our navMesh");
        navMesh.vertices = floorVerts.ToArray();
        navMesh.triangles = floorFaces.ToArray();
        navMesh.RecalculateBounds();
        navMesh.RecalculateNormals();
        curError = value;
    }

    public void calculateIntersects()
    {
        for (int i = 0; i < floorFaces.Count; i += 3)
            for (int j = 0; j < nonWalkableEdges.Count; i++)
                if (Vector3.Distance(nonWalkableEdges[j].start, floorVerts[floorFaces[i]]) < 20)    //assumes that no face has an edge of more then 20 units
                    if (nonWalkableEdges[j].intersects(floorVerts[floorFaces[i]],
                                                       floorVerts[floorFaces[i + 1]],
                                                       floorVerts[floorFaces[i + 2]]))
                        intersections.Add(nonWalkableEdges[j].intersectPoint);
    }

    /*
     * Splits the navmesh into smaller meshes with no more than 65000 vertexes, Unity's limit
     */
    public void splitNavmesh()
    {
        Vector3[] verts = floorVerts.ToArray();
        for (int i = 0; i < floorVerts.Count / 65000; i++)
        {
            GameObject go = Instantiate(meshBase, Vector3.zero, Quaternion.identity) as GameObject;
            Mesh m = go.GetComponent<MeshFilter>().sharedMesh;
            m.vertices = arraySlice(verts, i * 65000, (i + 1) * 65000);
        }
    }

    public Vector3[] arraySlice(Vector3[] ar, int s, int e)
    {
        Vector3[] newAr = new Vector3[e - s];
        for (int i = s; i < e; i++)
            newAr[i - s] = ar[i];
        return newAr;
    }

    /*
     * Divides the game world up into sectors, then determines which Navmesh faces lie within each sector
     */
    public void assignSectors()
    { 

    }

    public bool matchToFind(Vector3 v3)
    {
        return (v3 == toFind);
    }
}

public class Sector
{
    public List<Vector3> walkfaces = new List<Vector3>();
}

[System.Serializable]
public class JNavEdge
{
    float minX, maxX, minZ, maxZ, lineX, lineZ, avgY;
    public Vector3 start, end, normal, slope;

    public Vector3 intersectPoint
    {
        get
        {
            return new Vector3(lineX, avgY, lineX);
        }
    }

    public JNavEdge(Vector3 s, Vector3 e, Vector3 n)
    {
        start = s;
        end = e;
        normal = n;
    }

    public bool intersects( Vector3 vert1, Vector3 vert2, Vector3 vert3 )
    {
        //first, figure out the bounds of the triangle
        maxX = Mathf.Max(vert1.x, vert2.x);
        maxX = Mathf.Max(maxX, vert3.x);
        minX = Mathf.Min(vert1.x, vert2.x);
        minX = Mathf.Min(minX, vert3.x);
        maxZ = Mathf.Max(vert1.z, vert2.z);
        maxZ = Mathf.Max(maxZ, vert3.z);
        minZ = Mathf.Min(vert1.z, vert2.z);
        minZ = Mathf.Min(minZ, vert3.z);
        //Next find the average y of the polygon
        avgY = (vert1.y + vert2.y + vert3.y) / 3;
        //find the slope of this edge
        slope = (start - end);
        //Now set the Z value of the line to 0 and solve for x
        lineX = (slope.y * avgY) / slope.x;
        //Then, set the X value of the line to 0 and solve for z
        lineZ = (slope.y * avgY) / slope.z;
        //finally, determine if the points we just got lie within the bounds of the polygon we were given
        return ((lineX < maxX) && (lineX > minX) && (lineZ < maxZ) && (lineZ > minZ));
    }

    public override bool Equals(object obj)
    {
        JNavEdge rhs = (JNavEdge)obj;
        return ((rhs.start == start && rhs.end == end) || (rhs.start == end && rhs.end == start));
    }
}