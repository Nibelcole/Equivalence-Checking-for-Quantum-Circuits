
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
                throw new ArgumentException("Error: \"" + args[index] + "\" is not a valid command-line parameter."
                + "See -h or --help for a list of valid parameters.");
            }
            break;
        case "-h":
        case "--help":
            PrintHelp();
            return;
        case "-v":
        case "--verbose":
            verbose = true;
            break;
        case "-q":
        case "--qubits":
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException("Error: No argument for \"" + args[index] + "\" found.");
            }
            else if (!int.TryParse(args[index + 1], out qubits))
            {
                throw new ArgumentException("Error: Could not parse the argument for \"" + args[index] + "\".");
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
                throw new ArgumentException("Error: No argument for \"" + args[index] + "\" found.");
            }
            else if (!int.TryParse(args[index + 1], out qubits))
            {
                throw new ArgumentException("Error: Could not parse the argument for \"" + args[index] + "\".");
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
                throw new ArgumentException("Error: No argument for \"" + args[index] + "\" found.");
            }
            strategy = args[index + 1];
            index++;
            break;
    }
    index++;
}

if (qubits == -1)
{
    throw new ArgumentException("Error: An argument for \"-q\" (the number of qubits) is required. "
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

Run(verbose, qubits, filePath1, filePath2, circuit1, circuit2, b, strategy);

// This allows us to call the funtion for testing purposes without going through the command line.
static void Run(bool verbose, int qubits, string filePath1, string filePath2, List<DDMatrix> circuit1, List<DDMatrix> circuit2, int b, string strategy)
{
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
    }
    else
    {
        Console.WriteLine("Skipping simulation, as -b is zero. Continuing by proving the circuits are equal.");
    }


    // equivalence checking (proving G --> I <-- G' is equal to the Identity matrix)

    int check = EQChecker.Check(circuit1, circuit2, strategy, (int)Math.Pow(2, qubits));

    if (check == 1)
    {
        Console.WriteLine("------------------------\nResult: The two circuits are exactly equal. \n\t"
                + "G --> I <-- G\' is equal to the Identity matrix");
    }
    else if (check == 2)
    {
        Console.WriteLine("------------------------\nResult: The two circuits are equivalent. \n\t"
                + "G --> I <-- G\' only differs by a global phase from the Identity matrix of the same size");
    }
    else
    {
        Console.WriteLine("------------------------\nResult: The two circuits are not equal. \n\t"
                + "G --> I <-- G\' is not equal to the Identity matrix");
    }


}

static void PrintHelp()
{
    Console.WriteLine("Usage: ./ECfQC.exe [options] (-q <arg> | --qubits <args>) <filePath1> <filePath2>\n\n"+
                      "       ./ECfQC.exe (-h | --help)"+
           "Determines the equality and equivalency of two quantum circuits. \n"+
           "See README.md for more information.\n\n"+
           "Options:\n"+
           "  -h, --help                       displays this help message\n"+
           "  -v, --verbose                    enables verbose mode which prints out more information during the process\n"+
           "  -b <arg>                         the number of simulations to run\n"+
           "                                     The default is the amount of qubits divided by 2 or 1 if \n"+
           "                                     the argument given for -q is 1\n"+
           "                                     if <arg> is 0, simulation is skipped\n"+
           "                                     Note that it is possible for the program to run a smaller amount of \n"+
           "                                     simulations if a distinct random value can't be generated in a certain \n"+
           "                                     number of tries\n"+
           "  -s <arg>, --strategy <args>      the strategy to use when conducting G --> I <-- G'\n"+
           "                                     The default is \"look-ahead\"\n"+
           "                                     Available options are \"alternating\", \"alternating-balanced\" and \n"+
           "                                     \"look-ahead\"\n"+
           "                                     For more information, see README.md\n"+
           "Required arguments:\n"+
           "  -q <arg>, --qubits <args>        the number of qubits to be simulated\n"+
           "  <filepath1>, <filepath2>         path to a txt-file that contains a representation of a quantum circuit \n"+
           "                                     as a list of instruction divided by linebreaks (Environment.NewLine is \n"+
           "                                     used to differentiate between operating systems)\n"+
           "                                     Note that empty files are interpreted as a circuit returning the identity\n"+
           "    Instruction usage:\n"+
           "      <gate> [<q1>] [<q2>] [<q3>]\n"+
           "    Instruction parameters\n"+
           "      <gate>                       the gate to apply\n"+
           "                                     Available options are \"id\" (Identity), \"pX\" (Pauli-X), \n"+
           "                                     \"pY\" (Pauli-Y), \"pZ\" (Pauli-Z), \"h\" (Hadamard), \"cnot\" (C-Not), \n"+
           "                                     \"sw\" (Swap) and \"t\" (Toffoli)\n"+
           "                                     Depending on the type of gate, either 0, 1, 2 or 3 arguments have \n"+
           "                                     to be given\n"+
           "                                     Note that the only gate with 0 arguments is the identity\n"+
           "                                     For more information, see README.md\n"+
           "      <q1>, <q2>, <q3>             the qubits on which the given gate should be applied to\n"+
           "                                     Note that giving arguments with large distances between them can \n"+
           "                                     impact performance\n\n"+
           "More information can be obtained by reading the README.md or the seminar paper/presentation made for this project"
           );
}