using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP;


namespace RealSolarSystem
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class PlanetariumCameraFixer : MonoBehaviour
    {
        public void Start()
        {
            if (HighLogic.LoadedSceneHasPlanetarium && PlanetariumCamera.fetch)
            {
                print("*RSS* Fixing PCam. Min " + PlanetariumCamera.fetch.minDistance + ", Max " + PlanetariumCamera.fetch.maxDistance + ". Start " + PlanetariumCamera.fetch.startDistance + ", zoom " + PlanetariumCamera.fetch.zoomScaleFactor);
                PlanetariumCamera.fetch.maxDistance = 1e10f;
                print("Fixed. Min " + PlanetariumCamera.fetch.minDistance + ", Max " + PlanetariumCamera.fetch.maxDistance + ". Start " + PlanetariumCamera.fetch.startDistance + ", zoom " + PlanetariumCamera.fetch.zoomScaleFactor);
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class SolarPanelFixer : MonoBehaviour
    {
        public static bool fixedSolar = false;
        public void Start()
        {
            if (!fixedSolar && HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER))
            {
                fixedSolar = true;
                print("*RSS* Fixing Solar Panels");
                if (PartLoader.LoadedPartsList != null)
                {
                    foreach (AvailablePart ap in PartLoader.LoadedPartsList)
                    {
                        try
                        {
                            if (ap.partPrefab != null)
                            {
                                if (ap.partPrefab.Modules.Contains("ModuleDeployableSolarPanel"))
                                {
                                    ModuleDeployableSolarPanel sp = (ModuleDeployableSolarPanel)(ap.partPrefab.Modules["ModuleDeployableSolarPanel"]);
                                    /*ConfigNode node = new ConfigNode();
                                    sp.powerCurve.Save(node);
                                    foreach (ConfigNode.Value k in node.values)
                                    {
                                        string[] val = k.value.Split(' ');
                                        val[0] = (double.Parse(val[0]) * 11).ToString();
                                        string retval = "";
                                        foreach (string s in val)
                                            retval += s + " ";
                                        k.value = retval;
                                    }*/
                                    ConfigNode node = new ConfigNode("powerCurve");
                                    node.AddValue("key", "0 223.8 0 -.5");
                                    node.AddValue("key", "5.79e10 6.6736 -0.5 -0.5");
                                    node.AddValue("key", "1.08e11 1.9113 -0.5 -0.5");
                                    node.AddValue("key", "1.49e11 1.0 -0.1 -0.1");
                                    node.AddValue("key", "2.28e11 0.431 -.03 -.03");
                                    node.AddValue("key", "7.79e11 0.037 -.01 -.001");
                                    node.AddValue("key", "5.87e12 0 -0.001 0");

                                    sp.powerCurve = new FloatCurve();
                                    sp.powerCurve.Load(node);
                                    print("Fixed " + ap.name + " (" + ap.title + ")");
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        EditorPartList.Instance.Refresh();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class SSTChecker : MonoBehaviour
    {
        public double lastTime = 0;
        public double currentTime = 0;
        
        public void Update()
        {
            if(ScaledSpace.Instance == null)
                return;
            //currentTime = Planetarium.GetUniversalTime();
            //if(currentTime > lastTime + 1)
            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftAlt))
            {
                print("*SST* Printing Scaled Space transforms");
                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    Utils.DumpSST(t);
                }
                foreach (ScaledSpaceFader ssf in Resources.FindObjectsOfTypeAll(typeof(ScaledSpaceFader)))
                    Utils.DumpSSF(ssf);
            }
            /*if(HighLogic.LoadedSceneHasPlanetarium && PlanetariumCamera.fetch)
                PlanetariumCamera.fetch.maxDistance = 1500000000f;*/
        }
    }

    public class Utils : MonoBehaviour
    {
        public static void DumpSSF(ScaledSpaceFader ssf)
        {
            print("SSF BODY NAME: " + ssf.celestialBody.name);
            print("floatName = " + ssf.floatName);
            print("fadeStart = " + ssf.fadeStart);
            print("fadeEnd = " + ssf.fadeEnd);
        }
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

        public static void DumpCBT(PQSMod_CelestialBodyTransform c)
        {
            print("PQSM_CBT " + c.name + "(" + c.body.name + ")");
            print("deactivateAltitude = " + c.deactivateAltitude);
            print("planetFade.fadeStart = " + c.planetFade.fadeStart);
            print("planetFade.fadeEnd = " + c.planetFade.fadeEnd);
            print("planetFade.valueStart = " + c.planetFade.valueStart);
            print("planetFade.valueEnd = " + c.planetFade.valueEnd);
            int i = 0;
            if (c.secondaryFades != null)
            {
                foreach (PQSMod_CelestialBodyTransform.AltitudeFade af in c.secondaryFades)
                {
                    print("Secondary" + i + ".fadeStart = " + af.fadeStart);
                    print("Secondary" + i + ".fadeEnd = " + af.fadeEnd);
                    i++;
                }
            }
        }



        public static void DumpSST(Transform t)
        {
            print("Transform  = " + t.name);
            print("Scale = " + t.localScale.x + ", " + t.localScale.y + ", " + t.localScale.z);
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
    }

    public class CBRotation
    {
        string name;
        double axialTilt;
        double rotationPeriod;
        double initialRotation;

        public CBRotation(string newName, double newTilt, double newRotPeriod, double newInitialRot)
        {
            MonoBehaviour.print("*RSS* Adding override rotation for " + newName + ": Axial tilt " + newTilt + ", period " + newRotPeriod + ", initial " + newInitialRot);
            name = newName;
            axialTilt = newTilt;
            rotationPeriod = newRotPeriod;
            initialRotation = newInitialRot;
        }
        public QuaternionD Tilt()
        {
            return QuaternionD.AngleAxis(-axialTilt, Vector3d.back);
        }
        public double AngleAtTime(double time)
        {
            return (initialRotation + 360.0 / rotationPeriod * time) % 360.0;
        }
        public QuaternionD TiltedAngleAtTime(double time)
        {
            return TiltedAngle(AngleAtTime(time));
        }
        public QuaternionD TiltedAngle(double angle)
        {
            QuaternionD qAngle = QuaternionD.AngleAxis(angle, Vector3d.down);
            return Tilt() * qAngle;
        }
        public Vector3d AngularVelocity()
        {
            return Tilt() * Vector3d.down * (Math.PI * 2 / rotationPeriod);
        }
        public Vector3d zUpAngularVelocity()
        {
            return Tilt() * Vector3d.back * (Math.PI * 2 / rotationPeriod);
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class CBRotationFixer : MonoBehaviour
    {
        public static Dictionary<string, CBRotation> CBRotations = new Dictionary<string, CBRotation>();

        // call this on the sun. Will update recursively all children
        // only touches planets with rotations defined in CBRotations
        // and ignores others. Does not support inverseRotation.
        public void UpdateBody(CelestialBody body)
        {
            if (CBRotations.ContainsKey(body.name))
            {
                //print("*RSS CBBR* Updating " + body.name);
                CBRotation rot = CBRotations[body.name];
                body.angularVelocity = rot.AngularVelocity();
                body.zUpAngularVelocity = rot.zUpAngularVelocity();

                body.rotationAngle = rot.AngleAtTime(Planetarium.GetUniversalTime());
                body.directRotAngle = (body.rotationAngle - Planetarium.InverseRotAngle) % 360.0;
                body.angularV = body.angularVelocity.magnitude;
                QuaternionD qAngle = rot.TiltedAngle(body.directRotAngle);
                ((MonoBehaviour)body).transform.rotation = qAngle;
                body.rotation = qAngle;
            }
            if (body.orbitDriver)
                body.orbitDriver.UpdateOrbit();

            foreach (CelestialBody child in body.orbitingBodies)
                UpdateBody(child);
        }

        public void FixedUpdate()
        {
            //print("*RSSCBR* Running FixedUpdate");
            if (HighLogic.LoadedSceneHasPlanetarium || HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER))
            {
                //print("*RSSCBR* Correct Scene");
                if (Planetarium.fetch.Sun)
                    UpdateBody(Planetarium.fetch.Sun);
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class RealSolarSystem : MonoBehaviour
    {
        public void UpdateMass(CelestialBody body)
        {
            double rsq = body.Radius * body.Radius;
            body.gMagnitudeAtCenter = body.GeeASL * 9.81 * rsq;
            body.gravParameter = rsq * body.GeeASL * 9.81;
            body.Mass = body.gravParameter * (1 / 6.674E-11);
        }

        // converts mass to Gee ASL using a body's radius.
        public static double MassToGeeASL(double mass, double radius)
        {
            return mass * (6.674E-11 / 9.81) / (radius * radius);
        }

        // thanks to asmi for this!
        public static bool kerbinMapDecalsDone = false;


        public static bool done = false;
        public void Start()
        {
            ConfigNode RSSSettings = null;
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEM"))
                    RSSSettings = node;

            if(RSSSettings == null)
                throw new UnityException("*RSS* REALSOLARSYSTEM node not found!");

            print("*RSS* Printing CBTs");
            /*foreach (PQSMod_CelestialBodyTransform c in Resources.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                Utils.DumpCBT(c);*/

            // thanks to asmi for this!
            print("*RSS* fixing Kerbin mapdecals");
            // HARDCODED FOR NOW
            // For now does only Kerbin, and rescales ALL mapdecals.
            if(!kerbinMapDecalsDone && RSSSettings.GetNode("Kerbin") != null)
            {
                var mapDecals = FindObjectsOfType(typeof(PQSMod_MapDecalTangent)).OfType<PQSMod_MapDecalTangent>();
                foreach (var mapDecal in mapDecals)
                {
                    foreach (CelestialBody b in FlightGlobals.Bodies)
                        if (b.name.Equals("Kerbin"))
                        {
                            ConfigNode knode = RSSSettings.GetNode("Kerbin");
                            double radius;
                            if (knode.HasValue("Radius") && double.TryParse(knode.GetValue("Radius"), out radius))
                            {
                                mapDecal.position *= (float)(radius / b.Radius);
                                mapDecal.radius *= (radius / b.Radius);
                            }
                            break;
                        }
                }
                kerbinMapDecalsDone = true;
            }

            print("*RSS* fixing bodies");
            foreach (ConfigNode node in RSSSettings.nodes)
            {
                foreach (CelestialBody body in FlightGlobals.fetch.bodies) //Resources.FindObjectsOfTypeAll(typeof(CelestialBody))) //in FlightGlobals.fetch.bodies)
                {
                    if (body == null)
                        continue;

                    if (body.bodyName.Equals(node.name))
                    {
                        print("Fixing CB " + node.name);
                        double dtmp;
                        float ftmp;
                        double origRadius = body.Radius;
                        if(node.HasValue("Radius"))
                        {
                            if (double.TryParse(node.GetValue("Radius"), out dtmp))
                                body.Radius = dtmp;

                        }
                        if (node.HasValue("Mass"))
                        {
                            if (double.TryParse(node.GetValue("Mass"), out dtmp))
                            {
                                body.Mass = dtmp;
                                body.GeeASL = MassToGeeASL(dtmp, body.Radius);
                            }
                        }
                        if (node.HasValue("atmosphereScaleHeight"))
                        {
                            if (double.TryParse(node.GetValue("atmosphereScaleHeight"), out dtmp))
                                body.atmosphereScaleHeight = dtmp;
                        }
                        if (node.HasValue("atmosphereMultiplier"))
                        {
                            if (float.TryParse(node.GetValue("atmosphereMultiplier"), out ftmp))
                                body.atmosphereMultiplier = ftmp;
                        }
                        if (node.HasValue("maxAtmosphereAltitude"))
                        {
                            if (float.TryParse(node.GetValue("maxAtmosphereAltitude"), out ftmp))
                                body.maxAtmosphereAltitude = ftmp;
                        }
                        if (node.HasValue("staticPressureASL"))
                        {
                            if (double.TryParse(node.GetValue("maxAtmosphereAltitude"), out dtmp))
                                body.staticPressureASL = dtmp;
                        }
                        if (node.HasValue("rotationPeriod"))
                        {
                            if (double.TryParse(node.GetValue("rotationPeriod"), out dtmp))
                                body.rotationPeriod = dtmp;
                        }
                        if (node.HasValue("initialRotation"))
                        {
                            if (double.TryParse(node.GetValue("initialRotation"), out dtmp))
                                body.initialRotation = dtmp;
                        }
                        if (node.HasValue("inverseRotation"))
                        {
                            bool btmp;
                            if (bool.TryParse(node.GetValue("inverseRotation"), out btmp))
                                body.inverseRotation = btmp;
                        }
                        UpdateMass(body);

                        /*if (node.HasValue("axialTilt"))
                        {
                            if (!body.inverseRotation && double.TryParse(node.GetValue("axialTilt"), out dtmp))
                            {
                                CBRotationFixer.CBRotations.Add(body.name, new CBRotation(body.name, dtmp, body.rotationPeriod, body.initialRotation));
                                body.rotationPeriod = 0;
                            }
                        }*/


                        ConfigNode onode = node.GetNode("Orbit");
                        if (body.orbitDriver != null && body.orbit != null && onode != null)
                        {
                            if (onode.HasValue("semiMajorAxis"))
                            {
                                if (double.TryParse(onode.GetValue("semiMajorAxis"), out dtmp))
                                    body.orbit.semiMajorAxis = dtmp;
                            }
                            if (onode.HasValue("eccentricity"))
                            {
                                if (double.TryParse(onode.GetValue("eccentricity"), out dtmp))
                                    body.orbit.eccentricity = dtmp;
                            }
                            if (onode.HasValue("meanAnomaly"))
                            {
                                if (double.TryParse(onode.GetValue("meanAnomaly"), out dtmp))
                                    body.orbit.meanAnomaly = dtmp;
                            }

                            if (onode.HasValue("inclination"))
                            {
                                if (double.TryParse(onode.GetValue("inclination"), out dtmp))
                                    body.orbit.inclination = dtmp;
                            }
                            if (onode.HasValue("period"))
                            {
                                if (double.TryParse(onode.GetValue("period"), out dtmp))
                                    body.orbit.period = dtmp;
                            }
                            if (onode.HasValue("LAN"))
                            {
                                if (double.TryParse(onode.GetValue("LAN"), out dtmp))
                                    body.orbit.LAN = dtmp;
                            }
                            if (onode.HasValue("argumentOfPeriapsis"))
                            {
                                if (double.TryParse(onode.GetValue("argumentOfPeriapsis"), out dtmp))
                                    body.orbit.argumentOfPeriapsis = dtmp;
                            }
                            if (onode.HasValue("referenceBody"))
                            {
                                string bodyname = onode.GetValue("referenceBody");
                                if (body.orbit.referenceBody == null || !body.orbit.referenceBody.Equals(bodyname))
                                {
                                    foreach (CelestialBody b in FlightGlobals.Bodies)
                                    {
                                        if (b.name.Equals(bodyname))
                                        {
                                            if (body.orbit.referenceBody)
                                            {
                                                body.orbit.referenceBody.orbitingBodies.Remove(body);
                                            }
                                            b.orbitingBodies.Add(body);
                                            body.orbit.referenceBody = b;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (body.orbitDriver != null)
                        {
                            if(body.referenceBody != null)
                            {
                                body.hillSphere = body.orbit.semiMajorAxis * (1.0 - body.orbit.eccentricity) * Math.Pow(body.Mass / body.orbit.referenceBody.Mass, 1/3);
                                body.sphereOfInfluence = body.orbit.semiMajorAxis * Math.Pow(body.Mass / body.orbit.referenceBody.Mass, 0.4);
                            }   
	                        else
	                        {
		                        body.sphereOfInfluence = double.PositiveInfinity;
		                        body.hillSphere = double.PositiveInfinity;
	                        }
                            body.orbitDriver.QueuedUpdate = true;
                        }
                        body.CBUpdate();

                        if (body.Radius != origRadius)
                        {
                            // Scaled space
                            if (ScaledSpace.Instance != null)
                            {
                                float SSTScale = 1.0f;
                                if(node.HasValue("SSTScale"))
                                    float.TryParse(node.GetValue("SSTScale"), out SSTScale);
                                SSTScale *= (float)(body.Radius / origRadius);
                                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                                {
                                    if (t.name.Equals(node.name))
                                        t.localScale = new Vector3(t.localScale.x * SSTScale, t.localScale.y * SSTScale, t.localScale.z * SSTScale);
                                }
                            }

                            // Scaled space fader
                            float SSFMult = (float)(body.Radius / origRadius);
                            float SSFStart = -1, SSFEnd = -1;
                            if (node.HasValue("SSFStart"))
                            {
                                if (float.TryParse(node.GetValue("SSFStart"), out ftmp))
                                    SSFStart = ftmp;
                            }
                            if (node.HasValue("SSFEnd"))
                            {
                                if (float.TryParse(node.GetValue("SSFEnd"), out ftmp))
                                    SSFEnd = ftmp;
                            }
                            if (node.HasValue("SSFMult"))
                            {
                                if (float.TryParse(node.GetValue("SSFMult"), out ftmp))
                                    SSFMult = ftmp;
                            }

                            foreach (ScaledSpaceFader ssf in Resources.FindObjectsOfTypeAll(typeof(ScaledSpaceFader)))
                            {
                                if (ssf.celestialBody != null)
                                {
                                    if (ssf.celestialBody.name.Equals(node.name))
                                    {
                                        if (SSFStart > 0)
                                            ssf.fadeStart = SSFStart;
                                        else
                                            ssf.fadeStart *= SSFMult;

                                        if (SSFEnd > 0)
                                            ssf.fadeEnd = SSFEnd;
                                        else
                                            ssf.fadeEnd *= SSFMult;
                                    }
                                }
                            }
                            // The CBT that fades out the PQS
                            foreach (PQSMod_CelestialBodyTransform c in Resources.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                            {
                                try
                                {
                                    if (c.body != null)
                                    {
                                        if (c.body.name.Equals(node.name))
                                        {
                                            if (node.HasValue("PQSdeactivateAltitude"))
                                            {
                                                if (double.TryParse(node.GetValue("PQSdeactivateAltitude"), out dtmp))
                                                    c.deactivateAltitude = dtmp;
                                            }
                                            if (c.planetFade != null)
                                            {
                                                if (node.HasValue("PQSfadeStart"))
                                                {
                                                    if (float.TryParse(node.GetValue("PQSfadeStart"), out ftmp))
                                                        c.planetFade.fadeStart = ftmp;
                                                }
                                                if (node.HasValue("PQSfadeEnd"))
                                                {
                                                    if (float.TryParse(node.GetValue("PQSfadeEnd"), out ftmp))
                                                        c.planetFade.fadeEnd = ftmp;
                                                }
                                                if (c.secondaryFades != null)
                                                {
                                                    foreach (PQSMod_CelestialBodyTransform.AltitudeFade af in c.secondaryFades)
                                                    {
                                                        if (node.HasValue("PQSSecfadeStart"))
                                                        {
                                                            if (float.TryParse(node.GetValue("PQSSecfadeStart"), out ftmp))
                                                                af.fadeStart = ftmp;
                                                        }
                                                        if (node.HasValue("PQSSecfadeEnd"))
                                                        {
                                                            if (float.TryParse(node.GetValue("PQSSecfadeEnd"), out ftmp))
                                                                af.fadeEnd = ftmp;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }

                            // the Planet Quadtree Sphere
                            double PQSScaleFactor = 1.0;
                            // does nothing :(
                            // have to edit the PQSMods.
                            /*if (node.HasValue("PQSScaleFactor"))
                            {
                                if (double.TryParse(node.GetValue("PQSScaleFactor"), out dtmp))
                                    PQSScaleFactor = dtmp;
                            }*/
                            foreach (PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS)))
                            {
                                if (p.name.Equals(node.name))
                                {
                                    p.circumference = body.Radius * 2 * Math.PI;
                                    /*if (p.radius != body.Radius) // already tested above!
                                    {*/
                                    p.radiusMax = (p.radiusMax - p.radius) * PQSScaleFactor + body.Radius;
                                    p.radiusMin = (p.radiusMin - p.radius) * PQSScaleFactor + body.Radius;
                                    //p.radiusDelta = p.radiusMax - p.radiusMin;
                                    p.radiusDelta *= PQSScaleFactor;
                                    //}
                                    p.radius = body.Radius;
                                    p.radiusSquared = body.Radius * body.Radius;
                                    try
                                    {
                                        p.RebuildSphere();
                                    }
                                    catch
                                    {
                                    }
                                }
                                else if (p.name.Equals(node.name + "Ocean"))
                                {
                                    p.radius = body.Radius;
                                    try
                                    {
                                        p.RebuildSphere();
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            foreach (AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                            {
                                if (ag != null && ag.planet != null)
                                {
                                    if (ag.planet.name.Equals(node.name))
                                    {
                                        print("Found atmo for " + node.name + ": " + ag.name);
                                        ag.outerRadius = (float)body.Radius * 1.025f * ScaledSpace.InverseScaleFactor;
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
                        }
                    }
                }
            }
        }
    }
}

