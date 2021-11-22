using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{

    private HealthHandler healthHandler;
    private float thisMass;
    // Start is called before the first frame update
    void Start()
    {
        healthHandler = GetComponent<HealthHandler>();
        thisMass = GetComponent<Rigidbody>().mass;
    }

    public void OnCollisionEnter(Collision collision)
    {
        var collisionEnergy = collision.relativeVelocity.magnitude;
        var otherMass = collision.gameObject.GetComponent<Rigidbody>().mass;


        var totalMass = thisMass + otherMass;

        var damage = collisionEnergy * otherMass / totalMass / 10.0f;
        //healthHandler.DoDamage((int)damage);
        healthHandler.DoDamage(100);
    }

}
