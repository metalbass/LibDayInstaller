# MDB Files

## Brief

.MDB files contain Microsoft Jet Databases, presumably created through Microsoft Access.

They store DB info into disk, so it can be queried through SQL, as SQLite does. They can also be found inside zip files.

## Extraction

They are extracted through the MdbExtractor class that opens them through OleDb connections.
Destination files are TSV, representing each table extracted.
