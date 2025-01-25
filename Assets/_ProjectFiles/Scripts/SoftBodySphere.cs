using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodySphere : MonoBehaviour
{
    [SerializeField]
    private Transform m_parent;
    public GameObject objectToSpawn; // The prefab to spawn
    public int numObjects = 100;    // Total number of objects to spawn
    public float sphereRadius = 5f; // Radius of the sphere
    public Material meshMaterial;

    GameObject[] spawnedObjects;
    public List<Rigidbody> spawnedRigidbodies;
    private Mesh mesh;
    private bool isSpawned = false;

    [SerializeField]
    private Transform m_cameraTarget;

    [SerializeField]
    private GameObject m_eaterPrefab;
    private Eater m_eater;


    void Start()
    {
        mesh = new Mesh();
        SpawnObjectsInSphere();
        isSpawned = true;
    }

   void SpawnObjectsInSphere()
    {
        int numLatitudeBands = Mathf.RoundToInt(Mathf.Sqrt(numObjects));  // Adjust based on desired resolution
        int numLongitudeBands = Mathf.RoundToInt(numObjects / (float)numLatitudeBands);

        spawnedObjects = new GameObject[numLatitudeBands * numLongitudeBands];

        float phiIncrement = Mathf.PI * (3 - Mathf.Sqrt(5));  // Golden angle in radians

        for (int lat = 0; lat < numLatitudeBands; lat++)
        {
            for (int lon = 0; lon < numLongitudeBands; lon++)
            {
                float y = 1 - (lat / (float)(numLatitudeBands - 1)) * 2;  // Latitude position
                float radius = Mathf.Sqrt(1 - y * y);                      // Radius of circle at this latitude
                float theta = lon * Mathf.PI * 2 / numLongitudeBands;      // Longitude angle

                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);

                Vector3 position = new Vector3(x, y, z) * sphereRadius;

                int index = lat * numLongitudeBands + lon;
                spawnedObjects[index] = Instantiate(objectToSpawn, transform.position + position, Quaternion.identity);
            }
        }
        foreach (GameObject obj in spawnedObjects)
        {
            obj.AddComponent<Rigidbody>();
            spawnedRigidbodies.Add(obj.GetComponent<Rigidbody>());
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
                joint.spring = 2;
                joint.damper = 0.1f;
                joint.connectedBody = obj2.GetComponent<Rigidbody>();
            }
        }
        m_eater = Instantiate(m_eaterPrefab, transform.position, Quaternion.identity).GetComponent<Eater>();
        m_eater.followPosition = CalculateCenterOfPoints();
        m_eater.SetSize(sphereRadius);
        //SetupMeshRenderer();
    }
    void UpdateMeshFromObjects()
    {
        // Collect positions of all spawned objects
        Vector3[] vertices = new Vector3[spawnedObjects.Length];
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            vertices[i] = spawnedObjects[i].transform.position - transform.position;
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

        int numLatitudeBands = Mathf.RoundToInt(Mathf.Sqrt(vertices.Length)); // Number of latitude bands (rows)
        int numLongitudeBands = Mathf.RoundToInt(vertices.Length / (float)numLatitudeBands); // Number of longitude bands (columns)

        List<int> triangles = new List<int>();

        // Loop through the latitude bands
        for (int lat = 0; lat < numLatitudeBands - 1; lat++)
        {
            for (int lon = 0; lon < numLongitudeBands; lon++)
            {
                int current = lat * numLongitudeBands + lon;
                int nextLon = (lon + 1) % numLongitudeBands; // Wrap around horizontally for the last longitude

                // Triangle 1: current, nextLon + nextRow, current + nextLon
                int nextRow = (lat + 1) * numLongitudeBands + lon; // Vertex below current
                int nextRowNextLon = nextRow + nextLon - lon; // Diagonal vertex below

                // Add first triangle
                triangles.Add(nextRow);
                triangles.Add(current);
                triangles.Add(nextRowNextLon);

                // Add second triangle (using the same vertices)
                triangles.Add(current);
                triangles.Add(nextRowNextLon);
                triangles.Add(current + nextLon);
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

    Vector3 CalculateCenterOfPoints()
    {
        Vector3 sum = Vector3.zero;
        foreach (GameObject go in spawnedObjects)
        {
            Vector3 point = go.transform.position;
            sum += point;
        }

        return sum / spawnedObjects.Length;  // Average the points
    }

    private void Update()
    {
        if (isSpawned) { 
            //UpdateMeshFromObjects();
            Vector3 center = CalculateCenterOfPoints();
            m_cameraTarget.position = center;
            m_eater.followPosition = center;
        }
    }

}
