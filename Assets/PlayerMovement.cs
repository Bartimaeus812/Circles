using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

public class PlayerMovement : MonoBehaviour
{
    private InputTable input;
    public Rigidbody rb0, rb1;
    public GameObject ground;
    public float acceleration, jump_power, gravity, max_speed, max_gravity;

    private Vector3 velocity, accel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = gameObject.GetComponent<InputTable>();
    }

    // Update is called once per frame
    void Update()
    {
        float num = max_speed * input.move.x<0 ? -1 : 1;
        accel.x = Mathf.Lerp(accel.x,input.move.x==0 ? 0 : max_speed,acceleration*Time.deltaTime);
        num = max_speed * input.move.y<0 ? -1 : 1;
        accel.z = Mathf.Lerp(accel.z,input.move.y==0 ? 0 : max_speed,input.move.y*acceleration*Time.deltaTime);
        
        Collider[] cols = Physics.OverlapSphere(rb0.transform.position-ground.transform.position,0.3f,0);
        bool grounded = false;
        for (int i = 0; i<cols.Length; i++) {
            if (cols[i].gameObject.tag.Equals("Ground")) {
                grounded = true;
                break;
            }
        }

        if (grounded && input.jump) {
            velocity.y = jump_power;
            accel.y = 0;
        } else if (!grounded) {
            accel.y -= gravity * Time.deltaTime;
            if (accel.y>max_gravity) {
                accel.y = max_gravity;
            }
        }
        input.jump = false;
        velocity += accel*Time.deltaTime;
        rb0.linearVelocity = velocity;
    }
}
