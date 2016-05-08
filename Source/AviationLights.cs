using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace AviationLights

	//Originally made by RPGprayer, edited by BigNose, Why485, GROOV3ST3R, JDP and J.Random
	// Fixes for KSP 1.1 applied by MOARdV.
	//License: This file contains code from RPGprayers "Position/Navigation Lights". Used with permission.
{
	public static class navLightStates
	{
		public enum navLightState
		{
			Off = 0,
			Flash = 1,
			DoubleFlash = 2,
			Interval = 3,
			On = 4
		}
	}

	public class ModuleNavLight : PartModule
	{
		protected Color _navLightColor;

		[KSPField(isPersistant = true)]
		public int navLightSwitch = 0;

		private double _lastTimeFired;
		private GameObject LightOffsetParent;
		private Light mainLight;
		private Light glowLight;

		private const float INTENSITY_GLOW = 0.33f;
		private const float INTENSITY_OFFSET = 0.67f;
		bool b = false, flightStarted = false;

		[KSPField]
		public string Resource = "ElectricCharge";

		[KSPField]
		public float EnergyReq = 0;

		[KSPField]
		public float
			IntervalFlashMode = 0,
			Interval = 0,
			FlashOn = 0,
			FlashOff = 0;

		[KSPField]
		public Vector3 Color = Vector3.zero;

		public override void OnStart(PartModule.StartState state)
		{
			if (state == StartState.Editor) return;
			flightStarted = true;

			_navLightColor = new Color(Color.x, Color.y, Color.z);
			_lastTimeFired = Planetarium.GetUniversalTime();

			// Parent for main illumination light, used to move it slightly above the light.
			LightOffsetParent = new GameObject();
			LightOffsetParent.transform.position = base.gameObject.transform.position;
			LightOffsetParent.transform.rotation = base.gameObject.transform.rotation;
			LightOffsetParent.transform.parent = base.gameObject.transform;
			LightOffsetParent.transform.Translate(0.33f, 0.0f, 0.0f);

			// Main Illumination light
			LightOffsetParent.gameObject.AddComponent<Light>();
			LightOffsetParent.gameObject.GetComponentCached<Light>(ref mainLight);
			mainLight.color = _navLightColor;
			mainLight.intensity = 0;

			// Glow Illumination light
			base.gameObject.AddComponent<Light>();
			base.gameObject.GetComponentCached<Light>(ref glowLight);
			glowLight.color = _navLightColor;
			glowLight.intensity = 0;

			LightOffsetParent.gameObject.AddComponent<MeshRenderer>();
		}

		public override void OnUpdate()
		{
			if (!flightStarted) return;
			switch (navLightSwitch)
			{
				case (int)navLightStates.navLightState.Off:
				//Lights go to 'Off' mode
				mainLight.intensity = 0;
				glowLight.intensity = 0;
				break;
				case (int)navLightStates.navLightState.Flash:
				//Lights go to 'Flash' mode
				FlashBasedSwitcher(IntervalFlashMode);
				break;
				case (int)navLightStates.navLightState.DoubleFlash:
				//Lights go to 'Double Flash' mode
				DoubleFlashBasedSwitcher(IntervalFlashMode);
				break;
				case (int)navLightStates.navLightState.Interval:
				//Lights go to 'Interval' mode
				IntervalBasedSwitcher(Interval);
				break;
				case (int)navLightStates.navLightState.On:
				//Lights go to 'On' mode
				mainLight.intensity = INTENSITY_OFFSET;
				glowLight.intensity = INTENSITY_GLOW;
				break;
			}

			//Energy requirements check: if the light is not off and requires resources; request resource. If returned resource is less than requested; turn off
			if (navLightSwitch > 0 && EnergyReq > 0 && TimeWarp.deltaTime > 0 && part.RequestResource(Resource, EnergyReq * TimeWarp.deltaTime) == 0)
				navLightSwitch = (int)navLightStates.navLightState.Off;
		}

		public override string GetInfo()
		{
			if (EnergyReq > 0)
				return Resource + " : " + (EnergyReq * 60).ToString("0.0") + "/min.";
			else return "";
		}

		private void FlashBasedSwitcher(float FlashOn)
		{
			if (_lastTimeFired < Planetarium.GetUniversalTime() - FlashOn)
			{
				b = !b;
				IntervalFlashMode = b ? this.FlashOn : FlashOff;
				_lastTimeFired = Planetarium.GetUniversalTime();
				mainLight.intensity = (mainLight.intensity == INTENSITY_OFFSET) ? 0f : INTENSITY_OFFSET;
				glowLight.intensity = (glowLight.intensity == INTENSITY_GLOW) ? 0f : INTENSITY_GLOW;
			}
		}

		private void DoubleFlashBasedSwitcher(float FlashOn)
		{
			mainLight.intensity = 0;
			glowLight.intensity = 0;

			if (_lastTimeFired < Planetarium.GetUniversalTime() - FlashOn)
			{
				b = !b;
				IntervalFlashMode = b ? this.FlashOn : FlashOff;
				_lastTimeFired = Planetarium.GetUniversalTime();
				mainLight.intensity = (mainLight.intensity == INTENSITY_OFFSET) ? 0f : INTENSITY_OFFSET;
				glowLight.intensity = (glowLight.intensity == INTENSITY_GLOW) ? 0f : INTENSITY_GLOW;
			}
		}

		private void IntervalBasedSwitcher(float Interval)
		{
			if (_lastTimeFired < Planetarium.GetUniversalTime() - Interval)
			{
				_lastTimeFired = Planetarium.GetUniversalTime();
				mainLight.intensity = (mainLight.intensity == INTENSITY_OFFSET) ? 0f : INTENSITY_OFFSET;
				glowLight.intensity = (glowLight.intensity == INTENSITY_GLOW) ? 0f : INTENSITY_GLOW;
			}
		}

		[KSPAction("LightToggle", KSPActionGroup.None, guiName = "Light toggle")]
		public void LightToggle(KSPActionParam param)
		{
			OnEvent();
		}

		[KSPAction("FlashToggle", KSPActionGroup.None, guiName = "Flash toggle")]
		public void FlashToggle(KSPActionParam param)
		{
			FlashEvent();
		}

		[KSPAction("DoubleFlashToggle", KSPActionGroup.None, guiName = "Double Flash toggle")]
		public void DoubleFlashToggle(KSPActionParam param)
		{
			DoubleFlashEvent();
		}

		[KSPAction("IntervalToggle", KSPActionGroup.None, guiName = "Interval toggle")]
		public void IntervalToggle(KSPActionParam param)
		{
			IntervalEvent();
		}

		[KSPAction("Cycle", KSPActionGroup.None, guiName = "Cycle modes")]
		public void Cycle(KSPActionParam param)
		{
			_lastTimeFired = 0;

			if (navLightSwitch == 4)
				navLightSwitch = 0;
			else
				navLightSwitch++;
		}

		[KSPEvent(name = "FlashEvent", active = true, guiActive = true, guiName = "Flash")]
		public void FlashEvent()
		{
			_lastTimeFired = 0;

			if (navLightSwitch == 1)
				navLightSwitch = 0;
			else
				navLightSwitch = 1;
		}

		[KSPEvent(name = "DoubleFlashEvent", active = true, guiActive = true, guiName = "Double Flash")]
		public void DoubleFlashEvent()
		{
			_lastTimeFired = 0;

			if (navLightSwitch == 2)
				navLightSwitch = 0;
			else
				navLightSwitch = 2;
		}

		[KSPEvent(name = "IntervalEvent", active = true, guiActive = true, guiName = "Interval")]
		public void IntervalEvent()
		{
			_lastTimeFired = 0;

			if (navLightSwitch == 3)
				navLightSwitch = 0;
			else
				navLightSwitch = 3;
		}

		[KSPEvent(name = "OnEvent", active = true, guiActive = true, guiName = "Light")]
		public void OnEvent()
		{
			_lastTimeFired = 0;

			if (navLightSwitch == 4)
				navLightSwitch = 0;
			else
				navLightSwitch = 4;
		}
	}
}