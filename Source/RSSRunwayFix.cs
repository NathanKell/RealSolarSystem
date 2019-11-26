using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

namespace RealSolarSystem
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]

	public class RSSRunwayFix: MonoBehaviour
	{
		private bool hold = false;

		[KSPField(isPersistant = true)] 
		public bool debug = true;
		
		[KSPField(isPersistant = true)]
		public float holdThreshold = 2700;
		
		public float holdThresholdSqr;

		public float originalThreshold = 0;
		public float originalThresholdSqr = 0;

		private int layerMask = 1<<15;

		private int frameSkip = 0;
		private bool rwy = false;

		private bool waiting = false;
		private IEnumerator waitCoro = null;
		private bool coroComplete = false;

		private Vector3 down;

		private string[] collidersToFix =
		{
			"Section4", "Section3", "Section2", "Section1", "End27", "End09",
		};
		
		public void Start() {
			
			printDebug("Start");
			
			GameEvents.onVesselGoOffRails.Add (onVesselGoOffRails);
			GameEvents.onVesselGoOnRails.Add (onVesselGoOnRails);
			GameEvents.onVesselSwitching.Add (onVesselSwitching);
			GameEvents.onVesselSituationChange.Add (onVesselSituationChange);
			
			//GameEvents.onFloatingOriginShift.Add(onFloatingOriginShift);
		}

		public void OnDestroy() {
			printDebug("OnDestroy");
			GameEvents.onVesselGoOffRails.Remove (onVesselGoOffRails);
			GameEvents.onVesselGoOnRails.Remove (onVesselGoOnRails);
			GameEvents.onVesselSwitching.Remove (onVesselSwitching);
			GameEvents.onVesselSituationChange.Remove(onVesselSituationChange);
			
			//GameEvents.onFloatingOriginShift.Remove(onFloatingOriginShift);
		}

		public void onFloatingOriginShift(Vector3d v0, Vector3d v1)
		{
			if (!hold)
			{
				return;
			}
			printDebug($"RSSRWF: v0: {v0}, v1: {v1}, threshold: {FloatingOrigin.fetch.threshold}");
		}

		public void onVesselGoOnRails(Vessel v)
		{
			FloatingOrigin.fetch.threshold = originalThreshold;
			FloatingOrigin.fetch.thresholdSqr = originalThresholdSqr;
			hold = false;
		}
			
		public void onVesselGoOffRails(Vessel v) {

			printDebug("started");

			originalThreshold = FloatingOrigin.fetch.threshold;
			originalThresholdSqr = FloatingOrigin.fetch.thresholdSqr;

			printDebug($"original threshold={originalThreshold}");
			holdThresholdSqr = holdThreshold * holdThreshold;

			GameObject end09 = GameObject.Find(collidersToFix[0]);
			if (end09 == null)
			{
				printDebug("no end09 found");
				hold = false;
				return;
			}

			//combine(end09.transform.parent.gameObject);
			disableColliders();
			getDownwardVector();
			hold = true;
			waiting = false;
		}

		public void onVesselSwitching(Vessel from, Vessel to) {

			if (to == null || to.situation != Vessel.Situations.LANDED) { // FIXME: Do we need PRELAUNCH here?
				return;
			}

			getDownwardVector();
			waiting = false;
		}

		private void getDownwardVector()
		{
			Vessel v = FlightGlobals.ActiveVessel;
			down = (v.CoM - v.mainBody.transform.position).normalized * -1;
		}

		public void onVesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> data)
		{
			if (data.host != FlightGlobals.ActiveVessel)
			{
				return;
			}

			
			hold = data.to == Vessel.Situations.LANDED;
			printDebug($"vessel: {data.host.vesselName}, situation: {data.to}, hold: {hold}");

			if (!hold && FloatingOrigin.fetch.threshold > originalThreshold && originalThreshold > 0)
			{
				printDebug($"coro: {waitCoro}, complete: {coroComplete}");
				if (waitCoro != null && !coroComplete)
				{
					printDebug("stopping coro");
					StopCoroutine(waitCoro);
				}

				waitCoro = restoreThreshold();
				printDebug($"created new coro: {waitCoro}");

				coroComplete = false;
				StartCoroutine(waitCoro);
			}
		}

		private IEnumerator restoreThreshold()
		{
			printDebug($"in coro; hold={hold}, waiting={waiting}, alt={FlightGlobals.ActiveVessel.radarAltitude}");
			while (!hold && !waiting &&  FlightGlobals.ActiveVessel.radarAltitude < 10)
			{
				printDebug($"radar alt: {FlightGlobals.ActiveVessel.radarAltitude}, waiting 5 sec");
				waiting = true;
				yield return new WaitForSeconds(5);
				waiting = false;
				printDebug("waiting is over");
			}

			// Check again as situation could have changed
			if (!hold && FloatingOrigin.fetch.threshold > originalThreshold && originalThreshold > 0) {
				printDebug($"Restoring original thresholds ({FloatingOrigin.fetch.threshold} > {originalThreshold}), "+
				           $"alt={FlightGlobals.ActiveVessel.radarAltitude}");
				FloatingOrigin.fetch.threshold = originalThreshold;
				FloatingOrigin.fetch.thresholdSqr = originalThresholdSqr;
			}
			printDebug("coro finished");
			coroComplete = true;
		}
		
		public void FixedUpdate()
		{
			frameSkip++;
			if (frameSkip < 25)
			{
				return;
			}
			frameSkip = 0;
			
			if (!checkRunway())
			{
				if (rwy)
				{
					printDebug(
						$"rwy=false; threshold={FloatingOrigin.fetch.threshold}, original threshold={originalThreshold}");
					rwy = false;
				}
				
				return;
			}
			
			FloatingOrigin.fetch.threshold = holdThreshold;
			FloatingOrigin.fetch.thresholdSqr = holdThresholdSqr;
			
			if (!rwy)
			{
				printDebug($"rwy=true; threshold={FloatingOrigin.fetch.threshold}, original threshold={originalThreshold}");
				rwy = true;
			}

			FloatingOrigin.SetSafeToEngage(false);
		}
		
		private bool checkRunway()
		{
			if (!hold)
			{
				return false;
			}
			
			Vessel v = FlightGlobals.ActiveVessel;
			if (v.situation != Vessel.Situations.LANDED && v.situation != Vessel.Situations.PRELAUNCH)
			{
				return false;
			}
			
			getDownwardVector();
			
			RaycastHit raycastHit;
			bool hit = Physics.Raycast(v.transform.position, down, out raycastHit, 100, layerMask);
			if (!hit)
			{
				return false;
			}
			
			string colliderName = raycastHit.collider.gameObject.name;
			//printDebug($"hit collider: {colliderName}");
			if (colliderName != "runway_collider")
			{
				return false;
			}

			return true;
		}

		internal void printDebug(String message) {

			if (!debug)
				return;
			StackTrace trace = new StackTrace ();
			String caller = trace.GetFrame(1).GetMethod ().Name;
			int line = trace.GetFrame (1).GetFileLineNumber ();
			print ("RSSSteamRoller: " + caller + ":" + line + ": " + message);
		}

		private void disableColliders()
		{
			foreach (string c in collidersToFix)
			{
				GameObject o = GameObject.Find(c);
				if (o == null)
				{
					printDebug($"Object {c} not found, skipping");
					continue;
				}
				if (!o.activeInHierarchy)
				{
					printDebug($"{o.name} is not active, skipping");
					continue;
				}

				MeshCollider cl = o.GetComponentInChildren<MeshCollider>();
				if (cl == null)
				{
					printDebug($"No mesh collider in {c}");
					continue;
				}
				printDebug($"disabling {cl.name}");
				cl.enabled = false;
			}
		}
	}
}
