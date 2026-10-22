# Shadow of the Colossus Archipelago Client

## Requirements
To play Shadow of the Colossus on archipelago requires:

- A Windows or macOS machine
- [Archipelago](https://archipelago.gg) installed
- The latest copy of [the SOTC client](https://github.com/riezahughes/SOTC-Client/releases)
- The latest copy of [the SOTC AP World](https://github.com/riezahughes/SOTC-APWorld/releases)
- A legal copy of the NTSC-U Version of Shadow of the Colossus
- [PCSX2](https://pcsx2.net)

# Setup

- Install the APWorld in Archipelago
- Generate the templates
- Copy and paste a copy of the SOTC template into the `players` folder of your archipelago install. If it doesn't exist, make it.
- Edit the yaml however you like, and make sure to change the slot name
- Generate the game
- Host the game using the zip that's generated in your `output` folder of your archipelago install (either on your own server, locally or on [the archipelagos website](https://archipelago.gg/uploads) )
- Boot up the game in PCSX2.
- Run the client (macOS users: see [Running on macOS](#running-on-macos) below)
- Fill in the blanks to connect to the server
- Start playing!

# Client commands

Type these into the client window while it's running:

- `options`: shows the options your slot was generated with
- `goal`: shows your goal and how far along you are
- `hint` (or `hint <item>`): asks the server for a hint
- `update`: re-applies everything you've been sent so far
- `overlay`: turns the in-game overlay on or off (it's on by default). If it gets in the way, turn it off and follow what you've received in the client window instead.
- `release`: manually sends your goal completion to the server
- `debug`: prints information that's useful when reporting a problem
- `exit`: closes the client

# Running on macOS

## Which download?

From the [releases page](https://github.com/riezahughes/SOTC-Client/releases), grab the zip for your Mac:

- `SotcArchipelago_<version>_osx-arm64.zip` for Apple Silicon Macs (M1, M2, M3, M4...)
- `SotcArchipelago_<version>_osx-x64.zip` for older Intel Macs

Not sure which you have? Click the Apple menu > **About This Mac** and look at "Chip" (Apple M-something = arm64) or "Processor" (Intel = x64).

## First launch

1. Unzip the download.
2. macOS blocks apps from unidentified developers the first time you open them. To get past this, **right-click** (or Control-click) `SotcArchipelago` and choose **Open**, then **Open** again on the warning.
   - If macOS only offers "Move to Bin" / "Done", go to **System Settings > Privacy & Security**, scroll down, and click **Open Anyway** next to the SotcArchipelago message.
   - Alternatively, run this once in Terminal from the unzipped folder: `xattr -d com.apple.quarantine SotcArchipelago`
3. After the first time, you can just double-click it.

## Let the client read PCSX2 (once)

macOS stops other programs from reading PCSX2's memory, even with an administrator password. Run this once in Terminal to allow it, then **restart PCSX2**:

```
codesign --force --deep --sign - /Applications/PCSX2*.app
```

If PCSX2 is installed somewhere other than `/Applications`, change the path to match. You may need to do this again after updating PCSX2.

## Running the client

1. Start PCSX2 and boot Shadow of the Colossus **before** opening the client.
2. Double-click `SotcArchipelago`. A Terminal window will open.
3. **Enter your Mac password** when asked. macOS only lets programs with administrator access read another app's memory, which the client needs to see what's happening in PCSX2. Nothing appears as you type, that's normal. Press Enter when done.
4. The client looks for PCSX2's memory address by itself. If it finds it, you'll see `PCSX2 Memory Offset found at: 0x...` and can carry on to the next step. If it can't, it explains why and asks you to enter the address:
   1. Open a new Terminal window and run:
      ```
      sudo lldb -p $(pgrep -x PCSX2) -b -o 'p/x *(unsigned long long *)&EEmem' -o 'detach'
      ```
   2. Copy the `0x...` address from the output (or the whole line) and paste it into the client, then press Enter.
5. Fill in the server, port, slot name and password as normal and start playing!

> The memory address changes when PCSX2 is restarted. The client works it out fresh each time it starts, so just start PCSX2 first.

## Troubleshooting

- **"zsh: killed" or the window closes instantly**: the download may have been damaged or blocked. Re-download the zip and follow the first launch steps again.
- **Sorry, try again**: the password was wrong. Use the password you log in to your Mac with.
- **Not an administrator**: your macOS user account must be an admin account to run the client.
- **`task_for_pid failed with code 5`**: PCSX2 hasn't been re-signed yet (or an update undid it). Run the `codesign` command above and restart PCSX2.
- **PCSX2 hasn't allocated the PS2's memory yet**: the game hasn't started. Boot Shadow of the Colossus first, then open the client.
- **The overlay doesn't show up**: the in-game overlay is currently Windows-only. Follow what you've received in the client window.

# Development
- Clone the repository
- Create an appsettings.Local.Json in the root of the project
- Create an AP room on the archipelago website and fill the file with the following content:

```json
{
    "port": "portnumber",
    "slot": "slotname",
    "pass": "passifused"
}

```

- Running debug will now automatically connect you to the room without the need of you filling in the fields

Note: Sometimes the ap client can be funny in debug mode. When you run it, check visual studio's play button and see if you need to hit continue. There's a race condition under the hood. In a release you don't see it.
