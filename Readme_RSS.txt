Real Solar System WIP

Thanks to asmi for kicking me into to doing this and offering help along the way; to ferram for aeronautics and orbital help (and FAR, which is essential), to ZRM for many ideas and info, to yargnit for playtesting extraordinaire, and to everyone else who has been playtesting it so far in the Realism Overhaul thread and offered other help.


I take no responsibility for this breaking anything, even if it weren't a WIP and totally in alpha stage.

License: CC-BY-SA

Extract to KSP/GameData.

Start a new game and enjoy. Oh, and grab my Modular Fuels Continued v3 and set it to realistic Mass mode, or you'll have an even harder time than real life.

YOU REALLY SHOULD USE FAR WITH THIS. And make sure you're not using KIDS to scale the Isps of anything. Deadly Reentry needs more powerful heatshields (WIP, check the thread).

The planets are pretty blurry, BTW. I know.

Planets included (no new graphics for now, excepting Kerbin and Mun)
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

Known bugs:
*Parts are wobbly. Get Kerbal Joint Reinforcement by ferram
*Sometimes you get a black screen trying to launch. Revert, close KSP, restart, try again.
*Water disappears when looking right at it.
*Doubled terrain on some planets (I haven't made new scaledspace meshes yet)

Expect problems. But also expect glee.
===========================
Changelog
v5.2  \/
*Fixed Titan's atmosphere
*Fixed solar panels (I hope)

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