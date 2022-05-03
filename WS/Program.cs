using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace WS
{
    class Program
    {
        static void Main(string[] args)
        {

            string replacementfile = "replacement.json";
            string datafile = "data.json";
            List<Swap> swaps = new List<Swap>();
            List<string> messages = new List<string>();


            swaps = ReadJSONFileReplacement(replacementfile, swaps);
            ReadJSONFileMessage(datafile, messages);
            DeleteRepeateReplacement(swaps);
            SearchReplaceMessage(messages, swaps);
            WriteJSONFileDataNew(messages);           
        }
        
        public static void ReadJSONFileMessage(string fileNameJSON, List<string> messages)
        {
            using (StreamReader r = new StreamReader(fileNameJSON))
            {
                string json = r.ReadToEnd();
                messages = JsonConvert.DeserializeObject<List<string>>(json);
            }
        }
        
        public static void SearchReplaceMessage(List<string> messages, List<Swap> swaps)
        {
            int curcount = messages.Count;
            for (int i = 0; i < curcount; i++)
            {
                for (int k = swaps.Count - 1; k >= 0; k--)
                {
                    if (messages[i].Contains(swaps[k].replacement.ToString()))
                    {
                        if (swaps[k].source == null)
                        {
                            messages.RemoveAt(i);
                            curcount -= 1;
                            if (i > 0) i--;
                            break;
                        }
                        else
                            messages[i] = messages[i].Replace(swaps[k].replacement, swaps[k].source);
                    }
                }
            }
        }
        public static void WriteJSONFileDataNew(List<string> messages)
        {
            using (FileStream fs = new FileStream("dataNEW.json", FileMode.OpenOrCreate))
            {
                JsonSerializer.SerializeAsync<List<string>>(fs, messages);

                Console.WriteLine("Данные сохранены в файл dataNEW.json в папке /bin/debag/net5.0/");
            }
        }
    }
   
    public class Swap
    {
        public string replacement { get; set; }
        public string source { get; set; }
        
        public Swap()
        { }
        public Swap(string replacement, string source)
        {
            this.replacement = replacement;
            this.source = source;
        }
    }
    public class ListSwaps
    {
        public List<Swap> Swaps;
        public List<Swap> ReadJSONFileReplacement(string fileNameJSON)
        {
            using (StreamReader r = new StreamReader(fileNameJSON))
            {
                string json = r.ReadToEnd();
                Swaps = JsonConvert.DeserializeObject<List<Swap>>(json);
            }
            return Swaps;
        }
        public List<Swap> DeleteRepeateReplacement()
        {
            for (int i = 0; i < Swaps.Count; i++)
            {
                for (int j = i + 1; j < Swaps.Count; j++)
                {
                    if (Swaps[i].replacement == Swaps[j].replacement)
                    {
                        Swaps.RemoveAt(i);
                        break;
                    }
                }
            }
            return Swaps;
        }
    }
}
