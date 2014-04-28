using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;

namespace RealSolarSystem {
	[KSPAddonFixed(KSPAddon.Startup.TrackingStation, false, typeof(KSCSwitcher))]
	public class KSCSwitcher : MonoBehaviour {
        public List<ConfigNode> Sites;
		public List<string> siteNames;
		private bool showWindow;
		private Vector2 scrollPosition;

		public void Start() {
			showWindow = false;
			scrollPosition = Vector2.zero;
            Sites = new List<ConfigNode>();
			siteNames = new List<string>();
			ConfigNode RSSSettings = null;

			foreach(ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEM")) {
                RSSSettings = node;
			}
            if(RSSSettings == null) {
                throw new UnityException("*RSS* REALSOLARSYSTEM node not found!");
			}
			
			if(RSSSettings.HasNode("LaunchSites")) {
				ConfigNode node = RSSSettings.GetNode("LaunchSites");
				ConfigNode[] sites = node.GetNodes("Site");
	            foreach(ConfigNode site in sites) {
	                if(site.HasValue("Name")) {
						Sites.Add(site);
						siteNames.Add(site.GetValue("Name"));
	                }
	            }
			}
            print("*RSS* KSCSwitcher initialized");
		}
		
		public void OnGUI() {
			if(siteNames.Count < 1) { return; }
			
			GUI.skin = HighLogic.Skin;
			if(GUI.Button(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), "Launch Sites")) {
				showWindow = !showWindow;
			}
			if(showWindow) {
				GUILayout.BeginArea(new Rect(Screen.width - 300, Screen.height - 430, 300, 400));
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(400));
				foreach(string site in siteNames) {
					if(GUILayout.Button(site)) {
						setSite(site);
					}
				}
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
		}

