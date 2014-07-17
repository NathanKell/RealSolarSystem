using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using KSP.IO;

namespace RealSolarSystem {
	[KSPAddonFixed(KSPAddon.Startup.TrackingStation, false, typeof(KSCSwitcher))]
	public class KSCSwitcher : MonoBehaviour {
		public SortedList<string, LaunchSite> siteLocations;
		public static string activeSite;
		private bool showWindow;
		private bool showSites = true;
		private bool oldButton = false;
		private string curTooltip = "";
		private Vector2 scrollPosition;
		private Texture2D lsTexture;
		private Texture2D lsActiveTexture;
		private Texture2D lsButtonNormal;
		private Texture2D lsButtonHighlight;
		private Texture2D eyeButtonNormal;
		private Texture2D eyeButtonHighlight;
		private Texture2D magButtonNormal;
		private GUIStyle bStyle = null;
		private GUIStyle siteText = null;
		private GUIStyle infoLabel = null;

		public void Start() {
			showWindow = false;
			scrollPosition = Vector2.zero;
			siteLocations = KSCLoader.instance.Sites.getSitesGeographicalList();
			loadTextures();
			RenderingManager.AddToPostDrawQueue(2, this.onDraw);
			RenderingManager.AddToPostDrawQueue(3, this.onDrawGUI);
            print("*RSS* KSCSwitcher initialized");
		}
		
		public void OnDestroy() {
			RenderingManager.RemoveFromPostDrawQueue(2, this.onDraw);
			RenderingManager.RemoveFromPostDrawQueue(3, this.onDrawGUI);
		}
		
		public void onDrawGUI() {
			if(siteLocations.Count < 1) { return; }
			
			GUI.skin = HighLogic.Skin;
			if(bStyle == null) {
				bStyle = new GUIStyle(GUI.skin.button);
				bStyle.padding = new RectOffset();
				bStyle.contentOffset = new Vector2();
			}
			if(oldButton) {
				if(GUI.Button(new Rect(Screen.width - 100, 45, 100, 30), "Launch Sites")) {
					showWindow = !showWindow;
				}
			} else {
				if(GUI.Button(new Rect(Screen.width - 33, 45, 28, 28), (showWindow ? lsButtonHighlight : lsButtonNormal), bStyle)) {
					showWindow = !showWindow;
				}
			}
			if(GUI.Button(new Rect(Screen.width - 33, 78, 28, 28), (showSites ? eyeButtonHighlight : eyeButtonNormal), bStyle)) {
				showSites = !showSites;
			}
			if(showWindow) {
				if(oldButton) {
					GUILayout.BeginArea(new Rect(Screen.width - 333, 75, 300, 400));
				} else {
					GUILayout.BeginArea(new Rect(Screen.width - 333, 45, 300, 400));
				}
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(400));
				Color defColor = GUI.color;
				bool isActiveSite = false;
				foreach(KeyValuePair<string, LaunchSite> kvp in siteLocations) {
					isActiveSite = kvp.Value.name.Equals(activeSite);
					GUILayout.BeginHorizontal();
					if(GUILayout.Button(magButtonNormal, bStyle, GUILayout.MaxWidth(28))) {
						focusOnSite(kvp.Value.geographicLocation);
					}
					if(isActiveSite) {
						GUI.contentColor = XKCDColors.ElectricLime;
					}
					if(GUILayout.Button(new GUIContent(kvp.Value.displayName, kvp.Value.description))) {
						if(isActiveSite) {
							ScreenMessages.PostScreenMessage("Cannot set launch site to active site.", 2.5f, ScreenMessageStyle.LOWER_CENTER);
						} else {
							setSite(kvp.Key);
						}
					}
					GUI.contentColor = defColor;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				GUILayout.EndArea();
				
				GUI.backgroundColor = XKCDColors.AlmostBlack;
				if(curTooltip != "") {
					if(oldButton) {
						GUI.Label(new Rect(Screen.width - 633, 75, 300, 400), GUI.tooltip, infoLabel);
					} else {
						GUI.Label(new Rect(Screen.width - 633, 45, 300, 400), GUI.tooltip, infoLabel);
					}
				}
				if(Event.current.type == EventType.Repaint) {
					curTooltip = GUI.tooltip;
				}
			}
		}

