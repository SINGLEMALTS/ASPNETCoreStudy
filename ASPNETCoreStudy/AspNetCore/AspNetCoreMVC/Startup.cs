using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 각종 서비스 추가 (DI)
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // DI 서비스란? SRP (Single Responsibility Principle) : 1서비스에서 1기능
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // HTTP Request Pipeline
        // 어떤 HTTP 요청이 왔을 때 , 앱이 어떻게 응답하는지 일련의 과정
        // 1. IIS, Apache 등에 HTTP 요청
        // 2. ASP.NET Core 서버 (kestrel) 전달
        // 3. 미들웨어 적용
        // 4. Controller로 전달
        // 5. Controller에서 처리하고 View로 전달(return View())

        // HTTP Request Pipeline ( NodeJS 와 유사 )
        // 미들웨어 : HTTP request / responese 를 처리하는 중간 부품이라고 생각할 수 있음

        // [Request]
        // [파이프라인] <- 여기선 Configure 메서드 내부의 일련의 순서를 의미
        // [마지막 MVC Endpoint]
        // [파이프라인]
        // [Response]
        // 위의 과정은 위에서 아래로, 아래에서 위로 계속 반복

        // 미들웨어에서 처리한 결과물을 다른 미들웨어로 넘길 수 있음 ( 마지막 endpoint  전에 파이프라인에서 파이프라인으로 이동가능)
        // -> 순서가 중요할 수 있음

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            // CSS, JS, 이미지등 요청 받을 때 처리
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // 라우팅 패턴 설정
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
