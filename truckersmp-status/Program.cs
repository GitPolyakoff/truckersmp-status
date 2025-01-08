using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

namespace TruckersMPStatus
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.TreatControlCAsInput = true;
            while (Console.KeyAvailable) { Console.ReadKey(true); }

            string baseUrl = "https://api.truckersmp.com/v2";

            // Увеличенный размер консоли
            Console.SetWindowSize(160, 50);
            Console.SetBufferSize(160, 50);

            ShowWelcomeMessage();

            RestClient client = new RestClient(baseUrl);

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Blue;

                // Получение данных
                var servers = GetServers(client);
                int? gameTime = GetGameTime(client);
                var gameVersion = GetGameVersion(client);

                Console.Clear();

                var moscowTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Russian Standard Time");
                Console.WriteLine($"Московское время: {moscowTime:HH:mm:ss} | UTC: {DateTime.UtcNow:HH:mm:ss}");
                if (gameTime != null)
                {
                    TimeSpan time = TimeSpan.FromMinutes(gameTime.Value);
                    Console.WriteLine($"Текущее время игры: {time.Hours:D2}:{time.Minutes:D2}");
                }
                else
                {
                    Console.WriteLine("Текущее время игры: Не удалось получить.");
                }
                if (gameVersion != null)
                {
                    Console.WriteLine($"Поддерживаемая версия ETS2: {gameVersion.SupportedGameVersion}");
                    Console.WriteLine($"Поддерживаемая версия ATS: {gameVersion.SupportedAtsGameVersion}");
                }
                else
                {
                    Console.WriteLine("Не удалось получить данные о поддерживаемых версиях.");
                }

                PrintTable(servers);
                Thread.Sleep(1000);
            }
        }

        private static void ShowWelcomeMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(@"
████████╗██████╗ ██╗   ██╗ ██████╗██╗  ██╗███████╗██████╗ ███████╗███╗   ███╗██████╗         
╚══██╔══╝██╔══██╗██║   ██║██╔════╝██║ ██╔╝██╔════╝██╔══██╗██╔════╝████╗ ████║██╔══██╗        
   ██║   ██████╔╝██║   ██║██║     █████╔╝ █████╗  ██████╔╝███████╗██╔████╔██║██████╔╝        
   ██║   ██╔══██╗██║   ██║██║     ██╔═██╗ ██╔══╝  ██╔══██╗╚════██║██║╚██╔╝██║██╔═══╝         
   ██║   ██║  ██║╚██████╔╝╚██████╗██║  ██╗███████╗██║  ██║███████║██║ ╚═╝ ██║██║             
   ╚═╝   ╚═╝  ╚═╝ ╚═════╝  ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝╚═╝     ╚═╝╚═╝             
                                                                                             
███████╗████████╗ █████╗ ████████╗██╗   ██╗███████╗                                          
██╔════╝╚══██╔══╝██╔══██╗╚══██╔══╝██║   ██║██╔════╝                                          
███████╗   ██║   ███████║   ██║   ██║   ██║███████╗                                          
╚════██║   ██║   ██╔══██║   ██║   ██║   ██║╚════██║                                          
███████║   ██║   ██║  ██║   ██║   ╚██████╔╝███████║                                          
╚══════╝   ╚═╝   ╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚══════╝                                          
                                                                                             
██████╗ ██╗   ██╗    ██████╗  ██████╗ ██╗  ██╗   ██╗ █████╗ ██╗  ██╗ ██████╗ ███████╗███████╗
██╔══██╗╚██╗ ██╔╝    ██╔══██╗██╔═══██╗██║  ╚██╗ ██╔╝██╔══██╗██║ ██╔╝██╔═══██╗██╔════╝██╔════╝
██████╔╝ ╚████╔╝     ██████╔╝██║   ██║██║   ╚████╔╝ ███████║█████╔╝ ██║   ██║█████╗  █████╗  
██╔══██╗  ╚██╔╝      ██╔═══╝ ██║   ██║██║    ╚██╔╝  ██╔══██║██╔═██╗ ██║   ██║██╔══╝  ██╔══╝  
██████╔╝   ██║       ██║     ╚██████╔╝███████╗██║   ██║  ██║██║  ██╗╚██████╔╝██║     ██║     
╚═════╝    ╚═╝       ╚═╝      ╚═════╝ ╚══════╝╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚═╝     ╚═╝     
                                                                                             
        ");
            Thread.Sleep(2000);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine(@"
                                                                                                    
                                                           =@  @@@                                  
                                                 @@@@@@@@@@@@@ @@.                                  
                                           @@@@@@@@%%%%%%%%%%@ @@:                                  
                                      @@@@@@%%%%%%%%%%%%%%%%%@ @@                                   
                                  @@@@@@@@%%%%%%%%%%%%%%%%%%%@ @@                                   
                               +@@@@@    @%%%%%%%%%%%%%%%%%%%@ @@                                   
                             @@@@@       @%%%%%%%%%%%%%%%%%%%@ @@                                   
                           @@@@          @%%%%%%%%%%%%%%%%%%%@ @@                                   
                         @@@@            @%%%%%%%%%%%%%%%%%%%@ @@                                   
                        @@@@             @%%%%%%%%%%%%%%%%%%%@ @@                                   
                       @@@@              @%%%%%%%%%%%%%%%%%%%@ @@                                   
                      @@%@               @%%%%%%%%%%%%%%%%%%%@ @@                                   
                @@@@@@@%%@     .@@@@@@@@@@%%%%%%%%%%%%%%%%%%%@ @@                                   
          @@@@@@@%%%%%%%%@@@@@@@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%@ @@                                   
    @@@.@@@@%-::-%@@@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@ @@                                   
  @@@+ @@   #@@@@*   @@@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@ @@                                   
  @    =@@@@@@@@@@@@@@  @@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%@                                      
     @@@@@        -@@@@@  =::::::::::::::::::::::::::::::::+@@@@@@@@     @@@@@@@@@@@@@@      @@@@@@ 
  @@@@@@   @@* +@@   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ @@@  @@-  @@  @@@%%%@@  @@#  @@@  @@@ 
 @@@@@@  @#  @@@   @  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ =@@ @@  @@@:  @  @%%@@ @@  @@@@  @     
 @@@@@  @@ @@@@@@@  @  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  @@  @ @@@@@@@: @ @@%@ =@ @@@@@@@@ @    
 @@@@@  @  @@@@@@@. @  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  @@@ @@ @@@@@@@@ @ @@@@ @+ @@@@@@@@ @    
   **   %@  @@@@@- @@   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@        @ =@@@@@@ =@       @  @@@@@@  @    
         #@       @@                                              @%      @@         @@      @@     
             %*#                                                     : .                :.:-                                                                                   
        ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(@"
    ████████ ██████  ██    ██  ██████ ██   ██ ███████ ██████  ███████ ███    ███ ██████  
       ██    ██   ██ ██    ██ ██      ██  ██  ██      ██   ██ ██      ████  ████ ██   ██ 
       ██    ██████  ██    ██ ██      █████   █████   ██████  ███████ ██ ████ ██ ██████  
       ██    ██   ██ ██    ██ ██      ██  ██  ██      ██   ██      ██ ██  ██  ██ ██      
       ██    ██   ██  ██████   ██████ ██   ██ ███████ ██   ██ ███████ ██      ██ ██      
                                                                                     
        ");
            Thread.Sleep(2000);

            Console.Clear();
        }

        private static void PrintTable(ApiResponse servers)
        {
            Console.WriteLine("\nTruckersMP Servers");
            Console.WriteLine(new string('-', 150));
            Console.WriteLine("| {0,-30} | {1,-20} | {2,-10} | {3,-10} | {4,-10} | {5,-15} | {6,-10} | {7,-10} | {8,-10} |",
                "Название", "IP:Port", "Игра", "Игроки", "Очередь", "Speed Limiter", "Коллизии", "AFK", "Событие");
            Console.WriteLine(new string('-', 150));

            if (servers != null)
            {
                foreach (var server in servers.Response)
                {
                    Console.WriteLine("| {0,-30} | {1,-20} | {2,-10} | {3,-10} | {4,-10} | {5,-15} | {6,-10} | {7,-10} | {8,-10} |",
                        server.Name,
                        $"{server.Ip}:{server.Port}",
                        server.Game,
                        $"{server.Players}/{server.MaxPlayers}",
                        server.Queue,
                        server.SpeedLimiter ? "Вкл" : "Выкл",
                        server.Collisions ? "Да" : "Нет",
                        server.AfkEnabled ? "Вкл" : "Выкл",
                        server.Event ? "Да" : "Нет");
                }
            }
            else
            {
                Console.WriteLine("| {0,-148} |", "Не удалось получить данные о серверах.");
            }
            Console.WriteLine(new string('-', 150));
        }

        public static ApiResponse GetServers(RestClient client)
        {
            var request = new RestRequest("/servers", Method.Get);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            }
            return null;
        }

        public static int? GetGameTime(RestClient client)
        {
            var request = new RestRequest("/game_time", Method.Get);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<GameTimeResponse>(response.Content);
                return result.GameTime;
            }
            return null;
        }

        public static GameVersionResponse GetGameVersion(RestClient client)
        {
            var request = new RestRequest("/version", Method.Get);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<GameVersionResponse>(response.Content);
            }
            return null;
        }

        // Классы для десериализации JSON-ответов
        public class ApiResponse
        {
            public bool Error { get; set; }
            public List<Server> Response { get; set; }
        }

        public class Server
        {
            public string Name { get; set; }
            public string Ip { get; set; }
            public int Port { get; set; }
            public string Game { get; set; }
            public int Players { get; set; }
            public int Queue { get; set; }
            public int MaxPlayers { get; set; }
            public bool SpeedLimiter { get; set; }
            public bool Collisions { get; set; }
            public bool AfkEnabled { get; set; }
            public bool Event { get; set; }
        }

        public class GameTimeResponse
        {
            [JsonProperty("game_time")]
            public int GameTime { get; set; }
        }

        public class GameVersionResponse
        {
            [JsonProperty("supported_game_version")]
            public string SupportedGameVersion { get; set; }

            [JsonProperty("supported_ats_game_version")]
            public string SupportedAtsGameVersion { get; set; }
        }
    }
}