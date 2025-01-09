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
    public GameObject Collision, CrouchCollision, GroundPoint, CameraRoot;
    public float camera_sensitivity, camera_clamp, camera_speed, camera_dampening_range, camera_dampening_speed, move_acceleration, move_decceleration, move_max_speed, move_max_speed_crouched, gravity_acceleration, terminal_velocity, grounding_velocity;
    public bool crouched;
    private float groundRadius;
    private Vector3 velocity, groundOffset0, groundOffset1;
    private Vector2 cameraRotation, cameraSpeed;
    private bool grounded, grounding;
    private Collider[] groundCols;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = gameObject.GetComponent<InputTable>();
        rb = gameObject.GetComponent<Rigidbody>();
        groundRadius = GroundPoint.transform.localScale.x*GroundPoint.GetComponent<SphereCollider>().radius;
        velocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotate();
        GroundedCheck();
        Move();
        if (!grounded) {
            Gravity();
        }
        rb.linearVelocity = velocity;
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
        Debug.Log(cameraRotation);
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
        if (Mathf.Abs(CameraRoot.transform.rotation.x)<70) {
            CameraRoot.transform.Rotate(Vector3.right, rotY - cameraRotation.y, Space.Self);
        }
        if (Mathf.Abs(CameraRoot.transform.rotation.x)>70) {
            CameraRoot.transform.Rotate(Vector3.right, 70-CameraRoot.transform.rotation.x, Space.Self);
        } else if (CameraRoot.transform.rotation.x<-70) {
            CameraRoot.transform.Rotate(Vector3.right, -70-CameraRoot.transform.rotation.x, Space.Self);
        }
        cameraRotation = new Vector2(rotX,rotY);
    }

    void GroundedCheck() {
        Vector3 pos = crouched ? groundOffset1 : groundOffset0;
        groundCols = Physics.OverlapSphere(pos,groundRadius,0);
        if (!grounded && groundCols.Length>0) {
            grounding = true;
        }
        grounded = groundCols.Length > 0;
    }

    void Move() {
        float max = crouched ? move_max_speed_crouched : move_max_speed;
        Vector3 move = new Vector3(input.move.x,0,input.move.y);
        //need to rotate move to match current object rotation
        move = Vector3.RotateTowards(move,transform.rotation.eulerAngles,7,0);
        if (input.move.x==0) {
            velocity.x = Mathf.Lerp(velocity.x,0,move_decceleration*Time.deltaTime);
        } else {
            velocity.x = Mathf.Lerp(velocity.x,max*move.x,move_acceleration*Time.deltaTime);
        }
        if (input.move.y==0) {
            velocity.z = Mathf.Lerp(velocity.z,0,move_decceleration*Time.deltaTime);
        } else {
            velocity.z = Mathf.Lerp(velocity.z,max*move.y,move_acceleration*Time.deltaTime);
        }
    }

    void Gravity() {
        if (!grounded) {
            velocity.y = Mathf.Lerp(velocity.y,-terminal_velocity,gravity_acceleration*Time.deltaTime);
        } else if (grounding) {
            velocity.y = -grounding_velocity;
            grounding = false;
        }
    }
}
