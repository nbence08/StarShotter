using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCamera : MonoBehaviour
{
    public GameObject cameraLookAt;
    public float cameraSpeed = 1.0f;

    private float cameraHeight;
    private float cameraXZOffset;
    private double cameraAngle;

    const float PI = 3.141592653589793238f;


    // Start is called before the first frame update
    void Start()
    {
        var lookAt2Camera = cameraLookAt.transform.position - transform.position;
        cameraHeight = Mathf.Abs(lookAt2Camera.y);
        var cameraXZOffsetVec = new Vector3(lookAt2Camera.x, 0, lookAt2Camera.z);
        cameraXZOffset = cameraXZOffsetVec.magnitude;
        cameraAngle = PI;
    }

    // Update is called once per frame
    void Update()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 newPos = new Vector3();

        newPos.y = (cameraLookAt.transform.position.y + cameraHeight);
        newPos.x = cameraLookAt.transform.position.x + Mathf.Sin((float)cameraAngle) * cameraXZOffset;
        newPos.z = cameraLookAt.transform.position.z + Mathf.Cos((float)cameraAngle) * cameraXZOffset;
        cameraTransform.position = newPos;
        cameraTransform.LookAt(cameraLookAt.transform);
        
        HandleIO();

    }

    void HandleIO()
    {
        float cameraHorizontal = Input.GetAxis("Mouse X");
        float cameraVertical = Input.GetAxis("Mouse Y");
        float cameraScroll = Input.GetAxis("Mouse ScrollWheel");
        //cameraAngle += cameraHorizontal * Time.deltaTime * cameraSpeed;
        if (cameraHorizontal != 0 && Input.GetKey(KeyCode.Mouse1))
        {
            cameraAngle += cameraHorizontal * Time.deltaTime * cameraSpeed;
        }
        if (cameraVertical != 0 && Input.GetKey(KeyCode.Mouse1))
        {
            cameraHeight += cameraVertical * Time.deltaTime * cameraSpeed * 4.0f;
        }
        if (cameraScroll != 0)
        {
            cameraXZOffset -= cameraScroll * 4.0f;
        }
    }
}
