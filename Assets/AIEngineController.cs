using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEngineController : MonoBehaviour
{
    public GameObject patrolRoute;
    public GameObject hidingPlaces;
    public GameObject navPoints;
    public GameObject ShipNose;
    public GameObject Ent;

    public float EngineStrength;
    public float MaxAngVelocity;
    public float MaxSpeed;
    public float sensorRange;

    private List<GameObject> PatrolRouteList;
    private List<GameObject> HidingPlacesList;
    private List<GameObject> NavPointsList;

    private HealthHandler AIHPHandler;
    private Vector3 Target;
    private float RotDiffToTarget;
    private Rigidbody AIRB;
    private bool Turning = false;
    private bool Arrived = false;
    private float stopDist;

    private float distEpsilon = 1.0f;
    private int layerMask;
    // al the raycasting could be abstracted away into some functions
    void Start()
    {
        AIRB = GetComponent<Rigidbody>();

        PatrolRouteList = new List<GameObject>();
        HidingPlacesList = new List<GameObject>();
        NavPointsList = new List<GameObject>();

        var patrolRouteListSize = patrolRoute.transform.childCount;
        for (var i = 0; i < patrolRouteListSize; i++) PatrolRouteList.Add(patrolRoute.transform.GetChild(i).gameObject);

        var hidingPlacesListSize = hidingPlaces.transform.childCount;
        for (var i = 0; i < hidingPlacesListSize; i++) HidingPlacesList.Add(hidingPlaces.transform.GetChild(i).gameObject);

        var navPointsListSize = navPoints.transform.childCount;
        for (var i = 0; i < navPointsListSize; i++) NavPointsList.Add(navPoints.transform.GetChild(i).gameObject);

        AIRB.centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);

        /*var t0 = MaxSpeed * AIRB.mass / EngineStrength;

        stopDist = MaxSpeed * t0 - 0.5f * EngineStrength / AIRB.mass * t0 * t0 + 1.5f;*/
        Target = PatrolRouteList[patrolPoint].transform.position;
        stopDist = 1.0f;

        AIHPHandler = GetComponent<HealthHandler>();

        layerMask = int.MaxValue;
        layerMask -= 1 << 6;
    }

    private bool Chasing = false;
    private bool Escaping = false;
    private bool Escaped = false;
    // Update is called once per frame
    void Update()
    {
        Escape();
        if(!Escaping && !Escaped)
            Chase();
        if(!Escaping && !Chasing && !Escaped)
            Patrol();

        Indirection();

        TurnTowardsTarget();
        MoveToTarget();

    }

    private bool Indirecting = false;
    private Vector3 IndirectTarget;
    private void Indirection()
    {
        //marching with the begin of the ray should be abstractad into a separate function

        if (!Indirecting && !Arrived)
        {
            RaycastHit hitInfo;
            var begin = ShipNose.transform.position;
            var end = Target;
            var rayDir = end - begin;
            var ray = new Ray(begin, rayDir);

            if (Physics.Raycast(ray, out hitInfo, layerMask))
            {

                if ((hitInfo.point - Target).magnitude < 4.0f) return;
                else
                {
                    //actual indirection calculation
                    foreach(var navPoint in NavPointsList)
                    {
                        var indirectBegin = begin + rayDir;
                        var navPos = navPoint.transform.position;
                        var posToNav = new Ray(indirectBegin, navPos - indirectBegin);
                        var navToTarget = new Ray(navPos, Target);

                        RaycastHit pos2NavInfo, nav2TargetInfo;
                        bool pos2NavHit = Physics.Raycast(posToNav, out pos2NavInfo, layerMask);
                        bool pos2TargetHit = Physics.Raycast(navToTarget, out nav2TargetInfo);

                        if (pos2NavHit)
                        {
                            if ((pos2NavInfo.point - indirectBegin).magnitude + 2.0f < (navPos - indirectBegin).magnitude)
                                continue;
                        }
                        if (pos2TargetHit)
                        {
                            if ((nav2TargetInfo.point - navPos).magnitude + 2.0f < (Target - navPos).magnitude)
                                continue;
                        }
                        Indirecting = true;
                        IndirectTarget = Target;
                        Target = navPoint.transform.position;
                        return;
                    }
                }
            }
            else return;
        }
        if (Indirecting && Arrived)
        {
            Indirecting = false;
            Target = IndirectTarget;
        }
    }

    private void Escape()
    {
        if (!Indirecting && Escaping && Arrived)
        {
            Escaping = false;
            Escaped = true;
        }

        Vector3 ship2EntVector = (Ent.transform.position - transform.position);
        Vector3 ship2EntDir = ship2EntVector.normalized;

        Ray ship2Ent = new Ray(transform.position + ship2EntDir*distEpsilon*2.0f, ship2EntDir);
        RaycastHit entHitInfo;
        bool hit = Physics.Raycast(ship2Ent, out entHitInfo, layerMask);
        if (!Escaping && 
            ( !hit ||
            entHitInfo.rigidbody.gameObject.Equals(Ent) ||
            !((entHitInfo.point - ShipNose.transform.position).magnitude < ship2EntVector.magnitude) ))
        {
            Escaped = false;
        }

        //BeamCaster.CastBeam(cylinder, light, transform.position + ship2EntDir * distEpsilon * 2.0f, Ent.transform.position);

        if (!Escaping && !Escaped && AIHPHandler.Shield == 0.0f)
        {
            foreach (var escPt in HidingPlacesList)
            {
                var entPos = Ent.transform.position;
                var escPtPos = escPt.transform.position;
                Ray hide2Ent = new Ray(escPtPos, entPos - escPtPos);
                RaycastHit hitInfo;
                if (Physics.Raycast(hide2Ent, out hitInfo, layerMask))
                {
                    if ((hitInfo.point - escPtPos).magnitude < (entPos - escPtPos).magnitude -5)
                    {
                        Target = escPtPos;
                        Indirecting = false;
                        Escaping = true;
                        return;
                    }
                }
            }
        }
    }

    private void Chase()
    {
        var klingon2Ent = Ent.transform.position - ShipNose.transform.position;
        if (klingon2Ent.magnitude < sensorRange && !Indirecting)
        {
            Target = Ent.transform.position - klingon2Ent.normalized * 4.0f;
            Indirecting = false;
            Chasing = true;
        }
        var target2Ent = Ent.transform.position - Target;
        var indirect2Ent = Ent.transform.position - IndirectTarget;
        if ((target2Ent.magnitude > 6.0f || indirect2Ent.magnitude > 6.0f) && Chasing) {
            
        }
        if (Chasing && klingon2Ent.magnitude > sensorRange)
        {
            var closest = PatrolRouteList[0];
            var closestDist = (closest.transform.position - ShipNose.transform.position).magnitude;
            var closestIndex = 0;
            for (int i = 1; i < PatrolRouteList.Count; i++)
            {
                var iterPatrolPoint = PatrolRouteList[i];
                var dist = (iterPatrolPoint.transform.position - ShipNose.transform.position).magnitude;
                if (dist < closestDist)
                {
                    closestIndex = i;
                    closestDist = dist;
                    closest = iterPatrolPoint;
                }
            }
            patrolPoint = closestIndex;
            Target = closest.transform.position;
            if (Indirecting) Indirecting = false;
            Chasing = false;
        }
    }

    private int patrolPoint = 0;
    void Patrol()
    {
        if((PatrolRouteList[patrolPoint].transform.position - ShipNose.transform.position).magnitude < 1.0f)
        {
            patrolPoint += 1;
            patrolPoint %= 4;
            Target = PatrolRouteList[patrolPoint].transform.position;
            if (Indirecting) Indirecting = false;
        }
    }

    void TurnTowardsTarget()
    {
        var shipForward = transform.forward;
        var shipPos = transform.position;

        var diffRot = Quaternion.FromToRotation(shipForward, (shipPos - Target));

        var diff = diffRot.eulerAngles;
        RotDiffToTarget = diff.magnitude;
        if(RotDiffToTarget >= 180.0f)
        {
            RotDiffToTarget = Mathf.Abs(360.0f - RotDiffToTarget);
        }
        if (RotDiffToTarget > 0.01f)
        {
            Turning = true;
        }
        else
        {
            Turning = false;
            return;
        }

        var dir = diff;
        dir = new Vector3(dir.x > 180.0f ? -360.0f + dir.x : dir.x,
              dir.y > 180.0f ? -360.0f + dir.y : dir.y,
              dir.z > 180.0f ? -360.0f + dir.z : dir.z);
        dir = dir.normalized;
        if (RotDiffToTarget > 0.01f)
            transform.Rotate(dir * Time.deltaTime * MaxAngVelocity, Space.World);
    }

    void MoveToTarget()
    {
        var movement = AIRB.velocity;
        var targetDistance = (Target - ShipNose.transform.position).magnitude;
        Arrived = targetDistance < distEpsilon;
        if (RotDiffToTarget < 0.01f && movement.magnitude < MaxSpeed && targetDistance > stopDist)
        {
            //AIRB.AddForce(-AIShip.transform.forward * Time.deltaTime * EngineStrength);
            AIRB.velocity = (-transform.forward * MaxSpeed);
        }
        if(Turning || 
            RotDiffToTarget > 0.01f ||
            Arrived && movement.magnitude > 0.01f)
        {
            //AIRB.AddForce(-Time.deltaTime* EngineStrength * movement.normalized);
            AIRB.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
        if(targetDistance < 10.0f && !Indirecting)
        {
            AIRB.velocity *= 0.5f;
        }

    }
}