		public void onDraw() {
			if(siteLocations.Count < 1 || lsTexture == null || !showSites || !iconDisplayDistance()) { return; }

			CelestialBody Kerbin = getKSCBody();
			Vector3d planet_pos = ScaledSpace.LocalToScaledSpace(Kerbin.position);
			bool isActiveSite = false;
			foreach(KeyValuePair<string, LaunchSite> kvp in siteLocations) {
				Camera camera = MapView.MapCamera.camera;
				Vector3d point = Kerbin.GetWorldSurfacePosition(kvp.Value.geographicLocation.x, kvp.Value.geographicLocation.y, 0);
				if(!IsOccluded(point, Kerbin)) {
					isActiveSite = kvp.Value.name.Equals(activeSite);
					point = ScaledSpace.LocalToScaledSpace(point);
					point = camera.WorldToScreenPoint(point);
					Rect iconBound = new Rect((float) point.x, (float) (Screen.height - point.y), 28f, 28f);
					if(isActiveSite) {
						Graphics.DrawTexture(iconBound, lsActiveTexture);
					} else {
						Graphics.DrawTexture(iconBound, lsTexture);
					}

					if(iconBound.Contains(Event.current.mousePosition)) {
                        GUI.Label(new Rect((float)(point.x) + 28f, (float)(Screen.height - point.y) + 5f, 50, 20), kvp.Value.displayName, siteText);
						if(Event.current.type == EventType.mouseDown && Event.current.button == 0) {
							if(isActiveSite) {
								ScreenMessages.PostScreenMessage("Cannot set launch site to active site.", 2.5f, ScreenMessageStyle.LOWER_CENTER);
							} else {
								setSite(kvp.Key);
							}
						}
					}
				}
			}
		}
		
		public static CelestialBody getKSCBody() {
			CelestialBody Kerbin = FlightGlobals.Bodies.Find(body => body.name == "Kerbin");
            if(Kerbin == null) {
                Kerbin = FlightGlobals.Bodies.Find(body => body.name == "Earth"); // temp fix
            }
			return Kerbin;
		}
		
		public static bool setSite(ConfigNode KSC) {
			bool hasChanged = false;
            double dtmp;
            float ftmp;
            bool btmp;
			CelestialBody Kerbin = getKSCBody();
			var mods = Kerbin.pqsController.transform.GetComponentsInChildren(typeof(PQSMod), true);
			ConfigNode pqsCity = KSC.GetNode("PQSCity");
			ConfigNode pqsDecal = KSC.GetNode("PQSMod_MapDecalTangent");
			
			if(pqsCity == null) { return false; }

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
                    if (pqsCity.HasValue("repositionToSphereSurfaceAddHeight"))
                    {
                        if (bool.TryParse(pqsCity.GetValue("repositionToSphereSurfaceAddHeight"), out btmp))
                        {
                            mod.repositionToSphereSurfaceAddHeight = btmp;
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
			}

			if(hasChanged) {
				Kerbin.pqsController.RebuildSphere();
			}
			
			return hasChanged;
		}
		
		private void setSite(string name) {
			ConfigNode site = KSCLoader.instance.Sites.getSiteByName(name);
            if(site == null) { return; }
			
			if(KSCSwitcher.setSite(site)) {
				activeSite = name;
				LastKSC.fetch.lastSite = activeSite;
				ScreenMessages.PostScreenMessage("Launch site changed to " + name, 2.5f, ScreenMessageStyle.LOWER_CENTER);
				showWindow = false;
                KSCReset.shouldCameraBeReset = true;
                print("*RSS* Launch site updated.  Camera reset set to true");
            }
		}
		
		private void focusOnSite(Vector2d loc) {
			Debug.Log("Focusing on site");
			PlanetariumCamera camera = PlanetariumCamera.fetch;
			CelestialBody Kerbin = getKSCBody();
			Vector3d point = ScaledSpace.LocalToScaledSpace(Kerbin.GetWorldSurfacePosition(loc.x, loc.y, 0));
			Vector3 vec = ScaledSpace.LocalToScaledSpace(Kerbin.transform.localPosition);
			point = (point - vec).normalized * Kerbin.Radius;
			camera.SetCamCoordsFromPosition(new Vector3((float) point.x, (float) point.y, (float) point.z));
			
			// this works for RSS, may have to change for other sizes.
			// float distance = camera.startDistance * 3.5f;
			float distance = (float) (Kerbin.Radius * 0.00035);
			camera.SetDistance(distance);
		}
		
        private bool IsOccluded(Vector3d loc, CelestialBody body) {
            Vector3d camPos = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position);

            if(Vector3d.Angle(camPos - loc, body.position - loc) > 90) { return false; }
			return true;
        }
		
