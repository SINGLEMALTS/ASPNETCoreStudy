using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing.Constraints;
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

            // 라우팅 : 길잡이
            // HTTP request <-> 담당 handler 매핑하는 것

            // ASP.NET 초기 버전에서는 /hello.aspx와 같이 처리하는 파일 자체를 URL에 입력
            // 1) 파일 이름이 바뀌면? -> 클라 쪽 처리를 같이 하지 않으면 접속 불가능
            // 2) /hello.aspx?method=1&id=3 와 같이 QueryString 방식의 URL이였음
            // 지금 방식은 /hello/get/3

            // 기본 관례(Convention)는 Controller/Action/Id 형식
            // 다른 이름 지정하고 싶을 땐?
            // ~ API서버로 사용할 때, URL 주소가 어떤 역할을 하는지 더 명확하게 힌트를 주고 싶은경우(이름에 의미 부여)
            // Controller를 수정하지 않고 연결된 URL만 교체하고 싶다면 라우팅 설정! 
            // Routing이 적용되려면 [미들웨어 파이프라인]에 의해 전달이 되어야함.
            // ~중간에 에러가 난다거나, 특정 미들웨어가 흐름을 가로챘다면 X

            // 파이프라인 끝까지 도달했으면, MapControllerRoute에 의해 Routing 규칙이 결정
            // ~ 패턴을 이용한 방식으로 Routing 방식
            // ~ Attribute Routing 속성 사용하여 방식

            // Route Template (Pattern)
            // name : "default" -> 다수를 설정할 수 있다는 의미로 볼 수 있음(여러 개 중 default가 된다는 거니까)

            app.UseEndpoints(endpoints =>
            {
                // 규칙
                // api : literal value ( 고정 문자열 값? 꼭 필요 )
                // {controller} {action} : route parameter (꼭 필요)
                // {controller=Home}/{action=Index} : optional route parameter ( 없으면 알아서 기본값 설정)
                // {id?} : optional route parameter (없어도됨)
                // [주의!] : controller랑 action 무조건 정해져야 함. ( 매칭 or 기본값을 통해)

                // 라우팅 순서는 아래의 코드 순서대로 매칭함. 위에서부터. 매칭 되면 아래 무시.

                // Constraint 관련 (제약상황)
                // "{controller=Home}/{action=Index}/{id?}"
                // id가 광범위하다는 문제가 있음 문자열이 들어갈 수도 있으니
                // {cc:int} : 정수만
                // {cc:min(18)} : 18이상 정수만
                // {cc:length(5)} : 5글자 string

                // Default Value와 Constraint를 설정하는 2번째 방법 (Anonymous Object)

                // Match-All (일종의 조커카드 )
                // {*joker} *를 붙이면 모든 문자열 다 매칭

                // Redirection : 다른 URL로 토스!
                // Redirect(url) << URL 직접 만들어서 방법
                // - Url.Action 방법
                // - Url.RouteUrl 방법
                // RedirectToAction() 방법
                // RedirectToRoute() 방법


                endpoints.MapControllerRoute(
                    name: "test",
                    //pattern: "api/{controller=Home}/{action=Privacy}/{test?}");
                    //pattern: "api/{controller=Home}/{action=Privacy}/{test:int}",
                    pattern: "api/{test}",
                    defaults: new { controller = "Home", action = "Privacy" },
                    constraints: new { test = new IntRouteConstraint() });

                // 라우팅 패턴 설정
                endpoints.MapControllerRoute(
                        name: "default",
                        // = : 속성 기본값, ? : 옵션 속성
                        pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "joker",
                    pattern: "{*joker}", // 어떤 패턴이든 매칭 -> 라우트 순서를 마지막에 해야겠지?
                    defaults: new { controller = "Home", action = "Error" });
            });
        }
    }
}
