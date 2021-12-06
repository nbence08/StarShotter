using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject phaserCylinderPrefab;
    public Light phaserLightPrefab;
    public GameObject phaserBank;
    public GameObject ship;
    public GameObject klingonShip;

    private AudioSource phaserSoundSource;
    public float farDist = 100.0f;
    
    private GameObject cylinderInstance;
    private Light lightInstance;
    private HealthHandler klingonLifeHandler;
    private float weaponStrength;

    private int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        phaserSoundSource = phaserBank.GetComponent<AudioSource>();
        
        cylinderInstance = Instantiate(phaserCylinderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        lightInstance = Instantiate(phaserLightPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        cylinderInstance.SetActive(false);
        lightInstance.range = 0;

        klingonLifeHandler = klingonShip.GetComponent<HealthHandler>();
        weaponStrength = SceneTransitionInfo.EntWeaponStrength;

        layerMask = int.MaxValue;
        layerMask -= 1 << 7;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            

            RaycastHit hitInfo;
            var hit = Physics.Raycast(ray, out hitInfo, layerMask);
            var start = phaserBank.transform.position;
            Vector3 end;
            if (hit)
            {
                var obj = hitInfo.collider.gameObject;
                if (obj != (ship))
                    end = hitInfo.point;
                else
                    end = ray.GetPoint(farDist);
            }
            else
            {
                end = ray.GetPoint(farDist);
                var secondRay = new Ray(start, end);
                var secondHit = UnityEngine.Physics.Raycast(secondRay, out hitInfo, layerMask);
                if (secondHit)
                {
                    var obj = hitInfo.collider.gameObject;
                    if (obj != (ship))
                    {
                        end = hitInfo.point;
                    }
                }
            }
            phaserSoundSource.mute = false;

            if(hitInfo.rigidbody != null)
            {
                var other = hitInfo.rigidbody.gameObject;
                if (other.Equals(klingonShip))
                {
                    klingonLifeHandler.DoDamage(Time.deltaTime * weaponStrength);
                }
            }

            BeamCaster.CastBeam(cylinderInstance, lightInstance, start, end);

        }
        else
        {
            phaserSoundSource.mute = true;
            cylinderInstance.SetActive(false);
            lightInstance.range = 0;
        }
    }
}
