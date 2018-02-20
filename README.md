# AviationLights
Aviation Lights for Kerbal Space Program

This is a repository for the Aviation Lights mod, previously maintained by BigNose.

Source code for the MOARdV releases is available on [GitHub](https://github.com/MOARdV/AviationLights).

## CREDITS

From the original AviationLights [forum post](http://forum.kerbalspaceprogram.com/index.php?/topic/16801-105-aviation-lights-v38-16nov15/):

First things first: Big thanks for RPGprayer's "Position/Navigation Lights" AddOn, from which the Aviation Lights originated.

Additional credits go to Deadweasel, Why485, GROOV3ST3R, JDP and J.Random for their great help with this addon.

## INFORMATION

There are two types of standard aviation lights:

**Navigation Lights** ("Position Lights") are a safety feature on every plane bigger
than an ultra-light. They indicate the orientation of the plane for other air traffic, so other aircraft
know in which direction you're going. For that purpose, the international standard is a red light on the
left wingtip, a green one on the right, and a white nav light on the tail.

Example for the correct use in KSP:

![Aviation Lights](http://s14.directupload.net/images/120813/6cz23stq.png)

But, of course, this is KSP. You can just put them on where it looks cool. :D

**Warning Lights** (both "Strobe" and "Beacon") are flashing lights to enhance
visibility in bad weather and to warn air traffic and ground personnel.

Beacons are typically mounted on tip of the vertical tail on smaller planes, or in the middle of the
fuselage (top and/or bottom) on larger aircraft. They're red, bright and flashing to indicate that parts of the airplane
(engines) are moving or the airplane itself is about to move.

Strobes are very bright, white, fast blinking lights which are mounted on the wingtips (next to the
red/green navlights) and sometimes on the tail (next to the white navlight) on larger planes. They're
so bright that they remain off until the pilot lines up on the runway, so the ground personnel
won't be blinded if they stand right next to it.

In addition to the standard navigation and warning light colors, there are amber and blue lamp options.

## VAB/SPH CONFIGURATION

![Basic Menu](https://imageshack.com/a/img924/5732/ad9F9z.jpg)

Aviation Lights may be configured in the editors (VAB and SPH).  The basic part menu provides four flash mode selection
buttons which can be used to select the default illumination behavior of the light.  The selected mode is displayed
above the mode selection buttons.  If an aviation light is glowing in the editor, it will be on when the vessel spawns in
the Flight scene.  Note that lights do not flash in the editor.

A toggle button allows changes to be applied automatically to symmetry parts.

Options to toggle the various flash modes as well as to active a flash mode or switch off the light are all available in the
action groups editor.

Newer parts support additional configuration options.

![Configuration Menu](https://imageshack.com/a/img923/6828/pnyUw2.jpg)

A Type Preset slider allows the light to be configured as a
navigation light, strobe light, beacon light, or any other type defined in an Aviation Lights type preset. See PRESETS
below for information on how to configure custom types.

A Color Preset slider provides all of the standard Aviation Lights colors.  This allows a single part
to function as any colored aviation light.  Additional preset colors may be added.  See PRESETS below for
more information on how to configure custom colors.

### Advanced Tweakables

![Advanced Tweakables](https://imageshack.com/a/img923/6797/ZwnnqL.jpg)

When Advanced Tweakables are enabled, parts that support the Color Preset and Type Preset will also have individual
sliders to customize the RGB colors of the light, along with the intensity of the light and its range.  The RGB colors
reset if the Color Preset is changed.  The intensity and range reset if the Type Preset is changed.

## FLASH MODES

In addition to the conventional "Light on" and "Light off" settings, Aviation Lights may be configured to flash using one of three patterns.  The all-caps / all-lower below describes the pattern (ALL-CAPS = light is on, all-lower = light is off), with the
name reporting which config value controls how long the light spends in that state.

* **Flash**: `FLASHON-flashoff` - In this mode, the light flashes on and off.  However, instead of spending an equal amount of time on and off, the timing is different.
The `FlashOn` setting in the config controls how long the light will switch on, and `FlashOff` controls how long it remains off.
* **Double Flash**: `FLASHON-flashon-FLASHON-flashoff` - In this mode, the light flashes on and off.  The on time is a double flash - the light will turn on, turn off, and turn on again
before turning off for a longer period of time.  The `FlashOff` setting controls how long the light remains off, while `FlashOn` controls how long the light
remains on as well as how long it switches off between the double flashes.
* **Interval**: `INTERVAL-interval` - In this mode, the light flashes steadily on and off.  The amount of time spent on or off is controlled by the `Interval` setting in the config.

## PRESETS

Aviation Lights 4.0 and later supports *presets*.  There are two categories of presets, *type* and *color*.  The AL package includes its
default presets in GameData/AviationLights/Plugins/AviationLightsPresets.cfg.  This config file supports MM editing.  Players and modders may also add
their own custom presets in separate config files - AL will scan all valid config nodes for color and type presets.

### Type Presets

Type presets control the type of light that is configured.  Each type defines the intensity of the light, the range of the light, and the
intervals for the flash patterns.  Type presets are searched for in config nodes named `AVIATION_LIGHTS_PRESET_TYPES`.

```
AVIATION_LIGHTS_PRESET_TYPES
{
	name = DefaultAviationLightsTypes
	
	Type
	{
		name = strobe
		guiName = #AL_TypeStrobe

		flashOn = 0.1
		flashOff = 0.9
		interval = 0.4

		intensity = 1.0
		range = 10
	}
	
	...
}
```

* **name** - The name of the color (allowing MM to edit it).
* **guiName** - The name of the color that shows up in the Color Preset control.  This field supports localization.
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
	
	...
}
```

* **name** - The name of the color (allowing MM to edit it).
* **guiName** - The name of the color that shows up in the Color Preset control.  This field supports localization.
* **value** - The normalized RGB values of the color, ranging from 0 to 1.


## MODULE CONFIGURATION

There are a number of fields in ModuleNavLight that allow customization of a part. Default values are shown below.

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
   LensTransform = ""
   
   Tweakable = true
}
```

### Light Color

The light color values default to a white navigation light.  However, the config file allows other settings to be used for
pre-configured lights, such as the AL 3.x style Aviation Lights.  When Tweakable = true, the color can be changed using the
Color Preset control in the part menu,
and the Intensity and Range can be changed with the Type Preset control.  In addition, the color, intensity, and range may be edited
directly by enabling Advanced Tweakables.

* **Color**: The RGB color of the light.  Valid values are from 0 to 1 for each channel.
* **Intensity**: The intensity of the light.  Brighter lights should use larger values.  Valid numbers range from 0 to 8.  Nav lights use 0.5.
* **Range**: The range of the light, in meters.

### Flash Timing

The default flash timing pattern is for a navigation light.   Flash timing is one component of the Type Preset when
Tweakble = true, which means that custom timings may be overridden in the Editor.  If the custom timing does not also
have a type preset defined for it, it will not be possible for a player to restore custom timing on a light without removing it and attaching
a new one.

* **Interval**: How long the light stays on or stays off when it is in Interval mode.  This time is measured in seconds.
* **FlashOn**: How long the light stays on in Flash mode or Double Flash mode, and how long it stays off between the two flashes in Double Flash mode.
* **FlashOff**: How long the light stays off in Flash mode or Double Flash mode.

### Resources

The resource fields control the resource type and amount consumed per second.  By default, the parts require ElectricCharge, but they do not
actually consume energy.

* **Resource**: The name (from the RESOURCE_DEFINITION) of the resource consumed when this light is on.
* **EnergyReq**: The amount of the resource consumed per second.  If this value is zero, the light does not consume any resources.

### Advanced

These fields are advanced fields available for modders to create custom light parts that integrate with Aviation Lights.

* **LightOffset**: The displacement from the root gameObject of the model where the light should be added, in meters.  The default setting applies to the original Aviation
Lights parts, so it is not required for them.
* **LensTransform**: A semi-colon (';') delimited list of transforms in the model that contain lens textures.  AL will adjust the diffuse (_Color) and emissive (_EmissiveColor) tint on all of those transforms based on the current color of the light.  If this field is omitted, lens colors will not change to match the light's color.
* **Tweakable**: A boolean that controls whether the part's color and type may be changed in the editor.  With some custom
lights, allowing the colors and types to be changed in the Editor may result in poor-looking models in-flight.

There are persistent values not listed here.  These values are used to keep track of what state, and they should not be
set in a config file or edited in the persistent.sfs file.

## CHANGELOG

coming soon, 2018 - v4.00 (Redesign - MOARdV)

* Major overhaul of the plugin code.
* Fixed Double Flash mode (it used to flicker on).
* Added more configurable fields for customization.
* Added color and type presets selectable in the VAB.
* Made some light settings adjustable in the VAB using Advancted Tweakables.
* Expanded the documentation in the README.
* Added localization support.

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
