using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyWebhook
{
    class Program
    {
        private static IWebHookManager _whManager;
        private static IWebHookStore _whStore;
        private static WebHookSender _whSender;

        static void Main(string[] args)
        {
            _whStore = new MemoryWebHookStore();
            _whManager = new WebHookManager(_whStore, _whSender, new TraceLogger());

            SubscribeNewUser();
            SendWebhookAsync().Wait();

            Console.ReadLine();
        }

        private static void SubscribeNewUser()
        {
            var webhook = new WebHook();
            webhook.Filters.Add("event1");
            webhook.Properties.Add("StaticParamA", 10);
            webhook.Properties.Add("StaticParamB", 20);
            webhook.Secret = "PSBuMnbzZqVir4OnN4DE10IqB7HXfQ9l2";
            webhook.WebHookUri = new Uri("https://blackfriday.azurewebsites.net");

            _whStore.InsertWebHookAsync("user1", webhook);
        }

        private static async Task SendWebhookAsync()
        {
            var notifications = new List<NotificationDictionary> { new NotificationDictionary("event1", new { DynamicParamA = 100, DynamicParamB = 200 }) };
            var x = await _whManager.NotifyAsync("user1", notifications, predicate);
        }

        private static bool predicate(WebHook arg1, string arg2)
        {
            throw new NotImplementedException();
        }
    }
}