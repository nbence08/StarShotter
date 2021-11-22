using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCaster : MonoBehaviour
{
    //possible further upgrade by diversifying into multiple functions => SRP...
    public static void CastBeam(GameObject cylinder, Light light, Vector3 start, Vector3 end)
    {
        var beamLength = (end - start).magnitude;

        var middle = (start + end) / 2.0f;

        cylinder.transform.position = middle;

        var cylRot = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), (start - end));

        cylinder.transform.rotation = cylRot;
        var defaultScale = cylinder.transform.localScale;
        cylinder.transform.localScale = new Vector3(defaultScale.x, 0.5f * beamLength, defaultScale.z);

        light.transform.position = start;
        var lightRot = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, 1.0f), (end - start));
        light.transform.rotation = lightRot;
        light.transform.position = start;

        cylinder.SetActive(true);
        light.range = beamLength + 1.0f;
    }

    public static void RemoveBeam(GameObject cylinder, Light light)
    {
        cylinder.SetActive(false);
        light.range = 0;
    }
}
