using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KSP;
using System.IO;


namespace RealSolarSystem
{
    public class CBRotation
    {
        string name;
        double axialTilt;
        double rotationPeriod;
        double initialRotation;

        public CBRotation(string newName, double newTilt, double newRotPeriod, double newInitialRot)
        {
            MonoBehaviour.print("*RSS* Adding override rotation for " + newName + ": Axial tilt " + newTilt + ", period " + newRotPeriod + ", initial " + newInitialRot);
            name = newName;
            axialTilt = newTilt;
            rotationPeriod = newRotPeriod;
            initialRotation = newInitialRot;
        }
        public QuaternionD Tilt()
        {
            return QuaternionD.AngleAxis(-axialTilt, Vector3d.back);
        }
        public double AngleAtTime(double time)
        {
            return (initialRotation + 360.0 / rotationPeriod * time) % 360.0;
        }
        public QuaternionD TiltedAngleAtTime(double time)
        {
            return TiltedAngle(AngleAtTime(time));
        }
        public QuaternionD TiltedAngle(double angle)
        {
            QuaternionD qAngle = QuaternionD.AngleAxis(angle, Vector3d.down);
            return Tilt() * qAngle;
        }
        public Vector3d AngularVelocity()
        {
            return Tilt() * Vector3d.down * (Math.PI * 2 / rotationPeriod);
        }
        public Vector3d zUpAngularVelocity()
        {
            return Tilt() * Vector3d.back * (Math.PI * 2 / rotationPeriod);
        }
    }

    /*[KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class CBRotationFixer : MonoBehaviour
    {
        public static Dictionary<string, CBRotation> CBRotations = new Dictionary<string, CBRotation>();

        // call this on the sun. Will update recursively all children
        // only touches planets with rotations defined in CBRotations
        // and ignores others. Does not support inverseRotation.
        public void UpdateBody(CelestialBody body)
        {
            if (CBRotations.ContainsKey(body.name))
            {
                //print("*RSS CBBR* Updating " + body.name);
                CBRotation rot = CBRotations[body.name];
                body.angularVelocity = rot.AngularVelocity();
                body.zUpAngularVelocity = rot.zUpAngularVelocity();

                body.rotationAngle = rot.AngleAtTime(Planetarium.GetUniversalTime());
                body.directRotAngle = (body.rotationAngle - Planetarium.InverseRotAngle) % 360.0;
                body.angularV = body.angularVelocity.magnitude;
                body.rotation = rot.TiltedAngle(body.directRotAngle);
                body.transform.rotation = body.rotation;

            }
            if (body.orbitDriver)
                body.orbitDriver.UpdateOrbit();

            foreach (CelestialBody child in body.orbitingBodies)
                UpdateBody(child);
        }

        public void FixedUpdate()
        {
            //print("*RSSCBR* Running FixedUpdate");
            if (HighLogic.LoadedSceneHasPlanetarium || HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER))
            {
                //print("*RSSCBR* Correct Scene");
                if (Planetarium.fetch.Sun)
                    UpdateBody(Planetarium.fetch.Sun);
            }
        }
    }*/
}
