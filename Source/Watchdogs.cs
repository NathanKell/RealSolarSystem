using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
    // Checks to make sure useLegacyAtmosphere didn't get munged with
    // Could become a general place to prevent RSS changes from being reverted when our back is turned.
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class RSSWatchDog : MonoBehaviour
    {
        ConfigNode RSSSettings = null;
        public void Start()
        {
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEM"))
                RSSSettings = node;
            

            UpdateAtmospheres();
            GameEvents.onVesselSOIChanged.Add(OnVesselSOIChanged);
        }
        public void OnDestroy()
        {
            GameEvents.onVesselSOIChanged.Remove(OnVesselSOIChanged);
        }

        public void OnVesselSOIChanged(GameEvents.HostedFromToAction<Vessel, CelestialBody> evt)
        {

        }
        public void UpdateAtmospheres()
        {
            if (RSSSettings != null)
            {
                AtmosphereFromGround[] AFGs = (AtmosphereFromGround[])Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround));
                foreach (ConfigNode node in RSSSettings.nodes)
                {
                    foreach (CelestialBody body in FlightGlobals.Bodies)
                    {
                        print("*RSS* checking useLegacyAtmosphere for " + body.GetName());
                        if (node.HasValue("useLegacyAtmosphere"))
                        {
                            bool UseLegacyAtmosphere = true;
                            bool.TryParse(node.GetValue("useLegacyAtmosphere"), out UseLegacyAtmosphere);
                            //print("*RSSWatchDog* " + body.GetName() + ".useLegacyAtmosphere = " + body.useLegacyAtmosphere.ToString());
                            if (UseLegacyAtmosphere != body.useLegacyAtmosphere)
                            {
                                print("*RSSWatchDog* resetting useLegacyAtmosphere to " + UseLegacyAtmosphere.ToString());
                                body.useLegacyAtmosphere = UseLegacyAtmosphere;
                            }
                        }
                        if (node.HasNode("AtmosphereFromGround"))
                        {
                            foreach (AtmosphereFromGround ag in AFGs)
                            {
                                if (ag != null && ag.planet != null)
                                {
                                    // generalized version of Starwaster's code. Thanks Starwaster!
                                    if (ag.planet.name.Equals(node.name))
                                    {
                                        RealSolarSystem.UpdateAFG(body, ag, node.GetNode("AtmosphereFromGround"));
                                        print("*RSSWatchDog* reapplying AtmosphereFromGround settings");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}