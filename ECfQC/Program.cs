
// parsing command-line arguments

Console.WriteLine("Reading command line parameters.");

bool verbose = false;
int qubits = -1;
string filePath1 = "";
string filePath2 = "";
int b = -1; // the number of simulations; default is max(qubits/2, 1) (determined after qubits has been read from command-line)
string strategy = "look-ahead";

int index = 0;
while (index < args.Length)
{
    switch (args[index])
    {
        default:
            if (filePath1 == "")
            {
                filePath1 = args[index];
            }
            else if (filePath2 == "")
            {
                filePath2 = args[index];
            }
            else
            {
                throw new ArgumentException("Error: " + args[index] + "is not a valid command-line parameter."
                + "See -h or --help for a list of valid parameters.");
            }
            break;
        case "-h":
        case "--help":
            Console.WriteLine("help");
            break;
        case "-v":
        case "--verbose":
            verbose = true;
            break;
        case "-q":
        case "--qubits":
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("Error: No argument for " + args[index] + " found.");
            }
            else if (!int.TryParse(args[index + 1], out qubits))
            {
                throw new ArgumentException("Error: Could not parse the argument for " + args[index] + ".");
            }
            else if (qubits <= 0)
            {
                throw new ArgumentException("Error: Number of qubits must be a positive integer.");
            }
            index++;
            break;
        case "-b":
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("Error: No argument for " + args[index] + " found.");
            }
            else if (!int.TryParse(args[index + 1], out qubits))
            {
                throw new ArgumentException("Error: Could not parse the argument for " + args[index] + ".");
            }
            else if (qubits < 0)
            {
                throw new ArgumentException("Error: Amount of simulations must be a positive integer or zero.");
            }
            index++;
            break;
        case "-s":
        case "--strategy":
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("Error: No argument for " + args[index] + " found.");
            }
            strategy = args[index+1];
            index++;
            break;
    }
    index++;
}

if (qubits == -1)
{
    throw new ArgumentException("Error: An argument for -q (the number of qubits) is required. "
    + "See -h or --help for more information.");
}
if (filePath1 == "" || filePath2 == "")
{
    throw new ArgumentException("Error: No/Not enough filepaths were given. "
    + "See -h or --help for more information.");
}
if (b == -1)
{
    b = Math.Max(qubits / 2, 1);
}
else if (b > Math.Pow(2, qubits) - 1)
{
    throw new ArgumentException("Error: There cannot be more simulations than the amount of basis states. " +
    "Note that only basis states are checked and repeating inputs are avoided.");
}


// parsing and compiling files

var circuit1 = Parser.Parse(filePath1, qubits);
var circuit2 = Parser.Parse(filePath2, qubits);


// simulation

if (b != 0)
{
    int[] basisStates = new int[b];
    Random random = new Random();
    for (int i = 0; i < basisStates.Length; i++)
    {
        int r = random.Next() % b;
        int counter = 3; // the amount of retries that are done in case the newly chosen basis state 
                         // is already in the list
                         // This does not guarantee that no value is repeated, but makes it unlikely while 
                         // not sacrificing too much performance
        while (basisStates.Contains(r) && counter > 0)
        {
            r = random.Next() % b;
            counter--;
        }
    }

    for (int i = 0; i < basisStates.Length; i++)
    {
        var res = Simulator.Simulate(circuit1, circuit2, basisStates[i], (int)Math.Pow(2, qubits));
        if (!res.Item1)
        {
            Console.WriteLine("------------------------\nResult: The two circuits are not equal. "
            + "One simulation resulted in a counterexample:");
            Console.WriteLine("\tInput: |" + i + "〉\n\t"
            + "Result from file \"" + filePath1 + "\": " + res.Item2.ToString() + "\n\t"
            + "Result from file \"" + filePath2 + "\": " + res.Item3.ToString() + "\n"
            );
            return;
        }
    }
    Console.WriteLine("No simulation resulted in a counterexample. Continuing by proving the circuits are equal.");
} else
{
    Console.WriteLine("Skipping simulation, as -b is zero. Continuing by proving the circuits are equal.");
}


// equivalence checking (proving G --> I <-- G' is equal to the Identity matrix)

if (EQChecker.Check(circuit1,circuit2, strategy, (int)Math.Pow(2, qubits)))
{
    Console.WriteLine("------------------------\nResult: The two circuits are equal. "
            + "G --> I <-- G\' is equal to the Identity matrix");
} else
{
    Console.WriteLine("------------------------\nResult: The two circuits are not equal. "
            + "G --> I <-- G\' is not equal to the Identity matrix");
}