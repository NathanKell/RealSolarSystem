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
        CelestialBody body = FlightGlobals.getMainBody();

        public void Start()
        {
            ConfigNode RSSSettings = null;
            bool UseLegacyAtmosphere;
            float ftmp;

            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEM"))
                RSSSettings = node;

            if (RSSSettings != null)
            {
                if (RSSSettings.HasNode(body.GetName()))
                {
                    ConfigNode node = RSSSettings.GetNode(body.GetName());
                    print("*RSS* checking useLegacyAtmosphere for " + body.GetName());
                    if (node.HasValue("useLegacyAtmosphere"))
                    {
                        bool.TryParse(node.GetValue("useLegacyAtmosphere"), out UseLegacyAtmosphere);
                        print("*RSSWatchDog* " + body.GetName() + ".useLegacyAtmosphere = " + body.useLegacyAtmosphere.ToString());
                        if (UseLegacyAtmosphere != body.useLegacyAtmosphere)
                        {
                            print("*RSSWatchDog* resetting useLegacyAtmosphere to " + UseLegacyAtmosphere.ToString());
                            body.useLegacyAtmosphere = UseLegacyAtmosphere;
                        }
                    }
                    if (node.HasNode("AtmosphereFromGround"))
                    {
                        ConfigNode agnode = node.GetNode("AtmosphereFromGround");
                        foreach (AtmosphereFromGround ag in Resources.FindObjectsOfTypeAll(typeof(AtmosphereFromGround)))
                        {
                            if (ag != null && ag.planet != null)
                            {
                                // generalized version of Starwaster's code. Thanks Starwaster!
                                if (ag.planet.name.Equals(node.name))
                                {
                                    if (node.HasNode("AtmosphereFromGround"))
                                    {
                                        ConfigNode modNode = node.GetNode("AtmosphereFromGround");
                                        if (modNode.HasValue("outerRadius"))
                                        {
                                            if (float.TryParse(modNode.GetValue("outerRadius"), out ftmp))
                                                ag.outerRadius = ftmp * ScaledSpace.InverseScaleFactor;
                                        }
                                        else if (modNode.HasValue("outerRadiusAtmo"))
                                        {
                                            ag.outerRadius = ((float)body.Radius + body.maxAtmosphereAltitude) * ScaledSpace.InverseScaleFactor;
                                        }
                                        else if (modNode.HasValue("outerRadiusMult"))
                                        {
                                            if (float.TryParse(modNode.GetValue("outerRadiusMult"), out ftmp))
                                                ag.outerRadius = ftmp * (float)body.Radius * ScaledSpace.InverseScaleFactor;
                                        }

                                        if (modNode.HasValue("innerRadius"))
                                        {
                                            if (float.TryParse(modNode.GetValue("innerRadius"), out ftmp))
                                                ag.innerRadius = ftmp * ScaledSpace.InverseScaleFactor;
                                        }
                                        else if (modNode.HasValue("innerRadiusMult"))
                                        {
                                            if (float.TryParse(modNode.GetValue("innerRadiusMult"), out ftmp))
                                                ag.innerRadius = ftmp * ag.outerRadius;
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
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
