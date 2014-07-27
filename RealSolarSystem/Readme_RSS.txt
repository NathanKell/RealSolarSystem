Real Solar System
NathanKell
This mod will convert the Kerbol System into the (Real) Solar System, rescaling, moving, and changing KSP's planets, moons, etc. to match our own.

Thanks to asmi for kicking me into to doing this and offering so much help along the way; to ferram for aeronautics and orbital help (and FAR, which is essential), and for countless time spent helping others get the most from this mod (and me the most from my modding); to ZRM for many ideas and info; to yargnit and MedievalNerd for playtesting extraordinaire; to everyone else who offered suggestions, code, help, cool screenshots...

Supreme thanks to regex for code, and dimonnomid and SpacedInvader for incredible art for RSS, and pingopete for his work on RSS - EVE interoperability and atmosphere work for RSS. RSS would not look or perform the way it does (or have gotten released!) without their amazing contributions.

License: CC-BY-NC-SA
Includes code by Majiir (CompatibilityChecker, licensed as per source) and stupid_chris (ConfigNodeExtensions, licensed CC-BY-NC-SA). Used with permission.
Includes artwork by dimonnomid and SpacedInvader.
Some planetary imagery is derived from work by Steve Albers and NASA / Jet Propulsion Laboratory, and some from the Celestia Motherlode (itself in the main sourced from JPL). Used by permission of the licenses for non-commercial release.

Also included:
Module Manager (by sarbian, swamp_ig, and ialdabaoth). See thread for details, license, and source: http://forum.kerbalspaceprogram.com/threads/55219
Custom Biomes by Trueborn. See thread for details, license, and source: http://forum.kerbalspaceprogram.com/threads/66256
Custom Asteriods by Starstrider42. See thread for details, license, and source: http://forum.kerbalspaceprogram.com/threads/80483


INSTALLATION:
Extract to KSP/GameData.

TEXTURE INSTALLATION
Now, you must select a texture resolution. You may download a premade pack (8192, 4096, or 2048), or pick and choose. Any planets which do not have textures will not have their coloration, features, etc., changed, although they will be made larger.
You can get the textures from: https://nabaal.net/files/ksp/nathankell/RealSolarSystem/Textures/

NOTE: You really should play with the recommended mods. See the Realism Overhaul thread for details.

List of planets:
Mercury is represented by Moho
Venus is represented by Eve
Earth is represented by Kerbin
Moon is represented by Mun
Mars is represented by Duna
Phobos is represented by Bop
Deimos is represented by Gilly
Jupiter is represented by Jool
Io is represented by Pol
Europa is represented by Eeloo
Ganymede is represented by Tylo
Callisto is represented by Ike
Saturn is represented by Dres
Titan is represented by Laythe
Uranus is represented by Minmus
Pluto is represented by Vall

===========================
Changelog
v7.1  \/
*Update to Custom Biomes 1.6.4
*Give Earth's terrain more texture
*fix AFG bug.

v7.0  \/
*Scaled Space scaling/wrapping now plays nice with other mods and does not require OBJ files. Loads fast.
*regex: support changing orbitColor in Orbit nodes (as standard float RGBA color). dimonnomid adds colors for all RSS bodies.
*Fix to display the Display Name of the launch site when showing the icons on the planet in Tracking Station view.
*Fail gracefully when textures are missing.
*Include Custom Biomes and configs. Custom Biomes by Trueborn. Biome map by Subcidal. Only Earth supported so far.
*Include Custom Asteroids. Custom Asteroids by Starstrider42, config by SpacedInvader
*Support new images for every planet by default (will fail gracefully if not found)
*Added ability to edit and add more PQSMods, and to disable any PQSMod.
*Changed oceanColor (in Export node) to use 4-value color.
*Added ability to change the color ramp on the rim of scaled space shaders, either by specifying a file via SSRamp, or another body's by SSRampRef.
*Added ability to change specular color of scaled space shader via SSSpec.
*Refactored AtmosphereFromGround code to work better, allow changing more values, etc.
*Include new textures for all bodies thanks to SpacedInvader, dimonnomid, and the sources (see above)
*Include atmospher changes for all bodies
*Bug in initial orbital positions of all bodies is fixed. Note that craft in the SOI of the Sun will be off course, so BE CAREFUL. You may have to wait until your craft reach their destination SOIs before using v7.
*Added support for changing flight camera clipping distances (for use with EVE).
*Lowered max atmosphere altitudes to match approximate height of 1Pa dynamic pressure at 12,000m/s. Earth's is now 130km, for example.
*Compiled for 0.24.2 x64

