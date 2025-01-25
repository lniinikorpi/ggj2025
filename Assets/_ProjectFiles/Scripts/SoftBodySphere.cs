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
    public List<SpringJoint> spawnedJoints;
    private Mesh mesh;
    private bool isSpawned = false;

    [SerializeField]
    private Transform m_cameraTarget;

    [SerializeField]
    private GameObject m_eaterPrefab;
    private Eater m_eater;

    [SerializeField] float m_jointLengthTarget = .1f;
    [SerializeField] float m_springForceTarget = 4f;
    [SerializeField] float m_scaleTarget = .6f;
    [SerializeField] float growthspeed = 0;
    [SerializeField] float springlength = 0;
    [SerializeField] float ballscale = 0;
    private bool m_isGrowing = false;


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
                joint.spring = m_springForceTarget;
                joint.damper = 0.1f;
                joint.minDistance = m_jointLengthTarget;
                joint.maxDistance = m_jointLengthTarget;
                joint.connectedBody = obj2.GetComponent<Rigidbody>();
                spawnedJoints.Add(joint);
            }
        }
        m_eater = Instantiate(m_eaterPrefab, transform.position, Quaternion.identity).GetComponent<Eater>();
        m_eater.followPosition = CalculateCenterOfPoints();
        m_eater.SetSize(sphereRadius);
        //SetupMeshRenderer();
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

    public void Grow() {
        m_jointLengthTarget += springlength;
        m_springForceTarget *= growthspeed;
        m_scaleTarget *= ballscale;
        if (m_isGrowing)
        {
            return;
        }
        else { 
            StartCoroutine(GrowCoroutine());
        }
    }

    IEnumerator GrowCoroutine()
    {
        while (true)
        {
            bool isScaled = true;
            float step = 3 * Time.fixedDeltaTime;
            foreach (var joint in spawnedJoints)
            {
                if (joint.spring < m_springForceTarget)
                {
                    joint.spring += step;
                    isScaled = false;
                }
                else { 
                    joint.spring = m_springForceTarget;
                }
                if(joint.maxDistance < m_jointLengthTarget)
                {
                    joint.maxDistance += step;
                    joint.minDistance += step;
                    isScaled = false;
                }
                else
                {
                    joint.maxDistance = m_jointLengthTarget;
                    joint.minDistance = m_jointLengthTarget;
                }
            }
            foreach (var obj in spawnedObjects)
            {
                if (obj.transform.localScale.x < m_scaleTarget)
                {
                    obj.transform.localScale += Vector3.one * step;
                    isScaled = false;
                }
                else
                {
                    obj.transform.localScale = Vector3.one * m_scaleTarget;
                }
            }
            yield return new WaitForSeconds(.1f);
            if (isScaled)
            {
                m_isGrowing = false;
                break;
            }
        }
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
