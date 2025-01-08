using UnityEngine;

public class RigidBodyPush : MonoBehaviour
{
    public GameObject Player;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update() {
//        Player.transform.position = transform.position;
    }

    public void SetVelocity(Vector3 v) {
        rb.linearVelocity = v;
    }

}
