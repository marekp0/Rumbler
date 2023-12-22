# Rumbler

A mod for [Beat Saber](https://beatsaber.com/) that lets you choose exactly how
you want your controllers to vibrate or "rumble" while playing. Loosely based
on [nalulululuna's RumbleMod][1], but with more customization and updated for
newer versions of Beat Saber.

[1]: https://github.com/nalulululuna/RumbleMod

## Why?

After Beat Saber v1.29.1, the game completely changed how controller rumble
worked. In the old version of the game, controller rumble was done as a series
of short pulses, while newer versions appear to just vibrate the controller at
a constant frequency. I thought the new behavior was particularly unpleasant on
my Valve Index controllers, and there are no built-in options to change the
controller rumble parameters.

## Features

You can specify separate rumble parameters for each of these:

- Normal notes
- Chain start notes (short normal)
- Chain link notes (short weak)
- Bomb
- Bad cut
- Obstacles (walls)
- Sliders (arcs)
- Saber clash
- UI bumps

For each of the above, you can specify the following parameters:

- Strength
- Rumble duration (not applicable to obstacles, sliders, and saber clashing)
- Pulse frequency
- Pulse duration
- Pulse train period

![Diagram showing a series of pulses, annotated with what each of the above controls](./docs/pulse-train.webp)

## Installing

This mod is not available through [ModAssistant][2] at the moment.
 
1. Use [ModAssistant][2] to install `BSPIA` and `BeatSaberMarkupLanguage`
2. Download the latest release from the [Releases][3] page
3. Copy `Rumbler.dll` to your Beat Saber `Plugins` folder
    - On Steam, this is usually `C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\Plugins`

[2]: https://github.com/Assistant/ModAssistant
[3]: https://github.com/marekp0/Rumbler/releases

## Usage

1. Start Beat Saber
2. Go to `Settings` -> `Mod Settings` -> `Rumbler`
3. Toggle `Enable Rumbler` to `On`
4. Adjust the settings in the other tabs to your liking
    - The default settings are as close as possible to Beat Saber v1.29.1
      behavior, as tested on Valve Index controllers

## Caveats

- This has only been tested on the Valve Index controllers, YMMV with other controllers!
- This should work with any version of Beat Saber after v1.29.1, but has only
  been tested on v1.34.0 and later.

## Issues

If you run into any issues or have any feature requests, feel free to open an
issue on the [GitHub issue tracker][3].

[4]: https://github.com/marekp0/Rumbler/issues
