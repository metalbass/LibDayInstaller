# SMK Files

## Brief

Liberation Day contains several .SMK files. These files are encoded with the Smacker Video compression, from the RAD game tools.

So far, 3 different types of assets are known to be stored in them:

1. Static UI assets
2. Unit animations + their sfx
3. Cinematics

## Extraction

They are extracted through the SmackerVideoExtractor class, that uses FFmpeg to read through the SMK files.

## Pending work

1. Cinematics are not being exported
2. Static images contain a lot of unused space and background solid colors, so they eat a lot of unnecessary space that could be avoided. They could also be packed on spritesheets.
3. Animations are exported as individual frames, with the same issues than the previous point.
4. Animations sfxs are still not exported.
