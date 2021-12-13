# AviationLights
Aviation Lights for Kerbal Space Program

This is a repository for the Aviation Lights mod.

Source code for the MOARdV releases is available on [GitHub](https://github.com/MOARdV/AviationLights).

## CREDITS

From the original AviationLights [forum post](http://forum.kerbalspaceprogram.com/index.php?/topic/16801-105-aviation-lights-v38-16nov15/):

First things first: Big thanks for RPGprayer's "Position/Navigation Lights" AddOn, from which the Aviation Lights originated.

Additional credits go to Deadweasel, Why485, GROOV3ST3R, JDP and J.Random for their great help with this addon.  Thanks to
BigNose for keeping this mod going.

StoneBlue provided the Aviation Lights 4.0 configurable light.

This mod is maintained by MOARdV.

## INFORMATION

There are two types of standard aviation lights:

**Navigation Lights** ("Position Lights") are a safety feature on every plane bigger
than an ultra-light. They indicate the orientation of the plane so other aircraft
know which direction the aircraft is going. For that purpose, the international standard is a red light on the
left wingtip, a green one on the right, and a white nav light on the tail.

Example of the correct use in KSP:

![Aviation Lights](http://s14.directupload.net/images/120813/6cz23stq.png)

But, of course, this is KSP. You can just put them on where it looks cool. :D

**Warning Lights** (both "Strobe" and "Beacon") are flashing lights to enhance
visibility in bad weather and to warn air traffic and ground personnel.

Beacons are typically mounted on the tip of the vertical tail on smaller planes, or in the middle of the
fuselage (top and/or bottom) on larger aircraft. They're red, bright and flashing to indicate that parts of the airplane
(engines) are moving or the airplane itself is about to move.

Strobes are very bright, white, fast blinking lights which are mounted on the wingtips (next to the
red/green navlights) and sometimes on the tail (next to the white navlight) on larger planes. They're
so bright that they remain off until the pilot lines up on the runway, so the ground personnel
won't be blinded if they stand right next to it.

In addition to the standard navigation and warning light colors, there are amber and blue preset options.

## INSTALLATION

Download the latest version of Aviation Lights from the [Releases](https://github.com/MOARdV/AviationLights/releases) page.
Merge the GameData folder in the ZIP file with your KSP GameData folder.  Tell the computer to overwrite any
existing files.  When you have installed the mod correctly, it will look something like

```
+ GameData
	+ AviationLights
		+ Localization
		+ Parts
			+ lights
		+ Patches
		+ Plugins
```

**Note:** Config file and plugin changes in Aviation Lights 4.0 *may* cause lights on existing vessels to behave
oddly (including potentially not working) when upgrading from earlier versions of the mod.
The upgrade should not cause any loss of vessels.

### Localization

Aviation Lights 4.0 and later support localization.  The currently supported languages are en-us, es-es, fr-fr, pt-br, and zh-cn.

Translations would be appreciated.

### B9 Part Switcher

If you wish to use B9 Part Switcher instead of the stock part variants feature, rename MM_B9PartSwitch.nocfg to
MM_B9PartSwitch.cfg.  This will not update existing parts, so I don't recommend changing this in an existing save
without recovering craft and reconfiguring in the VAB.

## VAB/SPH CONFIGURATION

The basic light (Light, Aviation) is a configurable light, allowing one part to fill any of the
preset aviation light roles.  Using advanced tweakables, the color, range, and intensity may be customized even
more.  This light is found in the Utility menu by default (other mods may change this).

Aviation Lights may be configured in the editor (VAB and SPH).  The basic part menu provides a part variant selector to
choose which mode the light will use.  The selected mode is displayed
in the Toggle button next to the variant selector.  If an aviation light is enabled in the editor, it will be enabled when the vessel spawns in
the Flight scene.

A toggle button allows changes to be applied automatically to symmetry parts.

A Type Preset slider allows the light to be configured as a
navigation light, strobe light, or a beacon light.  Additional type presets may be added.  See PRESETS
below for information on how to configure custom types.

A Color Preset slider provides all of the standard Aviation Lights colors (white, red, green, blue, and amber).  This allows a single part
to function as any colored aviation light.  Additional preset colors may be added.  See PRESETS below for
more information on how to configure custom colors.

### Advanced Tweakables

![Advanced Tweakables](https://imageshack.com/a/img923/6797/ZwnnqL.jpg)

When Advanced Tweakables are enabled, parts that support the Color Preset and Type Preset will also have
sliders to customize the RGB colors of the light, as well as the intensity of the light and its range.  The RGB colors
reset if the Color Preset is changed.  The intensity and range reset if the Type Preset is changed.

If the part is configured as a spot light (the SpotAngle config field is greater than 0), the editor
will also allow toggling the light between a spot light or a point (omni-directional) light.

## FLASH MODES

In addition to the conventional "Light on" and "Light off" settings, Aviation Lights may be configured to flash using one of three patterns.
The all-caps / all-lower below describes the pattern (ALL-CAPS = light is on, all-lower = light is off), with the
name reporting which config value controls how long the light spends in that state.

* **Flash**: `FLASHON-flashoff` - In this mode, the light flashes on and off.  The time spent with the light on may be different than the time with the light off.
The `FlashOn` setting in the config controls how long the light will switch on, and `FlashOff` controls how long it remains off.
* **Double Flash**: `FLASHON-flashon-FLASHON-flashoff` - In this mode, the light flashes on and off.  The on time is a double flash - the light will turn on, turn off, and turn on again
before turning off for a different period of time.  The `FlashOff` setting controls how long the light remains off after the double flash, while `FlashOn` controls how long the light
remains on as well as how long it switches off between the double flashes.
* **Interval**: `INTERVAL-interval` - In this mode, the light flashes evenly on and off.  The amount of time spent on or off is controlled by the `Interval` setting in the config.

## PRESETS

Aviation Lights 4.0 and later supports *presets*.  There are two categories of presets, *type* and *color*.  The AL package includes its
default presets in GameData/AviationLights/Plugins/AviationLightsPresets.cfg.  This config file supports MM editing.  Players and modders may also add
their own custom presets in separate config files - AL will scan all applicable config nodes for color and type presets.

### Type Presets

Type presets control the type of light that is configured.  Each type defines the intensity of the light, the range of the light, and the
intervals for the flash patterns.  Type presets are searched for in config nodes named `AVIATION_LIGHTS_PRESET_TYPES`.

```
AVIATION_LIGHTS_PRESET_TYPES
{
	name = DefaultAviationLightsTypes
	
	Type
	{
		name = nav
		guiName = #AL_TypeNavigation

		flashOn = 0.5
		flashOff = 1.5
		interval = 1.0

		intensity = 0.5
		range = 10
	}
	
	... additional Type nodes
}
```

* **name** - The name of the preset type (making it easier to edit using MM).
* **guiName** - The name of the preset type that shows up in the Type Preset control.  This field supports localization.
* **flashOn**, **flashOff**, **interval** - Timing values for the flash modes of the light, as described above in FLASH MODES.
* **intensity** - How bright the light is.  Nav Lights, for instance, use 0.5.  Strobe or beacon lights may use values of 1 or higher (with a maximum of 8).
* **range** - The range of the light.

### Color Presets

Color presets are simply color options that may be selected in the VAB.  Color presets are searched for in config nodes named `AVIATION_LIGHTS_PRESET_COLORS`.

```
AVIATION_LIGHTS_PRESET_COLORS
{
	name = DefaultAviationLightsColors
	
	Color
	{
		name = white
		guiName = #AL_ColorWhite
		value = 1.00, 0.95, 0.91
	}
	
	... additional Color nodes
}
```

* **name** - The name of the color (making it easier to edit using MM).
* **guiName** - The name of the color that shows up in the Color Preset control.  This field supports localization.
* **value** - The normalized RGB values of the color, ranging from 0 to 1.


## MODULE CONFIGURATION

There are a number of fields in ModuleNavLight that allow creation of a custom of a part. Default values are shown below.

```
MODULE
{
   name = ModuleNavLight

   Color = 1.0, 0.95, 0.91

   Intensity = 0.5
   Range = 10.0

   Interval = 1.0
   FlashOn = 0.5
   FlashOff = 1.5

   Resource = ElectricCharge
   EnergyReq = 0.0

   LightOffset = 0.0, 0.0, 0.0
   LightRotation = 0, 0, 0
   SpotAngle = 0
   LensTransform = ""
   
   Tweakable = true
}
```

### Light Color

The light color values default to a white navigation light.  The color fields allow a part creator to set up
a light for a specific purpose by defining the color, intensity, and range of the light.
When Tweakable = true, the color may be changed using the Color Preset control in the part menu,
and the Intensity and Range may be changed with the Type Preset control.  In addition, the color, intensity, and range may be edited
directly by enabling Advanced Tweakables.

* **Color**: The RGB color of the light.  Valid values are from 0 to 1 for each channel.
* **Intensity**: The intensity of the light.  Brighter lights should use larger values.  Valid numbers range from 0 to 8.  Nav lights use 0.5.  Energy consumption
is affected by Intensity.
* **Range**: The range of the light, in meters.  This setting controls how far from the light any illumination is projected.  Objects outside this range are not illuminated by the light.

### Flash Timing

The default flash timing values are for a navigation light.   Flash timing is one component of the Type Preset when
Tweakable = true, which means that custom timings may be overridden in the Editor.  If the custom timing does not also
have a type preset defined for it, it will not be possible for a player to restore custom timing on a light without removing it and attaching
a new one.  All times are measured in seconds.

* **Interval**: How long the light stays on or stays off when it is in Interval mode.
* **FlashOn**: How long the light stays on in Flash mode or Double Flash mode, and how long it stays off between the two flashes in Double Flash mode.
* **FlashOff**: How long the light stays off in Flash mode or after the second flash in Double Flash mode.

### Resources

The resource fields control the resource type and amount consumed per second.  By default, the parts require ElectricCharge, but they do not
consume energy.

EnergyReq is affected by the Intensity of the light.  The EnergyReq listed in the part config is the amount of resources required for a light
with an Intensity of 1.0.  EnergyReq is scaled by the square of the Intensity, with a minimum scale of 0.25.  For example, a Nav Light that has an EnergyReq of 0.020
and an Intensity of 0.5 will actually use 0.005 EC (= 0.020 x (0.5 x 0.5)).  An Intensity of 2.0 will consume 4x the listed EnergyReq.

* **Resource**: The name (from the RESOURCE_DEFINITION) of the resource consumed when this light is on.
* **EnergyReq**: The amount of the resource consumed per second while switched on for a light of Intensity = 1.0.  If this value is zero, the light does not consume any resources.
Intensity modifies this value.

### Advanced

These fields are advanced fields available for modders to create custom light models that integrate with Aviation Lights.

* **LightOffset**: The displacement from the root gameObject of the model where the light should be added, in meters.
* **LightRotation**: Rotates the light's game object around the X, Y, and Z axis.  This field is only applicable for spot lights.
* **SpotAngle**: When SpotAngle is greater than zero, the light functions as a spot light instead of a point (omni-directional) light.  SpotAngle is the width of the spotlight in degrees.
* **LensTransform**: A semi-colon (';') delimited list of transforms in the model that contain lens textures.  AL will adjust the diffuse (_Color) and emissive (_EmissiveColor) tint on all of those transforms based on the current color of the light.  If this field is omitted, lens colors will not change to match the light's color.
* **Tweakable**: A boolean that controls whether the part's color and type may be changed in the editor.  With some custom
lights, such as lights with pre-tinted lenses, allowing the colors and types to be changed in the Editor may result in poor-looking models in-flight.

There are persistent values not listed here.  These values are used to keep track of internal state. They should not be
added to a config file or edited in the persistent.sfs file.

## CHANGELOG

unreleased - v4.1.3

* Removed the legacy (Aviation Lights 3.x) lights.
* Fix pt-br localization, also courtesy Lisias.

***

21 May 2021 - v4.1.2

* Added Brazilian Portuguese (pt-br) courtesy Lisias

***

5 July 2020 - v4.1.1

* Recompiled for KSP 1.10.0 (and finally updated the version file, alas).
* Fixed bug that enabled the spotlight toggle on non-tweakable lights (such as the old aviation lights), as noted by GitHub user Promyclon.

***
16 October 2019 - v4.1.0

* Recompiled for KSP 1.8.0
* Redesigned the VAB PAW components.

***
6 July 2019 - v4.0.8

* Added zh-cn localization courtesy duck1998.

***
25 February 2019 - v4.0.7

* Added fr-fr localization courtesy don-vip.
* Finally synchronized the DLL binary and the version file (forgot to update .version for 4.0.6).

***
21 December 2018 - v4.0.6

* Added es-es localization courtesy fitiales.  Pull request #15.
* Scaled energy consumption based on light intensity.  Issue #13.

***
16 October 2018 - v4.0.5.1

* Recompiled against KSP 1.5.0.
* Fixed: Typo in version file.

***
6 May 2018 - v4.0.5

* Fixed: Aviation Lights illuminate the surface of the planet (Issue #10).

***
9 April 2018 - v4.0.4

* Fixed: Flashing lights blink rapidly after warp during flight (Issue #8).

***
7 April 2018 - v4.0.3

* Implement correct symmetry updating behavior for the point/spot toggle (Issue #7).

***
7 April 2018 - v4.0.2

* Fixed: Emissive layer not switching correctly during flight (Issue #6).
* Tweakable lights configured as spot lights (SpotAngle > 0) have an Advanced Tweakable toggle that converts them to point (omni-directional) lights (Issue #7).

***
3 April 2018 - v4.0.1

* Added tags to the parts, including a Community Category Kit tag to place the lights in the CCK Lights category.

***
31 March 2018 - v4.0.0 (Redesign - MOARdV)

* Fixed: Double Flash mode.
* Major overhaul of the plugin code.
* Added more configurable fields for customization.
* Added color and type presets selectable in the VAB.
* Made some light settings adjustable in the VAB using Advancted Tweakables.
* Expanded the documentation in the README.
* Added localization support.
* Hid legacy lights in the Editor.  Included a MM patch to unhide them.

***
26MAY17 - v3.14 (MOARdV)

* Fix VAB/SPH NRE introduced in v3.11.

***
25MAY17 - v3.13 (KSP 1.3.0 fix - MOARdV)

* Recompiled for KSP 1.3.0.

***
19OCT16 - v3.12 (MOARdV)

* Re-enabled configuring light modes in the VAB/SPH (inadvertently removed them in 3.11).

***
12OCT16 - v3.11 (Real KSP 1.2 fix - MOARdV)

* Fixed some oversights in KSP 1.2.0 compatibility (Issue #1).
* Converted PNG to DDS and rescaled them to 128 x 128.
* Fixed minor bug where flash interval was reversed if the light was already on.

***
11OCT16 - v3.10 (KSP 1.2.x maintainance release - MOARdV)

* Fixed: Works with KSP 1.2.0

***
8MAY16 - v3.9 (KSP 1.1.x maintainance release - MOARdV)

* Fixed: Works with KSP 1.1.x.
* Changed: Put it on GitHub, added visual studio project files and all that stuff.

***
16NOV15 - v3.8 (Entry cost and physics - BigNose)

* Added: Lights now have double their normal price as entry cost in the tech tree
* Changed: Lights are now skipped by the physics engine (performance gain when using them in large numbers)

## LICENSE

This AddOn builds on content originally made by RPGprayer. Used with permission.

This work is licensed under the [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License](http://creativecommons.org/licenses/by-nc-sa/4.0/).
