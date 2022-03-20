﻿using Newtonsoft.Json;
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
            List<swap> swaps = new List<swap>();
            List<string> messages = new List<string>();

            using (StreamReader r = new StreamReader(replacementfile))
            {
                string json = r.ReadToEnd();
                swaps = JsonConvert.DeserializeObject<List<swap>>(json);
                foreach (var el in swaps)
                    Console.WriteLine($"new: {el.replacement}  old: {el.source}");
            }
            using (StreamReader r = new StreamReader(datafile))
            {
                string json = r.ReadToEnd();
                messages = JsonConvert.DeserializeObject<List<string>>(json);
                foreach (var el in messages)
                    Console.WriteLine($"text: {el}");
            }

            for (int i=0; i<swaps.Count; i++)
            {
                for(int j=i+1; j<swaps.Count; j++)
                {
                    if (swaps[i].replacement == swaps[j].replacement)
                    {
                        swaps.RemoveAt(i);
                        break;
                    }
                }
            }
            foreach (var el in swaps)
                Console.WriteLine($"replacementNew: {el.replacement}         swapto {el.source}");
            int curcount = messages.Count;
            for (int i=0; i<curcount; i++)
            {
                for (int k= swaps.Count - 1; k>=0; k--)
                {
                    if(messages[i].Contains(swaps[k].replacement.ToString()))
                    {
                        if (swaps[k].source == null)
                        {
                            messages.RemoveAt(i);
                            curcount -= 1;
                            if(i > 0) i--;
                            break;
                        }
                        else                        
                            messages[i] = messages[i].Replace(swaps[k].replacement, swaps[k].source);
                    }
                }
            }
            Console.WriteLine();
            foreach (var el in messages)
                Console.WriteLine($"dataNew: {el}");

            using (FileStream fs = new FileStream("dataNEW.json", FileMode.OpenOrCreate))
            {
                    JsonSerializer.SerializeAsync<List<string>>(fs, messages);
                
                Console.WriteLine("Data has been saved to file");
            }

            //StreamWriter streamWriter = new StreamWriter("dataNEW.json");
            ////using (FileStream fs = new FileStream("dataNEW.json", FileMode.OpenOrCreate))
            //{
            //    //message mess = new Person("Tom", 37);
            //    foreach(var m in messages)
            //    {
            //        streamWriter.WriteLine("\"" + m + "\"");
            //    }
            //    streamWriter.Close(); 

        }
    }
    public class swap
    {
        public string replacement { get; set; }
        public string source { get; set; }
    }
    public class message
    {
        public string text { get; set; }
    }
}