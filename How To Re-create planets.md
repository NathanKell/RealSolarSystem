I recommend doing this task in an advanced feature text editor meant for code. For example, I used Notepad++.

You will need Kopernicus ( from [Thomas's fork](https://github.com/ThomasKerman/Kopernicus/tree/development) ) as well as RealSolarSystem from 0.90, including its CustomBiomes and RSSTextures additions.

Review the most recent version (currently https://github.com/KSP-RO/RealSolarSystem) and select a planet that does not yet have a configuration.

Planets without atmospheres are more straightforward to convert over, and the current Moon configuration can be used as a guide. For planets with atmospheres, Mars or Earth can be used for guidance.

You can literally copy and paste a similar planet or body as a starting point. For example, to create Mercury you could copy the current Moon configuration from 'Body' and including all of the PQS sections.

Rename your copy (ie. name = Moon becomes name = Mercury) and review each line for necessary changes to reflect the body you are creating. Nearly all of this information already exists in the RealSolarSystem.cfg file. Ensure you use the new names, for instance 'longitudeOfAscendingNode' and not LAN.

The biome png from CustomBiomes will have to be renamed and relocated. The biome .att data will need to be converted from a 0 - 255 scale to a 0 - 1 scale. For example, you can see each former and new scaling for each of the Moon biomes in the current RSSKopernicus.cfg file. If you find making the new biomes difficult, you can leave this for later or someone else as it isn't critical for now.

Often the RealSolarSystem.cfg will show modifications of 'CelestialBodyScienceParams' for a body. These changes need to be moved to a 'ScienceValues' section in the new cfg.

The ScaledVersion Material section needs to be changed to use the appropriate textures for the planet you are recreating.

Just as for ScaledVersion the correct textures need to be used for the PQS. The old RealSolarSystem.cfg will tell you what PQSMods are modified or added, add those in the PQS node with their stats from the old cfg but with the names Kopernicus wants (generally remove PQSMod_ from the front). Note that RSS would only list changes to values; you may need to reference [this dump](https://www.dropbox.com/s/x95wxmnbd7metxs/Components.zip?dl=0) of KSP stock planet data to find the existing values. If the RealSolarSystem.cfg lists mods to disable, you need to add node for the body you're adding to the Finalize node at the end of the system file; examine how one is added for the Moon to see how to use it to dsiable PQSMods or make final changes.

I highly recommend doing each of the steps above one at a time then testing them ensure they will work as expected. I also recommend setting aside a copy of your config outside of KSP each time before you move onto the next step, so that you have a good known working version to go back to incase you run into trouble. Using this method it will remain clear which steps are complete and what remains to be changed.

Good luck, creator of worlds!


And for bodies with atmospheres, the pressures need to be converted from atmospheres to kPa and temperatures from Celcius to Kelvin. In addition the heights are given in meters now, not km. NathanKell suggested using Excel to do this and I found that quite successful. 

Pressure tables:
Kilometers needs to be converted to meters ( * 1000). Pressures need to be converted ( * 101.325) and the two values after that tell the curve how to fit between the points. Those values need to be converted by ( * 101.325 / 1000)

Temperature tables:
Kilometers needs to be converted to meters ( * 1000). Temperatures need to be converted by ( + 272.15) the following two values need to be divided by 1000 ( / 1000)

These can be complicated. If there are no planets without atmospheres left to create, or your eally want to do some of the work on an atmosphered planet you can just leave this portion undone. For example, if you make Venus from Eve without doing this, then the temp and pressure curves for Venus will just be those for Eve until this work is done later. Every bit of work helps!
