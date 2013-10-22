using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;


namespace RealSolarSystem
{
	[KSPAddon(KSPAddon.Startup.MainMenu, false)]
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
            print("pqsController = " + body.pqsController);
            print("terrainController = " + body.terrainController);
            /*if (body.terrainController != null)
            {
                print("PQSController: ");
                print("circ = " + body.terrainController.circ);
                print("horizonAngle = " + body.terrainController.horizonAngle);
                print("horizonDistance = " + body.terrainController.horizonDistance);
                print("parameterScaleFactor = " + body.terrainController.parameterScaleFactor);
                print("quadSize = " + body.terrainController.quadSize);
                print("radius = " + body.terrainController.radius);
                print("sphereColliderRadiusOffset = " + body.terrainController.sphereColliderRadiusOffset);
                print("visibleRadius = " + body.terrainController.visibleRadius);
                print("waterLevel = " + body.terrainController.waterLevel);
                print("waterThreshold = " + body.terrainController.waterThreshold);
            }*/
            if (body.pqsController != null)
            {
                // bool
                print("buildTangents = " + body.pqsController.buildTangents);
                print("isActive = " + body.pqsController.isActive);
                print("isAlive = " + body.pqsController.isAlive);
                print("isBuildingMaps = " + body.pqsController.isBuildingMaps);
                print("isDisabled = " + body.pqsController.isDisabled);
                print("isStarted = " + body.pqsController.isStarted);
                print("isSubdivisionEnabled = " + body.pqsController.isSubdivisionEnabled);
                print("isThinking = " + body.pqsController.isThinking);
                print("quadAllowBuild = " + body.pqsController.quadAllowBuild);
                print("surfaceRelativeQuads = " + body.pqsController.surfaceRelativeQuads);
                print("useCustomNormals = " + body.pqsController.useCustomNormals);
                print("useQuadUV = " + body.pqsController.useQuadUV);
                print("useSharedMaterial = " + body.pqsController.useSharedMaterial);
                print("circumference = " + body.pqsController.circumference);
                // double
                print("collapseAltitudeMax = " + body.pqsController.collapseAltitudeMax);
                print("collapseAltitudeValue = " + body.pqsController.collapseAltitudeValue);
                print("collapseDelta = " + body.pqsController.collapseDelta);
                print("collapseSeaLevelValue = " + body.pqsController.collapseSeaLevelValue);
                print("collapseThreshold = " + body.pqsController.collapseThreshold);
                print("detailAltitudeMax = " + body.pqsController.detailAltitudeMax);
                print("detailAltitudeQuads = " + body.pqsController.detailAltitudeQuads);
                print("detailDelta = " + body.pqsController.detailDelta);
                print("detailRad = " + body.pqsController.detailRad);
                print("detailSeaLevelQuads = " + body.pqsController.detailSeaLevelQuads);
                print("horizonAngle = " + body.pqsController.horizonAngle);
                print("horizonDistance = " + body.pqsController.horizonDistance);
                print("mapMaxHeight = " + body.pqsController.mapMaxHeight);
                print("mapOceanHeight = " + body.pqsController.mapOceanHeight);
                print("maxDetailDistance = " + body.pqsController.maxDetailDistance);
                print("minDetailDistance = " + body.pqsController.minDetailDistance);
                print("radius = " + body.pqsController.radius);
                print("radiusDelta = " + body.pqsController.radiusDelta);
                print("radiusMax = " + body.pqsController.radiusMax);
                print("radiusMin = " + body.pqsController.radiusMin);
                print("radiusSquared = " + body.pqsController.radiusSquared);
                print("subdivisionThreshold = " + body.pqsController.subdivisionThreshold);
                print("sx = " + body.pqsController.sx);
                print("sy = " + body.pqsController.sy);
                print("targetHeight = " + body.pqsController.targetHeight);
                print("targetSpeed = " + body.pqsController.targetSpeed);
                print("visibleAltitude = " + body.pqsController.visibleAltitude);
                print("visibleRadius = " + body.pqsController.visibleRadius);
                print("visRad = " + body.pqsController.visRad);
                print("visRadAltitudeMax = " + body.pqsController.visRadAltitudeMax);
                print("visRadAltitudeValue = " + body.pqsController.visRadAltitudeValue);
                print("visRadDelta = " + body.pqsController.visRadDelta);
                print("visRadSeaLevelValue = " + body.pqsController.visRadSeaLevelValue);
                print("parentSphere = " + body.pqsController.parentSphere);
                print("****************************************");
            }
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

                    body.pqsController.circumference = radius * 2 * Math.PI + 0.17;
                    body.pqsController.radiusMax = body.pqsController.radiusMax - body.pqsController.radius + radius;
                    body.pqsController.radiusMin = body.pqsController.radiusMin - body.pqsController.radius + radius;
                    body.pqsController.radius = radius;
                    body.pqsController.radiusSquared = radius * radius;
                    body.pqsController.RebuildSphere();
                }
            }
            print("##############################################");
            // Seems to be obsolete
            /*int i = 0;
            foreach(Planet p in Resources.FindObjectsOfTypeAll(typeof(Planet)))
            {
                print("Planet " + i);
                print("radius = " + p.radius);
                print("planetariumRadius = " + p.planetariumRadius);
            }*/
            // Seems to be obsolete
            int i = 0;

            //UnityEngine.Object[] scbodies = Resources.FindObjectsOfTypeAll(typeof(ScaledCelestialBody));
            ScaledCelestialBody[] scbodies = (ScaledCelestialBody[])ScaledCelestialBody.FindObjectsOfType(typeof(ScaledCelestialBody));
            print("Num SCB = " + scbodies.Length);
            foreach(ScaledCelestialBody b in scbodies)
            {
                print("SCB " + i);
                print("name = " + b.name);
                print("tgtname = " + b.tgtBody.name);
                print("scale = " + b.transform.localScale);
            }

        }
        public static bool done = false;
        void OnGUI()
        {
            if (HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION) && !done)
            {
                print("++++++++++++++++++++++++++++++++++++++++++++++++++");
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

