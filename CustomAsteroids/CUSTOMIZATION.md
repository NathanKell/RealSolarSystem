Custom(izing) Asteroids                         {#newbelts}
============

Asteroid definition files declare where asteroids (or comets, or other small bodies) appear in the game. The only such file included in the Custom Asteroids download is `GameData/CustomAsteroids/config/Basic Asteroids.cfg`. However, any .cfg file that follows the same format will be parsed by Custom Asteroids.

Within each file, each `ASTEROIDGROUP` block represents a single group of orbits. There is no limit to the number of `ASTEROIDGROUP` blocks you can place in a file.

There may also be up to one `DEFAULT` block across all files, which controls how many asteroids are found on a Kerbin intercept trajectory as in the stock game. If the player has multiple files with a `DEFAULT` block, only one will be used.

Basic Usage
------------

The most frequently used fields in each `ASTEROIDGROUP` block are the following:

* `name`: a unique, machine-readable name. Must not contain spaces.
* `title`: a descriptive name. If `RenameAsteroids = True` is set in the [settings file](@ref options), 
    this name will replace the generic "Ast." in the asteroids' name.
* `centralBody`: the name of the object the asteroids will orbit. Must exactly match the name of an 
    in-game celestial body.
* `spawnRate`: must be a nonnegative number. If `UseCustomSpawner = True` is set in the 
    [settings file](@ref options), this value gives the number of asteroids detected per Earth day. 
    If `UseCustomSpawner = False`, only the ratio to all the other `spawnRate` values matters.
* `orbitSize`: a block describing how far from `centralBody` the asteroid's orbit is found. Parameters:
    - `type`: Describes which orbital element is constrained by `min` and `max`. Allowed values are 
        SemimajorAxis, Periapsis, or Apoapsis.
    - `min`: The smallest value an asteroid from this group may have, in meters.
    - `max`: The largest value an asteroid from this group may have.
* `eccentricity`: a block describing what eccentricities an asteroid from the group may have.
    - `avg`: the average eccentricity of an asteroid in this population. Must be a nonnegative number. 
        Any specific asteroid may have any eccentricity; it is even possible, though very unlikely, 
        that an asteroid will appear on an unbound orbit.
* `inclination`: a block describing what inclinations an asteroid from the group may have.
    - `avg`: the average inclination of an asteroid in this population, in degrees. Should be a 
        nonnegative number. As with eccentricities, you may occasionally get some extreme values.

The `DEFAULT` block, if present, has only the `name` and `spawnRate` fields. These work analogously to the `name` and `spawnRate` fields for `ASTEROIDGROUP`. If no `DEFAULT` block is present, all asteroids will appear in one of the asteroid groups.

Advanced Usage
------------

The average number of known asteroids in each group -- if none are tracked -- will equal `spawnRate` times the average of `Options.MinUntrackedTime` and `Options.MaxUntrackedTime`. Set the value for `spawnRate` accordingly.

Each `ASTEROIDGROUP` block has six subfields corresponding to orbital parameters. Each orbital parameter has a block describing the distribution of that parameter:
* `dist`: the distribution from which the parameter will be drawn. Allowed values are Uniform, 
    LogUniform, Gaussian (Normal also accepted), Rayleigh, or Exponential.
* `min`: the minimum value of the parameter. Currently used by Uniform and LogUniform.
* `max`: the maximum value of the parameter. Currently used by Uniform and LogUniform.
* `avg`: the average value of the parameter. Currently used by Gaussian, Rayleigh, and Exponential.
* `stddev`: the standard deviation of the parameter. Currently used by Gaussian.

Allowed values of `min`, `max`, `avg`, and `stddev` are:
* A floating-point number, giving the exact value (in appropriate units) for the parameter
* A string of the form 'Ratio(&lt;planet&gt;.&lt;stat&gt;, &lt;value&gt;)', where &lt;planet&gt; 
    is the name of a celestial body, &lt;value&gt; is a floating-point multiplier, and 
    &lt;stat&gt; is one of 
    - rad: the radius of &lt;planet&gt;, in meters
    - soi: the sphere of influence of &lt;planet&gt;, in meters
    - sma: the semimajor axis of &lt;planet&gt;, in meters
    - per: the periapsis of &lt;planet&gt;, in meters
    - apo: the apoapsis of &lt;planet&gt;, in meters
    - ecc: the eccentricity of &lt;planet&gt;
    - inc: the inclination of &lt;planet&gt;, in degrees
    - ape: the argument of periapsis of &lt;planet&gt;, in degrees
    - lpe: the longitude of periapsis of &lt;planet&gt;, in degrees
    - lan: the longitude of ascending node of &lt;planet&gt;, in degrees
    - mna0: the mean anomaly (at game start) of &lt;planet&gt;, in degrees
    - mnl0: the mean longitude (at game start) of &lt;planet&gt;, in degrees

  For example, the string `Ratio(Jool.sma, 0.5)` means "half of Jool's semimajor axis, in meters".
* A string of the form 'Offset(&lt;planet&gt;.&lt;stat&gt;, &lt;value&gt;)', where &lt;planet&gt; 
    and &lt;stat&gt; have the same meanings as above, and &lt;value&gt; is the amount to add to 
    the celestial body's orbital element (units determined by &lt;stat&gt;). For example, the string 
    `Offset(Duna.per, -50000000)` means "50,000,000 meters less than Duna's periapsis", or just 
    beyond its sphere of influence.

The six orbital elements are:
* `orbitSize`: one of three parameters describing the size of the orbit, in meters. This is the 
    only orbital element that must *always* be given. Distribution defaults to LogUniform if 
    unspecified. The `orbitSize` node also has two additional options:
    - `type`: may be SemimajorAxis, Periapsis, or Apoapsis. Defaults to SemimajorAxis.
    - The `min`, `max`, or `avg` fields of `orbitSize` may take a string of the form 
    'Resonance(&lt;planet&gt;, &lt;m&gt;:&lt;n&gt;)', where &lt;planet&gt; is the name of a 
    celestial body, and &lt;m&gt; and &lt;n&gt; are positive integers. The string will be interpreted 
    as the semimajor axis needed to get an m:n mean-motion resonance with &lt;planet&gt;. For 
    example, the string `Resonance(Jool, 2:3)` gives the semimajor axis to complete 2 orbits for 
    every 3 orbits of Jool -- in other words, the semimajor axis of Eeloo.
* `eccentricity`: the eccentricity of the orbit. If omitted, defaults to circular orbits. Distribution 
    defaults to Rayleigh if unspecified. If the distribution is changed to one that uses `min` and 
    `max`, these values default to the 0-1 range.
* `inclination`: the inclination of the orbit, in degrees. If omitted, defaults to uninclined orbits. 
    Distribution defaults to Rayleigh if unspecified. The `inclination` node has one additional option:
    - `dist` may take the value Isotropic, which will randomly orient the orbital plane if 
        `ascNode` is kept at its default. `min`, `max`, `avg`, and `stddev` are ignored for an 
        Isotropic distribution.
* `periapsis`: the position of the periapsis, in degrees. If omitted, allows any angle. Distribution 
    defaults to Uniform if unspecified. The `periapsis` node also has one additional option:
    - `type`: the convention for placing the periapsis. May be Argument (angle from ascending 
        node) or Longitude (absolute position). Defaults to Argument.
* `ascNode`: the longitude of the ascending node, in degrees. If omitted, allows any angle. 
    Distribution defaults to Uniform if unspecified.
* `orbitPhase`: one of two parameters describing the asteroid's position along its orbit, in degrees. 
    If omitted, allows any angle. Distribution defaults to Uniform if unspecified. The `orbitPhase` 
    node also has two additional options:
    - `type`: the convention for measuring the asteroid's progress along its orbit. May be 
        MeanAnomaly (value proportional to time since periapsis) or MeanLongitude (value 
        proportional to time since zero phase angle). Defaults to MeanAnomaly.
    - `epoch`: the time at which the mean anomaly or mean longitude is measured. May be GameStart 
        or Now. Defaults to GameStart.
