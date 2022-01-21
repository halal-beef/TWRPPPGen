namespace TWRPPPGen
{
    internal class PropParser
    {
        public static Object LineSearcherlocker = new();
        /// <summary>
        /// Searches a property in the list.
        /// </summary>
        /// <param name="targetLine">Property that is needed</param>
        /// <param name="PropLines">The list containing the property list.</param>
        /// <returns>if the prop is found, returns the prop value, if it isn't found, returns "Prop Not Found."</returns>
        public static string LineSearcher(string targetLine, List<string> PropLines)
        {
            lock (LineSearcherlocker)
            {
                //Iterate through the list of props and search for that prop.
                for (int i = 0; i < PropLines.Count; i++)
                {
                    if (PropLines[i].Split('=')[0] == targetLine)
                    {
                        return PropLines[i].Split('=')[1];
                    }
                }
                return "Prop Not Found.";
            }
        }
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
