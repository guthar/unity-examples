using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRaiffeisenLogoToggle : MonoBehaviour
{
    public GameObject raiffeisenLogo;
    public int toggleSecconds = 10;
    public ScoreManager scoreManager;

    private bool raiffeisenLogoActive = false;
    private DateTime lastToggleDateTime = DateTime.Now;
    private TimeSpan toggleSecondsTimeSpan;

    // Start is called before the first frame update
    void Start()
    {
        toggleSecondsTimeSpan = new TimeSpan(0, 0, toggleSecconds);
        setRaiffeisenLogoActive(raiffeisenLogoActive);
    }

    // Update is called once per frame
    void Update()
    {
        if ((lastToggleDateTime + toggleSecondsTimeSpan) < DateTime.Now)
        {
            raiffeisenLogoActive = !raiffeisenLogoActive;
            setRaiffeisenLogoActive(raiffeisenLogoActive);
            lastToggleDateTime = DateTime.Now;
        }
    }

    void setRaiffeisenLogoActive(bool active)
    {
        raiffeisenLogo.SetActive(raiffeisenLogoActive);
        scoreManager.SetWeltspartag(raiffeisenLogoActive);
    }
}
