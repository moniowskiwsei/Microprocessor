using System.Globalization;
using System.Net.Http.Headers;

public class Microprocessor
{
    public IDictionary<string, Register> registers = new Dictionary<string, Register>();
    public IDictionary<string, Command> commands = new Dictionary<string, Command>();
    public static int memorySize = 0x10000;
    public byte[] memory = new byte[memorySize];

    public Microprocessor()
    {
        Init();
    }

    private void Init()
    {
        string[] RegisterNames = { "AL", "AH", "BL", "BH", "CL", "CH", "DL", "DH" };

        foreach (string RegisterName in RegisterNames)
        {
            registers.Add(RegisterName, new Register(RegisterName));
        }

        InitCommands();
    }

    private void InitCommands()
    {
        InitCommand(new Command("MOV", "Move value of one register to another", MoveCommand));
        InitCommand(new Command("XCHG", "Switch value of registers", XchgCommand));
        InitCommand(new Command("ADD", "Add two values", AddCommand));
        InitCommand(new Command("SUB", "Substract two values", SubCommand));
        InitCommand(new Command("AND", "Perform AND operation of two values", AndCommand));
        InitCommand(new Command("OR", "Perform OR operation of two values", OrCommand));
        InitCommand(new Command("XOR", "Perform XOR operation of two values", XorCommand));
        InitCommand(new Command("NOT", "Perform NOT operation on value", NotCommand));
        InitCommand(new Command("INC", "Adds 1 to vlaue", IncCommand));
        InitCommand(new Command("DEC", "Substracts 1 from value", DecCommand));
        InitCommand(new Command("EXIT", "Exit program", ExitCommand));

    }

    private void InitCommand(Command command)
    {
        commands.Add(command.name, command);
    }

    private static string ReadLine()
    {
        return Console.ReadLine() ?? "";
    }

    public void ReadCommand(string value)
    {
        string commandString = value.ToUpper();
        if (commandString == "") return;

        string[] commandArgs = commandString.Split(" ", 2);
        if (commands.ContainsKey(commandArgs[0]))
        {
            string[] args = commandArgs.LongLength > 1 ? commandArgs[1].Split(",").Select(arg => arg.Trim().ToUpper()).ToArray() : Array.Empty<string>();
            try
            {
                commands[commandArgs[0]].function.Invoke(args);
            }
            catch (Exception e)
            {
               throw new Exception(e.Message);
            }
        }
        else
        {
            throw new Exception("Unknown command");
        }
    }

    private bool IsValidRegister(string name)
    {
        return registers.ContainsKey(name);
    }    
    private bool IsValidMemoryAddress(string name)
    {
        if (!name.StartsWith("[")) return false;
        if (!name.EndsWith("]")) return false;
        string address = name[1..^1];
        int addressInt = StringToHexInt(address);
        if (addressInt > memorySize) return false;
        return true;
    }
    private bool IsValidRegisterOrMemoryAddress(string name)
    {
        return IsValidRegister(name) || IsValidMemoryAddress(name);
    }
    private int GetMemoryAddressFromString(string name)
    {
        string address = name[1..^1];
        return StringToHexInt(address);
    }
    private byte GetArgumentValue(string name)
    {
        if (IsValidRegister(name)) return registers[name].GetValue();
        if (IsValidMemoryAddress(name)) return memory[GetMemoryAddressFromString(name)];
        byte value = 0;
        StringToHex(name, ref value);
        return value;
    }    
    private void SetMemoryOrRegisterValue(string name, byte value)
    {
        if (IsValidRegister(name)) registers[name].SetValue(value);
        else if (IsValidMemoryAddress(name)) memory[GetMemoryAddressFromString(name)] = value;
        else throw new Exception("Cannot set value");
    }
    public static void StringToHex(string value, ref byte output)
    {
        try
        {
            output = byte.Parse(value, NumberStyles.HexNumber);
        }
        catch (FormatException e)
        {
            throw new Exception("Invalid hex number");
        }
    }
    public static int StringToHexInt(string value)
    {
        try
        {
            return int.Parse(value, NumberStyles.HexNumber);
        }
        catch (FormatException e)
        {
            throw new Exception("Invalid hex number");
        }
    }

    private void MoveCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");
        SetMemoryOrRegisterValue(args[0], GetArgumentValue(args[1]));
    }
    private void XchgCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (!IsValidRegisterOrMemoryAddress(args[0]) || !IsValidRegisterOrMemoryAddress(args[1])) throw new Exception("Wrong input passed");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");

        byte temp = GetArgumentValue(args[0]);
        SetMemoryOrRegisterValue(args[0], GetArgumentValue(args[1]));
        SetMemoryOrRegisterValue(args[1], temp);

    }
    private void AddCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");

        byte result = (byte)(GetArgumentValue(args[0]) + GetArgumentValue(args[1]));
        SetMemoryOrRegisterValue(args[0], result);
    }
    private void SubCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");

        byte result = (byte)(GetArgumentValue(args[0]) - GetArgumentValue(args[1]));
        SetMemoryOrRegisterValue(args[0], result);
    }
    private void AndCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");

        byte result = (byte)(GetArgumentValue(args[0]) & GetArgumentValue(args[1]));
        SetMemoryOrRegisterValue(args[0], result);
    }
    private void OrCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");

        byte result = (byte)(GetArgumentValue(args[0]) | GetArgumentValue(args[1]));
        SetMemoryOrRegisterValue(args[0], result);
    }
    private void XorCommand(string[] args)
    {
        if (args.Length != 2) throw new Exception("Wrong number of arguments");
        if (IsValidMemoryAddress(args[0]) && IsValidMemoryAddress(args[1])) throw new Exception("Two memory address passed");

        byte result = (byte)(GetArgumentValue(args[0]) ^ GetArgumentValue(args[1]));
        SetMemoryOrRegisterValue(args[0], result);
    }
    private void IncCommand(string[] args)
    {
        if (args.Length != 1) throw new Exception("Wrong number of arguments");
        AddCommand(new string[] { args[0], "1" });
    }
    private void DecCommand(string[] args)
    {
        if (args.Length != 1) throw new Exception("Wrong number of arguments");
        SubCommand(new string[] { args[0], "1" });
    }
    private void NotCommand(string[] args)
    {
        if (args.Length != 1) throw new Exception("Wrong number of arguments");

        byte result = (byte)~GetArgumentValue(args[0]);
        SetMemoryOrRegisterValue(args[0], result);
    }
    private void ExitCommand(string[] args)
    {
        Application.Exit();
    }

    public override string ToString()
    {
        string value = "Registers:\n";
        foreach (Register register in registers.Values)
        {
            value += $"\t{register}\n";
        }
        value += "Memory:\n";
        for(int i = 0; i < 16; i++)
        {
            value += $"\t{i}:{string.Format("{0:X2}", memory[i])}\n";
        }

        return value;
    }
}
