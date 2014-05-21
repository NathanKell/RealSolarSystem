using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
    public class Utils : MonoBehaviour
    {
        public static void MatchVerts(Mesh mesh, PQS pqs, double oceanHeight)
        {
            char sep = System.IO.Path.DirectorySeparatorChar;
            string filePath = KSPUtil.ApplicationRootPath + sep + "GameData" + sep + "RealSolarSystem" + sep + "Plugins"
                        + sep + "PluginData" + sep + pqs.name + "_match.txt";
            pqs.isBuildingMaps = true;

            Vector3[] vertices = new Vector3[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 v = mesh.vertices[i];
                double height = pqs.GetSurfaceHeight(v);
                if (height < oceanHeight)
                    height = oceanHeight;
                vertices[i] = v.normalized * (float)(1000.0 / 6000000.0 * height);
            }
            pqs.isBuildingMaps = false;
            mesh.vertices = vertices;
        }
        public static void CopyMesh(Mesh source, Mesh dest)
        {
            /*char sep = System.IO.Path.DirectorySeparatorChar;
            string filePath = KSPUtil.ApplicationRootPath + sep + "GameData" + sep + "RealSolarSystem" + sep + "Plugins"
                        + sep + "PluginData" + sep + source.name + "_copy.txt";
            using (StreamWriter sw = new StreamWriter(filePath))
            {*/
                Vector3[] verts = new Vector3[source.vertexCount];
                for (int i = 0; i < source.vertexCount; i++)
                {
                    verts[i].x = source.vertices[i].x;
                    verts[i].y = source.vertices[i].y;
                    verts[i].z = source.vertices[i].z;
                }
                dest.vertices = verts;

                int[] tris = new int[source.triangles.Length];
                for (int i = 0; i < source.triangles.Length; i++)
                    tris[i] = source.triangles[i];
                dest.triangles = tris;

                Vector2[] uvs = new Vector2[source.uv.Length];
                for (int i = 0; i < source.uv.Length; i++)
                {
                    uvs[i].x = source.uv[i].x;
                    uvs[i].y = source.uv[i].y;
                }
                dest.uv = uvs;

                Vector2[] uv2s = new Vector2[source.uv2.Length];
                for (int i = 0; i < source.uv2.Length; i++)
                {
                    uv2s[i].x = source.uv2[i].x;
                    uv2s[i].y = source.uv2[i].y;
                }
                dest.uv2 = uv2s;

                Vector3[] normals = new Vector3[source.normals.Length];
                for (int i = 0; i < source.normals.Length; i++)
                {
                    normals[i].x = source.normals[i].x;
                    normals[i].y = source.normals[i].y;
                    normals[i].z = source.normals[i].z;
                }
                dest.normals = normals;

                Vector4[] tangents = new Vector4[source.tangents.Length];
                for (int i = 0; i < source.tangents.Length; i++)
                {
                    tangents[i].w = source.tangents[i].w;
                    tangents[i].x = source.tangents[i].x;
                    tangents[i].y = source.tangents[i].y;
                    tangents[i].z = source.tangents[i].z;
                }
                dest.tangents = tangents;

                Color[] colors = new Color[source.colors.Length];
                for (int i = 0; i < source.colors.Length; i++)
                {
                    colors[i].r = source.colors[i].r;
                    colors[i].g = source.colors[i].g;
                    colors[i].b = source.colors[i].b;
                    colors[i].a = source.colors[i].a;
                }
                dest.colors = colors;

                Color32[] colors32 = new Color32[source.colors32.Length];
                for (int i = 0; i < source.colors32.Length; i++)
                {
                    colors32[i].r = source.colors32[i].r;
                    colors32[i].g = source.colors32[i].g;
                    colors32[i].b = source.colors32[i].b;
                    colors32[i].a = source.colors32[i].a;
                }
                dest.colors32 = colors32;
            //}
        }
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
            print("Scale = (" + t.localScale.x + ", " + t.localScale.y + ", " + t.localScale.z + ")");
            print("Pos = (" + t.position.x + ", " + t.position.y + ", " + t.position.z + "); lPos = (" + t.localPosition.x + ", " + t.localPosition.y + ", " + t.localPosition.z + ")");
            PrintComponents(t);
        }
        public static void DumpAllSST()
        {
            print("*RSS* Dumping ScaledSpace");
            if (ScaledSpace.Instance != null)
            {
                foreach (Transform t in ScaledSpace.Instance.scaledSpaceTransforms)
                    DumpSST(t);
            }
        }
        public static void PrintComponents(Transform t)
        {
            print("Transform " + t.name + " has components:");
            foreach (Component c in t.GetComponents(typeof(Component)).ToList())
                print(c.name + " (" + c.GetType() + ")");
        }
        public static void PrintTransformRecursive(Transform t)
        {
            PrintComponents(t);
            for (int i = 0; i < t.transform.childCount; i++)
                PrintTransformRecursive(t.transform.GetChild(i));
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
}
