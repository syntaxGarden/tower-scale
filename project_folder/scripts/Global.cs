using Godot;
using System;

public partial class Global : Node
{
    public string ScriptID { get { return GetScriptID(); } }
    internal string GetScriptID() { return "This is a global script that will be autoloaded."; }
}