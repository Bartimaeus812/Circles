using UnityEngine;
using UnityEngine.InputSystem;

public class InputTable : MonoBehaviour
{

    private PlayerInput playerInput;
    public Vector2 move;
    public bool attack, interact, next, jump, crouch, sprint;
    //contains time button held for all bool vars
    public float[] elapsed;

    void Start()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        elapsed = new float[6];
    }

    //for every bool value adds time held to delta array
    //assumes that all handlers for these actions set to false when unused
    void Update() {
        elapsed[0] = attack ? elapsed[0] + Time.deltaTime : 0;
        elapsed[1] = interact ? elapsed[1] + Time.deltaTime : 0;
        elapsed[2] = next ? elapsed[2] + Time.deltaTime : 0;
        elapsed[3] = jump ? elapsed[3] + Time.deltaTime : 0;
        elapsed[4] = crouch ? elapsed[4] + Time.deltaTime : 0;
        elapsed[5] = sprint ? elapsed[5] + Time.deltaTime : 0;
        for (int i = 0; i<6; i++) {
            if (elapsed[i]>=10) {
                elapsed[i] = 10;
            }
        }
    }

    void OnMove(InputValue v) {
        move = v.Get<Vector2>();
    }

    void OnAttack(InputValue v) {
        if (v.isPressed) {
            attack = true;
        }
    }

    void OnInteract(InputValue v) {
        if (v.isPressed) {
            interact = true;
        }
    }

    void OnNext(InputValue v) {
        if (v.isPressed) {
            next = true;
        }
    }

    void OnJump(InputValue v) {
        if (v.isPressed) {
            jump = true;
        }
    }

    void OnCrouch(InputValue v) {
        if (v.isPressed) {
            crouch = true;
        }
    }

    void OnSprint(InputValue v) {
        if (v.isPressed) {
            sprint = true;
        }
    }

}
