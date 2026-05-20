
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
            throw new ArgumentException("Error: While parsing "+path+", the following exception occured:"+e.Message);
        }
        string[] lines = str.Split([Environment.NewLine],StringSplitOptions.RemoveEmptyEntries);

        List<DDMatrix> list = new (lines.Length);
        for (int i = 0; i<lines.Length;i++)
        {
            string[] parts = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int[] arguments = new int[parts.Length-1];
            for (int k = 1; k<parts.Length;k++)
            {
                if (!int.TryParse(parts[k], out arguments[k-1]))
                {
                    throw new ArgumentException("Parsing Error in line "+i+": The argument "+parts[k]+" could not be parsed.");
                }
            }

            if (parts.Length >= 2 && DDMatrix.gates.ContainsKey(parts[0]))
            {
                var t = new DDMatrix(DDMatrix.gates[parts[0]].Item1);
                switch (DDMatrix.gates[parts[0]].Item2)
                {
                    case 1: DDMatrix.Extend(t,q,arguments[0]); break;
                    case 2: DDMatrix.Extend(t,q,arguments[0], arguments[1]); break;
                    case 3: DDMatrix.Extend(t,q,arguments[0], arguments[1], arguments[2]); break;
                    default: throw new Exception("Internal Error"); // This should not happen and is there for debugging purposes
                }
                list.Add(t);
            }
        }

        return list;
    }
}