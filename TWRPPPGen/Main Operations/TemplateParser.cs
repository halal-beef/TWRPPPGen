namespace TWRPPPGen
{
    internal class TemplateParser
    {
        /// <summary>
        /// Set-up and use a template to set values
        /// </summary>
        /// <param name="props">Prop list (needed to extract some extra props).</param>
        /// <param name="abiList">The list of ABIs of the device. (raw prop values)</param>
        /// <param name="arch64">if true, it will use the Template for ARM64</param>
        public static void SetupTemplate(string abiList, List<string> props, bool arch64 = false)
        {
            StringBuilder finalText = new();
            int i0 = 0;
            string[] template;

            //ABIs
            string[] abis = abiList.Split(',');

            if (arch64)
            {
                template = File.ReadAllLines(Environment.CurrentDirectory + @"\Templates\ARM64.template");
            }
            else
            {
                template = File.ReadAllLines(Environment.CurrentDirectory + @"\Templates\ARM.template");
            }
            
            for (int i = 0; i < template.Length; i++)
            {
                string[] varValue = template[i].Split('|');

                if (varValue.Length >= 3)
                {
                    string finalValue = varValue[0];

                    if (varValue[0].Contains("TARGET_CPU_ABI"))
                    {
                        finalValue += abis[0].Split('-')[0];
                    }

                    //If arch is 64 set the target cpu variant accordingly.
                    if (varValue[0].Contains("TARGET_CPU_VARIANT") && arch64)
                    {
                        finalValue += PropParser.LineSearcher("dalvik.vm.isa.arm64.variant", props);
                    }
                    else if (varValue[0].Contains("TARGET_CPU_VARIANT") && !arch64)
                    {
                        finalValue += PropParser.LineSearcher("dalvik.vm.isa.arm.variant", props);
                    }

                    if (varValue[0].Contains("TARGET_ARCH_VARIANT") 
                     || varValue[0].Contains("TARGET_2ND_CPU_ABI")
                     || varValue[0].Contains("TARGET_2ND_CPU_ABI2")) 
                    {
                        finalValue += abis[i0];
                        i0++; 
                    }

                    finalText.Append($"{finalValue}\n");
                    continue;
                }

                try
                {
                    finalText.Append(varValue[0] + varValue[1] + "\n");
                }
                catch
                {
                    finalText.Append(varValue[0] + "\n");
                }
            }
            Console.WriteLine(finalText.ToString());
        }
    }
}
