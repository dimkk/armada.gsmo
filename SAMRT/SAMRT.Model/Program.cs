using System;
using System.Configuration;

namespace SAMRT.Model
{
    class Program
    {
        protected static string GetConfigValue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(key, "Параметр не определен в конфигурационном файле");
            return value;
        }
        protected static string WebURL
        {
            get { return GetConfigValue("WebUrl"); }
        }
        protected static string Domain
        {
            get { return GetConfigValue("Domain"); }
        }
        protected static string Login
        {
            get { return GetConfigValue("Login"); }
        }
        protected static string Password
        {
            get { return GetConfigValue("Password"); }
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Развертывание модели данных...");
                Deploy();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Произошло необработанное исключение:");
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("Для продолжения нажмите Enter...");
            Console.ReadLine();
        }

        static void Deploy()
        {
            var model = new Model(WebURL, Login, Password, Domain);
            model.Deploy();
        }
    }
}
