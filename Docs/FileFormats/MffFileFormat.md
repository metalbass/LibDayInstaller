# FF Files

## Brief

Liberation Day contains 1 file called  .FF files look like a custom container created specifically for Liberation Day.

They store multiple SMK files that can be addressed by name, as the file contains an index that relates names to file offsets.

Note that we refer to this files as Mff, since they start with MFF'\0' magic bytes, while the extension is really .FF.

## Extraction

They are extracted through the MffExtractor class, that reads through the index and exports SMK files from it.

## Pending work

We still need to make sure we are exporting 100% of the data contained in this file.
