using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
    // Will fix the Planetarium camera zoom limits, the orbital lines, and the VAB limits
    // Also debug-prints some info abotu AtmosphereFromSpace
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class CameraFixer : MonoBehaviour
    {
        public static bool ready = false;
        public void Start()
        {
            foreach (AtmosphereFromSpace afs in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromSpace)))
            {
                try
                {
                    print("Found afs " + afs.name + ", tag " + afs.tag);
                    print("   Parent: " + afs.transform.parent.name);
                }
                catch (Exception e)
                {
                    print("Failed, " + e.Message);
                }
            }
            if (HighLogic.LoadedScene.Equals(GameScenes.MAINMENU))
                ready = true;
            if (!ready)
                return;

            if (HighLogic.LoadedSceneHasPlanetarium && PlanetariumCamera.fetch)
            {
                print("*RSS* Fixing PCam. Min " + PlanetariumCamera.fetch.minDistance + ", Max " + PlanetariumCamera.fetch.maxDistance + ". Start " + PlanetariumCamera.fetch.startDistance + ", zoom " + PlanetariumCamera.fetch.zoomScaleFactor);
                PlanetariumCamera.fetch.maxDistance = 1e10f;
                print("Fixed. Min " + PlanetariumCamera.fetch.minDistance + ", Max " + PlanetariumCamera.fetch.maxDistance + ". Start " + PlanetariumCamera.fetch.startDistance + ", zoom " + PlanetariumCamera.fetch.zoomScaleFactor);
            }
            // HoneyFox
            if (HighLogic.LoadedSceneHasPlanetarium && MapView.fetch != null)
            {
                try
                {
                    ConfigNode camNode = null;
                    foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS"))
                        camNode = node;
                    if (camNode != null)
                    {
                        camNode.TryGetValue("max3DlineDrawDist", ref MapView.fetch.max3DlineDrawDist);
                    }
                }
                catch (Exception e)
                {
                    print("MapView fixing failed: " + e.Message);
                }
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
                            camNode.TryGetValue("VABmaxHeight", ref c.maxHeight);
                            camNode.TryGetValue("VABmaxDistance", ref c.maxDistance);
                            camNode.TryGetValue("VABminDistance", ref c.minDistance);
                        }

                        foreach (SPHCamera c in Resources.FindObjectsOfTypeAll(typeof(SPHCamera)))
                        {
                            //print("SPH camera " + c.name + " has maxHeight = " + c.maxHeight + ", maxDistance = " + c.maxDistance + ", scrollHeight = " + c.scrollHeight);
                            camNode.TryGetValue("SPHmaxDistance", ref c.maxDistance);
                            camNode.TryGetValue("SPHminDistance", ref c.minDistance);
                        }
                        if (camNode.HasValue("editorExtentsMult"))
                            if (float.TryParse(camNode.GetValue("editorExtentsMult"), out ftmp))
                                EditorLogic.fetch.editorBounds.extents *= ftmp; // thanks, asmi!
                    }
                }
                catch (Exception e)
                {
                    print("Camera fixing failed: " + e.Message);
                }
            }
            if(HighLogic.LoadedScene == GameScenes.SPACECENTER) {
                PQSCity ksc = null;
                foreach(PQSCity city in Resources.FindObjectsOfTypeAll(typeof(PQSCity))) {
                    if(city.name == "KSC") {
                        ksc = city;
                        break;
                    }
                }
                if(ksc == null) {
                    Debug.Log("*RSS* could not find KSC to fix the camera.");
                    return;
                }
                foreach(SpaceCenterCamera2 cam in Resources.FindObjectsOfTypeAll(typeof(SpaceCenterCamera2))) {
                    if(ksc.repositionToSphere || ksc.repositionToSphereSurface) {
                        CelestialBody Kerbin = FlightGlobals.Bodies.Find(body => body.name == ksc.sphere.name);
                        if(Kerbin == null) {
                            Debug.Log("*RSS* could not find find the CelestialBody specified as KSC's sphere.");
                            return;
                        }
                        double nomHeight = Kerbin.pqsController.GetSurfaceHeight((Vector3d) ksc.repositionRadial.normalized) - Kerbin.Radius;
                        if(ksc.repositionToSphereSurface) {
                            nomHeight += ksc.repositionRadiusOffset;
                        }
                        cam.altitudeInitial = 0f - (float) nomHeight;
                    } else {
                        cam.altitudeInitial = 0f - (float) ksc.repositionRadiusOffset;
                    }
                    cam.ResetCamera();
                    Debug.Log("*RSS* fixed the Space Center camera.");
                }
            }
        }
    }
}
