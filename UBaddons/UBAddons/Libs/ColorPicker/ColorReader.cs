using EloBuddy;
using EloBuddy.SDK.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using UBAddons.Log;

namespace UBAddons.Libs.ColorPicker
{
    class ColorReader
    {
        private const string ColorFileName = "Colors.json";
        private static string UBAddonsPath = Path.Combine(EloBuddy.Sandbox.SandboxConfig.DataDirectory, "UBAddons");
        private static string FilePath = Path.Combine(UBAddonsPath, ColorFileName);
        //public static string ChampPath = Path.Combine(UBAddonsPath, Player.Instance.Hero.ToString());

        /// <summary>
        /// Read Saved Color File
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <returns></returns>
        public static Dictionary<string, Color> ReadDataFile()
        {
            if (!Directory.Exists(UBAddonsPath))
            {
                Debug.Print("Directory doesnt exist. Creating UBAddons folder", General.Console_Message.ColorPicker);
                Directory.CreateDirectory(UBAddonsPath);
                return new Dictionary<string, Color>();
            }
            if (!File.Exists(FilePath))
            {
                //Not any file here
                return new Dictionary<string,Color>();
            }
            try
            {
                //Begin convert to JToken
                return ReadJsonFile(FilePath).ToObject<Dictionary<string, Color>>();
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString(), General.Console_Message.Error);
            }
            return new Dictionary<string, Color>();
        }

        /// <summary>
        /// Read Color.json
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static JToken ReadJsonFile(string path)
        {
            JToken token;
            JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(path)));
            token = new JsonSerializer().Deserialize<JToken>(reader);
            return token;
        }

        /// <summary>
        /// Load Color from Saved file
        /// </summary>
        /// <param name="uniqueId">UniqueID menu</param>
        /// <param name="defaultColor">If here not saved color, will return this</param>
        /// <returns></returns>
        public static Color Load(string uniqueId, Color defaultColor)
        {
            var dictionary = ReadDataFile();
            var color = new Color();
            if (dictionary == new Dictionary<string, Color>())
            {
                Save(uniqueId, defaultColor);
            }
            else
            {
                //Has a key return saved color
                if (dictionary.ContainsKey(uniqueId))
                {
                    color = dictionary[uniqueId];
                }
                //No key? Let's save it
                else
                {
                    Save(uniqueId, defaultColor);
                }
            }
            return color.Equals(new Color()) ? defaultColor : color;
        }
        /// <summary>
        /// Save method
        /// </summary>
        /// <param name="uniqueId">UniqueID menu</param>
        /// <param name="CurrentValue">Color Save</param>
        public static void Save(string uniqueId, Color CurrentValue)
        {
            Debug.Print("Begin Save Color", General.Console_Message.ColorPicker);

            var Dic = ReadDataFile();

            //Not have file
            if (Dic == new Dictionary<string, Color>())
            {
                Dic = new Dictionary<string, Color>() 
                { 
                    { uniqueId, CurrentValue },
                };
            }
            // Found a file
            else
            {
                //need save = all value in menu
                if (Dic.ContainsKey(uniqueId))
                {
                    //Has a key, only change that Value;
                    Dic[uniqueId] = CurrentValue;
                }
                else
                {
                    //No Key, add new
                    Dic.Add(uniqueId, CurrentValue);
                }
            }
            //Convert to json
            string json = JsonConvert.SerializeObject(Dic, Formatting.Indented);
            //overwritten the file
            File.WriteAllText(FilePath, json);
        }
    }
}