		private void loadTextures() {
			Texture2D white = null;
			// hard-coded texture path because why not.
			try {
				lsButtonNormal = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/launch-site-normal", false);
				lsButtonHighlight = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/launch-site-highlight", false);
				if(lsButtonNormal == null) {
					oldButton = true;
				}
				lsTexture = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/launch-site-texture", false);
				lsActiveTexture = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/launch-site-active-texture", false);
				white = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/info-background", false);
				eyeButtonNormal = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/eye-normal", false);
				eyeButtonHighlight = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/eye-highlight", false);
				magButtonNormal = GameDatabase.Instance.GetTexture("RealSolarSystem/Plugins/Icons/magnifier-normal", false);
			} catch(Exception e) {
				Debug.Log("Could not load button textures for KSCSwitcher, reverting to old button style: " + e.StackTrace);
				oldButton = true;
			}

			siteText = new GUIStyle();
			siteText.padding = new RectOffset(0, 0, 0, 0);
			siteText.stretchWidth = true;
			siteText.margin = new RectOffset(0, 0, 0, 0);
			siteText.alignment = TextAnchor.MiddleLeft;
			siteText.fontStyle = FontStyle.Bold;
			siteText.normal.textColor = XKCDColors.BrightOrange;

			infoLabel = new GUIStyle();
			infoLabel.padding = new RectOffset(5, 5, 5, 5);
			infoLabel.stretchHeight = true;
			infoLabel.margin = new RectOffset(0, 0, 0, 0);
			infoLabel.alignment = TextAnchor.UpperLeft;
			infoLabel.fontStyle = FontStyle.Bold;
			infoLabel.normal.textColor = XKCDColors.ElectricLime;
			infoLabel.normal.background = white;
			infoLabel.richText = true;
			infoLabel.wordWrap = true;
		}
		
		private bool iconDisplayDistance() {
			CelestialBody Kerbin = getKSCBody();
			return MapView.MapCamera.Distance < 25000 && MapView.MapCamera.target.name == Kerbin.name;
		}
    }
	
	// provides a simple method for different classes to load, manage, and read the launch sites.
	public class KSCSiteManager {
        public List<ConfigNode> Sites;
		
		public KSCSiteManager() {
            Sites = new List<ConfigNode>();
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
	                if(site.HasValue("name")) {
						ConfigNode pqsCity = site.GetNode("PQSCity");
						if(pqsCity == null) { continue; }
						
                    	if(pqsCity.HasValue("latitude") && pqsCity.HasValue("longitude")) {
							Sites.Add(site);
						}
					}
				}
			}
			
