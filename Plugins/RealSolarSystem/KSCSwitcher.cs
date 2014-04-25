using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;

namespace RealSolarSystem {

    [KSPAddonFixed(KSPAddon.Startup.MainMenu, true, typeof(KSCSwitcher))]
    public class KSCSwitcher {
        public List<ConfigNode> Sites;

        public KSCSwitcher(ConfigNode nodes) {
            Sites = new List<ConfigNode>();

            foreach(ConfigNode site in nodes) {
                Sites.add(site);
            }
        }

        public List<string> getSiteNames() {
            List<string> retval = new List<string>();

            foreach(ConfigNode site in Sites) {
                if(site.HasValue("Name")) {
                    retval.add(site.GetValue("Name"));
                }
            }

            return retval;
        }

        public void setSite(string name) {
            ConfigNode site = getSite(name);
            if(site == null) { return; }

            foreach(PQS p in Resources.FindObjectsOfTypeAll(typeof(PQS))) {
                if(p.name.Equals("Kerbin")) {
                    bool hasChanged = false;
                    var mods = p.transform.GetComponentsInChildren(typeof(PQSMod), true);
                    foreach(var m in mods) {
                        foreach(ConfigNode modNode in site) {
                            if(modNode.name.Equals("PQSCity") && m.GetType().ToString().Equals(modNode.name)) {
                                PQSCity mod = m as PQSCity;
                                if(modNode.HasValue("KEYname")) {
                                    if(!(mod.name.Equals(modNode.GetValue("KEYname")))) {
                                        continue;
                                    }
                                }
                                if(modNode.HasValue("repositionRadial")) {
                                    mod.repositionRadial = KSPUtil.ParseVector3(modNode.GetValue("repositionRadial"));
                                }
                                if(modNode.HasValue("latitude") && modNode.HasValue("longitude")) {
                                    double lat, lon;
                                    double.TryParse(modNode.GetValue("latitude"), out lat);
                                    double.TryParse(modNode.GetValue("longitude"), out lon);
                                
                                    mod.repositionRadial = RealSolarSystem.LLAtoECEF(lat, lon, 0, body.Radius);
                                }
                                if(modNode.HasValue("reorientInitialUp")) {
                                    mod.reorientInitialUp = KSPUtil.ParseVector3(modNode.GetValue("reorientInitialUp"));
                                }
                                if(modNode.HasValue("repositionToSphere")) {
                                    if(bool.TryParse(modNode.GetValue("repositionToSphere"), out btmp)) {
                                        mod.repositionToSphere = btmp;
                                    }
                                }
                                if(modNode.HasValue("repositionToSphereSurface")) {
                                    if(bool.TryParse(modNode.GetValue("repositionToSphereSurface"), out btmp)) {
                                        mod.repositionToSphereSurface = btmp;
                                    }
                                }
                                if(modNode.HasValue("reorientToSphere")) {
                                    if(bool.TryParse(modNode.GetValue("reorientToSphere"), out btmp)) {
                                        mod.reorientToSphere = btmp;
                                    }
                                }
                                if(modNode.HasValue("repositionRadiusOffset")) {
                                    if(double.TryParse(modNode.GetValue("repositionRadiusOffset"), out dtmp)) {
                                        mod.repositionRadiusOffset = dtmp;
                                    }
                                }
                                if(modNode.HasValue("lodvisibleRangeMult")) {
                                    if(double.TryParse(modNode.GetValue("lodvisibleRangeMult"), out dtmp)) {
                                        foreach(PQSCity.LODRange l in mod.lod) {
                                            l.visibleRange *= (float)dtmp;
                                        }
                                    }
                                }
                                if(modNode.HasValue("reorientFinalAngle")) {
                                    if(float.TryParse(modNode.GetValue("reorientFinalAngle"), out ftmp)) {
                                        mod.reorientFinalAngle = ftmp;
                                    }
                                }
                                
                                hasChanged = true;
                                mod.OnSetup();
                            }

                            // KSC Flat area
                            if(modNode.name.Equals("PQSMod_MapDecalTangent")  && m.GetType().ToString().Equals(modNode.name)) {
                                // thanks to asmi for this!
                                PQSMod_MapDecalTangent mod = m as PQSMod_MapDecalTangent;
                                if(modNode.HasValue("position")) {
                                    mod.position = KSPUtil.ParseVector3(modNode.GetValue("position"));
                                }
                                if(modNode.HasValue("radius")) {
                                    if(double.TryParse(modNode.GetValue("radius"), out dtmp)) {
                                        mod.radius = dtmp;
                                    }
                                }
                                if(modNode.HasValue("heightMapDeformity")) {
                                    if(double.TryParse(modNode.GetValue("heightMapDeformity"), out dtmp)) {
                                        mod.heightMapDeformity = dtmp;
                                    }
                                }
                                if(modNode.HasValue("absoluteOffset")) {
                                    if(double.TryParse(modNode.GetValue("absoluteOffset"), out dtmp)) {
                                        mod.absoluteOffset = dtmp;
                                    }
                                }
                                if(modNode.HasValue("absolute")) {
                                    if(bool.TryParse(modNode.GetValue("absolute"), out btmp)) {
                                        mod.absolute = btmp;
                                    }
                                }
                                if(modNode.HasValue("rescaleToRadius")) {
                                    mod.position *= (float)(body.Radius / origRadius);
                                    mod.radius *= (body.Radius / origRadius);
                                }
                                if(modNode.HasValue("latitude") && modNode.HasValue("longitude")) {
                                    double lat, lon;
                                    double.TryParse(modNode.GetValue("latitude"), out lat);
                                    double.TryParse(modNode.GetValue("longitude"), out lon);
                                    
                                    mod.position = LLAtoECEF(lat, lon, 0, body.Radius);
                                }

                                hasChanged = true;
                                mod.OnSetup();
                            }
                        }
                    }

                    if(hasChanged) {
                        p.RebuildSphere();
                    }
                    break;
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
