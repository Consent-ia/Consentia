using System;
using System.Collections.Generic;

[Serializable]
public class GameStateData
{
    public string sceneName;

    // Player position (optional but recommended)
    public float playerX;
    public float playerY;
    public float playerZ;

    // Progress (example: which NPCs were interacted in current act)
    public List<string> interactedNpcNames = new();

    public string savedAtUtc; // for debugging / "most recent" save display
}