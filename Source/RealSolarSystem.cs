using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;


namespace RealSolarSystem
{
    [KSPAddonFixed(KSPAddon.Startup.SpaceCentre, false, typeof(KSCReset))]
    public class KSCReset : MonoBehaviour
    {
        public static bool shouldCameraBeReset = true;
        public void Start()
        {
            if (shouldCameraBeReset)
            {
                print("*RSS* Resetting scene to spacecenter");
                HighLogic.LoadScene(GameScenes.SPACECENTER);
                shouldCameraBeReset = false;
            }
        }
    }
    [KSPAddonFixed(KSPAddon.Startup.MainMenu, true, typeof(RealSolarSystem))]
    public class RealSolarSystem : MonoBehaviour
    {
        public static bool doneRSS = false;

        public static Vector3 LLAtoECEF(double lat, double lon, double alt, double radius)
        {
            const double degreesToRadians =  Math.PI / 180.0;
            lat = (lat-90) * degreesToRadians;
            lon *= degreesToRadians;
            double x, y, z;
            double n = radius; // for now, it's still a sphere, so just the radius
            x = (n + alt) * -1.0 * Math.Sin(lat) * Math.Cos(lon);
            y = (n + alt) * Math.Cos(lat); // for now, it's still a sphere, so no eccentricity
            z = (n + alt) * -1.0 * Math.Sin(lat) * Math.Sin(lon);
            return new Vector3((float)x, (float)y, (float)z);
        }
        public void GeeASLToOthers(CelestialBody body)
        {
            double rsq = body.Radius * body.Radius;
            body.gMagnitudeAtCenter = body.GeeASL * 9.81 * rsq;
            body.gravParameter = body.gMagnitudeAtCenter;
            body.Mass = body.gravParameter * (1 / 6.674E-11);
        }

        // converts mass to Gee ASL using a body's radius.
        public void MassToOthers(CelestialBody body)
        {
            double rsq = body.Radius * body.Radius;
            body.GeeASL = body.Mass * (6.674E-11 / 9.81) / rsq;
            body.gMagnitudeAtCenter = body.GeeASL * 9.81 * rsq;
            body.gravParameter = body.gMagnitudeAtCenter;
        }

        public static void GravParamToOthers(CelestialBody body)
        {
            double rsq = body.Radius * body.Radius;
            body.Mass = body.gravParameter * (1 / 6.674E-11);
            body.GeeASL = body.gravParameter / 9.81 / rsq;
            body.gMagnitudeAtCenter = body.gravParameter;
        }

        // thanks to asmi for this!
        public static bool kerbinMapDecalsDone = false;


        public static bool done = false;

        public AnimationCurve loadAnimationCurve(string[] curveData)
        {
            char[] cParams = new char[] { ' ', ',', ';', '\t' };
            AnimationCurve animationCurve = new AnimationCurve();
            try
            {
                for (int i = 0; i < curveData.Length; i++)
                {
                    string[] keyTmp = curveData[i].Split(cParams, StringSplitOptions.RemoveEmptyEntries);
                    if (keyTmp.Length == 4)
                    {
                        Keyframe key = new Keyframe();
                        key.time = float.Parse(keyTmp[0]);
                        key.value = float.Parse(keyTmp[1]);
                        key.inTangent = float.Parse(keyTmp[2]);
                        key.outTangent = float.Parse(keyTmp[3]);
                        animationCurve.AddKey(key);
                    }
                    else if (keyTmp.Length == 2)
                    {
                        Keyframe key = new Keyframe();
                        key.time = float.Parse(keyTmp[0]);
                        key.value = float.Parse(keyTmp[1]);
                        animationCurve.AddKey(key);
                    }
                    else
                    {
                        MonoBehaviour.print("*RSS* Invalid animationCurve data: animationCurve data must have exactly two or four parameters!");
                    }
                }
                return animationCurve;
            }
            catch (Exception e)
            {
                print("Caught exception while parsing animationcurve: " + e.Message);
                return null;
            }
        }

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

            print("*RSS* fixing bodies");
            double epoch = 0;
            bool useEpoch = false;
            bool doWrap = false;
            bool updateMass = true;
            bool compressNormals = false;
            bool spheresOnly = false;
            bool defaultAtmoScale = true;
            float SSAtmoScale = 1.0f;
            useEpoch = RSSSettings.TryGetValue("Epoch", ref epoch);
            RSSSettings.TryGetValue("wrap", ref doWrap);
            RSSSettings.TryGetValue("compressNormals", ref compressNormals);
            RSSSettings.TryGetValue("spheresOnly", ref spheresOnly);
            RSSSettings.TryGetValue("defaultAtmoScale", ref defaultAtmoScale);
            RSSSettings.TryGetValue("SSAtmoScale", ref SSAtmoScale);