        public void setSite(string name) {
            ConfigNode site = getSite(name);
            if(site == null) { return; }

            bool hasChanged = false;
            double dtmp;
            float ftmp;
            bool btmp;
			CelestialBody Kerbin = FlightGlobals.Bodies.Find(body => body.name == "Kerbin");
            if (Kerbin == null)
            {
                Kerbin = FlightGlobals.Bodies.Find(body => body.name == "Earth"); // temp fix
            }
			var mods = Kerbin.pqsController.transform.GetComponentsInChildren(typeof(PQSMod), true);
			ConfigNode pqsCity = site.GetNode("PQSCity");
			ConfigNode pqsDecal = site.GetNode("PQSMod_MapDecalTangent");
			
			if(pqsCity == null) { return; }

			foreach(var m in mods) {
                if(m.GetType().ToString().Equals("PQSCity")) {
                    PQSCity mod = m as PQSCity;
                    if(pqsCity.HasValue("KEYname")) {
                        if(!(mod.name.Equals(pqsCity.GetValue("KEYname")))) {
                            continue;
                        }
                    }
                    if(pqsCity.HasValue("repositionRadial")) {
                        mod.repositionRadial = KSPUtil.ParseVector3(pqsCity.GetValue("repositionRadial"));
                    }
                    if(pqsCity.HasValue("latitude") && pqsCity.HasValue("longitude")) {
                        double lat, lon;
                        double.TryParse(pqsCity.GetValue("latitude"), out lat);
                        double.TryParse(pqsCity.GetValue("longitude"), out lon);
                    
                        mod.repositionRadial = RealSolarSystem.LLAtoECEF(lat, lon, 0, Kerbin.Radius);
                    }
                    if(pqsCity.HasValue("reorientInitialUp")) {
                        mod.reorientInitialUp = KSPUtil.ParseVector3(pqsCity.GetValue("reorientInitialUp"));
                    }
                    if(pqsCity.HasValue("repositionToSphere")) {
                        if(bool.TryParse(pqsCity.GetValue("repositionToSphere"), out btmp)) {
                            mod.repositionToSphere = btmp;
                        }
                    }
                    if(pqsCity.HasValue("repositionToSphereSurface")) {
                        if(bool.TryParse(pqsCity.GetValue("repositionToSphereSurface"), out btmp)) {
                            mod.repositionToSphereSurface = btmp;
                        }
                    }
                    if(pqsCity.HasValue("reorientToSphere")) {
                        if(bool.TryParse(pqsCity.GetValue("reorientToSphere"), out btmp)) {
                            mod.reorientToSphere = btmp;
                        }
                    }
                    if(pqsCity.HasValue("repositionRadiusOffset")) {
                        if(double.TryParse(pqsCity.GetValue("repositionRadiusOffset"), out dtmp)) {
                            mod.repositionRadiusOffset = dtmp;
                        }
                    }
                    if(pqsCity.HasValue("lodvisibleRangeMult")) {
                        if(double.TryParse(pqsCity.GetValue("lodvisibleRangeMult"), out dtmp)) {
                            foreach(PQSCity.LODRange l in mod.lod) {
                                l.visibleRange *= (float)dtmp;
                            }
                        }
                    }
                    if(pqsCity.HasValue("reorientFinalAngle")) {
                        if(float.TryParse(pqsCity.GetValue("reorientFinalAngle"), out ftmp)) {
                            mod.reorientFinalAngle = ftmp;
                        }
                    }
                    print("*RSS* changed PQSCity");
                    
                    hasChanged = true;
                    mod.OnSetup();
                }

                // KSC Flat area
                if(pqsDecal != null && m.GetType().ToString().Equals("PQSMod_MapDecalTangent")) {
                    // thanks to asmi for this!
                    PQSMod_MapDecalTangent mod = m as PQSMod_MapDecalTangent;
                    if(pqsDecal.HasValue("position")) {
                        mod.position = KSPUtil.ParseVector3(pqsDecal.GetValue("position"));
                    }
                    if(pqsDecal.HasValue("radius")) {
                        if(double.TryParse(pqsDecal.GetValue("radius"), out dtmp)) {
                            mod.radius = dtmp;
                        }
                    }
                    if(pqsDecal.HasValue("heightMapDeformity")) {
                        if(double.TryParse(pqsDecal.GetValue("heightMapDeformity"), out dtmp)) {
                            mod.heightMapDeformity = dtmp;
                        }
                    }
                    if(pqsDecal.HasValue("absoluteOffset")) {
                        if(double.TryParse(pqsDecal.GetValue("absoluteOffset"), out dtmp)) {
                            mod.absoluteOffset = dtmp;
                        }
                    }
                    if(pqsDecal.HasValue("absolute")) {
                        if(bool.TryParse(pqsDecal.GetValue("absolute"), out btmp)) {
                            mod.absolute = btmp;
                        }
                    }
                    if(pqsDecal.HasValue("latitude") && pqsDecal.HasValue("longitude")) {
                        double lat, lon;
                        double.TryParse(pqsDecal.GetValue("latitude"), out lat);
                        double.TryParse(pqsDecal.GetValue("longitude"), out lon);
                        
                        mod.position = RealSolarSystem.LLAtoECEF(lat, lon, 0, Kerbin.Radius);
                    }
                    print("*RSS* changed MapDecal_Tangent");

                    hasChanged = true;
                    mod.OnSetup();
                }

                if(hasChanged) {
                    print("*RSS* Rebuilding");
					Kerbin.pqsController.RebuildSphere();
					ScreenMessages.PostScreenMessage("Launch site changed to " + name, 2.5f, ScreenMessageStyle.LOWER_CENTER);
					showWindow = false;
                    print("*RSS* Launch site change DONE");
                }
			}
		}

        private ConfigNode getSite(string name) {
            foreach(ConfigNode site in Sites) {
                if(site.HasValue("Name")) {
                    if(site.GetValue("Name").Equals(name)) {
                        return site;
                    }
                }
            }
            return null;
        }
    }
}
