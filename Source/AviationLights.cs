using System;
using UnityEngine;

//Originally made by RPGprayer, edited by BigNose, Why485, GROOV3ST3R, JDP and J.Random
// Fixes for KSP 1.1, additional fixes, and refactoring by MOARdV.
//License: This file contains code from RPGprayers "Position/Navigation Lights". Used with permission.
namespace AviationLights
{
    public class ModuleNavLight : PartModule
    {
        public enum NavLightState
        {
            Off = 0,
            Flash = 1,
            DoubleFlash = 2,
            Interval = 3,
            On = 4
        }

        protected Color _navLightColor;

        [KSPField(isPersistant = true)]
        public int navLightSwitch = 0;
        private NavLightState navLightState = NavLightState.Off;

        [KSPField]
        public float intensity = 0.67f;

        private double _lastTimeFired;
        private GameObject LightOffsetParent;
        private Light mainLight;

        [KSPField]
        public string Resource = "ElectricCharge";
        private int resourceId;

        [KSPField]
        public float EnergyReq = 0.0f;

        private float nextInterval = 0.0f;

        [KSPField]
        public float Interval = 0.0f;
        [KSPField]
        public float FlashOn = 0.0f;
        [KSPField]
        public float FlashOff = 0.0f;

        [KSPField]
        public Vector3 Color = Vector3.zero;

        [KSPField]
        public Vector3 LightOffset = new Vector3(0.33f, 0.0f, 0.0f);

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#AL_LightMode")]
        public string modeString;

        private int flashCounter = 0;

        public override void OnStart(PartModule.StartState state)
        {
            if (navLightSwitch < 0 || navLightSwitch > 4)
            {
                navLightSwitch = 0;
            }

            if (state == StartState.Editor)
            {
                UpdateMode(); // Update the mode string.
                return;
            }

            try
            {
                resourceId = PartResourceLibrary.Instance.resourceDefinitions[Resource].id;
            }
            catch
            {
                resourceId = PartResourceLibrary.ElectricityHashcode;
            }

            _navLightColor = new Color(Color.x, Color.y, Color.z);
            // XXX Refactor this 
            _lastTimeFired = Planetarium.GetUniversalTime();

            // Parent for main illumination light, used to move it slightly above the light.
            LightOffsetParent = new GameObject();
            LightOffsetParent.transform.position = base.gameObject.transform.position;
            LightOffsetParent.transform.rotation = base.gameObject.transform.rotation;
            LightOffsetParent.transform.parent = base.gameObject.transform;
            LightOffsetParent.transform.Translate(LightOffset);

            // Main Illumination light
            mainLight = LightOffsetParent.gameObject.AddComponent<Light>();
            mainLight.color = _navLightColor;
            mainLight.intensity = 0.0f;

            UpdateMode();
        }

        public override void OnUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            if (navLightState != NavLightState.Off && EnergyReq > 0.0f && TimeWarp.deltaTime > 0.0f)
            {
                if (vessel.RequestResource(part, resourceId, EnergyReq * TimeWarp.deltaTime, true) < EnergyReq * TimeWarp.deltaTime * 0.5f)
                {
                    // XXX Refactor this - don't change mode, just slam intensity to off.
                    // Lights out ... no power
                    navLightSwitch = (int)NavLightState.Off;
                    UpdateMode();
                }
            }

            switch (navLightState)
            {
                case NavLightState.Off:
                case NavLightState.On:
                    // No-op - taken care of when mode was selected.
                    break;
                case NavLightState.Flash:
                    // Lights go to 'Flash' mode
                    FlashBasedSwitcher();
                    break;
                case NavLightState.DoubleFlash:
                    // Lights go to 'Double Flash' mode
                    DoubleFlashBasedSwitcher();
                    break;
                case NavLightState.Interval:
                    // Lights go to 'Interval' mode
                    IntervalBasedSwitcher();
                    break;
            }
        }

        /// <summary>
        /// Provide VAB info.
        /// </summary>
        /// <returns></returns>
        public override string GetInfo()
        {
            if (EnergyReq > 0.0f)
            {
                string resourceUiName = string.Empty;
                PartResourceDefinition def;
                if (!string.IsNullOrEmpty(Resource))
                {
                    try
                    {
                        def = PartResourceLibrary.Instance.resourceDefinitions[Resource];
                        resourceId = def.id;
                    }
                    catch (Exception)
                    {
                        resourceId = PartResourceLibrary.ElectricityHashcode;
                        def = PartResourceLibrary.Instance.resourceDefinitions[resourceId];
                    }
                }
                else
                {
                    resourceId = PartResourceLibrary.ElectricityHashcode;
                    def = PartResourceLibrary.Instance.resourceDefinitions[resourceId];
                }
                resourceUiName = def.displayName;

                return KSP.Localization.Localizer.Format("#autoLOC_244201", resourceUiName, (EnergyReq * 60.0f).ToString("0.0"));
            }
            else
            {
                return string.Empty;
            }
        }

        private void UpdateLights(bool lightsOn)
        {
            if (lightsOn)
            {
                mainLight.intensity = intensity;
            }
            else
            {
                mainLight.intensity = 0.0f;
            }
        }

