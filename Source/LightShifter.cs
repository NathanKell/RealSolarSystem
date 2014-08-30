/*
 * LightShifter class provided courtesy of NovaSilisko, from Alternis Kerbol, used with permission.
 * Modified by Justin Bengtson.
 */

using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using UnityEngine;
 
namespace RealSolarSystem {
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class LightShifter : MonoBehaviour {
        void Start() {
            //The lights are instantiated on each scene startup, unlike planets which instantiate at the beginning of the game
            //so a more specific check has to be performed
            if(HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT) {
	            ConfigNode RSSSettings = null;

	            foreach(ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS")) {
	                RSSSettings = node;
				}
	
	            if(RSSSettings == null) {
	                print("*RSS* REALSOLARSYSTEMSETTINGS node not found, could not load light settings!");
					return;
				}

                GameObject sunLight = GameObject.Find("SunLight");
                GameObject scaledSunLight = GameObject.Find("Scaledspace SunLight");
 
                if(sunLight != null) {
                    print("*RSS* LightShifter: Found sunlight and scaled sunlight, shifting...");
 
	                if(RSSSettings.HasValue("sunlightColor")) {
	                    try {
	                        Vector4 col = KSPUtil.ParseVector4(RSSSettings.GetValue("sunlightColor"));
	                        Color c = new Color(col.x, col.y, col.z, col.w);
		                    sunLight.light.color = c;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as color4: original text: " + RSSSettings.GetValue("sunlightColor") + " --- exception " + e.Message);
	                    }
	                }

	                if(RSSSettings.HasValue("sunlightIntensity")) {
	                    try {
	                        float f = float.Parse(RSSSettings.GetValue("sunlightIntensity"));
		                    sunLight.light.intensity = f;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as float: original text: " + RSSSettings.GetValue("sunlightIntensity") + " --- exception " + e.Message);
	                    }
	                }

	                if(RSSSettings.HasValue("sunlightShadowStrength")) {
	                    try {
	                        float f = float.Parse(RSSSettings.GetValue("sunlightShadowStrength"));
                    		sunLight.light.shadowStrength = f;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as float: original text: " + RSSSettings.GetValue("sunlightShadowStrength") + " --- exception " + e.Message);
	                    }
	                }

	                if(RSSSettings.HasValue("scaledSunlightColor")) {
	                    try {
	                        Vector4 col = KSPUtil.ParseVector4(RSSSettings.GetValue("scaledSunlightColor"));
	                        Color c = new Color(col.x, col.y, col.z, col.w);
		                    scaledSunLight.light.color = c;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as color4: original text: " + RSSSettings.GetValue("scaledSunlightColor") + " --- exception " + e.Message);
	                    }
	                }

	                if(RSSSettings.HasValue("scaledSunlightIntensity")) {
	                    try {
	                        float f = float.Parse(RSSSettings.GetValue("scaledSunlightIntensity"));
                    		scaledSunLight.light.intensity = f;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as float: original text: " + RSSSettings.GetValue("scaledSunlightIntensity") + " --- exception " + e.Message);
	                    }
	                }
                } else {
                    print("*RSS* LightShifter: Couldn't find either sunlight or scaled sunlight");
                }
 
                if(HighLogic.LoadedScene == GameScenes.FLIGHT) {
                    GameObject IVASun = GameObject.Find("IVASun");
 
                    if(IVASun != null) {
                        print("LightShifter: Found IVA sun, shifting...");

		                if(RSSSettings.HasValue("IVASunColor")) {
		                    try {
		                        Vector4 col = KSPUtil.ParseVector4(RSSSettings.GetValue("IVASunColor"));
		                        Color c = new Color(col.x, col.y, col.z, col.w);
			                    IVASun.light.color = c;
		                    } catch(Exception e) {
		                        print("*RSS* Error parsing as color4: original text: " + RSSSettings.GetValue("IVASunColor") + " --- exception " + e.Message);
		                    }
		                }

		                if(RSSSettings.HasValue("IVASunIntensity")) {
		                    try {
		                        float f = float.Parse(RSSSettings.GetValue("IVASunIntensity"));
	                    		IVASun.light.intensity = f;
		                    } catch(Exception e) {
		                        print("*RSS* Error parsing as float: original text: " + RSSSettings.GetValue("IVASunIntensity") + " --- exception " + e.Message);
		                    }
		                }
                    } else {
                        print("*RSS* LightShifter: No IVA sun found.");
                    }
                }
 
                LensFlare sunLightFlare = sunLight.gameObject.GetComponent<LensFlare>();
 
                if(sunLightFlare != null) {
                    print("*RSS* LightShifter: Shifting LensFlare");

	                if(RSSSettings.HasValue("sunlightLensFlareColor")) {
	                    try {
	                        Vector4 col = KSPUtil.ParseVector4(RSSSettings.GetValue("sunlightLensFlareColor"));
	                        Color c = new Color(col.x, col.y, col.z, col.w);
		                    sunLightFlare.color = c;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as color4: original text: " + RSSSettings.GetValue("sunlightLensFlareColor") + " --- exception " + e.Message);
	                    }
	                }
                }
 
                DynamicAmbientLight ambientLight = FindObjectOfType(typeof(DynamicAmbientLight)) as DynamicAmbientLight;
 
                //Funny story behind locating this one. When I was typing "Light l" in the foreach function earlier, one of the suggestions
                //from the autocomplete was DynamicAmbientLight. Saved me a lot of trial and error looking for it to be sure.
 
                if(ambientLight != null) {
                    print("*RSS* LightShifter: Found DynamicAmbientLight. Shifting...");

	                if(RSSSettings.HasValue("ambientLightColor")) {
	                    try {
	                        Vector4 col = KSPUtil.ParseVector4(RSSSettings.GetValue("ambientLightColor"));
	                        Color c = new Color(col.x, col.y, col.z, col.w);
		                    ambientLight.vacuumAmbientColor = c;
	                    } catch(Exception e) {
	                        print("*RSS* Error parsing as color4: original text: " + RSSSettings.GetValue("ambientLightColor") + " --- exception " + e.Message);
	                    }
	                }
                } else {
                    print("*RSS* LightShifter: Couldn't find DynamicAmbientLight");
                }
            }
        }
    }
}
