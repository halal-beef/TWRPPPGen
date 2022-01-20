namespace TWRPPPGen
{
    internal class PropParser
    {
        /// <summary>
        /// Parses a default.prop file, and returns all the properties on it as a List
        /// </summary>
        /// <returns>List of strings containing the properties</returns>
        public static List<string> ParseFile(string PathToPropFile)
        {
            List<string> parsed = new();

            string[] fileLines = File.ReadAllLines(PathToPropFile);
            
            //Remove comments from file.
            for (int i = 0; i < fileLines.Length; i++)
            {
                if (!fileLines[i].Contains('#'))
                {
                    StringBuilder prop = new();
                    prop.Append(fileLines[i]);

                    parsed.Add(prop.ToString());
                }
                else
                {
                    parsed.Add("\n");
                }
            }

            //Return raw data.
            return parsed;
        }
    }
}
