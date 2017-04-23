# LibDay extractor tool
## Objective
The main objective for this tool is to understand and convert the original file formats the game has into something that can be used with more ease from the game's code.

This tool will be ran once by the end user as an installer, so the game can use the result afterwards. The tool needs the original game files so it can extract the assets from there. We will not provide the original game assets.

Ideally, the game code will have no knowledge of how the original data was stored and just consume it in the more straightforward way possible.

## File formats
The game uses different file formats to group all assets into a few number of archives. This will be updated as soon as we know more about what the game is using:

- [MBD files](FileFormats/MdbFileFormat.md)
- [SMK files](FileFormats/SmkFileFormat.md)
- [FF files](FileFormats/MffFileFormat.md)
- DBI files. Unknown to date.

## Pending work

A project on github has been set up for tracking work on the extractor tool. You can [check the progress there](https://github.com/metalbass/LibDay/projects/1).
