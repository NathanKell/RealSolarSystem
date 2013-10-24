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
            print("*Pressure curve:");
            dumpKeys(body.pressureCurve);
            print("*Temperature curve:");
            dumpKeys(body.temperatureCurve);

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
            /*if (body.pqsController != null)
            {
                DumpPQS(body.pqsController);
            }*/
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

        public static void dumpKeys(AnimationCurve c)
        {
            if (c == null)
                print("NULL");
            else if (c.keys.Length == 0)
                print("NO KEYS");
            else
                for (int i = 0; i < c.keys.Length; i++)
                    print("key," + i + " = " + c.keys[i].time + " " + c.keys[i].value + " " + c.keys[i].inTangent + " " + c.keys[i].outTangent);

        }
        public static bool MMyet = false;
        public double lastTime = 0;
        public double currentTime = 0;
        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            currentTime = Planetarium.GetUniversalTime();
            if(currentTime > lastTime + 1)
            {
                lastTime = currentTime;
                print("**RSS* checking atmo");
                print("Drag mult = " + FlightGlobals.DragMultiplier);
                print("FG static plain = " + FlightGlobals.getStaticPressure());
                print("FG static pos(" +  FlightGlobals.ship_position + "), altatpos " + FlightGlobals.getAltitudeAtPos(FlightGlobals.ship_position) + ", = " + FlightGlobals.getStaticPressure(FlightGlobals.ship_position));
                print("FG static alt(" +  FlightGlobals.ship_altitude + ") = " + FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude));
                print("FG: current CB = " + FlightGlobals.currentMainBody);
                CelestialBody body = FlightGlobals.currentMainBody;
                if(body != null)
                    print("Should be: " + Math.Pow(body.staticPressureASL * 2.71828183, -(FlightGlobals.ship_altitude / 1000.0 / body.atmosphereScaleHeight)));
                if(FlightGlobals.ActiveVessel != null)
                {
                    foreach(Part p in FlightGlobals.ActiveVessel.Parts)
                    {
                        print("Part " + p.name + " has static " + p.staticPressureAtm + " dyn " + p.dynamicPressureAtm);
                    }
                }
                print("--------------------------------");
            }
        }
        public void Start()
        {
            if (HighLogic.LoadedScene.Equals(GameScenes.MAINMENU))
            {
                MMyet = true;
                done = false;
            }
            if (!MMyet)
                return;

            print("*RSS* fixing bodies");
            print("SS Transforms");
            if (ScaledSpace.Instance != null)
            {
                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    if(!done) DumpSST(t);
                }
            }
            print("/////////////////////////////");
            print("BODIES");
            foreach (CelestialBody body in Resources.FindObjectsOfTypeAll(typeof(CelestialBody))) //in FlightGlobals.fetch.bodies)
            {
                if (body == null)
                    continue;
                if(!done) DumpBody(body);
                
                if(body.bodyName.Equals("Kerbin")) //&& HighLogic.LoadedScene.Equals(GameScenes.MAINMENU))
                {
                    print("Fixing Kerbin");
                    double rotPeriod = 86164.1; // 23hr, 56min, and 4.1s
                    double mass = 5.97219e24;
                    double radius = 6371000;
                    double sma = 147098290000;
                    float SSscale = 0.1f * (float)radius / 600000f;

                    body.Radius = radius;
                    body.atmosphereScaleHeight = 7.5;
                    body.rotationPeriod = rotPeriod;
                    //body.atmosphereMultiplier = body.atmosphereMultiplier * (float)radius / 600000f;
                    body.angularV = 2 * Math.PI / rotPeriod;
                    body.angularVelocity = new Vector3d(0, -body.angularV, 0);
                    body.GeeASL = 1;
                    body.gravParameter = 6.673 * mass;
                    body.Mass = mass;
                    body.gMagnitudeAtCenter = 9.81 * radius * radius;
                    body.maxAtmosphereAltitude = 105; //doesn't work to have it not be scale height. 135;
                    body.sphereOfInfluence = sma * Math.Pow(3.00246E-06, 0.4);
                    if (body.orbitDriver != null && body.orbitDriver.orbit != null)
                    {
                        body.orbitDriver.orbit.semiMajorAxis = sma;
                        body.orbitDriver.orbit.eccentricity = 0.01671123;
                        body.orbitDriver.orbit.meanAnomaly = 357.51716;
                        //body.orbitDriver.orbit.inclination = 1.57869;
                        body.orbitDriver.orbit.LAN = 348.73936;
                        body.orbitDriver.orbit.argumentOfPeriapsis = 114.20783;
                        body.hillSphere = body.orbitDriver.orbit.semiMajorAxis * (1.0 - body.orbitDriver.orbit.eccentricity) * Math.Pow(3.00246E-06, 1.0 / 3.0);
                        body.orbitDriver.QueuedUpdate = true;
                    }
                    try
                    {
                        body.CBUpdate();
                    }
                    catch
                    {
                    }
                    if (ScaledSpace.Instance != null)
                    {
                        foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                        {

                            if (t.name.Equals("Kerbin"))
                                t.localScale = new Vector3(SSscale, SSscale, SSscale);
                        }
                    }
                    foreach (PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS)))
                    {
                        if (p.name.Equals("Kerbin"))
                        {
                            if(!done) DumpPQS(p);
                            p.circumference = radius * 2 * Math.PI + 0.17;
                            p.radiusMax = p.radiusMax - p.radius + radius;
                            p.radiusMin = p.radiusMin - p.radius + radius;
                            p.radius = radius;
                            p.radiusSquared = radius * radius;
                            p.RebuildSphere();
                        }
                        else if (p.name.Equals("KerbinOcean"))
                        {
                            if(!done) DumpPQS(p);
                            p.radius = radius;
                            p.RebuildSphere();
                        }

                    }
                    foreach(AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                    {
                        if(ag != null && ag.planet != null)
                        {
                            if(ag.planet.name.Equals("Kerbin"))
                            {
                                print("Found atmo for Kebin: " + ag.name);
                                ag.outerRadius = (float)radius * 1.025f * ScaledSpace.InverseScaleFactor;
                                ag.outerRadius2 = ag.outerRadius * ag.outerRadius;
                                ag.innerRadius = ag.outerRadius * 0.975f;
                                ag.innerRadius2 = ag.innerRadius * ag.innerRadius;
                                ag.scale = 1f / (ag.outerRadius - ag.innerRadius);
                                ag.scaleDepth = -0.25f;
                                ag.scaleOverScaleDepth = ag.scale / ag.scaleDepth;
                                print("Atmo updated");
                            }
                        }
                    }
                   
                    /*foreach (PQSMod_CelestialBodyTransform cbt in PQSMod_CelestialBodyTransform.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                    {
                        if (cbt.body != null)
                            if (cbt.body.name.Equals("Kerbin"))
                                cbt.deactivateAltitude) = cbt.deactivateAltitude * radius / 600; // thanks, Kragrathea!
                    }*/
                }
                if (body.bodyName.Equals("Mun"))// && HighLogic.LoadedScene.Equals(GameScenes.MAINMENU))
                {
                    print("Fixing Mun");
                    double rotPeriod = 27.321582*24*60*60; // 23hr, 56min, and 4.1s
                    double mass = 7.3477e22;
                    double radius = 1737100;
                    double sma = 384399000;
                    float SSscale = 0.3f * (float)radius / 200000f; //(float)radius * 0.1f / 600000f;

                    body.Radius = radius;
                    //body.atmosphereScaleHeight = 7.5;
                    body.rotationPeriod = rotPeriod;
                    body.angularV = 2 * Math.PI / rotPeriod;
                    body.angularVelocity = new Vector3d(0, -body.angularV, 0);
                    body.GeeASL = 0.1654;
                    body.gravParameter = 6.673 * mass;
                    body.gMagnitudeAtCenter = 9.81 * body.GeeASL * radius * radius;
                    body.Mass = mass;
                    //body.maxAtmosphereAltitude = 135;
                    body.sphereOfInfluence = sma * Math.Pow(0.012303192, 0.4);
                    if (body.orbitDriver != null && body.orbitDriver.orbit != null)
                    {
                        body.orbitDriver.orbit.semiMajorAxis = sma;
                        body.orbitDriver.orbit.eccentricity = 0.0549;
                        body.orbitDriver.orbit.inclination = 5.145;
                        body.hillSphere = body.orbitDriver.orbit.semiMajorAxis * (1.0 - body.orbitDriver.orbit.eccentricity) * Math.Pow(0.012303192, 1.0 / 3.0);
                        body.orbitDriver.QueuedUpdate = true;
                    }
                    try
                    {
                        body.CBUpdate();
                    }
                    catch
                    {
                    }
                    if (ScaledSpace.Instance != null)
                    {
                        foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                        {

                            if (t.name.Equals("Mun"))
                                t.localScale = new Vector3(SSscale, SSscale, SSscale);
                        }
                    }
                    foreach (PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS)))
                    {
                        if (p.name.Equals("Mun"))
                        {
                            if(!done) DumpPQS(p);
                            p.circumference = radius * 2 * Math.PI;
                            p.radiusMax = p.radiusMax - p.radius + radius;
                            p.radiusMin = p.radiusMin - p.radius + radius;
                            p.radius = radius;
                            p.radiusSquared = radius * radius;
                            p.RebuildSphere();
                        }

                    }
                    foreach(AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                    {
                        if(ag !=null && ag.planet != null)
                        {
                            if (ag.planet.name != null && ag.planet.name.Equals("Mun"))
                            {
                                print("Found atmo for Mun: " + ag.name);
                                ag.outerRadius = (float)radius * 1.025f * ScaledSpace.InverseScaleFactor;
                                ag.outerRadius2 = ag.outerRadius * ag.outerRadius;
                                ag.innerRadius = ag.outerRadius * 0.975f;
                                ag.innerRadius2 = ag.innerRadius * ag.innerRadius;
                                ag.scale = 1f / (ag.outerRadius - ag.innerRadius);
                                ag.scaleDepth = -0.25f;
                                ag.scaleOverScaleDepth = ag.scale / ag.scaleDepth;
                                print("Atmo updated");
                            }
                        }
                    }


                    /*foreach (PQSMod_CelestialBodyTransform cbt in PQSMod_CelestialBodyTransform.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                    {
                        if (cbt.body != null)
                            if (cbt.body.name.Equals("Kerbin"))
                                cbt.deactivateAltitude) = cbt.deactivateAltitude * radius / 600; // thanks, Kragrathea!
                    }*/
                }

            }
            if(!done)
            {
                print("##############################################");
                /*foreach (PQSMod_CelestialBodyTransform cbt in PQSMod_CelestialBodyTransform.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                {
                    if (cbt.body != null)
                        print("CBT " + cbt.body.name + " dA = " + cbt.deactivateAltitude);
                }*/
                /*foreach (PQSMod_AltitudeAlpha aa in PQSMod_AltitudeAlpha.FindObjectsOfTypeAll(typeof(PQSMod_AltitudeAlpha)))
                {
                    //if (aa.body != null)
                    if(aa.sphere != null)
                        print("AA " + aa.name + " attach " + aa.sphere.name + ", aD = " + aa.atmosphereDepth);
                }*/
                print("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                foreach (PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS)))
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
                print("ScaledCelestialBody");
                foreach (ScaledCelestialBody b in Resources.FindObjectsOfTypeAll(typeof(ScaledCelestialBody)))
                {
                    print("SCB " + i);
                    print("name = " + b.name);
                    print("tgtname = " + b.tgtBody.name);
                    print("scale = " + b.transform.localScale);
                    i++;
                }
                i = 0;
                print("PQSMod_AerialPerspectiveMaterial");
                foreach(PQSMod_AerialPerspectiveMaterial apm in Resources.FindObjectsOfTypeAll(typeof(PQSMod_AerialPerspectiveMaterial)))
                {
                    print("apm " + i);
                    print("name = " + apm.name);
                    print("Sphere = " + apm.sphere.name);
                    print("Atmo Depth = " + apm.atmosphereDepth);
                    print("height fallof = " + apm.heightFalloff);
                    i++;
                }
                i = 0;
                //print("SGT_Skysphere");
                //SGT_Monobehaviour

            }
            done = true;
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