			Debug.Log("*RSS* loaded " + Sites.Count + " launch sites.");
		}
		
		public ConfigNode getSiteByName(string name) {
            foreach(ConfigNode site in Sites) {
                if(site.HasValue("name")) {
                    if(site.GetValue("name").Equals(name)) {
                        return site;
                    }
                }
            }
            return null;
		}
		
		public SortedList<string, LaunchSite> getSitesGeographicalList() {
			SortedList<string, LaunchSite> siteLocations = new SortedList<string, LaunchSite>();
            double lat, lon, dtmp;

			foreach(ConfigNode site in KSCLoader.instance.Sites.Sites) {
				ConfigNode pqsCity = site.GetNode("PQSCity");
				LaunchSite temp = new LaunchSite();
				temp.name = site.GetValue("name");
				temp.displayName = site.GetValue("displayName");
				double.TryParse(pqsCity.GetValue("latitude"), out lat);
                double.TryParse(pqsCity.GetValue("longitude"), out lon);
				temp.geographicLocation = new Vector2d(lat, lon);
                if(site.HasValue("description")) {
					temp.description = site.GetValue("description");
				}
				if(site.HasValue("availableFromUT")) {
            		if(double.TryParse(site.GetValue("availableFromUT"), out dtmp)) {
						temp.availableFromUT = dtmp;
					}
				}
				if(site.HasValue("availableUntilUT")) {
            		if(double.TryParse(site.GetValue("availableUntilUT"), out dtmp)) {
						temp.availableUntilUT = dtmp;
					}
				}
				siteLocations.Add(temp.name, temp);
			}
			
			return siteLocations;
		}
	}

	// Taniwha graciously offered the use of this code/method for saving our settings per save game.
	// I've changed where appropriate and reformatted because of 1TBS.
	public class LastKSC : ScenarioModule {
		public string lastSite = "";
		private static LastKSC instance;
		
		public static LastKSC fetch {
			get {
				if(instance == null) {
					Game g = HighLogic.CurrentGame;
					instance = g.scenarios.Select(s => s.moduleRef).OfType<LastKSC>().SingleOrDefault();
				}
				return instance;
			}
		}
		
		public override void OnLoad(ConfigNode config) {
			if(config.HasValue("LastLaunchSite")) {
				lastSite = config.GetValue("LastLaunchSite");
			}
			if(HighLogic.LoadedScene == GameScenes.TRACKSTATION) {
				KSCSwitcher.activeSite = lastSite;
			}
		}
		
		public override void OnSave(ConfigNode config) {
			config.AddValue("LastLaunchSite", lastSite);
		}

		public static void CreateSettings(Game game) {
			if(!game.scenarios.Any(p=>p.moduleName == typeof(LastKSC).Name)) {
				ProtoScenarioModule proto = game.AddProtoScenarioModule(typeof (LastKSC), GameScenes.TRACKSTATION);
				proto.Load(ScenarioRunner.fetch);
			}
		}
	}

	public class KSCLoader {
		public static KSCLoader instance;
		public KSCSiteManager Sites = new KSCSiteManager();

		void onGameStateCreated(Game game) {
			LastKSC.CreateSettings(game);
			if(HighLogic.LoadedScene == GameScenes.SPACECENTER) {
				foreach(ProtoScenarioModule m in HighLogic.CurrentGame.scenarios) {
					if(m.moduleName == "LastKSC") {
						LastKSC l = (LastKSC) m.Load(ScenarioRunner.fetch);
						if(l.lastSite.Length > 0) {
							// found a site, load it
							ConfigNode site = Sites.getSiteByName(l.lastSite);
				            if(site == null) { return; }
							KSCSwitcher.setSite(site);
							Debug.Log("*RSS* set the initial launch site to " + l.lastSite);
						}
					}
				}
			}
		}

		public KSCLoader() {
			GameEvents.onGameStateCreated.Add(onGameStateCreated);
		}
	}

	[KSPAddon(KSPAddon.Startup.Instantly, false)]
	public class ScenarioSpawn : MonoBehaviour {
		void Start() {
			KSCLoader.instance = new KSCLoader();
			enabled = false;
		}
	}
	
	public class LaunchSite {
        public string name { get; set; }
        public string displayName { get; set; }
		public string description { get; set; }
		public Vector2d geographicLocation { get; set; }
		public double availableFromUT { get; set; }
		public double availableUntilUT { get; set; }
		
		public LaunchSite() {
			this.name = "";
            this.displayName = "";
			this.description = "";
			this.availableFromUT = 0.0;
			this.availableUntilUT = 0.0;
			this.geographicLocation = Vector2d.zero;
		}
	}
}
