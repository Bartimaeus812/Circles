using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollower : MonoBehaviour
{


    public GameObject CameraPos0, CameraPos1;
    private GameObject Target;
    private bool toPosition, moving, targeted;
    public float delta;
    private Vector2 position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Target = toPosition ? CameraPos1 : CameraPos0;
        transform.position = Target.transform.position;
        moving = false;
        targeted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving) {
            if (!targeted) {
                Target = toPosition ? CameraPos1 : CameraPos0;
                targeted = true;
            }
            if (Vector3.Distance(transform.position,Target.transform.position)>delta) {
                Vector3.MoveTowards(transform.position,Target.transform.position,delta);
            } else {
                transform.position = Target.transform.position;
                moving = false;
            }
        } else {
            transform.position = Target.transform.position;
        }
    }

    public void MoveTo(bool b) {
        toPosition = b;
        moving = true;
        targeted = false;
    }
}
