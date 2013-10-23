using System;
using System.Collections.Generic;
using UnityEngine;
using KSP;


namespace RealSolarSystem
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
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
                DumpPQS(body.pqsController);
            }
        }
        public static void DumpPQS(PQS pqs)
        {
            // bool
            print("PQS " + pqs.name);
            print("buildTangents = " + pqs.buildTangents);
            print("isActive = " + pqs.isActive);
            print("isAlive = " + pqs.isAlive);
            print("isBuildingMaps = " + pqs.isBuildingMaps);
            print("isDisabled = " + pqs.isDisabled);
            print("isStarted = " + pqs.isStarted);
            print("isSubdivisionEnabled = " + pqs.isSubdivisionEnabled);
            print("isThinking = " + pqs.isThinking);
            print("quadAllowBuild = " + pqs.quadAllowBuild);
            print("surfaceRelativeQuads = " + pqs.surfaceRelativeQuads);
            print("useCustomNormals = " + pqs.useCustomNormals);
            print("useQuadUV = " + pqs.useQuadUV);
            print("useSharedMaterial = " + pqs.useSharedMaterial);
            print("circumference = " + pqs.circumference);
            // double
            print("collapseAltitudeMax = " + pqs.collapseAltitudeMax);
            print("collapseAltitudeValue = " + pqs.collapseAltitudeValue);
            print("collapseDelta = " + pqs.collapseDelta);
            print("collapseSeaLevelValue = " + pqs.collapseSeaLevelValue);
            print("collapseThreshold = " + pqs.collapseThreshold);
            print("detailAltitudeMax = " + pqs.detailAltitudeMax);
            print("detailAltitudeQuads = " + pqs.detailAltitudeQuads);
            print("detailDelta = " + pqs.detailDelta);
            print("detailRad = " + pqs.detailRad);
            print("detailSeaLevelQuads = " + pqs.detailSeaLevelQuads);
            print("horizonAngle = " + pqs.horizonAngle);
            print("horizonDistance = " + pqs.horizonDistance);
            print("mapMaxHeight = " + pqs.mapMaxHeight);
            print("mapOceanHeight = " + pqs.mapOceanHeight);
            print("maxDetailDistance = " + pqs.maxDetailDistance);
            print("minDetailDistance = " + pqs.minDetailDistance);
            print("radius = " + pqs.radius);
            print("radiusDelta = " + pqs.radiusDelta);
            print("radiusMax = " + pqs.radiusMax);
            print("radiusMin = " + pqs.radiusMin);
            print("radiusSquared = " + pqs.radiusSquared);
            print("subdivisionThreshold = " + pqs.subdivisionThreshold);
            print("sx = " + pqs.sx);
            print("sy = " + pqs.sy);
            print("targetHeight = " + pqs.targetHeight);
            print("targetSpeed = " + pqs.targetSpeed);
            print("visibleAltitude = " + pqs.visibleAltitude);
            print("visibleRadius = " + pqs.visibleRadius);
            print("visRad = " + pqs.visRad);
            print("visRadAltitudeMax = " + pqs.visRadAltitudeMax);
            print("visRadAltitudeValue = " + pqs.visRadAltitudeValue);
            print("visRadDelta = " + pqs.visRadDelta);
            print("visRadSeaLevelValue = " + pqs.visRadSeaLevelValue);
            print("parentSphere = " + pqs.parentSphere);
            print("****************************************");
        }
        public static bool done = false;

        public static void DumpSST(Transform t)
        {
            print("Transform  = " + t.name);
            print("Scale = " + t.localScale + "; lossyScale = " + t.lossyScale);
            print("Pos = " + t.position + "; lPos = " + t.localPosition);
        }
        public void Start()
        {
            if (!HighLogic.LoadedScene.Equals(GameScenes.MAINMENU) || done)
                return;
            done = true;

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
                    body.maxAtmosphereAltitude = 135;
                    //body.sphereOfInfluence = 84159286.4796305 * 35; // guessing at scale factor?

                    foreach (PQS p in PQS.FindObjectsOfTypeAll(typeof(PQS)))
                    {
                        if (p.name.Equals("Kerbin"))
                        {
                            DumpPQS(p);
                            p.circumference = radius * 2 * Math.PI + 0.17;
                            p.radiusMax = p.radiusMax - p.radius + radius;
                            p.radiusMin = p.radiusMin - p.radius + radius;
                            p.radius = radius;
                            p.radiusSquared = radius * radius;
                            p.RebuildSphere();
                        }
                        else if (p.name.Equals("KerbinOcean"))
                        {
                            DumpPQS(p);
                            p.radius = radius;
                            p.RebuildSphere();
                        }

                    }

                    //body.orbit.semiMajorAxis = 147098290000;
                    //Vessel v;
                    body.orbitDriver.orbit.semiMajorAxis = 147098290000;
                    body.orbitDriver.orbit.eccentricity = 0.01671123;
                    body.orbitDriver.orbit.meanAnomaly = 357.51716;
                    //body.orbitDriver.orbit.inclination = 1.57869;
                    body.orbitDriver.orbit.LAN = 348.73936;
                    body.orbitDriver.orbit.argumentOfPeriapsis = 114.20783;

                   
                    /*foreach (PQSMod_CelestialBodyTransform cbt in PQSMod_CelestialBodyTransform.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                    {
                        if (cbt.body != null)
                            if (cbt.body.name.Equals("Kerbin"))
                                cbt.deactivateAltitude) = cbt.deactivateAltitude * radius / 600; // thanks, Kragrathea!
                    }*/
                }
                if (body.bodyName.Equals("Mun"))
                {
                    double rotPeriod = 27.321582*24*60*60; // 23hr, 56min, and 4.1s
                    double mass = 7.3477e22;
                    double radius = 1737100;
                    body.Radius = radius;
                    //body.atmosphereScaleHeight = 7.5;
                    body.rotationPeriod = rotPeriod;
                    body.angularV = 2 * Math.PI / rotPeriod;
                    body.angularVelocity = new Vector3d(0, -body.angularV, 0);
                    body.GeeASL = 0.1654;
                    body.gravParameter = 6.673 * mass;
                    body.gMagnitudeAtCenter = 9.81 * body.GeeASL * radius * radius;
                    //body.maxAtmosphereAltitude = 135;
                    //body.sphereOfInfluence = 84159286.4796305 * 35; // guessing at scale factor?

                    foreach (PQS p in PQS.FindObjectsOfTypeAll(typeof(PQS)))
                    {
                        if (p.name.Equals("Mun"))
                        {
                            DumpPQS(p);
                            p.circumference = radius * 2 * Math.PI;
                            p.radiusMax = p.radiusMax - p.radius + radius;
                            p.radiusMin = p.radiusMin - p.radius + radius;
                            p.radius = radius;
                            p.radiusSquared = radius * radius;
                            p.RebuildSphere();
                        }

                    }

                    //body.orbit.semiMajorAxis = 147098290000;
                    //Vessel v;
                    body.orbitDriver.orbit.semiMajorAxis = 384399000 ;
                    body.orbitDriver.orbit.eccentricity = 0.0549;
                    //body.orbitDriver.orbit.meanAnomaly = 357.51716;
                    body.orbitDriver.orbit.inclination = 5.145;
                    //body.orbitDriver.orbit.LAN = 348.73936;
                    //body.orbitDriver.orbit.argumentOfPeriapsis = 114.20783;


                    /*foreach (PQSMod_CelestialBodyTransform cbt in PQSMod_CelestialBodyTransform.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                    {
                        if (cbt.body != null)
                            if (cbt.body.name.Equals("Kerbin"))
                                cbt.deactivateAltitude) = cbt.deactivateAltitude * radius / 600; // thanks, Kragrathea!
                    }*/
                }
            }
            print("##############################################");
            /*foreach (PQSMod_CelestialBodyTransform cbt in PQSMod_CelestialBodyTransform.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
            {
                if (cbt.body != null)
                    print("CBT " + cbt.body.name + " dA = " + cbt.deactivateAltitude);
            }*/
            foreach (PQSMod_AltitudeAlpha aa in PQSMod_AltitudeAlpha.FindObjectsOfTypeAll(typeof(PQSMod_AltitudeAlpha)))
            {
                //if (aa.body != null)
                if(aa.sphere != null)
                    print("AA " + aa.name + " attach " + aa.sphere.name + ", aD = " + aa.atmosphereDepth);
            }
            print("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            foreach (PQS p in PQS.FindObjectsOfTypeAll(typeof(PQS)))
            {
                DumpPQS(p);
            }
            print("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
            foreach (var g in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (((GameObject)g).name.ToLower().Contains("sky") || ((GameObject)g).name.ToLower().Contains("atmo"))
                    print("Found " + ((GameObject)g).name + ", type " + g.GetType());
            }
            
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
            print("SS Transforms");
            i = 0;
            if (ScaledSpace.Instance != null)
            {
                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    DumpSST(t);
                    float scale = 0.1f * 6371.0f / 600.0f;
                    if (t.name.Equals("Kerbin"))
                        t.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
        public static bool redone = false;
        public static CelestialBody tgtbody = null;
        void OnGUI()
        {
            if (HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION) && !redone)
            {
                if (tgtbody != (MapView.MapCamera.target).celestialBody)
                {
                    tgtbody = (MapView.MapCamera.target).celestialBody;
                    print("++++++++++++++++++++++++++++++++++++++++++++++++++");
                    foreach (CelestialBody body in FlightGlobals.fetch.bodies)
                    {
                        if (body == null)
                            continue;
                        if (body.bodyName.Equals("Kerbin"))
                            DumpBody(body);
                    }
                    print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    if (ScaledSpace.Instance != null)
                    {
                        foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                        {
                            DumpSST(t);
                        }
                    }
                }
                //redone = true;
            }
        }
    }
}

