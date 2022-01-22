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

            string template;

            //ABIs
            string[] abis = abiList.Split(',');

            //Load the correct template into memory
            if (arch64)
            {
                template = File.ReadAllText(Environment.CurrentDirectory + @"\Templates\ARM64.template");
            }
            else
            {
                template = File.ReadAllText(Environment.CurrentDirectory + @"\Templates\ARM.template");
            }
            

            template = template.Replace("[ABI]", $"{PropParser.LineSearcher("ro.product.cpu.abi", props)}");

            if (arch64)
            {
                template = template.Replace("[CPU_VARIANT]", $"{PropParser.LineSearcher("dalvik.vm.isa.arm64.variant", props)}");
            } 
            else if (!arch64)
            {
                template = template.Replace("[CPU_VARIANT]", $"{PropParser.LineSearcher("dalvik.vm.isa.arm.variant", props)}");
            }
            template = template.Replace("[ABI_LIST[0]]", abis[0]);
            template = template.Replace("[ABI_LIST[1]]", abis[1]);
            template = template.Replace("[ABI_LIST[2]]", abis[2]);

            finalText.Append(template);
            
            Console.WriteLine(finalText.ToString());
        }
    }
}
