# DVD Corner Screensaver (Windows 10)

A native Windows screensaver version of the DVD corner simulator.

## Install
1. Download the `DVD-Corner-Screensaver-Windows-10` artifact from the latest successful GitHub Actions run.
2. Extract the ZIP.
3. Run `INSTALL.bat` (it copies the screensaver into Program Files and opens Windows Screen Saver Settings).
4. Choose **DVD Corner Screensaver** in the Screen saver dropdown and set the wait time.

The screensaver exits when the user presses a key, clicks, or moves the mouse.

## Build
The GitHub Actions workflow builds a self-contained x64 Windows executable and packages it as a `.scr` file. It uses the existing DVD logo SVG from the repository.
