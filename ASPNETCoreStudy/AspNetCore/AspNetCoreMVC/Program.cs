using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMVC
{
    public class Program
    {
        // �ܼ־۰� ������ static void Main
        // Program�� �Ž����� ���� Http ����, IIS ��� ���� �� ���� �ٲ��� ����
        // Startup�� �������� ����( �̵���� ����, Dependency injection ��, �ʿ信 ���� �߰� �Ǵ� ����)
        public static void Main(string[] args)
        {
            // 3. Build()���� IHost�� ����
            // 4. Run() ���� -> Listen�� ����
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // 1. ���� �ɼ� ������ ����
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // 2. Startup Ŭ���� ����
                    webBuilder.UseStartup<Startup>();
                });
    }
}