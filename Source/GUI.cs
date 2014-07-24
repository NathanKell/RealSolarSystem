using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
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
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("AFG EDITOR");
            GUILayout.EndHorizontal();
            if (afg != null)
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
                string inner;
                string outer;
                float rf;
                float gf;
                float bf;
                float Esunf;
                float Krf;
                float Kmf;
                float innerf;
                float outerf;

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
                GUILayout.BeginHorizontal();
                GUILayout.Label("InnerR");
                inner = GUILayout.TextField(afg.innerRadius.ToString(), 15);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("OuterR");
                outer = GUILayout.TextField(afg.outerRadius.ToString(), 15);
                GUILayout.EndHorizontal();


                /*if (GUI.changed)
                {*/
                    float.TryParse(rt, out rf);
                    float.TryParse(gt, out gf);
                    float.TryParse(bt, out bf);
                    float.TryParse(ESunt, out Esunf);
                    float.TryParse(Krt, out Krf);
                    float.TryParse(Kmt, out Kmf);
                    float.TryParse(inner, out innerf);
                    float.TryParse(outer, out outerf);
                //}
                if (GUILayout.Button("Apply"))
                {
                    try
                    {
                        afg.waveLength.r = rf;
                        afg.waveLength.g = gf;
                        afg.waveLength.b = bf;
                        afg.ESun = Esunf;
                        afg.Km = Kmf;
                        afg.Kr = Krf;
                        afg.KrESun = Krf * Esunf;
                        afg.KmESun = Kmf * Esunf;
                        afg.Kr4PI = Krf * 4f * (float)Math.PI;
                        afg.Km4PI = Kmf * 4f * (float)Math.PI;
                        afg.innerRadius = innerf * ScaledSpace.InverseScaleFactor;
                        afg.outerRadius = outerf * ScaledSpace.InverseScaleFactor;

                        // compute relations
                        afg.scale = 1f / (afg.outerRadius - afg.innerRadius);
                        afg.scaleDepth = -0.25f;
                        afg.scaleOverScaleDepth = afg.scale / afg.scaleDepth;
                        afg.outerRadius2 = afg.outerRadius * afg.outerRadius;
                        afg.innerRadius2 = afg.innerRadius * afg.innerRadius;

                        // set params
                        MethodInfo setMaterial = afg.GetType().GetMethod("SetMaterial", BindingFlags.NonPublic | BindingFlags.Instance);
                        setMaterial.Invoke(afg, new object[] { true });
                    }
                    catch (Exception e)
                    {
                        print("*RSS* *ERROR* setting AtmosphereFromGround " + afg.name + " for body " + FlightGlobals.getMainBody().name + ": " + e);
                    }
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No atmosphere");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("for current body");
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
