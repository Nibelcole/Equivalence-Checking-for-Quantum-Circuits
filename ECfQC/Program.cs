// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

Console.WriteLine("Reading command line parameters.");

bool verbose = false;
int qubits = -1;
string filePath1 = "";
string filePath2 = "";

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
            verbose = true;
            break;
        case "-q":
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
                throw new ArgumentException("Error: Number of qubits must be a positive integer");
            }
            index++;
            break;
    }
    index++;
}

if (qubits ==-1)
{
    throw new ArgumentException("Error: An argument for -q (the number of qubits) is required. "
    +"See -h or --help for more information.");
}
if (filePath1 == "" || filePath2 == "")
{
    throw new ArgumentException("Error: No/Not enough filepaths were given. "
    +"See -h or --help for more information.");
}

var circuit1 = Parser.Parse(filePath1, qubits);
var circuit2 = Parser.Parse(filePath2, qubits);