        // Periodic strobe with differing on and off times.
        private void FlashBasedSwitcher()
        {
            float interval = nextInterval;
            if (_lastTimeFired < Planetarium.GetUniversalTime() - interval)
            {
                flashCounter = (flashCounter + 1) & 1;

                nextInterval = ((flashCounter & 1) == 1) ? FlashOn : FlashOff;
                _lastTimeFired = Planetarium.GetUniversalTime();
                UpdateLights((flashCounter & 1) == 1);
            }
        }

        // Periodic strobe with two strobes of equal length close together.
        private void DoubleFlashBasedSwitcher()
        {
            float interval = nextInterval;

            if (_lastTimeFired < Planetarium.GetUniversalTime() - nextInterval)
            {
                flashCounter = (flashCounter + 1) & 3;
                nextInterval = (flashCounter > 0) ? FlashOn : FlashOff;

                _lastTimeFired = Planetarium.GetUniversalTime();
                UpdateLights((flashCounter & 1) == 1);
            }
        }

        // Periodic strobe with equal on and off times.
        private void IntervalBasedSwitcher()
        {
            float interval = Interval;
            if (_lastTimeFired < Planetarium.GetUniversalTime() - interval)
            {
                flashCounter = (flashCounter + 1) & 1;

                _lastTimeFired = Planetarium.GetUniversalTime();
                UpdateLights((flashCounter & 1) == 1);
            }
        }

        //--- "Toggle" action group events
        [KSPAction("#autoLOC_6001405", KSPActionGroup.None)]
        public void LightToggle(KSPActionParam param)
        {
            LightOnEvent();
        }

        [KSPAction("#AL_ToggleFlash", KSPActionGroup.None)]
        public void FlashToggle(KSPActionParam param)
        {
            LightFlashEvent();
        }

        [KSPAction("#AL_ToggleDoubleFlash", KSPActionGroup.None)]
        public void DoubleFlashToggle(KSPActionParam param)
        {
            LightDoubleFlashEvent();
        }

        [KSPAction("#AL_ToggleInterval", KSPActionGroup.None)]
        public void IntervalToggle(KSPActionParam param)
        {
            LightIntervalEvent();
        }

        [KSPAction("#AL_CycleModes", KSPActionGroup.None)]
        public void Cycle(KSPActionParam param)
        {
            navLightSwitch = (navLightSwitch + 1) % 5;

            UpdateMode();
        }

        //--- "Set" action group events
        [KSPAction("#autoLOC_6001406", KSPActionGroup.None)]
        public void LightOnAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.On;

            UpdateMode();
        }

        [KSPAction("#AL_SetFlash", KSPActionGroup.None)]
        public void LightFlashAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.Flash;

            UpdateMode();
        }

        [KSPAction("#AL_SetDoubleFlash", KSPActionGroup.None)]
        public void LightDoubleFlashAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.DoubleFlash;

            UpdateMode();
        }

        [KSPAction("#AL_SetInterval", KSPActionGroup.None)]
        public void LightIntervalAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.Interval;

            UpdateMode();
        }

        [KSPAction("#autoLOC_6001407", KSPActionGroup.None)]
        public void LightOffAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.Off;

            UpdateMode();
        }

        private void UpdateMode()
        {
            _lastTimeFired = 0.0;
            flashCounter = 0;

            switch (navLightSwitch)
            {
                case (int)NavLightState.Off:
                default:
                    navLightSwitch = (int)NavLightState.Off; // Trap invalid values from the config file
                    navLightState = NavLightState.Off;
                    modeString = KSP.Localization.Localizer.GetStringByTag("#autoLOC_6001073");
                    break;
                case (int)NavLightState.Flash:
                    navLightState = NavLightState.Flash;
                    modeString = KSP.Localization.Localizer.GetStringByTag("#AL_ModeFlash");
                    break;
                case (int)NavLightState.DoubleFlash:
                    navLightState = NavLightState.DoubleFlash;
                    modeString = KSP.Localization.Localizer.GetStringByTag("#AL_ModeDoubleFlash");
                    break;
                case (int)NavLightState.Interval:
                    navLightState = NavLightState.Interval;
                    modeString = KSP.Localization.Localizer.GetStringByTag("#AL_ModeInterval");
                    break;
                case (int)NavLightState.On:
                    navLightState = NavLightState.On;
                    modeString = KSP.Localization.Localizer.GetStringByTag("#autoLOC_6001074");
                    break;
            }

            if (mainLight != null)
            {
                mainLight.intensity = (navLightState == NavLightState.On) ? intensity : 0.0f;
            }
        }

        //--- Part context menu events
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001405")]
        public void LightOnEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.On) ? (int)NavLightState.Off : (int)NavLightState.On;

            UpdateMode();
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_ToggleFlash")]
        public void LightFlashEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.Flash) ? (int)NavLightState.Off : (int)NavLightState.Flash;

            UpdateMode();
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_ToggleDoubleFlash")]
        public void LightDoubleFlashEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.DoubleFlash) ? (int)NavLightState.Off : (int)NavLightState.DoubleFlash;

            UpdateMode();
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_ToggleInterval")]
        public void LightIntervalEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.Interval) ? (int)NavLightState.Off : (int)NavLightState.Interval;

            UpdateMode();
        }
    }
}
