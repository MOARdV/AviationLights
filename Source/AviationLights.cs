using System;
using System.Collections.Generic;
using UnityEngine;

// Originally made by RPGprayer, edited by BigNose, Why485, GROOV3ST3R, JDP and J.Random
// Fixes for KSP 1.1, additional fixes, and refactoring by MOARdV.
// License: This file contains code from RPGprayers "Position/Navigation Lights". Used with permission.
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

        public class TypePreset
        {
		    public float flashOn;
		    public float flashOff;
		    public float interval;
		    public float intensity;
		    public float range;
        }

        [KSPField(isPersistant = true)]
        public int navLightSwitch = (int)NavLightState.Off;
        private NavLightState navLightState = NavLightState.Off;

        [KSPField(isPersistant = true)]
        public int toggleMode = (int)NavLightState.Flash;

        [KSPField]
        public string Resource = "ElectricCharge";
        private int resourceId;

        [KSPField]
        public float EnergyReq = 0.0f;

        [KSPField(isPersistant = true)]
        public float Interval = 1.0f;

        [KSPField(isPersistant = true)]
        public float FlashOn = 0.5f;

        [KSPField(isPersistant = true)]
        public float FlashOff = 1.5f;

        [KSPField(isPersistant = true)]
        public Vector3 Color = Vector3.zero;
        private List<Vector3> presetColorValues;

        [KSPField(guiActiveEditor = true, guiName = "#AL_TypePreset")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public int typePreset = 0;
        private List<TypePreset> presetTypes;

        [KSPField(guiActiveEditor = true, guiName = "#AL_ColorPreset")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public int colorPreset = 0;

        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001402", advancedTweakable = true)]
        [UI_FloatRange(stepIncrement = 0.05f, maxValue = 1.0f, minValue = 0.0f, affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public float lightR;

        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001403", advancedTweakable = true)]
        [UI_FloatRange(stepIncrement = 0.05f, maxValue = 1.0f, minValue = 0.0f, affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public float lightG;

        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001404", advancedTweakable = true)]
        [UI_FloatRange(stepIncrement = 0.05f, maxValue = 1.0f, minValue = 0.0f, affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public float lightB;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "#AL_LightIntensity", advancedTweakable = true)]
        [UI_FloatRange(minValue = 0.0f, stepIncrement = 0.25f, maxValue = 8.0f, affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public float Intensity = 1.0f;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName = "#AL_LightRange", advancedTweakable = true)]
        [UI_FloatRange(minValue = 1.0f, stepIncrement = 1.0f, maxValue = 50.0f, affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
        public float Range = 10.0f;

        [KSPField]
        public Vector3 LightOffset = new Vector3(0.33f, 0.0f, 0.0f);

        [KSPField]
        public string LensTransform = string.Empty;
        private Material[] lensMaterial = new Material[0];
        private readonly int colorProperty = Shader.PropertyToID("_Color");
        private readonly int emissiveColorProperty = Shader.PropertyToID("_EmissiveColor");

        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "#AL_LightMode")]
        public string modeString;

        private int flashCounter = 0;

        private float nextInterval = 0.0f;
        private float elapsedTime = 0.0f;

        private GameObject lightOffsetParent;
        private Light mainLight;
        private BaseEvent toggleBaseEvent;

        /// <summary>
        /// Initialize game object / light, set up mode status and flasher controls.
        /// </summary>
        public void Start()
        {
            if (!string.IsNullOrEmpty(LensTransform))
            {
                string[] lensNames = LensTransform.Split(';');
                List<Material> materials = new List<Material>();
                MeshRenderer[] mrs = gameObject.transform.GetComponentsInChildren<MeshRenderer>();

                for (int i = 0; i < lensNames.Length; ++i)
                {
                    string lensName = lensNames[i].Trim();
                    MeshRenderer lens = Array.Find<MeshRenderer>(mrs, x => x.name == lensName);
                    // Do I really need to test lens.material?
                    if (lens != null && lens.material != null)
                    {
                        materials.Add(lens.material);
                    }
                }

                lensMaterial = materials.ToArray();
            }

            // Sanity check, in case someone decided to manually edit the save
            // game or add this to the part config.
            if (navLightSwitch < 0 || navLightSwitch > 4)
            {
                navLightSwitch = 0;
            }

            toggleBaseEvent = Events["ToggleEvent"];

            try
            {
                resourceId = PartResourceLibrary.Instance.resourceDefinitions[Resource].id;
            }
            catch
            {
                resourceId = PartResourceLibrary.ElectricityHashcode;
            }

            Intensity = Mathf.Clamp(Intensity, 0.0f, 8.0f);

            Color newColor = new Color(Color.x, Color.y, Color.z);
            if (lensMaterial.Length > 0)
            {
                for (int i = 0; i < lensMaterial.Length; ++i)
                {
                    lensMaterial[i].SetColor(colorProperty, newColor);
                    lensMaterial[i].SetColor(emissiveColorProperty, newColor);
                }
            }

            // Initialize the sliders for advanced tweakables.
            lightR = Color.x;
            lightG = Color.y;
            lightB = Color.z;

            // Parent for main illumination light, used to move it slightly above the light.
            lightOffsetParent = new GameObject("AL_light");
            lightOffsetParent.transform.position = base.gameObject.transform.position;
            lightOffsetParent.transform.rotation = base.gameObject.transform.rotation;
            lightOffsetParent.transform.parent = base.gameObject.transform;
            lightOffsetParent.transform.Translate(LightOffset);

            // Main Illumination light
            mainLight = lightOffsetParent.gameObject.AddComponent<Light>();
            mainLight.color = newColor;
            // Restore the light iff we're in the editor and the light's on.  If it's off, or we're in flight, it'll be updated later.
            mainLight.intensity = (HighLogic.LoadedSceneIsEditor && navLightSwitch != (int)NavLightState.Off) ? Intensity : 0.0f;
            mainLight.range = Range;

            UpdateMode();

            if (HighLogic.LoadedSceneIsEditor)
            {
                SetupChooser();
            }
        }

        /// <summary>
        /// For the editor, load the color presets so the player can adjust colors in the VAB.
        /// </summary>
        private void SetupChooser()
        {
            ConfigNode[] colorPresetNodes = GameDatabase.Instance.GetConfigNodes("AVIATION_LIGHTS_PRESET_COLORS");
            List<string> colorNames = new List<string>();
            presetColorValues = new List<Vector3>();
            for (int presetNode = 0; presetNode < colorPresetNodes.Length; ++presetNode)
            {
                ConfigNode[] colors = colorPresetNodes[presetNode].GetNodes("Color");
                for(int colorIndex = 0; colorIndex < colors.Length; ++colorIndex)
                {
                    string guiName = string.Empty;
                    Vector3 value = new Vector3(0.0f, 0.0f, 0.0f);
                    if (colors[colorIndex].TryGetValue("guiName", ref guiName) && colors[colorIndex].TryGetValue("value", ref value))
                    {
                        if (colorNames.Contains(guiName) == false)
                        {
                            colorNames.Add(guiName);
                            presetColorValues.Add(value);
                        }
                    }
                }
            }

            BaseField chooseField = Fields["colorPreset"];
            if (colorNames.Count > 0)
            {
                UI_ChooseOption chooseOption = (UI_ChooseOption)chooseField.uiControlEditor;
                chooseOption.options = colorNames.ToArray();
                chooseOption.onFieldChanged = ColorPresetChanged;
            }
            else
            {
                // No colors?  No preset slider.
                chooseField.guiActiveEditor = false;
            }

            ConfigNode[] typePresetNodes = GameDatabase.Instance.GetConfigNodes("AVIATION_LIGHTS_PRESET_TYPES");
            List<string> presetNames = new List<string>();
            presetTypes = new List<TypePreset>();
            for (int presetNode = 0; presetNode < typePresetNodes.Length; ++presetNode)
            {
                ConfigNode[] types = typePresetNodes[presetNode].GetNodes("Type");
                for (int typeIndex = 0; typeIndex < types.Length; ++typeIndex)
                {
                    string guiName = string.Empty;
                    float flashOn = 0.0f, flashOff = 0.0f, interval = 0.0f, intensity = 0.0f, range = 0.0f;
                    if (types[typeIndex].TryGetValue("guiName", ref guiName) && 
                        types[typeIndex].TryGetValue("flashOn", ref flashOn) &&
                        types[typeIndex].TryGetValue("flashOff", ref flashOff) &&
                        types[typeIndex].TryGetValue("interval", ref interval) &&
                        types[typeIndex].TryGetValue("intensity", ref intensity) &&
                        types[typeIndex].TryGetValue("range", ref range))
                    {
                        if (presetNames.Contains(guiName) == false)
                        {
                            presetNames.Add(guiName);
                            
                            TypePreset type = new TypePreset();
                            type.flashOn = flashOn;
                            type.flashOff = flashOff;
                            type.interval = interval;
                            type.intensity = intensity;
                            type.range = range;

                            presetTypes.Add(type);
                        }
                    }
                }
            }

            chooseField = Fields["typePreset"];
            if (presetNames.Count > 0)
            {
                UI_ChooseOption chooseOption = (UI_ChooseOption)chooseField.uiControlEditor;
                chooseOption.options = presetNames.ToArray();
                chooseOption.onFieldChanged = TypePresetChanged;
            }
            else
            {
                // No colors?  No preset slider.
                chooseField.guiActiveEditor = false;
            }
        }

        private void TypePresetChanged(BaseField field, object oldFieldValueObj)
        {
            TypePreset newtype = presetTypes[typePreset];

            FlashOn = newtype.flashOn;
            FlashOff = newtype.flashOff;
            Interval = newtype.interval;
            Intensity = newtype.intensity;
            Range = newtype.range;
        }

        /// <summary>
        /// Callback to manage changes to the preset colors slider.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oldFieldValueObj"></param>
        private void ColorPresetChanged(BaseField field, object oldFieldValueObj)
        {
            Color = presetColorValues[colorPreset];
            lightR = Color.x;
            lightG = Color.y;
            lightB = Color.z;
        }

        /// <summary>
        /// Check to update lights.
        /// </summary>
        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (navLightState != NavLightState.Off && EnergyReq > 0.0f && TimeWarp.deltaTime > 0.0f)
                {
                    if (vessel.RequestResource(part, resourceId, EnergyReq * TimeWarp.deltaTime, true) < EnergyReq * TimeWarp.deltaTime * 0.5f)
                    {
                        mainLight.intensity = 0.0f;
                        return;
                    }
                }

                elapsedTime += TimeWarp.deltaTime;

                switch (navLightState)
                {
                    case NavLightState.Off:
                    case NavLightState.On:
                        // No-op - taken care of when mode was selected.
                        break;

                    case NavLightState.Flash:
                        // Lights are in 'Flash' mode
                        if (elapsedTime >= nextInterval)
                        {
                            elapsedTime -= nextInterval;

                            flashCounter = (flashCounter + 1) & 1;

                            nextInterval = ((flashCounter & 1) == 1) ? FlashOn : FlashOff;

                            UpdateLights((flashCounter & 1) == 1);
                        }
                        break;

                    case NavLightState.DoubleFlash:
                        // Lights are in 'Double Flash' mode
                        if (elapsedTime >= nextInterval)
                        {
                            elapsedTime -= nextInterval;

                            flashCounter = (flashCounter + 1) & 3;

                            nextInterval = (flashCounter > 0) ? FlashOn : FlashOff;

                            UpdateLights((flashCounter & 1) == 1);
                        }
                        break;

                    case NavLightState.Interval:
                        // Lights are in 'Interval' mode
                        if (elapsedTime >= Interval)
                        {
                            elapsedTime -= Interval;

                            flashCounter = (flashCounter + 1) & 1;
                            UpdateLights((flashCounter & 1) == 1);
                        }
                        break;
                }
            }
            else if (HighLogic.LoadedSceneIsEditor)
            {
                // Account for any tweakable tweaks.
                Color.x = lightR;
                Color.y = lightG;
                Color.z = lightB;

                if (lensMaterial.Length > 0)
                {
                    Color newColor = new Color(Color.x, Color.y, Color.z);
                    for (int i = 0; i < lensMaterial.Length; ++i)
                    {
                        lensMaterial[i].SetColor(colorProperty, newColor);
                        lensMaterial[i].SetColor(emissiveColorProperty, newColor);
                    }
                }

                // Not sure how to track time in the VAB, so just use solid light intensity.
                mainLight.intensity = (navLightState != NavLightState.Off) ? Intensity : 0.0f;
                mainLight.range = Range;
                mainLight.color = new Color(Color.x, Color.y, Color.z);
            }
        }

        /// <summary>
        /// Provide the VAB module display name.
        /// </summary>
        /// <returns></returns>
        public override string GetModuleDisplayName()
        {
            return "#AL_ModuleDisplayName";
        }

        /// <summary>
        /// Provide VAB info.
        /// </summary>
        /// <returns></returns>
        public override string GetInfo()
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

            if (EnergyReq > 0.0f)
            {
                return KSP.Localization.Localizer.Format("#autoLOC_244201", resourceUiName, (EnergyReq * 60.0f).ToString("0.0"));
            }
            else
            {
                return KSP.Localization.Localizer.Format("#AL_NoEnergy", resourceUiName);
            }
        }

        /// <summary>
        /// Toggle the light on or off.
        /// </summary>
        /// <param name="lightsOn"></param>
        private void UpdateLights(bool lightsOn)
        {
            if (lightsOn)
            {
                mainLight.intensity = Intensity;
            }
            else
            {
                mainLight.intensity = 0.0f;
            }
        }

        /// <summary>
        /// Update settings based on navLightSwitch changing, reset counters, etc.
        /// </summary>
        private void UpdateMode()
        {
            elapsedTime = 0.0f;
            flashCounter = 0;

            switch (navLightSwitch)
            {
                case (int)NavLightState.Off:
                default:
                    navLightSwitch = (int)NavLightState.Off; // Trap invalid values from the config file
                    navLightState = NavLightState.Off;
                    break;
                case (int)NavLightState.Flash:
                    navLightState = NavLightState.Flash;
                    nextInterval = FlashOff;
                    break;
                case (int)NavLightState.DoubleFlash:
                    navLightState = NavLightState.DoubleFlash;
                    nextInterval = FlashOff;
                    break;
                case (int)NavLightState.Interval:
                    navLightState = NavLightState.Interval;
                    break;
                case (int)NavLightState.On:
                    navLightState = NavLightState.On;
                    break;
            }

            switch (toggleMode)
            {
                case (int)NavLightState.Off:
                case (int)NavLightState.Flash:
                default:
                    // For the toggle mode, always force the default to ModeFlash.
                    modeString = KSP.Localization.Localizer.GetStringByTag("#AL_ModeFlash");
                    toggleBaseEvent.guiName = "#AL_ToggleFlash";
                    break;
                case (int)NavLightState.DoubleFlash:
                    modeString = KSP.Localization.Localizer.GetStringByTag("#AL_ModeDoubleFlash");
                    toggleBaseEvent.guiName = "#AL_ToggleDoubleFlash";
                    break;
                case (int)NavLightState.Interval:
                    modeString = KSP.Localization.Localizer.GetStringByTag("#AL_ModeInterval");
                    toggleBaseEvent.guiName = "#AL_ToggleInterval";
                    break;
                case (int)NavLightState.On:
                    modeString = KSP.Localization.Localizer.GetStringByTag("#autoLOC_6001074");
                    toggleBaseEvent.guiName = "#autoLOC_6001405";
                    break;
            }

            if (HighLogic.LoadedSceneIsFlight)
            {
                mainLight.intensity = (navLightState == NavLightState.On) ? Intensity : 0.0f;
            }
        }

        //--- "Toggle" action group actions ----------------------------------

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

        //--- "Set" action group actions -------------------------------------

        [KSPAction("#autoLOC_6001406", KSPActionGroup.None)]
        public void LightOnAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.On;
            toggleMode = navLightSwitch;

            UpdateMode();
        }

        [KSPAction("#AL_SetFlash", KSPActionGroup.None)]
        public void LightFlashAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.Flash;
            toggleMode = navLightSwitch;

            UpdateMode();
        }

        [KSPAction("#AL_SetDoubleFlash", KSPActionGroup.None)]
        public void LightDoubleFlashAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.DoubleFlash;
            toggleMode = navLightSwitch;

            UpdateMode();
        }

        [KSPAction("#AL_SetInterval", KSPActionGroup.None)]
        public void LightIntervalAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.Interval;
            toggleMode = navLightSwitch;

            UpdateMode();
        }

        [KSPAction("#autoLOC_6001407", KSPActionGroup.None)]
        public void LightOffAction(KSPActionParam param)
        {
            navLightSwitch = (int)NavLightState.Off;

            UpdateMode();
        }

        //--- Part context menu events ---------------------------------------

        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "#AL_ToggleFlash")]
        public void ToggleEvent()
        {
            navLightSwitch = (navLightSwitch == toggleMode) ? (int)NavLightState.Off : toggleMode;

            UpdateMode();
        }

        [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001405")]
        public void LightOnEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.On) ? (int)NavLightState.Off : (int)NavLightState.On;
            toggleMode = (int)NavLightState.On;

            UpdateMode();
        }

        [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#AL_ToggleFlash")]
        public void LightFlashEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.Flash) ? (int)NavLightState.Off : (int)NavLightState.Flash;
            toggleMode = (int)NavLightState.Flash;

            UpdateMode();
        }

        [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#AL_ToggleDoubleFlash")]
        public void LightDoubleFlashEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.DoubleFlash) ? (int)NavLightState.Off : (int)NavLightState.DoubleFlash;
            toggleMode = (int)NavLightState.DoubleFlash;

            UpdateMode();
        }

        [KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#AL_ToggleInterval")]
        public void LightIntervalEvent()
        {
            navLightSwitch = (navLightSwitch == (int)NavLightState.Interval) ? (int)NavLightState.Off : (int)NavLightState.Interval;
            toggleMode = (int)NavLightState.Interval;

            UpdateMode();
        }
    }
}
