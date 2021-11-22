using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineControl : MonoBehaviour
{
    public Light impulseLeft;
    public Light impulseRight;
    public float engineStrength;
    public float torqueStrength;

    public float reverseCoeff;
    public float thrusterCoeff;

    Rigidbody EnterpriseRB;
    // Start is called before the first frame update
    void Start()
    {
        EnterpriseRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var delta = Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            var forward = new Vector4(0.0f, 0.0f, -1.0f);
            forward = transform.rotation * forward;
            EnterpriseRB.AddForce(delta * engineStrength * forward);
            impulseLeft.intensity = 1.0f;
            impulseRight.intensity = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            var forward = new Vector4(0.0f, 0.0f, reverseCoeff);
            forward = transform.rotation * forward;
            EnterpriseRB.AddForce(delta * engineStrength * forward);
            impulseLeft.intensity = 0.0f;
            impulseRight.intensity = 0.0f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            var forward = new Vector4(-thrusterCoeff, 0.0f, 0.0f);
            forward = transform.rotation * forward;
            EnterpriseRB.AddForce(delta * engineStrength * forward);
            impulseLeft.intensity = 0.0f;
            impulseRight.intensity = 0.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            var forward = new Vector4(thrusterCoeff, 0.0f, 0.0f);
            forward = transform.rotation * forward;
            EnterpriseRB.AddForce(delta * engineStrength * forward);
            impulseLeft.intensity = 0.0f;
            impulseRight.intensity = 0.0f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            var forward = new Vector4(0.0f, thrusterCoeff, 0.0f);
            forward = transform.rotation * forward;
            EnterpriseRB.AddForce(delta * engineStrength * forward);
            impulseLeft.intensity = 0.0f;
            impulseRight.intensity = 0.0f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            var forward = new Vector4(0.0f, -thrusterCoeff, 0.0f);
            forward = transform.rotation * forward;
            EnterpriseRB.AddForce(delta * engineStrength * forward);
            impulseLeft.intensity = 0.0f;
            impulseRight.intensity = 0.0f;
        }
        else if (Input.GetKey(KeyCode.Delete))
        {
            var movement = EnterpriseRB.velocity.normalized;
            EnterpriseRB.AddForce(-delta * engineStrength * movement);
        }
        else
        {
            impulseLeft.intensity = 0.0f;
            impulseRight.intensity = 0.0f;
        }

        if (Input.GetKey(KeyCode.Home))
        {
            var forward = new Vector4(0.0f, 0.0f, -1.0f);
            forward = transform.rotation * forward;
            
            EnterpriseRB.AddTorque(torqueStrength * delta * forward);
        }
        else if (Input.GetKey(KeyCode.PageUp))
        {
            var forward = new Vector4(0.0f, 0.0f, 1.0f);
            forward = transform.rotation * forward;

            EnterpriseRB.AddTorque(torqueStrength * delta * forward);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            var forward = new Vector4(0.0f, 1.0f, 0.0f);
            forward = transform.rotation * forward;

            EnterpriseRB.AddTorque(torqueStrength * delta * forward);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            var forward = new Vector4(0.0f, -1.0f, 0.0f);
            forward = transform.rotation * forward;

            EnterpriseRB.AddTorque(torqueStrength * delta * forward);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            var forward = new Vector4(1.0f, 0.0f, 0.0f);
            forward = transform.rotation * forward;

            EnterpriseRB.AddTorque(torqueStrength * delta * forward);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            var forward = new Vector4(-1.0f, 0.0f, 0.0f);
            forward = transform.rotation * forward;

            EnterpriseRB.AddTorque(torqueStrength * delta * forward);
        }
        else if (Input.GetKey(KeyCode.Mouse2))
        {
            /*var camDir = Camera.main.transform.forward;
            var entDir = transform.forward;
            camDir.Normalize();
            entDir.Normalize();

            var dir = Quaternion.FromToRotation(entDir, camDir).eulerAngles;
            dir = new Vector3(dir.x > 180.0f ? 360.0f - dir.x : -dir.x,
                              dir.y > 180.0f ? 360.0f - dir.y : -dir.y,
                              dir.z > 180.0f ? 360.0f - dir.z : -dir.z);
            dir.Normalize();*/

            var camRot = Camera.main.transform.rotation;
            var entRot = transform.rotation;

            var forward = new Vector3(0.0f, 0.0f, 1.0f);
            var entDir = entRot * -forward;
            var camDir = camRot * forward;

            var diffRot = Quaternion.FromToRotation(entDir, camDir);

            var diff = diffRot.eulerAngles;
            var dir = diff;
            dir = new Vector3(dir.x > 180.0f ? -360.0f + dir.x : dir.x,
                  dir.y > 180.0f ? -360.0f + dir.y : dir.y,
                  dir.z > 180.0f ? -360.0f + dir.z : dir.z);
            dir = dir.normalized;

            var dirMag = Mathf.Min(dir.magnitude / Mathf.Sqrt(3 * 180 * 180) + 0.5f, 1.0f);

            EnterpriseRB.AddTorque(torqueStrength * Mathf.Max(dirMag, 0.2f) * delta * dir);
        }
        else 
        {
            //Enterprise will automatically cancel out any angular velocity
            var av = EnterpriseRB.angularVelocity;
            if(av.magnitude < 2.0f)
                EnterpriseRB.AddTorque( -0.2f*(torqueStrength * delta * 2.0f * EnterpriseRB.angularVelocity.normalized));
            else
                EnterpriseRB.AddTorque(-(torqueStrength * delta * 2.0f * EnterpriseRB.angularVelocity.normalized));
        }
    }
}