            // get spherical scaledspace mesh
            MeshFilter joolMesh = null;
            if (ScaledSpace.Instance != null)
            {
                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    if (t.name.Equals("Jool"))
                        joolMesh = (MeshFilter)t.GetComponent(typeof(MeshFilter));
                }
                print("*RSS* InverseScaleFactor = " + ScaledSpace.InverseScaleFactor);
            }
            foreach (ConfigNode node in RSSSettings.nodes)
            {
                foreach (CelestialBody body in FlightGlobals.fetch.bodies) //Resources.FindObjectsOfTypeAll(typeof(CelestialBody))) //in FlightGlobals.fetch.bodies)
                {
                    if (body.name.Equals(node.name))
                    {
                        print("Fixing CB " + node.name);
                        double dtmp;
                        float ftmp;
                        int itmp;
                        bool btmp;
                        double origRadius = body.Radius;
                        double origAtmo = body.maxAtmosphereAltitude;
                        
                        node.TryGetValue("bodyName", ref body.bodyName);
                        node.TryGetValue("bodyDescription", ref body.bodyDescription);
                        node.TryGetValue("Radius", ref body.Radius);
                        
                        if (node.TryGetValue("Mass", ref body.Mass))
                        {
                            MassToOthers(body);
                            updateMass = false;
                        }
                        if (node.TryGetValue("GeeASL", ref body.GeeASL))
                        {
                            GeeASLToOthers(body);
                            updateMass = false;
                        }
                        if (node.TryGetValue("gravParameter", ref body.gravParameter))
                        {
                            GravParamToOthers(body);
                            updateMass = false;
                        }

                        node.TryGetValue("atmosphere", ref body.atmosphere);
                        node.TryGetValue("atmosphereScaleHeight", ref body.atmosphereScaleHeight);
                        node.TryGetValue("atmosphereMultiplier", ref body.atmosphereMultiplier);
                        node.TryGetValue("maxAtmosphereAltitude", ref body.maxAtmosphereAltitude);
                        node.TryGetValue("staticPressureASL", ref body.staticPressureASL);
                        node.TryGetValue("useLegacyAtmosphere", ref body.useLegacyAtmosphere);
                        if (!body.useLegacyAtmosphere)
                        {
                            ConfigNode PCnode = node.GetNode("pressureCurve");
                            if (PCnode != null)
                            {
                                string[] curve = PCnode.GetValues("key");
                                body.altitudeMultiplier = 1f;
                                body.pressureMultiplier = 1f;
                                AnimationCurve pressureCurve = loadAnimationCurve(curve);
                                if (pressureCurve != null)
                                    body.pressureCurve = pressureCurve;
                                else
                                {
                                    body.useLegacyAtmosphere = true;
                                    Debug.LogWarning("Unable to load pressureCurve data for " + body.name + ": Using legacy atmosphere");
                                }
                                print("*RSS* finished with " + body.GetName() + ".pressureCurve (" + body.pressureCurve.keys.Length.ToString() + " keys)");
                            }
                            else
                            {
                                print("*RSS* useLegacyAtmosphere = False but pressureCurve not found!");
                            }
                        }
                        if (node.HasNode("temperatureCurve"))
                        {
                            ConfigNode TCnode = node.GetNode("temperatureCurve");
                            if (TCnode != null)
                            {
                                string[] curve = TCnode.GetValues("key");
                                AnimationCurve temperatureCurve = loadAnimationCurve(curve);
                                if (temperatureCurve != null)
                                {
                                    body.temperatureCurve = temperatureCurve;
                                    print("*RSS* found and loaded temperatureCurve data for " + body.name);
                                }
                            }
                        }
                        node.TryGetValue("rotationPeriod", ref body.rotationPeriod);
                        node.TryGetValue("tidallyLocked", ref body.tidallyLocked);
                        node.TryGetValue("initialRotation", ref body.initialRotation);
                        node.TryGetValue("inverseRotation", ref body.inverseRotation);
                        
                        if(updateMass)
                            GeeASLToOthers(body);

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
                            if (useEpoch)
                                body.orbit.epoch = epoch;

                            onode.TryGetValue("semiMajorAxis", ref body.orbit.semiMajorAxis);
                            onode.TryGetValue("eccentricity", ref body.orbit.eccentricity);
                            
                            bool anomFix = false;
                            if (onode.TryGetValue("meanAnomalyAtEpoch", ref body.orbit.meanAnomalyAtEpoch))
                            {
                                anomFix = true;
                            }
                            if (onode.TryGetValue("meanAnomalyAtEpochD", ref body.orbit.meanAnomalyAtEpoch))
                            {
                                body.orbit.meanAnomalyAtEpoch *= 0.0174532925199433;
                                anomFix = true;
                            }
                            onode.TryGetValue("inclination", ref body.orbit.inclination);
                            onode.TryGetValue("period", ref body.orbit.period);
                            onode.TryGetValue("LAN", ref body.orbit.LAN);
                            onode.TryGetValue("argumentOfPeriapsis", ref body.orbit.argumentOfPeriapsis);
                            if(onode.HasValue("orbitColor"))
                            {
                                Vector4 col = KSPUtil.ParseVector4(onode.GetValue("orbitColor"));
                                Color c = new Color(col.x, col.y, col.z, col.w);
                                body.GetOrbitDriver().orbitColor = c;
                            }
                            string bodyname = "";
                            if (onode.TryGetValue("referenceBody", ref bodyname))
                            {
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

                        // Scaled space fader
                        float SSFMult = 1.0f;
                        float SSFStart = -1, SSFEnd = -1;
                        node.TryGetValue("SSFStart", ref SSFStart);
                        node.TryGetValue("SSFEnd", ref SSFEnd);
                        node.TryGetValue("SSFMult", ref SSFMult);

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
                                        node.TryGetValue("PQSdeactivateAltitude", ref c.deactivateAltitude);
                                        if (c.planetFade != null)
                                        {
                                            node.TryGetValue("PQSfadeStart", ref c.planetFade.fadeStart);
                                            node.TryGetValue("PQSfadeEnd", ref c.planetFade.fadeEnd);
                                            if (c.secondaryFades != null)
                                            {
                                                foreach (PQSMod_CelestialBodyTransform.AltitudeFade af in c.secondaryFades)
                                                {
                                                    node.TryGetValue("PQSSecfadeStart", ref af.fadeStart);
                                                    node.TryGetValue("PQSSecfadeEnd", ref af.fadeEnd);
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
                            if (body.Radius != origRadius)
                            {
                                PQSs.Add(node.name);
                                PQSs.Add(node.name + "Ocean");
                            }
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

                                        // PQS members
                                        if(pqsNode.HasValue("maxLevel"))
                                        {
                                            if (int.TryParse(pqsNode.GetValue("maxLevel"), out itmp))
                                            {
                                                p.maxLevel = itmp;
                                                PQSCache.PresetList.GetPreset(p.name).maxSubdivision = itmp;
                                            }
                                        }
                                        if (pqsNode.HasValue("maxQuadLenghtsPerFrame"))
                                        {
                                            if (float.TryParse(pqsNode.GetValue("maxQuadLenghtsPerFrame"), out ftmp))
                                            {
                                                p.maxQuadLenghtsPerFrame = ftmp;
                                            }
                                        }
                                        if (pqsNode.HasValue("visRadAltitudeMax"))
                                        {
                                            if (double.TryParse(pqsNode.GetValue("visRadAltitudeMax"), out dtmp))
                                            {
                                                p.visRadAltitudeMax = dtmp;
                                            }
                                        }
                                        if (pqsNode.HasValue("visRadAltitudeValue"))
                                        {
                                            if (double.TryParse(pqsNode.GetValue("visRadAltitudeValue"), out dtmp))
                                            {
                                                p.visRadAltitudeValue = dtmp;
                                            }
                                        }
                                        if (pqsNode.HasValue("visRadSeaLevelValue"))
                                        {
                                            if (double.TryParse(pqsNode.GetValue("visRadSeaLevelValue"), out dtmp))
                                            {
                                                p.visRadSeaLevelValue = dtmp;
                                            }
                                        }


                                        // PQSMods
                                        var mods = p.transform.GetComponentsInChildren(typeof(PQSMod), true);
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
                                                    if (modNode.HasValue("heightMapOffset"))
                                                    {
                                                        if (double.TryParse(modNode.GetValue("heightMapOffset"), out dtmp))
                                                            mod.heightMapOffset = dtmp;
                                                        print("*RSS* Set offset " + mod.heightMapOffset);
                                                    }
                                                    if (modNode.HasValue("heightMap"))
                                                    {
                                                        Texture2D map = new Texture2D(4,4, TextureFormat.Alpha8, false);
                                                        map.LoadImage(System.IO.File.ReadAllBytes(modNode.GetValue("heightMap")));
                                                        //print("*RSS* MapSO: depth " + mod.heightMap.Depth + "(" + mod.heightMap.Width + "x" + mod.heightMap.Height + ")");
                                                        //System.IO.File.WriteAllBytes("oldHeightmap.png", mod.heightMap.CompileToTexture().EncodeToPNG());
                                                        //DestroyImmediate(mod.heightMap);
                                                        //mod.heightMap = ScriptableObject.CreateInstance<MapSO>();
                                                        mod.heightMap.CreateMap(MapSO.MapDepth.Greyscale, map);
                                                        DestroyImmediate(map);
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
                                                if (modNode.name.Equals("PQSLandControl") && m.GetType().ToString().Equals(modNode.name))
                                                {
                                                    PQSLandControl mod = m as PQSLandControl;

                                                    if (modNode.HasValue("vHeightMax"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("vHeightMax"), out ftmp))
                                                            mod.vHeightMax = ftmp;
                                                    }
                                                    if (modNode.HasValue("altitudeBlend"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("altitudeBlend"), out ftmp))
                                                            mod.altitudeBlend = ftmp;
                                                    }
                                                    if (modNode.HasValue("altitudeFrequency"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("altitudeFrequency"), out ftmp))
                                                            mod.altitudeFrequency = ftmp;
                                                    }
                                                    if (modNode.HasValue("altitudePersistance"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("altitudePersistance"), out ftmp))
                                                            mod.altitudePersistance = ftmp;
                                                    }
                                                    if (modNode.HasValue("latitudeBlend"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("latitudeBlend"), out ftmp))
                                                            mod.latitudeBlend = ftmp;
                                                    }
                                                    if (modNode.HasValue("latitudeFrequency"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("latitudeFrequency"), out ftmp))
                                                            mod.latitudeFrequency = ftmp;
                                                    }
                                                    if (modNode.HasValue("latitudePersistance"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("latitudePersistance"), out ftmp))
                                                            mod.latitudePersistance = ftmp;
                                                    }
                                                    if (modNode.HasValue("longitudeBlend"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("longitudeBlend"), out ftmp))
                                                            mod.longitudeBlend = ftmp;
                                                    }
                                                    if (modNode.HasValue("longitudeFrequency"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("longitudeFrequency"), out ftmp))
                                                            mod.longitudeFrequency = ftmp;
                                                    }
                                                    if (modNode.HasValue("longitudePersistance"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("longitudePersistance"), out ftmp))
                                                            mod.longitudePersistance = ftmp;
                                                    }
                                                    foreach (ConfigNode lcNode in modNode.GetNodes("LandClass"))
                                                    {
                                                        bool found = false;
                                                        string lcName = lcNode.GetValue("landClassName");
                                                        foreach (PQSLandControl.LandClass lc in mod.landClasses)
                                                        {
                                                            if (lc.landClassName.Equals(lcName))
                                                            {
                                                                found = true;
                                                                if (lcNode.HasValue("color"))
                                                                {
                                                                    Vector4 col = KSPUtil.ParseVector4(lcNode.GetValue("color"));
                                                                    lc.color = new Color(col.x, col.y, col.z, col.w);
                                                                }
                                                                if (lcNode.HasValue("noiseColor"))
                                                                {
                                                                    Vector4 col = KSPUtil.ParseVector4(lcNode.GetValue("noiseColor"));
                                                                    lc.noiseColor = new Color(col.x, col.y, col.z, col.w);
                                                                }

                                                                // ranges
                                                                if(lcNode.HasNode("altitudeRange"))
                                                                {
                                                                    ConfigNode range = lcNode.GetNode("altitudeRange");
                                                                    if (range.HasValue("startStart"))
                                                                        if (double.TryParse(range.GetValue("startStart"), out dtmp))
                                                                            lc.altitudeRange.startStart = dtmp;
                                                                    if (range.HasValue("startEnd"))
                                                                        if (double.TryParse(range.GetValue("startEnd"), out dtmp))
                                                                            lc.altitudeRange.startEnd = dtmp;
                                                                    if (range.HasValue("endStart"))
                                                                        if (double.TryParse(range.GetValue("endStart"), out dtmp))
                                                                            lc.altitudeRange.endStart = dtmp;
                                                                    if (range.HasValue("endEnd"))
                                                                        if (double.TryParse(range.GetValue("endEnd"), out dtmp))
                                                                            lc.altitudeRange.endEnd = dtmp;
                                                                }
                                                                if (lcNode.HasNode("latitudeRange"))
                                                                {
                                                                    ConfigNode range = lcNode.GetNode("latitudeRange");
                                                                    if (range.HasValue("startStart"))
                                                                        if (double.TryParse(range.GetValue("startStart"), out dtmp))
                                                                            lc.latitudeRange.startStart = dtmp;
                                                                    if (range.HasValue("startEnd"))
                                                                        if (double.TryParse(range.GetValue("startEnd"), out dtmp))
                                                                            lc.latitudeRange.startEnd = dtmp;
                                                                    if (range.HasValue("endStart"))
                                                                        if (double.TryParse(range.GetValue("endStart"), out dtmp))
                                                                            lc.latitudeRange.endStart = dtmp;
                                                                    if (range.HasValue("endEnd"))
                                                                        if (double.TryParse(range.GetValue("endEnd"), out dtmp))
                                                                            lc.latitudeRange.endEnd = dtmp;
                                                                }
                                                                if (lcNode.HasNode("longitudeRange"))
                                                                {
                                                                    ConfigNode range = lcNode.GetNode("longitudeRange");
                                                                    if (range.HasValue("startStart"))
                                                                        if (double.TryParse(range.GetValue("startStart"), out dtmp))
                                                                            lc.longitudeRange.startStart = dtmp;
                                                                    if (range.HasValue("startEnd"))
                                                                        if (double.TryParse(range.GetValue("startEnd"), out dtmp))
                                                                            lc.longitudeRange.startEnd = dtmp;
                                                                    if (range.HasValue("endStart"))
                                                                        if (double.TryParse(range.GetValue("endStart"), out dtmp))
                                                                            lc.longitudeRange.endStart = dtmp;
                                                                    if (range.HasValue("endEnd"))
                                                                        if (double.TryParse(range.GetValue("endEnd"), out dtmp))
                                                                            lc.longitudeRange.endEnd = dtmp;
                                                                }
                                                                if (lcNode.HasValue("latitudeDouble"))
                                                                {
                                                                    if (bool.TryParse(lcNode.GetValue("latitudeDouble"), out btmp))
                                                                        lc.latitudeDouble = btmp;
                                                                }
                                                                if (lcNode.HasValue("minimumRealHeight"))
                                                                    if (double.TryParse(lcNode.GetValue("minimumRealHeight"), out dtmp))
                                                                        lc.minimumRealHeight = dtmp;
                                                                if (lcNode.HasValue("alterRealHeight"))
                                                                    if (double.TryParse(lcNode.GetValue("alterRealHeight"), out dtmp))
                                                                        lc.alterRealHeight = dtmp;
                                                                if (lcNode.HasValue("alterApparentHeight"))
                                                                    if (float.TryParse(lcNode.GetValue("alterApparentHeight"), out ftmp))
                                                                        lc.alterApparentHeight = ftmp;


                                                                break; // don't need to find any more
                                                            }
                                                        }
                                                        if (!found)
                                                            print("*RSS* LandClass " + lcName + " not found in PQSLandControl for PQS " + p.name + " of CB " + body.name);
                                                    }
                                                    mod.OnSetup();
                                                }

                                                // City
                                                if (modNode.name.Equals("PQSCity") && m.GetType().ToString().Equals(modNode.name))
                                                {
                                                    PQSCity mod = m as PQSCity;
                                                    if (modNode.HasValue("KEYname"))
                                                        if (!(mod.name.Equals(modNode.GetValue("KEYname"))))
                                                            continue;

                                                    if (modNode.HasValue("repositionRadial"))
                                                    {
                                                        mod.repositionRadial = KSPUtil.ParseVector3(modNode.GetValue("repositionRadial"));
                                                    }
                                                    if (modNode.HasValue("latitude") && modNode.HasValue("longitude"))
                                                    {
                                                        double lat, lon;
                                                        double.TryParse(modNode.GetValue("latitude"), out lat);
                                                        double.TryParse(modNode.GetValue("longitude"), out lon);

                                                        mod.repositionRadial = LLAtoECEF(lat, lon, 0, body.Radius);
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
                                                    if (modNode.HasValue("repositionToSphereSurfaceAddHeight"))
                                                    {
                                                        if (bool.TryParse(modNode.GetValue("repositionToSphereSurfaceAddHeight"), out btmp))
                                                            mod.repositionToSphereSurfaceAddHeight = btmp;
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

                                                    if (modNode.HasValue("reorientFinalAngle"))
                                                    {
                                                        if (float.TryParse(modNode.GetValue("reorientFinalAngle"), out ftmp))
                                                            mod.reorientFinalAngle = ftmp;
                                                    }
                                                        
                                                    mod.OnSetup();
                                                }
                                                // KSC Flat area
                                                if(modNode.name.Equals("PQSMod_MapDecalTangent")  && m.GetType().ToString().Equals(modNode.name))
                                                {
                                                    // thanks to asmi for this!
                                                    PQSMod_MapDecalTangent mod = m as PQSMod_MapDecalTangent;
                                                    if (modNode.HasValue("position"))
                                                    {
                                                        mod.position = KSPUtil.ParseVector3(modNode.GetValue("position"));
                                                    }
                                                    if (modNode.HasValue("radius"))
                                                    {
                                                        if (double.TryParse(modNode.GetValue("radius"), out dtmp))
                                                            mod.radius = dtmp;
                                                    }
                                                    if (modNode.HasValue("heightMapDeformity"))
                                                    {
                                                        if (double.TryParse(modNode.GetValue("heightMapDeformity"), out dtmp))
                                                            mod.heightMapDeformity = dtmp;
                                                    }
                                                    if (modNode.HasValue("absoluteOffset"))
                                                    {
                                                        if (double.TryParse(modNode.GetValue("absoluteOffset"), out dtmp))
                                                            mod.absoluteOffset = dtmp;
                                                    }

                                                    if (modNode.HasValue("absolute"))
                                                    {
                                                        if (bool.TryParse(modNode.GetValue("absolute"), out btmp))
                                                            mod.absolute = btmp;
                                                    }
                                                    if(modNode.HasValue("rescaleToRadius"))
                                                    {
                                                        mod.position *= (float)(body.Radius / origRadius);
                                                        mod.radius *= (body.Radius / origRadius);
                                                    }
                                                    if (modNode.HasValue("latitude") && modNode.HasValue("longitude"))
                                                    {
                                                        double lat, lon;
                                                        double.TryParse(modNode.GetValue("latitude"), out lat);
                                                        double.TryParse(modNode.GetValue("longitude"), out lon);

                                                        mod.position = LLAtoECEF(lat, lon, 0, body.Radius);
                                                    }
                                                    mod.OnSetup();
                                                }
                                            }
                                        }
                                        if (pqsNode.HasNode("Add"))
                                        {
                                            foreach (ConfigNode modNode in pqsNode.GetNode("Add").nodes)
                                            {
                                                if (modNode.name.Equals("PQSMod_VertexColorMapBlend"))
                                                {
                                                    /*CelestialBody cbDuna = null;
                                                    foreach (CelestialBody b in FlightGlobals.Bodies)
                                                        if (b.name.Equals("Duna"))
                                                            cbDuna = b;

                                                    PQSMod_VertexColorMapBlend dunaColor = cbDuna.transform.GetComponentInChildren<PQSMod_VertexColorMapBlend>();*/
                                                    GameObject tempObj = new GameObject();


                                                    PQSMod_VertexColorMapBlend colorMap = (PQSMod_VertexColorMapBlend)tempObj.AddComponent(typeof(PQSMod_VertexColorMapBlend));

                                                    tempObj.transform.parent = p.gameObject.transform;
                                                    colorMap.sphere = p;

                                                    Texture2D map = new Texture2D(4, 4, TextureFormat.RGB24, false);
                                                    map.LoadImage(System.IO.File.ReadAllBytes(modNode.GetValue("vertexColorMap")));
                                                    colorMap.vertexColorMap = ScriptableObject.CreateInstance<MapSO>();
                                                    colorMap.vertexColorMap.CreateMap(MapSO.MapDepth.RGB, map);
                                                        
                                                    colorMap.blend = 1.0f;
                                                    if (modNode.HasValue("blend"))
                                                        if (float.TryParse(modNode.GetValue("blend"), out ftmp))
                                                            colorMap.blend = ftmp;
                                                        
                                                    colorMap.order = 9999993;
                                                    if (modNode.HasValue("order"))
                                                        if (int.TryParse(modNode.GetValue("order"), out itmp))
                                                            colorMap.order = itmp;

                                                    colorMap.modEnabled = true;
                                                    DestroyImmediate(map);
                                                }
                                            }
                                        }
                                        if (pqsNode.HasNode("Disable"))
                                        {
                                            foreach (ConfigNode modNode in pqsNode.GetNode("Disable").nodes)
                                            {
                                                if (modNode.name.Equals("PQSLandControl"))
                                                {
                                                    List<PQSLandControl> modList = p.transform.GetComponentsInChildren<PQSLandControl>(true).ToList();
                                                    foreach (PQSLandControl m in modList)
                                                    {
                                                        m.modEnabled = false;
                                                    }
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
                        // Scaled space
                        Transform scaledSpaceTransform = null;
                        Transform atmo = null;
                        if (ScaledSpace.Instance != null)
                        {
                            float SSTScale = 1.0f;
                            node.TryGetValue("SSTScale", ref SSTScale);
                            foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                            {
                                if (t.name.Equals(node.name))
                                {
                                    scaledSpaceTransform = t;
                                    // replace
                                    int replaceColor = 0;
                                    string path = "";
                                    if (node.HasValue("SSColor"))
                                    {
                                        replaceColor = 1;
                                        path = node.GetValue("SSColor");
                                    }
                                    if (node.HasValue("SSColor32"))
                                    {
                                        replaceColor = 2;
                                        path = node.GetValue("SSColor32");
                                    }
                                    if (replaceColor > 0)
                                    {
                                        Texture2D map = new Texture2D(4, 4, replaceColor == 1 ? TextureFormat.RGB24: TextureFormat.RGBA32, true);
                                        map.LoadImage(System.IO.File.ReadAllBytes(path));
                                        map.Compress(true);
                                        map.Apply(true, true);
                                        Texture oldColor = t.gameObject.renderer.material.GetTexture("_MainTex");
                                        foreach(Material m in Resources.FindObjectsOfTypeAll(typeof(Material)))
                                        {
                                            if(m.GetTexture("_MainTex") == oldColor)
                                                m.SetTexture("_MainTex", map);
                                        }
                                        DestroyImmediate(oldColor);
                                        // shouldn't be needed - t.gameObject.renderer.material.SetTexture("_MainTex", map);
                                    }
                                    if (node.HasValue("SSBump"))
                                    {
                                        Texture2D map = new Texture2D(4, 4, TextureFormat.RGB24, true);
                                        map.LoadImage(System.IO.File.ReadAllBytes(node.GetValue("SSBump")));
                                        if(compressNormals)
                                            map.Compress(true);
                                        map.Apply(true, true);
                                        Texture oldBump = t.gameObject.renderer.material.GetTexture("_BumpMap");
                                        if (oldBump != null)
                                        {
                                            foreach (Material m in Resources.FindObjectsOfTypeAll(typeof(Material)))
                                            {
                                                if (m.GetTexture("_BumpMap") == oldBump)
                                                    m.SetTexture("_BumpMap", map);
                                            }
                                            DestroyImmediate(oldBump);
                                        }
                                    }

                                    // Fix mesh
                                    bool rescale = true;
                                    bool doWrapHere = doWrap;
                                    node.TryGetValue("wrap", ref doWrapHere);
                                    bool sphereVal = spheresOnly;
                                    bool sphereHere = node.TryGetValue("useSphericalSSM", ref sphereVal);
                                    float origLocalScale = t.localScale.x; // assume uniform scale
                                    if (body.pqsController != null && doWrapHere)
                                    {
                                        MeshFilter m = (MeshFilter)t.GetComponent(typeof(MeshFilter));
                                        if (m == null || m.mesh == null)
                                        {
                                            print("*RSS* Failure getting SSM for " + body.pqsController.name + ": mesh is null");
                                        }
                                        else
                                        {
                                            if (sphereVal)
                                            {
                                                Mesh tMesh = new Mesh();
                                                Utils.CopyMesh(joolMesh.mesh, tMesh); 
                                                print("*RSS* using Jool scaledspace mesh (spherical) for body " + body.pqsController.name);
                                                float scaleFactor = (float)(origRadius / 6000000.0 / origLocalScale); // scale mesh to original CB's size
                                                Utils.ScaleVerts(tMesh, scaleFactor);
                                                tMesh.RecalculateBounds();
                                                m.mesh = tMesh;
                                                // do normal rescaling below.
	                                        }
	                                        else
	                                        {
	                                            char sep = System.IO.Path.DirectorySeparatorChar;
	                                            string filePath = KSPUtil.ApplicationRootPath + sep + "GameData" + sep + "RealSolarSystem" + sep + "Plugins"
	                                                        + sep + "PluginData" + sep + t.name;

                                                filePath += ".obj";
                                                bool wrap = true;
	                                            try
	                                            {
	                                                if (File.Exists(filePath))
	                                                {
                                                        ProfileTimer.Push("LoadSSM_" + body.name);
                                                        Mesh tMesh = new Mesh();
                                                        Utils.CopyMesh(joolMesh.mesh, tMesh);

                                                        float scaleFactor = (float)(origRadius / body.Radius / origLocalScale); // scale mesh to original CB's size

                                                        ObjLib.UpdateMeshFromFile(tMesh, filePath, scaleFactor);
                                                        tMesh.RecalculateBounds();
                                                        m.mesh = tMesh;
                                                        wrap = false;
                                                        // do normal rescaling below.
                                                        print("*RSS* Loaded " + filePath + " and wrapped.");
                                                        ProfileTimer.Pop("LoadSSM_" + body.name);
	                                                }
	                                            }
	                                            catch (Exception e)
	                                            {
	                                                print("Exception loading mesh " + filePath + " for " + t.name + ": " + e.Message);
	                                            }
	                                            if (wrap)
	                                            {
	                                                try
	                                                {
	                                                    print("*RSS* wrapping ScaledSpace mesh " + m.name + " to PQS " + body.pqsController.name);
                                                        ProfileTimer.Push("WrapSSM_" + body.name);
                                                        Mesh tMesh = new Mesh();
                                                        Utils.CopyMesh(joolMesh.mesh, tMesh);
                                                        Utils.MatchVerts(tMesh, body.pqsController, body.ocean ? body.Radius : 0.0);
                                                        tMesh.RecalculateNormals();
                                                        ObjLib.UpdateTangents(tMesh);
	                                                    print("*RSS* wrapped.");
	                                                    try
	                                                    {
	                                                        ObjLib.MeshToFile(m, filePath);
	                                                    }
	                                                    catch (Exception e)
	                                                    {
	                                                        print("*RSS* Exception saving wrapped mesh " + filePath + ": " + e.Message);
	                                                    }
	                                                    print("*RSS*: Done wrapping and exporting. Setting scale");
                                                        float scaleFactor = (float)(origRadius / body.Radius / origLocalScale); // scale mesh to original CB's size
                                                        Utils.ScaleVerts(tMesh, scaleFactor);
                                                        tMesh.RecalculateBounds();
                                                        m.mesh = tMesh;
                                                        // do normal rescaling below.
                                                        ProfileTimer.Pop("WrapSSM_" + body.name);
	                                                }
	                                                catch (Exception e)
	                                                {
	                                                    print("*RSS* Exception wrapping: " + e.Message);
                                                    }
                                                }
                                            }
                                        }
                                        atmo = t.FindChild("Atmosphere");
                                    }
                                    if (rescale)
                                    {
                                        float scaleFactor = (float)(body.Radius / origRadius * SSTScale);
                                        t.localScale = new Vector3(t.localScale.x * scaleFactor, t.localScale.y * scaleFactor, t.localScale.z * scaleFactor);
                                    }
                                    else
                                    {
                                        // rescale only atmo
                                        if (atmo != null)
                                        {
                                            print("*RSS* found atmo transform for " + node.name);
                                            float scaleFactor = SSAtmoScale; // default to global default
                                            if (!node.TryGetValue("SSAtmoScale", ref scaleFactor)) // if no override multiplier
                                            {
                                                if (defaultAtmoScale) // use stock KSP multiplier
                                                    scaleFactor *= 1.025f;
                                                else // or use atmosphere height-dependent multiplier
                                                    scaleFactor *= (float)((body.Radius + body.maxAtmosphereAltitude) / body.Radius);
                                            }
                                            scaleFactor *= origLocalScale / t.localScale.x * (float)(body.Radius / origRadius); // since our parent transform changed, we no longer are the same scale as the planet.
                                            atmo.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                                            print("*RSS* final scale of atmo for " + body.name + " in scaledspace: " + atmo.localScale.x);
                                        }
                                    }
                                    print("*RSS* final scale of " + body.name + " in scaledspace: " + t.localScale.x);
                                }
                            }
                        }
                        foreach (AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                        {
                            //print("*RSS* Found AG " + ag.name + " " + (ag.tag == null ? "" : ag.tag) + ". Planet " + (ag.planet == null ? "NULL" : ag.planet.name));
                            if (ag != null && ag.planet != null)
                            {
                                // generalized version of Starwaster's code. Thanks Starwaster!
                                if (ag.planet.name.Equals(node.name))
                                {
                                    print("Found atmo for " + node.name + ": " + ag.name);
                                    if (node.HasNode("AtmosphereFromGround"))
                                    {
                                        ConfigNode modNode = node.GetNode("AtmosphereFromGround");
                                        if (modNode.TryGetValue("outerRadius", ref ag.outerRadius))
                                        {
                                            ag.outerRadius *= ScaledSpace.InverseScaleFactor;
                                        }
                                        else if (modNode.HasValue("outerRadiusAtmo"))
                                        {
                                            ag.outerRadius = ((float)body.Radius + body.maxAtmosphereAltitude) * ScaledSpace.InverseScaleFactor;
                                        }
                                        else if (modNode.TryGetValue("outerRadiusMult", ref ag.outerRadius))
                                        {
                                            ag.outerRadius *= (float)body.Radius * ScaledSpace.InverseScaleFactor;
                                        }

                                        if (modNode.TryGetValue("innerRadius", ref ag.innerRadius))
                                        {
                                            ag.innerRadius *= ScaledSpace.InverseScaleFactor;
                                        }
                                        else if (modNode.TryGetValue("innerRadiusMult", ref ag.innerRadius))
                                        {
                                            ag.innerRadius *= ag.outerRadius;
                                        }
                                        modNode.TryGetValue("doScale", ref ag.doScale);
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
                                            Vector4 col = KSPUtil.ParseVector4(modNode.GetValue("invWaveLength"));
                                            ag.invWaveLength = new Color(col.x, col.y, col.z, col.w);
                                        }
                                        if (modNode.HasValue("waveLength"))
                                        {
                                            Vector4 col = KSPUtil.ParseVector4(modNode.GetValue("waveLength"));
                                            ag.waveLength = new Color(col.x, col.y, col.z, col.w);
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
                                    MethodInfo setMaterial = ag.GetType().GetMethod("SetMaterial", BindingFlags.NonPublic | BindingFlags.Instance);
                                    setMaterial.Invoke(ag, new object[] { true });

                                    print("Atmo updated");
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
                                    spNode.TryGetValue("LandedDataValue", ref body.scienceValues.LandedDataValue);
                                    spNode.TryGetValue("SplashedDataValue", ref body.scienceValues.SplashedDataValue);
                                    spNode.TryGetValue("FlyingLowDataValue", ref body.scienceValues.FlyingLowDataValue);
                                    spNode.TryGetValue("FlyingHighDataValue", ref body.scienceValues.FlyingHighDataValue);
                                    spNode.TryGetValue("InSpaceLowDataValue", ref body.scienceValues.InSpaceLowDataValue);
                                    spNode.TryGetValue("InSpaceHighDataValue", ref body.scienceValues.InSpaceHighDataValue);
                                    spNode.TryGetValue("RecoveryValue", ref body.scienceValues.RecoveryValue);
                                    spNode.TryGetValue("flyingAltitudeThreshold", ref body.scienceValues.flyingAltitudeThreshold);
                                    spNode.TryGetValue("spaceAltitudeThreshold", ref body.scienceValues.spaceAltitudeThreshold);
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
                                    if (exportNode.HasValue("ocean"))
                                    {
                                        if (bool.TryParse(exportNode.GetValue("ocean"), out btmp))
                                            ocean = btmp;
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
                                    Texture2D[] outputMaps = bodyPQS.CreateMaps(res, maxHeight, ocean, oceanHeight, oceanColor);
                                    System.IO.File.WriteAllBytes(KSPUtil.ApplicationRootPath + System.IO.Path.DirectorySeparatorChar + body.name + "1.png", outputMaps[0].EncodeToPNG());
                                    System.IO.File.WriteAllBytes(KSPUtil.ApplicationRootPath + System.IO.Path.DirectorySeparatorChar + body.name + "2.png", outputMaps[1].EncodeToPNG());
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
            doneRSS = true;
        }
    }

}

