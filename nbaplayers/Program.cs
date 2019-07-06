using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace nbaplayers
{
    class Program
    {
        private enum Choise
        {
            OpenFile,
            ChoosePath,
            Years,
            Rating
        }

        private static string _enteredData;

        private static int _maxYears, _minRating, _number;

        static void Main(string[] args)
        {
            List<Player> players;

            CheckData(ref _enteredData, Choise.OpenFile, "Wrong path or file! Try again. ",
                "Input path to JSON file: ");

            using (var file = new StreamReader(_enteredData))
            {
                var json = file.ReadToEnd();
                players = JsonConvert.DeserializeObject<List<Player>>(json);
            }

            CheckData(ref _enteredData, Choise.Years,"The entered information is not a year. Try again. ",
                "Input maximum number of years the player has played in the league to qualify: ");

            CheckData(ref _enteredData, Choise.Rating,"The entered information is not a number. Try again. ",
                "Input minimum rating the player should have to qualify: ");

            CheckData(ref _enteredData, Choise.ChoosePath, "Directory does not exist! Try again. ",
                "Input path for the CSV file: ");

            var csvFile = _enteredData + "\\generatedCsv.txt";

            using (var file = new StreamWriter(csvFile))
            {
                file.WriteLine("Name, Rating");

                IOrderedEnumerable<Player> sortedPlayers = players.OrderByDescending(p => p.Rating);

                foreach (var player in sortedPlayers)
                {
                    if (DateTime.Now.Year - player.PlayingSince <= _maxYears && player.Rating >= _minRating)
                        file.WriteLine(player.Name + ", " + player.Rating);
                }

                Console.WriteLine("File generated! Check it at: " + csvFile);
            }

            Process.Start(csvFile);
        }

        private static bool ChangeCondition(Choise condition, string enteredData)
        {
            bool cond;
            switch (condition)
            {
                case Choise.OpenFile:
                    cond = File.Exists(_enteredData) && Path.GetExtension(_enteredData) == ".json";
                    return cond;
                case Choise.ChoosePath:
                    cond = Directory.Exists(_enteredData);
                    return cond;
                default:
                    cond = int.TryParse(enteredData, out _number);

                    if (condition == Choise.Years)
                        _maxYears = _number;
                    else
                        _minRating = _number;

                    return cond;
            }
        }

        private static void CheckData(ref string enteredData, Choise condition, string retryMessage,
            string conditionMessage)
        {
            Console.WriteLine(conditionMessage);
            while (true)
            {
                enteredData = Console.ReadLine();
                if (ChangeCondition(condition, enteredData))
                    break;

                Console.WriteLine(retryMessage);
            }
        }
    }
}