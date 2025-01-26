using UnityEngine;

public class Mover : MonoBehaviour
{
    private Rigidbody m_rigidBody;
    private Eatable m_eatable;
    [SerializeField] private float m_speed = 1;
    [SerializeField] private Transform m_raycastStart;

    private Vector3 m_startPos;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_eatable = GetComponent<Eatable>();
        if (m_rigidBody)
        {
            m_rigidBody.isKinematic = true;
        }
        m_startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (m_eatable.isEaten) {
            return;
        }
        if (m_rigidBody)
        {
            m_rigidBody.MovePosition(transform.position + transform.forward * m_speed * Time.deltaTime);
        }
        else
        {
            transform.position += transform.forward * m_speed * Time.deltaTime;
        }
        if (Physics.SphereCast(m_raycastStart.position, .5f,  transform.forward, out RaycastHit hit, 1))
        {
            if (hit.collider)
            {
                int direction = Random.Range(-180, 180);
                transform.Rotate(0, direction, 0);
            }
        }

        if(Vector3.Distance(m_startPos, transform.position) > 50)
        {
            GetComponent<Eatable>().isEaten = true;
        }
    }
}
