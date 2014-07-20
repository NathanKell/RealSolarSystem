Version History                         {#changelog}
============

Custom Asteroids conforms to version 2.0.0 of the [Semantic Versioning specification](http://semver.org/spec/v2.0.0.html). 
All version numbers are to be interpreted as described therein. In addition to the actual [public API](http://starstrider42.github.io/Custom-Asteroids/), the [format of asteroid population files](@ref newbelts) will be considered the API for the purpose of versioning.

Version 1.0.0
------------

### New Features 

* Some asteroids will now stay on the stock KSP trajectory, which intercepts Kerbin's sphere of influence. The number of such asteroids may be changed by modifying or removing the `DEFAULT` clause in `Basic Asteroids.cfg`.
* Asteroids can now be labeled by the group they belong to. This option is on by default, but may be disabled in the [settings file](@ref options).
* Asteroid orbital elements can now be expressed in terms of the properties of planets or moons. This reduces the amount of math the config-writer has to do, and makes config files more compatible with other solar system mods.
* Support for version checkers using Tyrope and cybutek's `.version` file format.
* Support for using Module Manager to customize downloaded configs.

### Changes 

* Asteroid groups now have a unique `name` field and a human-readable `title` field, consistent with the format of most other KSP configs. **THIS BREAKS COMPATIBILITY** with the version 0.2 format.
* The position of an orbit's periapsis can now be set by constaining either the argument of periapsis or longitude of periapsis.
* Added support for Gaussian, isotropic, and exponential distributions.

Version 0.2.1
------------

### Bug Fixes 

* Neither asteroids nor vessels will be corrupted when undocking from an asteroid.

Version 0.2.0
------------

### New Features 

* Custom Asteroids will now scan the KSP install for asteroid configuration files. This should make it easier to personalize asteroid sets without conflicting with the mod install.
* Completely new configuration file format. The new format makes much smarter use of default settings, and the distributions assumed for each orbital element are no longer hardcoded.
* Custom Asteroids can now control all six orbital elements.
* Orbit size can be set by constraining semimajor axis, periapsis, or apoapsis. Orbit phase can be set by constraining mean anomaly or mean longitude. These two options give configuration-writers more control over where asteroids will and won't appear.

### Changes 

* Stock configs now have many more minor planet groups
* Added units to config file documentation
* Reorganized code to support asteroid modifications other than orbits in future releases.

Version 0.1.0
------------
* Initial Release
