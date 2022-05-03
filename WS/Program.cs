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

            ListSwaps listSwaps = new ListSwaps();
            listSwaps.ReadJSONFileReplacement(replacementfile);
            listSwaps.DeleteRepeateReplacement();

            ListMessage listMessage = new ListMessage();
            listMessage.ReadJSONFileMessage(datafile);

            listMessage.SearchReplaceMessage(listSwaps);

            listMessage.WriteJSONFileDataNew();
                      
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
    public class ListMessage
    {
        public List<string> messages;

        public List<string> ReadJSONFileMessage(string fileNameJSON)
        {
            using (StreamReader r = new StreamReader(fileNameJSON))
            {
                string json = r.ReadToEnd();
                messages = JsonConvert.DeserializeObject<List<string>>(json);
            }
            return messages;
        }

        public List<string> SearchReplaceMessage(ListSwaps listSwaps)
        {
            int curcount = messages.Count;
            for (int i = 0; i < curcount; i++)
            {
                for (int k = listSwaps.Swaps.Count - 1; k >= 0; k--)
                {
                    if (messages[i].Contains(listSwaps.Swaps[k].replacement.ToString()))
                    {
                        if (listSwaps.Swaps[k].source == null)
                        {
                            messages.RemoveAt(i);
                            curcount -= 1;
                            if (i > 0) i--;
                            break;
                        }
                        else
                            messages[i] = messages[i].Replace(listSwaps.Swaps[k].replacement, listSwaps.Swaps[k].source);
                    }
                }
            }
            return messages;
        }

        public void WriteJSONFileDataNew()
        {
            using (FileStream fs = new FileStream("dataNEW.json", FileMode.OpenOrCreate))
            {
                JsonSerializer.SerializeAsync<List<string>>(fs, messages);

                Console.WriteLine("Данные сохранены в файл dataNEW.json в папке /bin/debag/net5.0/");
            }            
        }
    }
}
