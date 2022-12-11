# Gideon

Gideon Ofnir, the All-Knowing, is here to carefully handle your MENU_Knowledge files for you. (Tool for saving high-resolution icons in ELDEN RING modding)

**This is only for ELDEN RING**, not the other franchise games. It could technically work in other games, but I'd have to make that happen first.

You might need the Oodle DLL from your game folder, or perhaps not, I'm not sure.

## What's this for?

Adding new icons for new items to [ELDEN RING Reforged](https://www.nexusmods.com/eldenring/mods/541), I noticed that this is done with the following obnoxious workflow:

* Extract `00_Solo.tpfbhd`.
* Extract one of the `MENU_Knowledge_XXX.tpf.dcx` archives within, using Yabber.
* Replace the DDS within the extracted TPF archive with yours.
* Change the Yabber index file to reflect the new filename.
* Re-pack the newly made TPF.
* Repeat for EVERY SINGLE ICON!!!
* Re-pack `00_Solo.tpfbhd`.

It is enough to drive a man insane. No more!

## Usage

Point Gideon at a folder containing DDS files named in the required `MENU_Knowledge_XXX.dds` pattern, and your `00_Solo.tpfbhd` file, and watch him go!

There are three ways to run the tool:

* Run `Gideon.exe` directly from Windows. You will receive user-friendly prompts for the paths.
* Run `Gideon.exe` with the argument `-c` or `--console` to receive prompts for the paths.
* Run `Gideon.exe` with two arguments, the first being the path to your DDS files, the second being the path to the TPFBHD file.

The file will replace existing files with their new versions, and add new files automagically. No additional work required.

## Disclaimer

I'm fairly new at C# and Souls Modding, so there are no guarantees for perfect function. Write me on Discord or write an issue here if something's off.

## Credits

 * Thank you to [TKGP](https://github.com/JKAnderson) for his [SoulsFormats library](https://github.com/JKAnderson/SoulsFormats) which is the only reason this even exists.
 * Kirnifr for [ELDEN RING Reforged](https://www.nexusmods.com/eldenring/mods/541) and the privilege of working on it