# LibDay extractor tool
## Objective
The main objective for this tool is to understand and convert the original file formats the game has into something that can be used with more ease from the game's code.

This tool will be run once by the end user as an installer, so the game can afterwards. The tool needs the original game files so it can extract the assets from there. We will not provide the original game assets.

Ideally, the game code will have no knowledge of how the original data was stored and just consume it in the more straight forward way possible.

## File formats
The game uses diferent file formats to group all assets into a few number of formats. This will get updated as soon as we know more about what the game is using:

- MBD files. Microsoft Jet Databases. They store DB info into disk, so it can be queried through SQL, as SQLite does; can also be found inside zip files. They get extracted through the MdbExtractor class, that opens them through OleDb connections.
- SMK files. Smacker Video from RAD game tools. This stores images (e.g. interface assets), in-game animations and cinematics. They get extracted through the SmackerVideoExtractor, that opens them through FFmpeg.
- FF file. Seems like a custom format that stores multiple SMK files.
- DBI files. Unknown to date, but they may contain in-game sprites, but may also include sounds.
- LibDayEn.dll. A compiled code library (presumed C++) that probably contains in-game strings.

## Pending work

- Specify how the animations are going to be used from Unity, since they are now all extracted to RGBA32 PNGs that have no transparency and instead use different key colors.
- Most SMK files are static images, that get exported into individual frames, but others are actually cinematics that are not getting exported, since they should probably converted to some format (i.e. MP4) that can be played from Unity with ease.
- DBI files. We need to understand whats inside of them and export that into something that we can understand and use later in the game.
- Export language strings from the different language DLLs the game may have.
- Small refactor so all FileFormatExtractor classes can implement an interface that allows to relate them to file extensions and folders instead of calling them manually.
- Check different game assets providers (GOG may be one) to ensure the extractor works with them. The extractor has only been tested over the original CD disk.
- Profile both CPU and Memory usage, so we can speed up importing times a bit.