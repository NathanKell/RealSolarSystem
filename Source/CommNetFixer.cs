using CommNet;
using System;
using UnityEngine;

namespace RealSolarSystem
{
    [KSPAddon (KSPAddon.Startup.SpaceCentre, false)]

    public class RSSCommNetSettings : MonoBehaviour
    {
        void Start ()
        {
            try
            {
                //  Set the default CommNet parameters for RealSolarSystem.

                Debug.Log("[RealSolarSystem]: Updating the CommNet settings...");

                HighLogic.CurrentGame.Parameters.CustomParams<CommNetParams>().enableGroundStations = true;
                HighLogic.CurrentGame.Parameters.CustomParams<CommNetParams>().occlusionMultiplierAtm = 1.0f;
                HighLogic.CurrentGame.Parameters.CustomParams<CommNetParams>().occlusionMultiplierVac = 1.0f;
            }
            catch (Exception e)
            {
                Debug.Log("[RealSolarSystem]: RSSCommNetSettings.Start() caught an exception: " + e);
            }
        }
    }
}
