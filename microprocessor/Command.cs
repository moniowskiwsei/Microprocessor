public struct Command
{
    public string name;
    public string description;
    public Action<string[]> function;

    public Command(string name, string description, Action<string[]> function)
    {
        this.name = name;
        this.description = description;
        this.function = function;
    }

    public Command(string name, Action<string[]> function) : this(name, "", function) { }
}
