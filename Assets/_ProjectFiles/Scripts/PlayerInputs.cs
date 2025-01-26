using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputs : MonoBehaviour
{
    private SoftBodySphere m_softBodySphere;
    private Vector3 m_movement;
    [SerializeField]
    private float m_movementSpeed = 10f;
    [SerializeField]
    private float m_lookSpeed = 2f;

    [SerializeField]
    private Transform m_cameraTarget;
    [SerializeField]
    private Transform m_playerForwardTransform;

    float m_pitch = 0f;
    float m_yaw = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_softBodySphere = GetComponent<SoftBodySphere>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_pitch != 0)
        {
            m_cameraTarget.Rotate(Vector3.up, m_pitch * Time.deltaTime * m_lookSpeed);
            m_pitch = 0;
        }
        //if (m_yaw != 0)
        //{
        //    m_cameraTarget.Rotate(Vector3.right, m_yaw * Time.deltaTime * m_lookSpeed);
        //    m_yaw = 0;
        //}
        m_playerForwardTransform.eulerAngles = new Vector3(0, m_cameraTarget.eulerAngles.y, 0);
    }

    public void OnMove(InputValue value) {
        m_movement = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
    }

    public void OnLook(InputValue value)
    {
        Vector2 look = value.Get<Vector2>();
        m_yaw = -look.y;
        m_pitch = look.x;
    }

    private void FixedUpdate()
    {
        if (m_movement == Vector3.zero)
        {
            return;
        }
        //Vector3 dir = m_playerForwardTransform.TransformDirection(m_movement) * m_movementSpeed;
        for (int i = 0; i < m_softBodySphere.spawnedRigidbodies.Count; i++)
        {
            m_softBodySphere.spawnedRigidbodies[i].AddForce(m_movement * m_movementSpeed * m_softBodySphere.GetScaleMultiplier());
        }

    }

    public void OnQuit() {
        SceneTransition sceneTransition = FindFirstObjectByType<SceneTransition>();
        if (sceneTransition == null) {
            return;
        }
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            return;
        }
        sceneTransition.OnTransitionDone.AddListener(() => {
                SceneManager.LoadScene(0);
            }
        );
        sceneTransition.StartTransitionOut();
    }

    public void OnCompleteLevel() { 
        if(GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}
