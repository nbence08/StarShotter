using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthHandler : MonoBehaviour
{
    public float Hull;
    public float Shield;

    public GameObject ShieldEllipsoid;
    public bool AI = false;

    public GameObject HealthBar;
    public GameObject ShieldBar;

    private float MaxHull;
    private float MaxShield;
    private float DefScaleX;
    /*
    handling health and shieldbar should be done by creating a descendant class of this class
    */

    void Start()
    {
        //if (!AI)
        {
            MaxHull = Hull;
            MaxShield = Shield;
            DefScaleX = HealthBar.transform.localScale.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Shield > 0.0f) ShieldEllipsoid.SetActive(true);
        if (Shield == 0.0f) ShieldEllipsoid.SetActive(false);

        var healthScale = HealthBar.transform.localScale;
        var shieldScale = ShieldBar.transform.localScale;
        healthScale.x = DefScaleX * Hull / MaxHull;
        shieldScale.x = DefScaleX * Shield / MaxShield;
        HealthBar.transform.localScale = healthScale;
        ShieldBar.transform.localScale = shieldScale;
    }

    public void DoDamage(float damage)
    {
        if (damage >= Shield && Shield > 0.0f)
        {
            Shield = 0.0f;

            damage -= damage - Shield;
        }
        else if (Shield >= 0 && Shield > 0.0f)
            Shield -= damage;
        if (damage >= Hull && Shield == 0.0f)
        {
            Hull -= damage;
            if (AI)
            {
                SceneTransitionInfo.EntWon = true;
                SceneTransitionInfo.KlingonWon = false;
            } else
            {
                SceneTransitionInfo.EntWon = false;
                SceneTransitionInfo.KlingonWon = true;
            }
            SceneManager.LoadScene("MenuScene");
        }
        else if ( Shield == 0.0f)
            Hull -= damage;
        //if (!AI)
        {

        }
    }
}
