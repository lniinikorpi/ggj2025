using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class SoftBodySphere : MonoBehaviour
{
    [SerializeField]
    private Transform m_parent;
    public GameObject objectToSpawn; // The prefab to spawn
    public int numObjects = 100;    // Total number of objects to spawn
    public float sphereRadius = 5f; // Radius of the sphere
    public Material meshMaterial;

    GameObject[] spawnedObjects;
    private Mesh mesh;
    private bool isSpawned = false;

    void Start()
    {
        mesh = new Mesh();
        //SpawnObjectsInSphere();
        //StartCoroutine(WaitForShit());
    }

    IEnumerator WaitForShit()
    {
        yield return new WaitForSeconds(5);
        SpawnObjectsInSphere();
    }

    void SpawnObjectsInSphere()
    {    // Calculate the number of latitude and longitude bands
        int numLatitudeBands = Mathf.RoundToInt(Mathf.Sqrt(numObjects));  // Adjust based on desired resolution
        int numLongitudeBands = Mathf.RoundToInt(numObjects / (float)numLatitudeBands);

        // Create an array to hold the positions of the spawned objects
        spawnedObjects = new GameObject[numLatitudeBands * numLongitudeBands];

        // Use the golden ratio to distribute objects in a spherical pattern
        float phiIncrement = Mathf.PI * (3 - Mathf.Sqrt(5));  // Golden angle in radians

        for (int lat = 0; lat < numLatitudeBands; lat++)
        {
            for (int lon = 0; lon < numLongitudeBands; lon++)
            {
                // Calculate the normalized position for the latitude and longitude
                float y = 1 - (lat / (float)(numLatitudeBands - 1)) * 2;  // Linear spacing between -1 and 1 for latitude
                float radius = Mathf.Sqrt(1 - y * y);                      // Radius of the circle at this latitude

                // Calculate theta (longitude angle) based on the current index
                float theta = lon * Mathf.PI * 2 / numLongitudeBands;

                // Calculate Cartesian coordinates for the point
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);

                // Convert the coordinates to world space
                Vector3 position = transform.position + new Vector3(x, y, z) * sphereRadius;

                // Store the object in the correct position in the spawnedObjects array
                int index = lat * numLongitudeBands + lon;
                Debug.Log("Max: " + spawnedObjects.Length + " index: " + index);
                spawnedObjects[index] = Instantiate(objectToSpawn, position, Quaternion.identity);
            }
        }
        foreach (GameObject obj in spawnedObjects)
        {
            obj.AddComponent<Rigidbody>();
        }
        foreach (GameObject obj in spawnedObjects)
        {
            foreach (GameObject obj2 in spawnedObjects)
            {
                if (obj == obj2)
                {
                    continue;
                }
                SpringJoint joint = obj.AddComponent<SpringJoint>();
                joint.connectedBody = obj2.GetComponent<Rigidbody>();
            }
        }
        SetupMeshRenderer();
    }
    void UpdateMeshFromObjects()
    {
        // Collect positions of all spawned objects
        Vector3[] vertices = new Vector3[spawnedObjects.Length];
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            vertices[i] = spawnedObjects[i].transform.position;
        }

        // Generate triangles using a simple method
        int[] triangles = GenerateTriangles(vertices);

        // Update the mesh with vertices and triangles
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();  // Ensure correct lighting
        mesh.RecalculateBounds();

        // Assign the mesh to the MeshFilter
        GetComponent<MeshFilter>().mesh = mesh;
    }
    int[] GenerateTriangles(Vector3[] vertices)
    {
        if (vertices.Length < 3) return new int[0];

        // Calculate approximate latitude and longitude counts
        int numLatitudeBands = Mathf.RoundToInt(Mathf.Sqrt(vertices.Length)); // Number of vertical layers
        int numLongitudeBands = vertices.Length / numLatitudeBands;          // Number of points per horizontal layer

        List<int> triangles = new List<int>();

        for (int lat = 0; lat < numLatitudeBands - 1; lat++) // Stop before the last latitude band
        {
            for (int lon = 0; lon < numLongitudeBands; lon++)
            {
                int current = lat * numLongitudeBands + lon;         // Current vertex
                int nextLon = (lon + 1) % numLongitudeBands;         // Next vertex in the same latitude band
                int nextRow = (lat + 1) * numLongitudeBands + lon;   // Vertex directly below
                int nextRowNextLon = nextRow + nextLon - lon;        // Diagonal vertex below

                if (nextRow < vertices.Length) // Ensure we don't reference out-of-bounds indices
                {
                    // First triangle
                    triangles.Add(current);
                    triangles.Add(nextRow);
                    triangles.Add(nextRowNextLon);

                    // Second triangle
                    triangles.Add(current);
                    triangles.Add(nextRowNextLon);
                    triangles.Add(current + nextLon);
                }
            }
        }

        return triangles.ToArray();
    }

    void SetupMeshRenderer()
    {
        // Attach a MeshRenderer and assign a material to make the mesh visible
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }
        renderer.material = meshMaterial;

        // Attach a MeshFilter if not already present
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null)
        {
            gameObject.AddComponent<MeshFilter>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*foreach (GameObject obj in spawnedObjects)
            {
                Destroy(obj);
            }*/
            //spawnedObjects.Clear();
            SpawnObjectsInSphere();
            isSpawned = true;
        }
        if (isSpawned) { 
            UpdateMeshFromObjects();
        }
    }
}
