# GamedataChecker

A tool to run-check gamedata files on binaries

> [!NOTE]  
> This tool is pretty experimental currently.

# Commands

- `check`
	- alias: `c`
	- switches:
		- `--gamedata` specify a gamedata file, usage: `--binary path/to/gamedata.json`
		- `--binary` specify a binary file, usage: `--binary path/to/binary.extension`
	- usage: `GDC check --gamedata "path/to/gamedata.json" --gamedata "path/to/another/randomgamedata.json" --binary "path/to/server.dll" --binary "path/to/libserver.so" --binary "path/to/engine2.dll" PAUSE`
		-  note that the `PAUSE` is not needed in the end
	- returns: amount of total errors across every gamedata specified