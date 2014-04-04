using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;

namespace RealSolarSystem
{
    // Fixes powerCurves of all solar panels by replacing the existing powerCurve with the
    // specified in the RealSolarSystemSettings.cfg file
    [KSPAddonFixed(KSPAddon.Startup.MainMenu, true, typeof(SolarPanelFixer))]
    public class SolarPanelFixer : MonoBehaviour
    {

        public void FixSP(ModuleDeployableSolarPanel sp, ConfigNode curveNode)
        {
            sp.powerCurve = new FloatCurve();
            sp.powerCurve.Load(curveNode);
        }
        public static bool fixedSolar = false;
        public void Start()
        {
            if (!fixedSolar && HighLogic.LoadedScene.Equals(GameScenes.MAINMENU))
            {
                fixedSolar = true;
                print("*RSS* Fixing Solar Panels");
                if (PartLoader.LoadedPartsList != null)
                {
                    ConfigNode curveNode = null;
                    foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("REALSOLARSYSTEMSETTINGS"))
                        curveNode = node.GetNode("powerCurve");
                    if (curveNode != null)
                    {
                        foreach (Part p in Resources.FindObjectsOfTypeAll(typeof(Part)))
                        {
                            try
                            {
                                if (p.Modules.Contains("ModuleDeployableSolarPanel"))
                                {
                                    ModuleDeployableSolarPanel sp = (ModuleDeployableSolarPanel)(p.Modules["ModuleDeployableSolarPanel"]);
                                    FixSP(sp, curveNode);
                                    print("Fixed " + p.name + " (" + p.partInfo.title + ")");
                                }
                            }
                            catch (Exception e)
                            {
                                print("Solar panel fixing failed for " + p.name + ": " + e.Message);
                            }
                        }
                        ConfigNode[] allParts = GameDatabase.Instance.GetConfigNodes("PART");
                        for (int j = 0; j < allParts.Count(); j++)
                        {
                            try
                            {
                                ConfigNode pNode = allParts[j];
                                if (pNode.nodes == null || pNode.nodes.Count <= 0)
                                    continue;
                                for (int i = 0; i < pNode.nodes.Count; i++)
                                {
                                    ConfigNode node = pNode.nodes[i];
                                    if (node.name.Equals("MODULE"))
                                    {
                                        if (node.HasValue("name") && node.GetValue("name").Equals("ModuleDeployableSolarPanel"))
                                        {
                                            node.RemoveNode("powerCurve");
                                            node.AddNode(curveNode);
                                            print("Fixed part config " + pNode.GetValue("name") + " (" + pNode.GetValue("title") + ")");
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                print("Solar panel fixing of part confignodes failed: " + e.Message);
                            }
                        }
                    }
                    try
                    {
                        EditorPartList.Instance.Refresh();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
