Custom Asteroids Options                         {#options}
============

When it is first run, Custom Asteroids creates a config file in `GameData/CustomAsteroids/PluginData/Custom Asteroids Settings.cfg`. This file may be used to toggle the following options:

* `RenameAsteroids`: if True (the default), asteroids will be named by the group they belong to. If False, asteroids will keep their default names.
* `UseCustomSpawner`: if True (the default), asteroids will be discovered at a steady rate. If False, new asteroids will be discovered any time there are no untracked asteroids, as in the stock game.

* `MinUntrackedTime`: the minimum number of Earth days an asteroid can stay untracked before it disappears. NOT affected by `UseCustomSpawner`. Must be nonnegative.
* `MaxUntrackedTime`: the maximum number of Earth days an asteroid can stay untracked before it disappears. NOT affected by `UseCustomSpawner`. Must be positive, and must be no less than `MinUntrackedTime`.

* `VersionNumber`: the plugin version for which the options were written. **DO NOT CHANGE THIS**. Custom Asteroids uses this field for backward compatibility support.
