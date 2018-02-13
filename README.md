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

**Navigation Lights** (often called "Position Lights") are a safety feature on every plane bigger than an ultra-light. They indicate the orientation of the plane for other air traffic, so they know in which direction you're going. For that, the international standard of a red light on the left wingtip, a green one on the right and a white on the tail was set.

Example for the correct use in KSP:

![Aviation Lights](http://s14.directupload.net/images/120813/6cz23stq.png)

But, of course, this is KSP. You can just put them on where it looks cool. :D

**Warning Lights** (more commonly referred to as "Strobe&Beacon") are flashing lights to enhance detection range in bad weather and to warn other air traffic and ground personnel.

Beacons are mostly mounted on tip of the vertical tail on smaller planes, or in the middle of the fuselage (top and/or bottom). They're red, bright and flashing to indicate that parts of the airplane (engines) or the airplanes itself are moving or about to be moved.

Strobes are very bright, white, fast blinking lights which are mounted on the wingtips (next to the red/green navlights) and sometimes on the tail (next to the white navlight) on larger planes. They're in fact so bright, that they remain off until the pilot lines up on the runway, so the ground personnel won't get blinded if they stand right next to it.

Due to popular request, I now also included the amber warning light by Deadweasel, which I just adapted to the new file system and values.

## LIGHT MODES

In addition to the generic "Light on" and "Light off" settings, Aviation Lights may be configured to flash using one of three patterns.  The all-caps / all-lower describes the pattern (ALL-CAPS = light is on, all-lower = light is off), with the variable
that controls how long the light spends on or off is reported by the name.

* **Interval**: `INTERVAL-interval` - In this mode, the light flashes steadily on and off.  The amount of time spent on or off is controlled by the `Interval` setting in the config.
* **Flash**: `FLASHON-flashoff` - In this mode, the light flashes on and off.  However, instead of spending an equal amount of time on and off, the timing can be altered.
The `FlashOn` setting in the config controls how long the light will switch on, and `FlashOff` controls how long it remains off.
* **Double Flash**: `FLASHON-flashon-FLASHON-flashoff` - In this mode, the light flashes on and off.  The on time is a double flash - the light will turn on, turn off, and turn on again
before turning off for a longer period of time.  The `FlashOff` setting controls how long the light remains off, while `FlashOn` controls how long the light
remains on as well as how long it switches off between the double flashes.

## MODULE CONFIGURATION

There are a number of fields in ModuleNavLight that allow customization of a part. Default values are shown below.

```
MODULE
{
   name = ModuleNavLight

   Color = 0.0, 0.0, 0.0
   Intensity = 1.0
   Range = 10.0

   Interval = 1.0
   FlashOn = 0.5
   FlashOff = 1.5

   Resource = ElectricCharge
   EnergyReq = 0.0

   LightOffset = 0.33, 0.0, 0.0
}
```

* **Color**: The RGB color of the light.  Valid values are from 0 to 1 for each channel.  You will want to override this field, or the light won't do anything useful.  This value may be changed in the VAB when Advanced Tweakables is enabled.
* **Intensity**: The intensity of the light.  Brighter lights should use larger values.  Valid numbers range from 0 to 8.  The included beacon and strobe use 1.0, the nav lights use 0.5.  This value may be changed in the VAB when Advanced Tweakables is enabled.
* **Range**: The range of the light, in meters.  This value may be changed in the VAB when Advanced Tweakables is enabled.
* **Interval**: How long the light stays on or stays off when it is in Interval mode.  This time is measured in seconds.
* **FlashOn**: How long the light stays on in Flash mode or Double Flash mode, and how long it stays off between the two flashes in Double Flash mode.
* **FlashOff**: How long the light stays off in Flash mode or Double Flash mode.
* **Resource**: The name (from the RESOURCE_DEFINITION) of the resource consumed when this light is on.
* **EnergyReq**: The amount of the resource consumed per second.  If this value is zero, the light does not consume any resources.
* **LightOffset**: The displacement from the root gameObject of the model where the light should be added, in meters.  The default setting applies to the original Aviation
Lights parts, so it is not required for them.

While `Color` is adjustable in the VAB, the current Aviation Lights models do not support changing the color of the part's lens.

There are persistent values not listed here.  These values are used to keep track of what state, and they should not be
set in a config file or edited in the persistent.sfs file.

## CHANGELOG

coming soon, 2018 - v4.00 (The Big Redesign - MOARdV)

* Major overhaul of the plugin code.
* Add more configurable fields for customization.
* Make some light settings adjustable in the VAB using Advancted Tweakables.
* Support localization.

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
