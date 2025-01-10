using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

public class PlayerMovement : MonoBehaviour
{
    private InputTable input;
//    public Rigidbody Collision, CrouchCollision;
    private Rigidbody rb;
    public GameObject Collision, CrouchCollision, GroundPoint, CameraRoot, StairClimb;
    public float camera_sensitivity, camera_speed, camera_dampening_range, camera_dampening_speed, move_acceleration, move_decceleration, move_max_speed, move_max_speed_crouched, gravity_acceleration, terminal_velocity, grounding_velocity, stair_climb;
    public bool crouched;
    private float groundRadius, moveSpeed;
    private Vector3 velocity, groundOffset0, groundOffset1, stairHalfs;
    private Vector2 cameraRotation, cameraSpeed;
    private bool grounded, grounding, stairs;
    private Collider[] groundCols;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = gameObject.GetComponent<InputTable>();
        rb = gameObject.GetComponent<Rigidbody>();
        groundRadius = GroundPoint.transform.localScale.x*GroundPoint.GetComponent<SphereCollider>().radius;
        velocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.Locked;
        stairHalfs = StairClimb.GetComponent<BoxCollider>().size/2;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotate();
        if (Math.Abs(rb.linearVelocity.y)<0.5f) {
            GroundedCheck();
        }
        Move();
        Gravity();
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y+velocity.y, velocity.z);
    }

    void CameraRotate() {
        float rotX = input.look.x * camera_sensitivity;
        float rotY = input.look.y * camera_sensitivity;
        rotX = Mathf.Clamp(rotX,-180,180);
        cameraRotation.x -= rotX;
        cameraRotation.y += rotY;

        //from https://discussions.unity.com/t/add-camera-damping-at-rotation-and-position/780878/2
        cameraSpeed = Vector2.one * camera_speed;
        if (Mathf.Abs(cameraRotation.x)<camera_dampening_range) {
            float alpha = cameraRotation.x/camera_dampening_range;
            cameraSpeed.x = Mathf.Lerp(camera_dampening_speed,camera_speed,alpha);
        }
        if (Mathf.Abs(cameraRotation.y)<camera_dampening_range) {
            float alpha = cameraRotation.y/camera_dampening_range;
            cameraSpeed.y = Mathf.Lerp(camera_dampening_speed,cameraSpeed.y,alpha);
        }
//        Debug.Log(cameraRotation);
        cameraSpeed *= Time.deltaTime;
        if (Mathf.Abs(cameraRotation.x)<0.5f) {
            cameraSpeed.x = cameraRotation.x;
        }
        if (Mathf.Abs(cameraRotation.y)<0.5f) {
            cameraSpeed.y = cameraRotation.y;
        }
        rotX = Mathf.Lerp(cameraRotation.x, 0, cameraSpeed.x);
        rotY = Mathf.Lerp(cameraRotation.y, 0, cameraSpeed.y);
        transform.RotateAround(transform.position, Vector3.up, rotX-cameraRotation.x);
        CameraRoot.transform.Rotate(Vector3.right, rotY - cameraRotation.y, Space.Self);
        cameraRotation = new Vector2(rotX,rotY);
    }

    void GroundedCheck() {
        Vector3 pos = crouched ? groundOffset1 : groundOffset0;
        groundCols = Physics.OverlapSphere(pos,groundRadius,3);
        if (!grounded && groundCols.Length>0) {
            grounding = true;
        }
        grounded = groundCols.Length > 0;
        Collider[] cols = Physics.OverlapBox(StairClimb.transform.position,stairHalfs,StairClimb.transform.rotation,3);
        stairs = cols.Length>0;
    }

    void Move() {
        float max = crouched ? move_max_speed_crouched : move_max_speed;
        if (input.move==Vector2.zero) {
            moveSpeed = Mathf.Lerp(moveSpeed,0,move_decceleration*Time.deltaTime);
        } else {
            moveSpeed = Mathf.Lerp(moveSpeed,max,move_acceleration*Time.deltaTime);
        }

        //from https://discussions.unity.com/t/rotate-a-vector3-direction/14722/2
        Vector3 move = input.move.x*transform.right + input.move.y*transform.forward;
        velocity.x = moveSpeed*move.x;
        velocity.z = moveSpeed*move.z;
    }

    void Gravity() {
        if (!grounded) {
            if (rb.linearVelocity.y<0f) {
                velocity.y = Mathf.Lerp(velocity.y,-terminal_velocity,gravity_acceleration*Time.deltaTime);
            }
        } else if (grounding) {
            velocity.y = -grounding_velocity;
            grounding = false;
        }
        if (stairs) {
            velocity.y = stair_climb;
        }
    }
}
