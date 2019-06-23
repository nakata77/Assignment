using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace nbaplayers
{
    class Program
    {
        private static string _enetredData;

        private static int _maxYears, _minRating, _number;

        private static bool _cond;

        private static void ChangeCondition(string arg, string enteredData)
        {
            switch (arg)
            {
                case "openFile":
                    _cond = File.Exists(_enetredData) && Path.GetExtension(_enetredData) == ".json";
                    break;
                case "choosePath":
                    _cond = Directory.Exists(_enetredData);
                    break;
                default:
                    _cond = int.TryParse(enteredData, out _number);

                    if (arg == "years")
                        _maxYears = _number;
                    else
                        _minRating = _number;

                    break;
            }
        }

        private static void CheckData(ref string enteredData, string condition, string retryMessage, 
            string conditionMessage)
        {
            Console.WriteLine(conditionMessage);
            while (true)
            {
                enteredData = Console.ReadLine();
                ChangeCondition(condition, enteredData);
                if (_cond)
                    break;

                Console.WriteLine(retryMessage);
            }
        }

        static void Main(string[] args)
        {
            List<Player> players;

            CheckData(ref _enetredData, "openFile", "Wrong path or file! Try again: ",
                "Input path to JSON file: ");

            using (var r = new StreamReader(_enetredData))
            {
                var json = r.ReadToEnd();
                players = JsonConvert.DeserializeObject<List<Player>>(json);
            }

            CheckData(ref _enetredData, "years","The entered information is not a year. Try again: ",
                "Input maximum number of years the player has played in the league to qualify: ");

            CheckData(ref _enetredData, "rating","The entered information is not a number. Try again: ",
                "Input minimum rating the player should have to qualify: ");

            CheckData(ref _enetredData, "choosePath", "Directory does not exist! Try again: ",
                "Input path for the CSV file: ");

            var csvFile = _enetredData + "\\generatedCsv.txt";

            if (File.Exists(csvFile))
                File.Delete(csvFile);

            using (var file = new StreamWriter(csvFile, true))
            {
                file.WriteLine("Name, Rating");

                foreach (var player in players)
                {
                    if (DateTime.Now.Year - player.PlayingSince <= _maxYears && player.Rating >= _minRating)
                        file.WriteLine(player.Name + ", " + player.Rating);
                }

                Console.WriteLine("File generated! Check it at: " + csvFile);
            }
        }
    }
}