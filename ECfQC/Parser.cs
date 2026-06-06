
public static class Parser
{
    public static List<DDMatrix> Parse(string path, int q)
    {
        string str = "";
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                str = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            throw new ArgumentException("Error: While parsing " + path + ", the following exception occured:" + e.Message);
        }
        string[] lines = str.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

        List<DDMatrix> list = new(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int[] arguments = new int[parts.Length - 1];

            // error handling
            if (!DDMatrix.gates.ContainsKey(parts[0]))
            {
                throw new ArgumentException("Parsing Error in line " + i + ": The gate \"" + parts[0] + "\" is " +
                "not a known type of gate.");
            }

            if (DDMatrix.gates[parts[0]].Item2 != parts.Length - 1)
            {
                throw new ArgumentException("Parsing Error in line " + i + ": Expected " +
                DDMatrix.gates[parts[0]].Item2 + " arguments for gate \"" + parts[0] +
                "\", but got " + (parts.Length - 1) + " arguments instead.");
            }

            for (int k = 1; k < parts.Length; k++)
            {
                if (!int.TryParse(parts[k], out arguments[k - 1]))
                {
                    throw new ArgumentException("Parsing Error in line " + i + ": The argument \""+ parts[k] + 
                    "\" for the gate \""+parts[0]+"\" could not be parsed.");
                }
                if (arguments[k-1] <0 || arguments[k-1] >= q)
                {
                    throw new ArgumentException("Parsing Error in line " + i + ": The argument \"" + parts[k] + 
                    "\" for the gate \""+parts[0]+"\" is not in [0, number of qubits).");
                }
            }

            for (int k = 0; k<arguments.Length;k++)
            {
                for (int m = k+1;m<arguments.Length;m++)
                {
                    if (arguments[k] == arguments[m])
                    {
                        throw new ArgumentException("Parsing Error in line " + i + ": The "
                        + "arguments for the gate \""+ parts[0]+"\" contain duplicates, "
                        + "which is not allowed.");
                    }
                }
            }

            // end of error handling

            DDMatrix t = DDMatrix.gates[parts[0]].Item1.Copy();
            switch (DDMatrix.gates[parts[0]].Item2)
            {
                case 0: t = DDMatrix.Extend(t, q); break;
                case 1: t = DDMatrix.Extend(t, q, arguments[0]); break;
                case 2: t = DDMatrix.Extend(t, q, arguments[0], arguments[1]); break;
                case 3: t = DDMatrix.Extend(t, q, arguments[0], arguments[1], arguments[2]); break;
                default: throw new Exception("Internal Error"); // This should not happen and is there for debugging purposes
            }
            list.Add(t);

        }

        return list;
    }
}