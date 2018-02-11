using System;
using UnityEngine;

//Originally made by RPGprayer, edited by BigNose, Why485, GROOV3ST3R, JDP and J.Random
// Fixes for KSP 1.1 applied by MOARdV.
//License: This file contains code from RPGprayers "Position/Navigation Lights". Used with permission.
namespace AviationLights
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
        private int resourceId;

        [KSPField]
        public float EnergyReq = 0.0f;

        [KSPField]
        public float IntervalFlashMode = 0.0f;
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

        public override void OnStart(PartModule.StartState state)
        {
            if (state == StartState.Editor) return;

            try
            {
                resourceId = PartResourceLibrary.Instance.resourceDefinitions[Resource].id;
            }
            catch
            {
                resourceId = PartResourceLibrary.ElectricityHashcode;
            }

            flightStarted = true;

            _navLightColor = new Color(Color.x, Color.y, Color.z);
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

            // Glow Illumination light
            glowLight = base.gameObject.AddComponent<Light>();
            glowLight.color = _navLightColor;
            glowLight.intensity = 0.0f;
        }

        public override void OnUpdate()
        {
            if (!flightStarted) return;

            switch (navLightSwitch)
            {
                case (int)navLightStates.navLightState.Off:
                    // Lights go to 'Off' mode
                    mainLight.intensity = 0.0f;
                    glowLight.intensity = 0.0f;
                    break;
                case (int)navLightStates.navLightState.Flash:
                    // Lights go to 'Flash' mode
                    FlashBasedSwitcher(IntervalFlashMode);
                    break;
                case (int)navLightStates.navLightState.DoubleFlash:
                    // Lights go to 'Double Flash' mode
                    DoubleFlashBasedSwitcher(IntervalFlashMode);
                    break;
                case (int)navLightStates.navLightState.Interval:
                    // Lights go to 'Interval' mode
                    IntervalBasedSwitcher(Interval);
                    break;
                case (int)navLightStates.navLightState.On:
                    // Lights go to 'On' mode
                    mainLight.intensity = INTENSITY_OFFSET;
                    glowLight.intensity = INTENSITY_GLOW;
                    break;
            }

            // Energy requirements check: if the light is not off and requires resources, request resource. If returned resource is less than requested; turn off
            if (navLightSwitch > 0 && EnergyReq > 0.0f && TimeWarp.deltaTime > 0.0f)
            {
                if (vessel.RequestResource(part, resourceId, EnergyReq * TimeWarp.deltaTime, true) < EnergyReq * TimeWarp.deltaTime * 0.5f)
                {
                    navLightSwitch = (int)navLightStates.navLightState.Off;
                }
            }
        }

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

        private void FlashBasedSwitcher(float FlashOn)
        {
            if (_lastTimeFired < Planetarium.GetUniversalTime() - FlashOn)
            {
                b = !b;
                IntervalFlashMode = b ? this.FlashOn : FlashOff;
                _lastTimeFired = Planetarium.GetUniversalTime();
                mainLight.intensity = (mainLight.intensity == INTENSITY_OFFSET) ? 0.0f : INTENSITY_OFFSET;
                glowLight.intensity = (glowLight.intensity == INTENSITY_GLOW) ? 0.0f : INTENSITY_GLOW;
            }
        }

        private void DoubleFlashBasedSwitcher(float FlashOn)
        {
            mainLight.intensity = 0.0f;
            glowLight.intensity = 0.0f;

            if (_lastTimeFired < Planetarium.GetUniversalTime() - FlashOn)
            {
                b = !b;
                IntervalFlashMode = b ? this.FlashOn : FlashOff;
                _lastTimeFired = Planetarium.GetUniversalTime();
                mainLight.intensity = (mainLight.intensity == INTENSITY_OFFSET) ? 0.0f : INTENSITY_OFFSET;
                glowLight.intensity = (glowLight.intensity == INTENSITY_GLOW) ? 0.0f : INTENSITY_GLOW;
            }
        }

        private void IntervalBasedSwitcher(float Interval)
        {
            if (_lastTimeFired < Planetarium.GetUniversalTime() - Interval)
            {
                _lastTimeFired = Planetarium.GetUniversalTime();
                mainLight.intensity = (mainLight.intensity == INTENSITY_OFFSET) ? 0.0f : INTENSITY_OFFSET;
                glowLight.intensity = (glowLight.intensity == INTENSITY_GLOW) ? 0.0f : INTENSITY_GLOW;
            }
        }

        [KSPAction("#AL_LightToggle", KSPActionGroup.None)]
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
            _lastTimeFired = 0.0;
            b = false;

            navLightSwitch = (navLightSwitch + 1) % 5;
            //if (navLightSwitch == 4)
            //{
            //    navLightSwitch = 0;
            //}
            //else
            //{
            //    navLightSwitch++;
            //}
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_SetFlash")]
        public void LightFlashEvent()
        {
            _lastTimeFired = 0.0;
            b = false;
            if (mainLight != null)
            {
                mainLight.intensity = 0.0f;
                glowLight.intensity = 0.0f;
            }

            if (navLightSwitch == 1)
            {
                navLightSwitch = 0;
            }
            else
            {
                navLightSwitch = 1;
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_SetDoubleFlash")]
        public void LightDoubleFlashEvent()
        {
            _lastTimeFired = 0.0;
            b = false;
            if (mainLight != null)
            {
                mainLight.intensity = 0.0f;
                glowLight.intensity = 0.0f;
            }

            if (navLightSwitch == 2)
            {
                navLightSwitch = 0;
            }
            else
            {
                navLightSwitch = 2;
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_SetInterval")]
        public void LightIntervalEvent()
        {
            _lastTimeFired = 0.0;
            b = false;
            if (mainLight != null)
            {
                mainLight.intensity = 0.0f;
                glowLight.intensity = 0.0f;
            }

            if (navLightSwitch == 3)
            {
                navLightSwitch = 0;
            }
            else
            {
                navLightSwitch = 3;
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#AL_SetLight")]
        public void LightOnEvent()
        {
            _lastTimeFired = 0.0;
            b = false;

            if (navLightSwitch == 4)
            {
                navLightSwitch = 0;
            }
            else
            {
                navLightSwitch = 4;
            }
        }
    }
}
