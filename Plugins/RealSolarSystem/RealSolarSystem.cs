using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP;
//using KSP.IO;
using System.IO;


namespace RealSolarSystem
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class CameraFixer : MonoBehaviour
    {
        public void Start()
        {
            if (HighLogic.LoadedSceneHasPlanetarium && PlanetariumCamera.fetch)
            {
                print("*RSS* Fixing PCam. Min " + PlanetariumCamera.fetch.minDistance + ", Max " + PlanetariumCamera.fetch.maxDistance + ". Start " + PlanetariumCamera.fetch.startDistance + ", zoom " + PlanetariumCamera.fetch.zoomScaleFactor);
                PlanetariumCamera.fetch.maxDistance = 1e10f;
                print("Fixed. Min " + PlanetariumCamera.fetch.minDistance + ", Max " + PlanetariumCamera.fetch.maxDistance + ". Start " + PlanetariumCamera.fetch.startDistance + ", zoom " + PlanetariumCamera.fetch.zoomScaleFactor);
            }
            if (HighLogic.LoadedSceneIsEditor)
            {
                try
                {
                    ConfigNode camNode = null;
                    foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS"))
                        camNode = node;
                    if (camNode != null)
                    {
                        float ftmp;
                        foreach (VABCamera c in Resources.FindObjectsOfTypeAll(typeof(VABCamera)))
                        {
                            //print("VAB camera " + c.name + " has maxHeight = " + c.maxHeight + ", maxDistance = " + c.maxDistance + ", scrollHeight = " + c.scrollHeight);
                            if (camNode.HasValue("VABmaxHeight"))
                                if (float.TryParse(camNode.GetValue("VABmaxHeight"), out ftmp))
                                    c.maxHeight = ftmp;

                            if (camNode.HasValue("VABmaxDistance"))
                                if (float.TryParse(camNode.GetValue("VABmaxDistance"), out ftmp))
                                    c.maxDistance = ftmp;
                        }

                        foreach (SPHCamera c in Resources.FindObjectsOfTypeAll(typeof(SPHCamera)))
                        {
                            //print("SPH camera " + c.name + " has maxHeight = " + c.maxHeight + ", maxDistance = " + c.maxDistance + ", scrollHeight = " + c.scrollHeight);
                            if (camNode.HasValue("SPHmaxDistance"))
                                if (float.TryParse(camNode.GetValue("SPHmaxDistance"), out ftmp))
                                    c.maxDistance = ftmp;
                        }
                        if (camNode.HasValue("editorExtentsMult"))
                            if (float.TryParse(camNode.GetValue("editorExtentsMult"), out ftmp))
                                EditorLogic.fetch.editorBounds.extents *= ftmp; // thanks, asmi!
                    }
                }
                catch(Exception e)
                {
                    print("Camera fixing failed: " + e.Message);
                }
            }

        }
    }

    [KSPAddonFixed(KSPAddon.Startup.MainMenu, true, typeof(SolarPanelFixer))]
    public class SolarPanelFixer : MonoBehaviour
    {

        public void FixSP(ModuleDeployableSolarPanel sp, ConfigNode curveNode)
        {
            sp.powerCurve = new FloatCurve();
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
            /*ConfigNode node = new ConfigNode("powerCurve");
            node.AddValue("key", "0 223.8 0 -.5");
            node.AddValue("key", "57909100000 6.6736 -0.5 -0.5");
            node.AddValue("key", "108208000000 1.9113 -0.5 -0.5");
            node.AddValue("key", "149598261000 1.0 -0.1 -0.1");
            node.AddValue("key", "227939100000 0.431 -.03 -.03");
            node.AddValue("key", "778547200000 0.037 -.01 -.001");
            node.AddValue("key", "5874000000000 0 -0.001 0");

                                        
            sp.powerCurve.Load(node);*/
            sp.powerCurve.Load(curveNode);
        }
        public static bool fixedSolar = false;
        public void Start()
        {
            if (!fixedSolar && HighLogic.LoadedScene.Equals(GameScenes.MAINMENU))
            {
                fixedSolar = true;
                print("*RSS* Fixing Solar Panels");
                if (PartLoader.LoadedPartsList != null)
                {
                    ConfigNode curveNode = null;
                    foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS"))
                        curveNode = node.GetNode("powerCurve");
                    if (curveNode != null)
                    {
                        foreach(Part p in Resources.FindObjectsOfTypeAll(typeof(Part)))
                        {
                            try
                            {
                               if (p.Modules.Contains("ModuleDeployableSolarPanel"))
                                {
                                    ModuleDeployableSolarPanel sp = (ModuleDeployableSolarPanel)(p.Modules["ModuleDeployableSolarPanel"]);
                                    FixSP(sp, curveNode);
                                    print("Fixed " + p.name + " (" + p.partInfo.title + ")");
                                }
                            }
                            catch (Exception e)
                            {
                                print("Solar panel fixing failed for " + p.name + ": " + e.Message);
                            }
                        }
                        ConfigNode[] allParts = GameDatabase.Instance.GetConfigNodes("PART");
                        for (int j = 0; j < allParts.Count(); j++)
                        {
                            try
                            {
                                ConfigNode pNode = allParts[j];
                                if (pNode.nodes == null || pNode.nodes.Count <= 0)
                                    continue;
                                for (int i = 0; i < pNode.nodes.Count; i++)
                                {
                                    ConfigNode node = pNode.nodes[i];
                                    if (node.name.Equals("MODULE"))
                                    {
                                        if (node.HasValue("name") && node.GetValue("name").Equals("ModuleDeployableSolarPanel"))
                                        {
                                            node.RemoveNode("powerCurve");
                                            node.AddNode(curveNode);
                                            print("Fixed part config " + pNode.GetValue("name") + " (" + pNode.GetValue("title") + ")");
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                print("Solar panel fixing of part confignodes failed: " + e.Message);
                            }
                        }
                        // unneeded
                        /*if(HighLogic.CurrentGame != null)
                        {
                            try
                            {
                                if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.flightState.protoVessels != null)
                                {
                                    foreach (ProtoVessel pv in HighLogic.CurrentGame.flightState.protoVessels)
                                    {
                                        if (pv.protoPartSnapshots == null)
                                            continue;
                                        foreach (ProtoPartSnapshot ps in pv.protoPartSnapshots)
                                        {
                                            if (ps.modules == null)
                                                continue;
                                            foreach (ProtoPartModuleSnapshot pm in ps.modules)
                                            {
                                                if (pm.moduleName.Equals("ModuleDeployableSolarPanel"))
                                                {
                                                    if (pm.moduleValues.HasNode("powerCurve"))
                                                    {
                                                        pm.moduleValues.RemoveNode("powerCurve");
                                                        pm.moduleValues.AddNode(curveNode);
                                                        print("Fixed " + ps.partName + " (vessel " + pv.vesselName + ")");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                print("Solar panel fixing for current vessels failed: " + e.Message);
                            }
                        }*/
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
        public static bool fixedTimeWarp = false;

        public void Start()
        {
            fixedTimeWarp = false;
        }

        public void Update()
        {
            if (ScaledSpace.Instance == null)
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

            // PQS dump
            if (HighLogic.LoadedSceneIsFlight && Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftAlt))
            {
                if (FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.mainBody != null)
                {
                    if (FlightGlobals.ActiveVessel.mainBody.pqsController == null)
                        print("*RSS* mainbody PQS null!");
                    else
                        Utils.DumpPQS(FlightGlobals.ActiveVessel.mainBody.pqsController);
                }
            }


            // Fix Timewarp
            if (!fixedTimeWarp && TimeWarp.fetch)
            {
                fixedTimeWarp = true;
                ConfigNode twNode = null;
                foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS"))
                    twNode = node.GetNode("timeWarpRates");
                float ftmp;
                if (twNode != null)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (twNode.HasValue("rate" + i))
                            if (float.TryParse(twNode.GetValue("rate" + i), out ftmp))
                                TimeWarp.fetch.warpRates[i] = ftmp;
                    }
                }

                /*if (twNode != null)
                {
                    //print("*RSS* Adding new TimeWarp level");
                    List<float> ltmp = TimeWarp.fetch.altitudeLimits.ToList();
                    ltmp.Add(2f);
                    TimeWarp.fetch.altitudeLimits = ltmp.ToArray();
                    ltmp = TimeWarp.fetch.warpRates.ToList();
                    ltmp.Add(1000000f);
                    TimeWarp.fetch.warpRates = ltmp.ToArray();
                    TimeWarp.fetch.warpHighButton.xFrames += 1;
                    TimeWarp.fetch.warpHighButton.xSteps += 1;
                    TimeWarp.fetch.warpHighButton.nStates += 1;

                    // do final update for all SoIs and hillSpheres and periods
                    foreach (CelestialBody body in FlightGlobals.fetch.bodies)
                    {
                        //body.resetTimeWarpLimits();
                        body.timeWarpAltitudeLimits[7] = 1000000000;
                    }
                }*/
            }
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

    /*[KSPAddon(KSPAddon.Startup.EveryScene, false)]
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
                body.rotation = rot.TiltedAngle(body.directRotAngle);
                body.transform.rotation = body.rotation;

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
    }*/

    [KSPAddonFixed(KSPAddon.Startup.MainMenu, true, typeof(RealSolarSystem))]
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

            if (RSSSettings == null)
                throw new UnityException("*RSS* REALSOLARSYSTEM node not found!");

            /*print("*RSS* Printing CBTs");
            foreach (PQSMod_CelestialBodyTransform c in Resources.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                Utils.DumpCBT(c);*/

            // thanks to asmi for this!
            print("*RSS* fixing Kerbin mapdecals");
            // HARDCODED FOR NOW
            // For now does only Kerbin, and rescales ALL mapdecals.
            if (!kerbinMapDecalsDone && RSSSettings.GetNode("Kerbin") != null)
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
            double epoch = 0;
            bool useEpoch = false;
            if (RSSSettings.HasValue("Epoch"))
                if (double.TryParse(RSSSettings.GetValue("Epoch"), out epoch))
                    useEpoch = true;
                else
                    useEpoch = false;

            foreach (ConfigNode node in RSSSettings.nodes)
            {
                foreach (CelestialBody body in FlightGlobals.fetch.bodies) //Resources.FindObjectsOfTypeAll(typeof(CelestialBody))) //in FlightGlobals.fetch.bodies)
                {
                    if (body.bodyName.Equals(node.name))
                    {
                        print("Fixing CB " + node.name);
                        double dtmp;
                        float ftmp;
                        int itmp;
                        bool btmp;
                        double origRadius = body.Radius;
                        if (node.HasValue("Radius"))
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
                        if (node.HasValue("GeeASL"))
                        {
                            if (double.TryParse(node.GetValue("GeeASL"), out dtmp))
                                body.GeeASL = dtmp;
                        }
                        if (node.HasValue("atmosphere"))
                        {
                            if (bool.TryParse(node.GetValue("atmosphere"), out btmp))
                                body.atmosphere = btmp;
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
                            if (double.TryParse(node.GetValue("staticPressureASL"), out dtmp))
                                body.staticPressureASL = dtmp;
                        }
                        if (node.HasValue("rotationPeriod"))
                        {
                            if (double.TryParse(node.GetValue("rotationPeriod"), out dtmp))
                                body.rotationPeriod = dtmp;
                        }
                        if (node.HasValue("tidallyLocked"))
                        {
                            if (bool.TryParse(node.GetValue("rotationPeriod"), out btmp))
                                body.tidallyLocked = btmp;
                        }
                        if (node.HasValue("initialRotation"))
                        {
                            if (double.TryParse(node.GetValue("initialRotation"), out dtmp))
                                body.initialRotation = dtmp;
                        }
                        if (node.HasValue("inverseRotation"))
                        {
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
                            if(useEpoch)
                                body.orbit.epoch = epoch;

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
                            bool anomFix = false;
                            if (onode.HasValue("meanAnomalyAtEpoch"))
                            {
                                if (double.TryParse(onode.GetValue("meanAnomalyAtEpoch"), out dtmp))
                                {
                                    body.orbit.meanAnomalyAtEpoch = dtmp;
                                    anomFix = true;
                                }
                            }
                            if (onode.HasValue("meanAnomalyAtEpochD"))
                            {
                                if (double.TryParse(onode.GetValue("meanAnomalyAtEpochD"), out dtmp))
                                {
                                    body.orbit.meanAnomalyAtEpoch = dtmp * 0.0174532925199433;
                                    anomFix = true;
                                }
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
                            if (anomFix)
                            {
                                // assume eccentricity < 1.0
                                body.orbit.meanAnomaly = body.orbit.meanAnomalyAtEpoch; // let KSP handle epoch
                                body.orbit.orbitPercent = body.orbit.meanAnomalyAtEpoch / 6.2831853071795862;
                                body.orbit.ObTAtEpoch = body.orbit.orbitPercent * body.orbit.period;
                            }
                        }
                        // SOI and HillSphere done at end
                        body.CBUpdate();

                        if (body.Radius != origRadius)
                        {
                            // Scaled space fader
                            float SSFMult = 1.0f;
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
                                        if (SSFStart >= 0)
                                            ssf.fadeStart = SSFStart;
                                        else
                                            ssf.fadeStart *= SSFMult;

                                        if (SSFEnd >= 0)
                                            ssf.fadeEnd = SSFEnd;
                                        else
                                            ssf.fadeEnd *= SSFMult;
                                    }
                                }
                            }
                            // The CBT that fades out the PQS
                            // Should probably do this as just another PQSMod, actually.
                            foreach (PQSMod_CelestialBodyTransform c in Resources.FindObjectsOfTypeAll(typeof(PQSMod_CelestialBodyTransform)))
                            {
                                try
                                {
                                    if (c.body != null)
                                    {
                                        if (c.body.name.Equals(node.name))
                                        {
                                            print("Found CBT for " + node.name);
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
                                catch (Exception e)
                                {
                                    print("CBT fix for " + node.name + " failed: " + e.Message);
                                }
                            }
                            print("Did CBT for " + node.name);

                            // the Planet Quadtree Sphere
                            List<string> PQSs = new List<string>();
                            bool custom = false;
                            if (node.HasNode("PQS"))
                            {
                                foreach (ConfigNode n in node.GetNode("PQS").nodes)
                                    PQSs.Add(n.name);
                                custom = true;
                            }
                            else
                            {
                                PQSs.Add(node.name);
                                PQSs.Add(node.name + "Ocean");
                            }
                            foreach (string pName in PQSs)
                            {
                                print("Finding PQS " + pName);
                                foreach (PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS)))
                                {
                                    if (p.name.Equals(pName))
                                    {
                                        if (body.pqsController != p)
                                            if (body.pqsController != p.parentSphere)
                                                continue;
                                        p.radius = body.Radius;
                                        print("Editing PQS " + pName + ", radius = " + p.radius);
                                        if (custom) // YES, THIS IS SILLY
                                        // I SHOULD JUST WRITE A REAL C# EXTENSIBLE LOADER
                                        // Oh well. Hacks are quicker.
                                        {
                                            ConfigNode pqsNode = node.GetNode("PQS").GetNode(pName);
                                            var mods = ((MonoBehaviour)p).transform.GetComponentsInChildren(typeof(PQSMod), true);
                                            foreach (var m in mods)
                                            {
                                                foreach (ConfigNode modNode in pqsNode.nodes)
                                                {
                                                    if (modNode.name.Equals("PQSMod_VertexSimplexHeightAbsolute") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VertexSimplexHeightAbsolute mod = m as PQSMod_VertexSimplexHeightAbsolute;
                                                        if (modNode.HasValue("deformity"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("deformity"), out dtmp))
                                                                mod.deformity = dtmp;
                                                        }
                                                        if (modNode.HasValue("persistence"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("persistence"), out dtmp))
                                                                mod.persistence = dtmp;
                                                        }
                                                        if (modNode.HasValue("frequency"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("frequency"), out dtmp))
                                                                mod.frequency = dtmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_VertexHeightNoiseVertHeightCurve2") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VertexHeightNoiseVertHeightCurve2 mod = m as PQSMod_VertexHeightNoiseVertHeightCurve2;
                                                        if (modNode.HasValue("deformity"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("deformity"), out ftmp))
                                                                mod.deformity = ftmp;
                                                        }
                                                        if (modNode.HasValue("ridgedAddFrequency"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("ridgedAddFrequency"), out ftmp))
                                                                mod.ridgedAddFrequency = ftmp;
                                                        }
                                                        if (modNode.HasValue("ridgedSubFrequency"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("ridgedSubFrequency"), out ftmp))
                                                                mod.ridgedSubFrequency = ftmp;
                                                        }
                                                        if (modNode.HasValue("ridgedAddOctaves"))
                                                        {
                                                            if (int.TryParse(modNode.GetValue("ridgedAddOctaves"), out itmp))
                                                                mod.ridgedAddOctaves = itmp;
                                                        }
                                                        if (modNode.HasValue("simplexHeightStart"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("simplexHeightStart"), out ftmp))
                                                                mod.simplexHeightStart = ftmp;
                                                        }
                                                        if (modNode.HasValue("simplexHeightEnd"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("simplexHeightEnd"), out ftmp))
                                                                mod.simplexHeightEnd = ftmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_VertexRidgedAltitudeCurve") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VertexRidgedAltitudeCurve mod = m as PQSMod_VertexRidgedAltitudeCurve;
                                                        if (modNode.HasValue("deformity"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("deformity"), out ftmp))
                                                                mod.deformity = ftmp;
                                                        }
                                                        if (modNode.HasValue("ridgedAddFrequency"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("ridgedAddFrequency"), out ftmp))
                                                                mod.ridgedAddFrequency = ftmp;
                                                        }
                                                        if (modNode.HasValue("ridgedAddOctaves"))
                                                        {
                                                            if (int.TryParse(modNode.GetValue("ridgedAddOctaves"), out itmp))
                                                                mod.ridgedAddOctaves = itmp;
                                                        }
                                                        if (modNode.HasValue("simplexHeightStart"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("simplexHeightStart"), out ftmp))
                                                                mod.simplexHeightStart = ftmp;
                                                        }
                                                        if (modNode.HasValue("simplexHeightEnd"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("simplexHeightEnd"), out ftmp))
                                                                mod.simplexHeightEnd = ftmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    ///
                                                    if (modNode.name.Equals("PQSMod_VertexHeightMap") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VertexHeightMap mod = m as PQSMod_VertexHeightMap;
                                                        if (modNode.HasValue("heightMapDeformity"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("heightMapDeformity"), out dtmp))
                                                                mod.heightMapDeformity = dtmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_AltitudeAlpha") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_AltitudeAlpha mod = m as PQSMod_AltitudeAlpha;
                                                        if (modNode.HasValue("atmosphereDepth"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("atmosphereDepth"), out dtmp))
                                                                mod.atmosphereDepth = dtmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_AerialPerspectiveMaterial") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_AerialPerspectiveMaterial mod = m as PQSMod_AerialPerspectiveMaterial;
                                                        if (modNode.HasValue("heightMapDeformity"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("heightMapDeformity"), out ftmp))
                                                                mod.atmosphereDepth = ftmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_VertexSimplexHeight") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VertexSimplexHeight mod = m as PQSMod_VertexSimplexHeight;
                                                        if (modNode.HasValue("deformity"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("deformity"), out dtmp))
                                                                mod.deformity = dtmp;
                                                        }
                                                        if (modNode.HasValue("persistence"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("persistence"), out dtmp))
                                                                mod.persistence = dtmp;
                                                        }
                                                        if (modNode.HasValue("frequency"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("frequency"), out dtmp))
                                                                mod.frequency = dtmp;
                                                        }
                                                        if (modNode.HasValue("octaves"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("octaves"), out dtmp))
                                                                mod.octaves = dtmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_VertexHeightNoiseVertHeight") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VertexHeightNoiseVertHeight mod = m as PQSMod_VertexHeightNoiseVertHeight;
                                                        if (modNode.HasValue("deformity"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("deformity"), out ftmp))
                                                                mod.deformity = ftmp;
                                                        }
                                                        if (modNode.HasValue("frequency"))
                                                        {
                                                            if (float.TryParse(modNode.GetValue("frequency"), out ftmp))
                                                                mod.frequency = ftmp;
                                                        }
                                                        if (modNode.HasValue("octaves"))
                                                        {
                                                            if (int.TryParse(modNode.GetValue("octaves"), out itmp))
                                                                mod.octaves = itmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                    if (modNode.name.Equals("PQSMod_VoronoiCraters") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSMod_VoronoiCraters mod = m as PQSMod_VoronoiCraters;
                                                        if (modNode.HasValue("KEYvoronoiSeed"))
                                                            if (int.Parse(modNode.GetValue("KEYvoronoiSeed")) != mod.voronoiSeed)
                                                                continue;

                                                        if (modNode.HasValue("deformation"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("deformation"), out dtmp))
                                                                mod.deformation = dtmp;
                                                        }
                                                        if (modNode.HasValue("voronoiFrequency"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("voronoiFrequency"), out dtmp))
                                                                mod.voronoiFrequency = dtmp;
                                                        }
                                                        mod.OnSetup();
                                                    }

                                                    // City
                                                    if (modNode.name.Equals("PQSCity") && m.GetType().ToString().Equals(modNode.name))
                                                    {
                                                        PQSCity mod = m as PQSCity;
                                                        if (modNode.HasValue("KEYname"))
                                                            if(!(mod.name.Equals(modNode.GetValue("KEYname"))))
                                                                continue;

                                                        if (modNode.HasValue("repositionRadial"))
                                                        {
                                                            mod.repositionRadial = KSPUtil.ParseVector3(modNode.GetValue("repositionRadial"));
                                                        }
                                                        if (modNode.HasValue("reorientInitialUp"))
                                                        {
                                                            mod.reorientInitialUp = KSPUtil.ParseVector3(modNode.GetValue("reorientInitialUp"));
                                                        }
                                                        if (modNode.HasValue("repositionToSphere"))
                                                        {
                                                            if (bool.TryParse(modNode.GetValue("repositionToSphere"), out btmp))
                                                                mod.repositionToSphere = btmp;
                                                        }
                                                        if (modNode.HasValue("repositionToSphereSurface"))
                                                        {
                                                            if (bool.TryParse(modNode.GetValue("repositionToSphereSurface"), out btmp))
                                                                mod.repositionToSphereSurface = btmp;
                                                        }
                                                        if (modNode.HasValue("reorientToSphere"))
                                                        {
                                                            if (bool.TryParse(modNode.GetValue("reorientToSphere"), out btmp))
                                                                mod.reorientToSphere = btmp;
                                                        }
                                                        if (modNode.HasValue("repositionRadiusOffset"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("repositionRadiusOffset"), out dtmp))
                                                                mod.repositionRadiusOffset = dtmp;
                                                        }
                                                        if (modNode.HasValue("lodvisibleRangeMult"))
                                                        {
                                                            if (double.TryParse(modNode.GetValue("lodvisibleRangeMult"), out dtmp))
                                                                foreach (PQSCity.LODRange l in mod.lod)
                                                                    l.visibleRange *= (float)dtmp;
                                                        }
                                                        mod.OnSetup();
                                                    }
                                                }
                                            }
                                        }
                                        try
                                        {
                                            print("Rebuilding sphere " + p.name);
                                            //p.ResetSphere();
                                            p.RebuildSphere();
                                        }
                                        catch (Exception e)
                                        {
                                            print("Rebuild sphere for " + node.name + " failed: " + e.Message);
                                        }
                                    }
                                }
                            }
                            foreach (AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                            {
                                if (ag != null && ag.planet != null)
                                {
                                    // generalized version of Starwaster's code. Thanks Starwaster!
                                    if (ag.planet.name.Equals(node.name))
                                    {
                                        print("Found atmo for " + node.name + ": " + ag.name);
                                        if (node.HasNode("AtmosphereFromGround"))
                                        {
                                            ConfigNode modNode = node.GetNode("AtmosphereFromGround");
                                            if (modNode.HasValue("outerRadius"))
                                            {
                                                if (float.TryParse(modNode.GetValue("outerRadius"), out ftmp))
                                                    ag.outerRadius = ftmp * ScaledSpace.InverseScaleFactor;
                                            }
                                            else if (modNode.HasValue("outerRadiusAtmo"))
                                            {
                                                ag.outerRadius = ((float)body.Radius + body.maxAtmosphereAltitude) * ScaledSpace.InverseScaleFactor;
                                            }
                                            else if (modNode.HasValue("outerRadiusMult"))
                                            {
                                                if (float.TryParse(modNode.GetValue("outerRadiusMult"), out ftmp))
                                                    ag.outerRadius = ftmp * (float)body.Radius * ScaledSpace.InverseScaleFactor;
                                            }

                                            if (modNode.HasValue("innerRadius"))
                                            {
                                                if (float.TryParse(modNode.GetValue("innerRadius"), out ftmp))
                                                    ag.innerRadius = ftmp * ScaledSpace.InverseScaleFactor;
                                            }
                                            else if (modNode.HasValue("innerRadiusMult"))
                                            {
                                                if (float.TryParse(modNode.GetValue("innerRadiusMult"), out ftmp))
                                                    ag.innerRadius = ftmp * ag.outerRadius;
                                            }

                                            if (modNode.HasValue("doScale"))
                                            {
                                                if (bool.TryParse(modNode.GetValue("doScale"), out btmp))
                                                    ag.doScale = btmp;
                                            }
                                            if (modNode.HasValue("transformScale"))
                                            {
                                                if (float.TryParse(modNode.GetValue("transformScale"), out ftmp) && ag.transform != null)
                                                    ag.transform.localScale = new Vector3(ftmp, ftmp, ftmp);
                                            }
                                            else if (modNode.HasValue("transformAtmo"))
                                            {
                                                ag.transform.localScale = Vector3.one * ((float)(body.Radius + body.maxAtmosphereAltitude) / (float)body.Radius);
                                            }

                                            if (modNode.HasValue("invWaveLength"))
                                            {
                                                Vector3 col = KSPUtil.ParseVector3(modNode.GetValue("invWaveLength"));
                                                ag.invWaveLength = new Color(col.x, col.y, col.z);
                                            }
                                            if (modNode.HasValue("waveLength"))
                                            {
                                                Vector3 col = KSPUtil.ParseVector3(modNode.GetValue("waveLength"));
                                                ag.waveLength = new Color(col.x, col.y, col.z);
                                            }
                                        }
                                        else
                                        {
                                            // the defaults
                                            ag.outerRadius = (float)body.Radius * 1.025f * ScaledSpace.InverseScaleFactor;
                                            ag.innerRadius = ag.outerRadius * 0.975f;
                                        }
                                        ag.outerRadius2 = ag.outerRadius * ag.outerRadius;
                                        ag.innerRadius2 = ag.innerRadius * ag.innerRadius;
                                        ag.scale = 1f / (ag.outerRadius - ag.innerRadius);
                                        ag.scaleDepth = -0.25f;
                                        ag.scaleOverScaleDepth = ag.scale / ag.scaleDepth;
                                        print("Atmo updated");
                                    }
                                }
                            }
                            // Scaled space
                            if (ScaledSpace.Instance != null)
                            {
                                float SSTScale = 1.0f;
                                if (node.HasValue("SSTScale"))
                                    float.TryParse(node.GetValue("SSTScale"), out SSTScale);
                                SSTScale *= (float)(body.Radius / origRadius);
                                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                                {
                                    if (t.name.Equals(node.name))
                                    {
                                        t.localScale = new Vector3(t.localScale.x * SSTScale, t.localScale.y * SSTScale, t.localScale.z * SSTScale);
                                    }
                                }
                            }

                            // Science
                            if (node.HasNode("CelestialBodyScienceParams"))
                            {
                                ConfigNode spNode = node.GetNode("CelestialBodyScienceParams");
                                if (body.scienceValues != null)
                                {
                                    foreach (ConfigNode.Value val in spNode.values)
                                    {
                                        // meh, for now hard-code it. Saves worry of GIGO.
                                        /*if(body.scienceValues.GetType().GetField(val.name) != null)
                                            if(float.TryParse(val.value, out ftmp))
                                                body.scienceValues.GetType().GetField(val.name).SetValue(*/
                                        if (spNode.HasValue("LandedDataValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("LandedDataValue"), out ftmp))
                                                body.scienceValues.LandedDataValue = ftmp;
                                        }
                                        if (spNode.HasValue("SplashedDataValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("SplashedDataValue"), out ftmp))
                                                body.scienceValues.SplashedDataValue = ftmp;
                                        }
                                        if (spNode.HasValue("FlyingLowDataValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("FlyingLowDataValue"), out ftmp))
                                                body.scienceValues.FlyingLowDataValue = ftmp;
                                        }
                                        if (spNode.HasValue("FlyingHighDataValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("FlyingHighDataValue"), out ftmp))
                                                body.scienceValues.FlyingHighDataValue = ftmp;
                                        }
                                        if (spNode.HasValue("InSpaceLowDataValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("InSpaceLowDataValue"), out ftmp))
                                                body.scienceValues.InSpaceLowDataValue = ftmp;
                                        }
                                        if (spNode.HasValue("InSpaceHighDataValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("InSpaceHighDataValue"), out ftmp))
                                                body.scienceValues.InSpaceHighDataValue = ftmp;
                                        }
                                        if (spNode.HasValue("RecoveryValue"))
                                        {
                                            if (float.TryParse(spNode.GetValue("RecoveryValue"), out ftmp))
                                                body.scienceValues.RecoveryValue = ftmp;
                                        }
                                        if (spNode.HasValue("flyingAltitudeThreshold"))
                                        {
                                            if (float.TryParse(spNode.GetValue("flyingAltitudeThreshold"), out ftmp))
                                                body.scienceValues.flyingAltitudeThreshold = ftmp;
                                        }
                                        if (spNode.HasValue("spaceAltitudeThreshold"))
                                        {
                                            if (float.TryParse(spNode.GetValue("spaceAltitudeThreshold"), out ftmp))
                                                body.scienceValues.spaceAltitudeThreshold = ftmp;
                                        }
                                    }
                                }
                            }

                            // texture rebuild
                            if (node.HasNode("Export"))
                            {
                                try
                                {
                                    int res = 2048;
                                    bool ocean = false;
                                    Color oceanColor;
                                    double maxHeight, oceanHeight;
                                    PQS bodyPQS = null;
                                    foreach (PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS)))
                                        if (p.name.Equals(body.name))
                                        {
                                            bodyPQS = p;
                                            break;
                                        }
                                    if (bodyPQS != null)
                                    {
                                        maxHeight = bodyPQS.radiusDelta * 0.5;
                                        oceanHeight = 0;
                                        ocean = body.ocean;
                                        oceanColor = new Color(0.1255f, 0.22353f, 0.35683f);
                                        ConfigNode exportNode = node.GetNode("Export");
                                        if (exportNode.HasValue("resolution"))
                                        {
                                            if (int.TryParse(exportNode.GetValue("resolution"), out itmp))
                                                res = itmp;
                                        }
                                        if (exportNode.HasValue("maxHeight"))
                                        {
                                            if (double.TryParse(exportNode.GetValue("maxHeight"), out dtmp))
                                                maxHeight = dtmp;
                                        }

                                        if (exportNode.HasValue("oceanHeight"))
                                        {
                                            if (double.TryParse(exportNode.GetValue("oceanHeight"), out dtmp))
                                                oceanHeight = dtmp;
                                        }
                                        if (exportNode.HasValue("oceanColor"))
                                        {
                                            ocean = true;
                                            Vector3 col = KSPUtil.ParseVector3(exportNode.GetValue("oceanColor"));
                                            oceanColor = new Color(col.x, col.y, col.z);
                                        }
                                        /*Texture2D KerbinScaledSpace300 = null;
                                        Texture2D KerbinScaledSpace401 = null;
                                        foreach (Texture2D tex in Resources.FindObjectsOfTypeAll(typeof(Texture2D)))
                                        {
                                            if (tex.name.Equals("KerbinScaledSpace300"))
                                                KerbinScaledSpace300 = tex;
                                            if (tex.name.Equals("KerbinScaledSpace401"))
                                                KerbinScaledSpace401 = tex;
                                        }*/
                                        Texture2D[] kerbinTextures = bodyPQS.CreateMaps(res, maxHeight, ocean, oceanHeight, oceanColor);
                                        /*foreach (Texture2D t in kerbinTextures)
                                        {
                                            MonoBehaviour.DontDestroyOnLoad(t);
                                        }*/
                                        System.IO.File.WriteAllBytes(KSPUtil.ApplicationRootPath + "/" + body.name + "1.png", kerbinTextures[0].EncodeToPNG());
                                        System.IO.File.WriteAllBytes(KSPUtil.ApplicationRootPath + "/" + body.name + "2.png", kerbinTextures[1].EncodeToPNG());
                                        /*foreach (Material mat in Resources.FindObjectsOfTypeAll(typeof(Material)))
                                        {
                                            if (mat.mainTexture.name.Equals("KerbinScaledSpace300"))
                                            {
                                                mat.mainTexture = kerbinTextures[0];
                                                try
                                                {
                                                    Resources.UnloadAsset(KerbinScaledSpace300);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            if (mat.mainTexture.name.Equals("KerbinScaledSpace401"))
                                            {
                                                mat.mainTexture = kerbinTextures[1];
                                                try
                                                {
                                                    Resources.UnloadAsset(KerbinScaledSpace401);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                            if (mat.GetTexture("_BumpMap") != null)
                                            {
                                                if (mat.GetTexture("_BumpMap").name.Equals("KerbinScaledSpace300"))
                                                {
                                                    mat.SetTexture("_BumpMap", kerbinTextures[0]);
                                                    try
                                                    {
                                                        Resources.UnloadAsset(KerbinScaledSpace300);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                if (mat.GetTexture("_BumpMap").name.Equals("KerbinScaledSpace401"))
                                                {
                                                    mat.SetTexture("_BumpMap", kerbinTextures[1]);
                                                    try
                                                    {
                                                        Resources.UnloadAsset(KerbinScaledSpace401);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                            }
                                        }*/
                                    }
                                }
                                catch (Exception e)
                                {
                                    print("Export for " + node.name + " failed: " + e.Message);
                                }
                            }
                        }
                    }
                }
            }
            // do final update for all SoIs and hillSpheres and periods
            foreach (CelestialBody body in FlightGlobals.fetch.bodies)
            {
                try
                {
                    if (body.orbitDriver != null)
                    {
                        if (body.referenceBody != null)
                        {
                            body.hillSphere = body.orbit.semiMajorAxis * (1.0 - body.orbit.eccentricity) * Math.Pow(body.Mass / body.orbit.referenceBody.Mass, 1 / 3);
                            body.sphereOfInfluence = body.orbit.semiMajorAxis * Math.Pow(body.Mass / body.orbit.referenceBody.Mass, 0.4);
                            if (body.sphereOfInfluence < body.Radius * 1.5 || body.sphereOfInfluence < body.Radius + 20000.0)
                                body.sphereOfInfluence = Math.Max(body.Radius * 1.5, body.Radius + 20000.0); // sanity check

                            body.orbit.period = 2 * Math.PI * Math.Sqrt(Math.Pow(body.orbit.semiMajorAxis, 2) / 6.674E-11 * body.orbit.semiMajorAxis / (body.Mass + body.referenceBody.Mass));
                        }
                        else
                        {
                            body.sphereOfInfluence = double.PositiveInfinity;
                            body.hillSphere = double.PositiveInfinity;
                        }
                        // doesn't seem needed - body.orbitDriver.QueuedUpdate = true;
                    }
                    try
                    {
                        body.CBUpdate();
                    }
                    catch (Exception e)
                    {
                        print("CBUpdate for " + body.name + " failed: " + e.Message);
                    }
                }
                catch (Exception e)
                {
                    print("Final update bodies failed: " + e.Message);
                }
            }
            print("*RSS* Done loading!");
        }

    }
    // From Starwaster
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AFGEditor : MonoBehaviour
    {
        private static Rect windowPosition = new Rect(64, 64, Screen.width / 8, Screen.height / 4);
        private static GUIStyle windowStyle = null;
        private AtmosphereFromGround afg = null;
        private Boolean GUIOpen;
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftAlt))
            {
                GUIOpen = !GUIOpen;
            }
        }
        public void Awake()
        {
            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }
        private void OnDraw()
        {
            if (GUIOpen)
            {
                //print("[AFG Editor] OnDraw");
                if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
                    windowPosition = GUI.Window(69105, windowPosition, ShowGUI, "Wavelength Parameters", windowStyle);
            }
        }

        public void Start()
        {
            if (afg == null)
                afg = findAFG();
            //print("[AFGEditor] Start()");
            windowStyle = new GUIStyle(HighLogic.Skin.window);
            windowStyle.stretchHeight = true;
        }
        public AtmosphereFromGround findAFG()
        {

            CelestialBody mainBody = FlightGlobals.getMainBody();
            //AtmosphereFromGround afg = null;
            foreach (AtmosphereFromGround afgt in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
            {
                if (afgt.planet == mainBody)
                {
                    afg = afgt;
                    //print("[AFG Editor] Found Atmosphere");
                }
            }
            return afg;
        }

        private void ShowGUI(int windowID)
        {
            //print("[AFG Editor]: ShowGUI");
            Color waveLength;
            waveLength.a = afg.waveLength.a;
            //float innerRadius;
            //float atmoHeightScale;
            //float eSun;
            //float Kr;
            //float Km;

            /*
            GUILayout.BeginVertical();
            //GUI.Label(new Rect(65, 40, 200, 30), "Wavelength Parameters");
            
            GUILayout.Label("Red: " + afg.waveLength.r.ToString());
            waveLength.r = GUILayout.HorizontalSlider(afg.waveLength.r, -2.0F, 2.0F);
            GUILayout.Label("Green: " + afg.waveLength.g.ToString());
            waveLength.g = GUILayout.HorizontalSlider(afg.waveLength.g, -2.0F, 2.0F);
            GUILayout.Label("Blue: " + afg.waveLength.b.ToString());
            waveLength.b = GUILayout.HorizontalSlider(afg.waveLength.b, -2.0F, 2.0F);
            eSun = GUILayout.HorizontalSlider(afg.ESun, 0.0f, 1.0f);
            Kr = GUILayout.HorizontalSlider(afg.Kr, 0.0f, 1.0f);
            Km = GUILayout.HorizontalSlider(afg.Km, 0.0f, 1.0f);
            GUILayout.EndVertical();
            */

            string rt;
            string gt;
            string bt;
            string ESunt;
            string Krt;
            string Kmt;


            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Red");
            rt = GUILayout.TextField(afg.waveLength.r.ToString(), 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Green");
            gt = GUILayout.TextField(afg.waveLength.g.ToString(), 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Blue");
            bt = GUILayout.TextField(afg.waveLength.b.ToString(), 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("ESun");
            ESunt = GUILayout.TextField(afg.ESun.ToString(), 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Kr");
            Krt = GUILayout.TextField(afg.Kr.ToString(), 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Km");
            Kmt = GUILayout.TextField(afg.Km.ToString(), 10);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                float.TryParse(rt, out afg.waveLength.r);
                float.TryParse(gt, out afg.waveLength.g);
                float.TryParse(bt, out afg.waveLength.b);
                float.TryParse(ESunt, out afg.ESun);
                float.TryParse(Krt, out afg.Kr);
                float.TryParse(Kmt, out afg.Km);
            }
            //afg.waveLength = waveLength;
            //afg.ESun = eSun;
            //afg.Kr = Kr;
            //afg.Km = Km;

            //GUILayout.BeginVertical();
            //GUILayout.Label("innerRadiusScale");

            //GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
    /// <summary>
    /// KSPAddon with equality checking using an additional type parameter. Fixes the issue where AddonLoader prevents multiple start-once addons with the same start scene.
    /// By Majiir
    /// </summary>
    public class KSPAddonFixed : KSPAddon, IEquatable<KSPAddonFixed>
    {
        private readonly Type type;

        public KSPAddonFixed(KSPAddon.Startup startup, bool once, Type type)
            : base(startup, once)
        {
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) { return false; }
            return Equals((KSPAddonFixed)obj);
        }

        public bool Equals(KSPAddonFixed other)
        {
            if (this.once != other.once) { return false; }
            if (this.startup != other.startup) { return false; }
            if (this.type != other.type) { return false; }
            return true;
        }

        public override int GetHashCode()
        {
            return this.startup.GetHashCode() ^ this.once.GetHashCode() ^ this.type.GetHashCode();
        }
    }
}

