using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponController : MonoBehaviour
{

    public float weaponsRange;
    public GameObject playerShip;
    public GameObject AIShip;
    public GameObject disruptorBank;
    public GameObject Targets;
    

    public GameObject disruptorCylinderPrefab;
    public Light disruptorLightPrefab;

    public float waitTimeRange;
    public float shootTimeRange;

    private GameObject cylinderInstance;
    private Light lightInstance;
    private AudioSource disruptorSoundSource;
    private HealthHandler entHealthHandler;
    private List<GameObject> TargetsList;

    // Start is called before the first frame update
    private float waitTime;
    private bool waiting = true;
    private float shootTime;
    private float errorSize;

    private Vector3 error;
    public int layerMask;
    void Start()
    {
        disruptorSoundSource = disruptorBank.GetComponent<AudioSource>();

        cylinderInstance = Instantiate(disruptorCylinderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        lightInstance = Instantiate(disruptorLightPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        cylinderInstance.SetActive(false);
        lightInstance.range = 0;
        errorSize = SceneTransitionInfo.KlingonErrorSize;

        entHealthHandler = playerShip.GetComponent<HealthHandler>();

        TargetsList = new List<GameObject>();

        var patrolRouteListSize = Targets.transform.childCount;
        for (var i = 0; i < patrolRouteListSize; i++) TargetsList.Add(Targets.transform.GetChild(i).gameObject);

        layerMask = int.MaxValue;
        layerMask -= 1 << 6;
    }

    // Update is called once per frame
    private int RandomTarget = 0;
    void Update()
    {
        if ((playerShip.transform.position - transform.position).magnitude > weaponsRange)
        {
            BeamCaster.RemoveBeam(cylinderInstance, lightInstance);
            disruptorSoundSource.mute = true;
            return;
        }

        if (shootTime <= 0 && !waiting)
        {
            waitTime = Random.Range(0.0f, waitTimeRange);
            BeamCaster.RemoveBeam(cylinderInstance, lightInstance);
            waiting = true;
        }
        if (waitTime <= 0 && waiting)
        {
            RandomTarget = (int)Random.Range(-0.49f, TargetsList.Count);
            shootTime = Random.Range(0.0f, shootTimeRange);
            error = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized * errorSize;
            waiting = false;
        }
        if (waiting)
        {
            waitTime -= Time.deltaTime;
            disruptorSoundSource.mute = true;
            return;
        }
        if (!waiting)
        {
            shootTime -= Time.deltaTime;

            var begin = disruptorBank.transform.position;
            var rayDir = (TargetsList[RandomTarget].transform.position - begin).normalized;
            var ray = new Ray(begin, rayDir);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, layerMask))
            {

                if (hitInfo.rigidbody != null)
                {
                    if (!hitInfo.rigidbody.gameObject.Equals(playerShip))
                    {
                        disruptorSoundSource.mute = true;
                        BeamCaster.RemoveBeam(cylinderInstance, lightInstance);
                        return;
                    }
                    else
                    {
                        disruptorSoundSource.mute = false;
                        Ray erroredRay = new Ray(begin + rayDir, (hitInfo.point + error) - (begin + rayDir));
                        if(Physics.Raycast(erroredRay, out hitInfo, layerMask))
                        {
                            BeamCaster.CastBeam(cylinderInstance, lightInstance, begin, hitInfo.point);
                            if(hitInfo.rigidbody != null)
                            {
                                var other = hitInfo.rigidbody.gameObject;
                                if (other.Equals(playerShip))
                                {
                                    entHealthHandler.DoDamage(Time.deltaTime);
                                }
                            }
                        } else
                        {
                            BeamCaster.CastBeam(cylinderInstance, lightInstance, begin, erroredRay.GetPoint(100.0f));
                        }
                    }
                }
            }
        }
    }
}
