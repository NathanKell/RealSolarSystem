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
        protected bool isCompatible = true;
        public static bool ready = false;
        public void Start()
        {
            if (!CompatibilityChecker.IsAllCompatible())
            {
                isCompatible = false;
                return;
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
                        float ftmp = 1f;
                        camNode.TryGetValue("editorMaxDistance", ref EditorBounds.Instance.cameraMaxDistance);
                        camNode.TryGetValue("editorMinDistance", ref EditorBounds.Instance.cameraMinDistance);
                        /*if (camNode.TryGetValue("editorExtentsMult", ref ftmp))
                                EditorBounds.Instance.constructionBounds.extents *= ftmp;*/
                    }
                }
                catch (Exception e)
                {
                    print("Editor camera fixing failed: " + e.Message);
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
    //Agathorn
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class KSCReset : MonoBehaviour
    {
        protected bool isCompatible = true;
        public static bool shouldCameraBeReset = true;
        public void Start()
        {
            if (!CompatibilityChecker.IsAllCompatible())
            {
                isCompatible = false;
                return;
            }
            if (shouldCameraBeReset)
            {
                //HighLogic.LoadScene(GameScenes.SPACECENTER);
                // Disabled until further testing; causes Exception-spam from the KSC hit detector.
                shouldCameraBeReset = false;
            }
        }
    }

}
