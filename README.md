# Welcome to the Advent of Code for Unity Package!

The Advent of Code for Unity Package simplifies the Advent of Code challenge setup process. It handles the download of input files to the Resources folder and automates the creation of both scripts and test scripts. 

With this package, you can focus on solving Advent of Code puzzles without worrying about the manual setup steps.

## Installation

To install the package, add the following line to your Unity project's `Packages/manifest.json` file under the `dependencies` section:

```json
"dependencies": {
  "com.jturesson.adventofcode": "https://github.com/Jturesson90/advent-of-code-unity.git"
}
```

## Usage
### Get your session cookie Id.
To download the inputs to Unity we need the session cookie from the homepage. This session id will be stored locally on your machine with [UnityEditor.EditorPrefs](https://docs.unity3d.com/ScriptReference/EditorPrefs.html). So it wont be a part of your commits to version control.

- Go to https://adventofcode.com
- Make sure you have logged in
- Open developer tools by rightclick, inspect or `ctrl+shift+j` if you are using windows/linux.
- Open the `Application tab` and look for the `session` entry. Copy that. 
- Open `Unity` and open the Menu `Window/Advent of Code/Settings`
- `Paste` the copied `session` to the first textfield and press `Log In`

If success you can now choose which `year` and `day` you want to `setup`.

### Setup
So what happens when you press `Setup`?
- It downloads the `input` to a `TextAsset` to `Resources/AdventOfCode/[Year]/[Day]`
- It creates the Puzzle Scripts to `Assets/AdventOfCode/[Year]/[Code]/[Day].cs`
- It creates the Puzzle Test Scripts to `Assets/AdventOfCode/[Year]/[Tests]/[Day].cs`
- On test `Setup` it fetches the input from Resources

## Contributing Guidelines
We welcome contributions! Before contributing, please read our [Contributing Guidelines](CONTRIBUTE.md).
