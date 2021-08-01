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
        // 콘솔앱과 동일한 static void Main
        // Program은 거시적인 설정 Http 서버, IIS 사용 여부 등 거의 바뀌지 않음
        // Startup은 세부적인 설정( 미들웨어 설정, Dependency injection 등, 필요에 따라 추가 또는 삭제)
        public static void Main(string[] args)
        {
            // 3. Build()에서 IHost를 만듬
            // 4. Run() 구동 -> Listen을 시작
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // 1. 각종 옵션 설정을 세팅
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // 2. Startup 클래스 지정
                    webBuilder.UseStartup<Startup>();

                    //// DI의 부품들 생명주기를 알아서 잡아주도록 설정
                    //webBuilder.UseDefaultServiceProvider(opt => opt.ValidateScopes = true);
                });
    }
}
