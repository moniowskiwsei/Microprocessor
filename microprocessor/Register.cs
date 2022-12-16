using microprocessor;

public class Register : IByteContainer
{
    public readonly string Name;
    public byte Value;
    public Register(string name)
    {
        Name = name;
        Value = 0;
    }

    public void SetValue(byte value)
    {
        Value = value;
    }

    public byte GetValue()
    {
        return Value;
    }

    public string GetName()
    {
        return Name;
    }

    public override string ToString()
    {
        return $"{Name}: {string.Format("{0:X2}", Value)}";
    }
}