ID replacement tool for Morrowind.

Replaces IDs in all contexts, including scripts and dialogue results.

Usage: mwlathe.exe <input.esm/esp> <output.esm/esp> <id_map>

For id_map: \<old ID>,\<new ID>

Make sure id_map is headerless and without quotes!

Options:

-s \<separator> | Set custom separator for id_map. Mandatory for non-csv/tsv files

-b | Replace IDs within book texts. Useful for ex. PositionCell markers, but unsafe with plaintext IDs
