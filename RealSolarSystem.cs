using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;


namespace RealSolarSystem
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class RealSolarSystem : MonoBehaviour
    {
        public static void DumpBody(CelestialBody body)
        {
            print("BODY NAME: " + body.name);
            print("altitudeMultiplier = " + body.altitudeMultiplier);
            print("altitudeOffset = " + body.altitudeOffset);
            print("angularV = " + body.angularV);
            print("angularVelocity = " + body.angularVelocity);
            print("atmoshpereTemperatureMultiplier = " + body.atmoshpereTemperatureMultiplier);
            print("atmosphere = " + body.atmosphere);
            print("atmosphereContainsOxygen = " + body.atmosphereContainsOxygen);
            print("atmosphereMultiplier = " + body.atmosphereMultiplier);
            print("atmosphereScaleHeight = " + body.atmosphereScaleHeight);


            print("bodyDescription = " + body.bodyDescription);
            print("bodyName = " + body.bodyName);

            print("directRotAngle = " + body.directRotAngle);
            print("GeeASL = " + body.GeeASL);
            print("gMagnitudeAtCenter = " + body.gMagnitudeAtCenter);
            print("gravParameter = " + body.gravParameter);
            print("hillSphere = " + body.hillSphere);
            print("initialRotation = " + body.initialRotation);
            print("inverseRotation = " + body.inverseRotation);
            print("inverseRotThresholdAltitude = " + body.inverseRotThresholdAltitude);
            print("Mass = " + body.Mass);
            print("maxAtmosphereAltitude = " + body.maxAtmosphereAltitude);
            print("ocean = " + body.ocean);




            print("pressureMultiplier = " + body.pressureMultiplier);
            print("Radius = " + body.Radius);
            print("rotates = " + body.rotates);
            print("rotation = " + body.rotation);
            print("rotationAngle = " + body.rotationAngle);
            print("rotationPeriod = " + body.rotationPeriod);

            print("sphereOfInfluence = " + body.sphereOfInfluence);
            print("staticPressureASL = " + body.staticPressureASL);


            print("tidallyLocked = " + body.tidallyLocked);

            print("use_The_InName = " + body.use_The_InName);
            print("useLegacyAtmosphere = " + body.useLegacyAtmosphere);
            print("zUpAngularVelocity = " + body.zUpAngularVelocity);

            print("****************************************");
        }

        public void Start()
        {
            print("*RSS* Dumping bodies");
            foreach (CelestialBody body in FlightGlobals.fetch.bodies)
            {
                if (body == null)
                    continue;
                DumpBody(body);
                
                if(body.bodyName.Equals("Kerbin"))
                {
                    double rotPeriod = 86164.1; // 23hr, 56min, and 4.1s
                    double mass = 5.97219e24;
                    double radius = 6371000;
                    body.Radius = radius;
                    body.atmosphereScaleHeight = 7.5;
                    body.rotationPeriod = rotPeriod;
                    body.angularV = 2 * Math.PI / rotPeriod;
                    body.angularVelocity = new Vector3d(0, -body.angularV, 0);
                    body.GeeASL = 1;
                    body.gravParameter = 6.673 * mass;
                    body.gMagnitudeAtCenter = 9.81 * radius * radius;
                    body.maxAtmosphereAltitude = 105;
                    body.sphereOfInfluence = 84159286.4796305 * 35; // guessing at scale factor?
                    


                    //body.orbit.semiMajorAxis = 147098290000;
                    //Vessel v;
                    body.orbitDriver.orbit.semiMajorAxis = 147098290000;
                }
            }
            print("##############################################");
        }
        public static bool done = false;
        void OnGUI()
        {
            if (HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION) && !done)
            {
                foreach (CelestialBody body in FlightGlobals.fetch.bodies)
                {
                    if (body == null)
                        continue;
                    if (body.bodyName.Equals("Kerbin"))
                        DumpBody(body);
                }
                done = true;
            }
        }
    }
}