v6.2  \/
*PQS->ScaledSpace wrapping now works and caches correctly. Wrap now defaults to false for backwards compatibility. RSS will export obj files (with extra lines using keyword t for vertex tangents) for all wrapped meshes, and import them (if they exist) instead of wrapping. NOTE: If you change any PQS settings (let alone changing RSS configs!) you MUST delete all .obj files in GameData/RealSolarSystem/PluginData! NOTE 2: If you don't already have cached meshes, and/or you delete them all, KSP WILL APPEAR TO HANG on the "Loading..." screen right before Main Menu. This is NORMAL. Let it run. It takes me about 15 minutes. After that, you'll get your usual load times.
*regex: fixed Space Center initial camera height (for when camera is under terrain)
*tons of launch sites from eggrobin!
*regex: fixes for KSCSwitcher; new icon; now saves and loads selected KSC location with your persistence file; shows descriptions; icons hidden on zoomout; can click to set map to focus on selected location.
*Update to ModuleManager 2.1.5
*tweaked Earth's horizon glow a bit

v6.1  \/
*Fixed only changing things if radius differs
*Fixed to work on non-Windows platforms (regex)
*Fixed many atmosphere and temperature curves (eggrobin)
*Fixed misc typos
*Added Moon retexture from SpacedInvader
*Added bodyName support
*Parsing changes, using stupid_chris's ConfigNodeExtensions
*Added KSCSwitcher from regex: can switch KSC to other presets in the Tracking Station
*Added many, many launch site configurations (compiled by eggrobin from the work by Captain Party, ferram, and others)
*Added new descriptions to all Celestial Bodies, thanks to TheKosmonaut/SpaceAnt
*Speedup in timewarpchecker from swamp_ig
*Added support for PQS->scaledspace wrapping (for real this time; didn't work before).
*Added support for replacing scaledspace meshes with spheres (use this for now, importing is broken)
*Added support for compressing normals (defaults to true)

v6    \/
*Changed Kerbin's heightmap, textures, and PQSMods to resemble Earth (allow replacing heightmaps, SS diffuse, and SS normal maps)
*PQSCity and MapDecalTangent can be positioned by latitude/longitude
*Modify scaledspace meshes based on PQS terrain automatically on game load
*Add many new PQSMod settings and limited add/remove support.
*Fixed orbit lines (thanks HoneyFox!)
*Added pressureCurve support with curves for Earth, Venus, and Mars (megathanks Starwaster!)
*Added temperatureCurve support (nothing here yet)
*Fixed a typo with tidally locked orbits (thanks eggrobin!)
*Converted orbits to Earth-relative inclination, to support axial tilt (megathanks eggrobbin!)
*Recompiled for .23.5

v5.6  \/
*Fixed wavelenght color parsing to take four arguments (colors are RGBA after all)
*Added compatibility patches for VisualEnhancements and (thanks, jrandom!) SCANSat
 
v5.5  \/
*Fixed for KSP 0.23

v5.4  \/
*Solar panels finally fixed (really!) thanks to StarWaster.
*Fixed atmo editor keychange

v5.3  \/
*Changed atmo editor to ALT-G
*Fixed Mean Anomaly to be Mean Anomaly at Epoch, fixed handling of it to actually work
*Added new root-of-config property Epoch, for setting starting date. Now, KSP year 0 is 1950.
*Support enabling/disabling atmosphere
*Used metaphor's planet config changes
*Added ability to change science parameters for all bodies (includes config by Medieval Nerd)
*Fixed SOI not respecting minimums

v5.2  \/
*Fixed Titan's atmosphere
*Fixed solar panels (I hope)
*Added configurable atmospheres per Starwaster's work.

v5.1  \/
*Fixed bug where scaledspace meshes were being faded too early (now missing planets appear)
*Fixed Duna's atmosphere end height in the info box.

v5 -- \/
*Every KSP body is now a proxy for a body in the real solar system. However, only the terrains of Kerbin and Mun are edited; the others are merely rescaled.
*Added configurable timewarp. By default 5 and 50 are removed and instead 1 million x and 6m x are added on at the end.
*Edited terrain generator for Kerbin and Mun.
*More support for parameters in the config file.
*Using Majiir's KSPAddonFixed.
*Moved KSC to be nearer the shore.

v4 -- \/
*Switched to new config file system
*Fixed max zoomout on map camera
*Fixed some scaledspace issues
*Changed VAB/SPH camera for more freedom (with a fix: thanks, asmi!)
*Removed 5x and 50x timewarp; added 1 million x and 10m ex timewarp.
*Adjusted solar panels to inverse-square law

v3 -- \/
*Automatically multiplies distances in solar panels' powerCurves by 11 so that they yield rated charge.
*Fixed Kerbin's SMA

v2.1 -- \/
*Went back to scaling Mun's scaledspace transform for now
*Fixed Mun's inclination to avg vs. Earth's equator (since we can't tilt Kerbin's rot axis, we tilt what orbits it)

v2 -- \/
*Added asmi's fix to get KSC, KSC2, and Island land to show up
*Fixed Minmus period, changed Minmus inclination and made it like a captured rock.
*No longer changing Mun's scaledspace representation (it defaults to 0 anyway).

v1 -- \/
*First stable release.