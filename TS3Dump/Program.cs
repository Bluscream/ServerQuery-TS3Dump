using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using TentacleSoftware.TeamSpeakQuery;
using TentacleSoftware.TeamSpeakQuery.NotifyResult;
using TentacleSoftware.TeamSpeakQuery.ServerQueryResult;

namespace TS3Dump
{
    class Program
    {
        static ServerQueryClient client;
        static IniData cfg;
        static void Main(string[] args)
        {
            cfg = Config.LoadConfig();
            Start();
            Console.ReadKey(true);
            Dispose();
        }
        static void Start()
        {
            Connect();
            Login();
            Use();
            Subscribe();
            client.KeepAlive(TimeSpan.FromMinutes(2));
        }
        static void Connect()
        {
            client = new ServerQueryClient(cfg["server"]["address"], int.Parse(cfg["server"]["queryport"]), TimeSpan.FromSeconds(1));
            ServerQueryBaseResult connected = client.Initialize().Result;
            Console.WriteLine("connected {0}", connected.Success);
        }
        static void Login()
        {
            if (string.IsNullOrWhiteSpace(cfg["server"]["password"]))
            {
                Console.WriteLine("login skipped, password is empty");
            } else
            {
                ServerQueryBaseResult login = client.Login(cfg["server"]["login"], cfg["server"]["password"]).Result;
                Console.WriteLine("login {0} {1} {2}", login.Success, login.ErrorId, login.ErrorMessage);
            }
        }
        static void Use()
        {
            ServerQueryBaseResult use = client.Use(UseServerBy.Port, int.Parse(cfg["server"]["voiceport"])).Result;
            Console.WriteLine("use {0} {1} {2}", use.Success, use.ErrorId, use.ErrorMessage);
        }
        static void Subscribe()
        {
            client.ConnectionClosed += onConnectionClosed;
            Console.WriteLine("registered for ConnectionClosed");
            client.NotifyClientEnterView += onNotifyClientEnterView;
            Console.WriteLine("registered for NotifyClientEnterView");
        }
        static void onConnectionClosed(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Connection closed, trying to reconnect");
            Dispose();
            Start();
        }
        static void onNotifyClientEnterView(object source, NotifyClientEnterViewResult notification)
        {
            Console.WriteLine("ClientEnterView {0}: {1}", notification.Clid, notification.ClientNickname);
            ClientInfoResult clientInfo = client.ClientInfo(int.Parse(notification.Clid)).Result;
            Console.WriteLine("clientInfo {0} {1} {2} {3}", clientInfo.Success, clientInfo.ErrorId, clientInfo.ClientVersion, clientInfo.ClientPlatform);
        }
        static void Dispose()
        {
            ServerQueryBaseResult unregister = client.ServerNotifyUnregister().Result;
            Console.WriteLine("unregister {0} {1} {2}", unregister.Success, unregister.ErrorId, unregister.ErrorMessage);

            ServerQueryBaseResult logout = client.Logout().Result;
            Console.WriteLine("logout {0} {1} {2}", logout.Success, logout.ErrorId, logout.ErrorMessage);

            ServerQueryBaseResult quit = client.Quit().Result;
            Console.WriteLine("quit {0} {1} {2}", quit.Success, quit.ErrorId, quit.ErrorMessage);
        }
    }
}
