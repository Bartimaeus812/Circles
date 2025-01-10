using Unity.VisualScripting;
using UnityEngine;

public class ClipCorrection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col) {
        Debug.Log(col.gameObject.name);
        if (col.transform.position.y<transform.position.y) {
            transform.position += 0.1f * Vector3.up;
        }
    }
}
