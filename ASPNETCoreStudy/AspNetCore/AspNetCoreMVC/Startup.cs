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
        // ���� ���� �߰� (DI)
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // DI ���񽺶�? SRP (Single Responsibility Principle) : 1���񽺿��� 1���
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // HTTP Request Pipeline
        // � HTTP ��û�� ���� �� , ���� ��� �����ϴ��� �Ϸ��� ����
        // 1. IIS, Apache � HTTP ��û
        // 2. ASP.NET Core ���� (kestrel) ����
        // 3. �̵���� ����
        // 4. Controller�� ����
        // 5. Controller���� ó���ϰ� View�� ����(return View())

        // HTTP Request Pipeline ( NodeJS �� ���� )
        // �̵���� : HTTP request / responese �� ó���ϴ� �߰� ��ǰ�̶�� ������ �� ����

        // [Request]
        // [����������] <- ���⼱ Configure �޼��� ������ �Ϸ��� ������ �ǹ�
        // [������ MVC Endpoint]
        // [����������]
        // [Response]
        // ���� ������ ������ �Ʒ���, �Ʒ����� ���� ��� �ݺ�

        // �̵����� ó���� ������� �ٸ� �̵����� �ѱ� �� ���� ( ������ endpoint  ���� ���������ο��� �������������� �̵�����)
        // -> ������ �߿��� �� ����

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

            // CSS, JS, �̹����� ��û ���� �� ó��
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // ����� : ������
            // HTTP request <-> ��� handler �����ϴ� ��

            // ASP.NET �ʱ� ���������� /hello.aspx�� ���� ó���ϴ� ���� ��ü�� URL�� �Է�
            // 1) ���� �̸��� �ٲ��? -> Ŭ�� �� ó���� ���� ���� ������ ���� �Ұ���
            // 2) /hello.aspx?method=1&id=3 �� ���� QueryString ����� URL�̿���
            // ���� ����� /hello/get/3

            // �⺻ ����(Convention)�� Controller/Action/Id ����
            // �ٸ� �̸� �����ϰ� ���� ��?
            // ~ API������ ����� ��, URL �ּҰ� � ������ �ϴ��� �� ��Ȯ�ϰ� ��Ʈ�� �ְ� �������(�̸��� �ǹ� �ο�)
            // Controller�� �������� �ʰ� ����� URL�� ��ü�ϰ� �ʹٸ� ����� ����! 
            // Routing�� ����Ƿ��� [�̵���� ����������]�� ���� ������ �Ǿ����.
            // ~�߰��� ������ ���ٰų�, Ư�� �̵��� �帧�� ����ë�ٸ� X

            // ���������� ������ ����������, MapControllerRoute�� ���� Routing ��Ģ�� ����
            // ~ ������ �̿��� ������� Routing ���
            // ~ Attribute Routing �Ӽ� ����Ͽ� ���

            // Route Template (Pattern)
            // name : "default" -> �ټ��� ������ �� �ִٴ� �ǹ̷� �� �� ����(���� �� �� default�� �ȴٴ� �Ŵϱ�)

            app.UseEndpoints(endpoints =>
            {
                // ��Ģ
                // api : literal value ( ���� ���ڿ� ��? �� �ʿ� )
                // {controller} {action} : route parameter (�� �ʿ�)
                // {controller=Home}/{action=Index} : optional route parameter ( ������ �˾Ƽ� �⺻�� ����)
                // {id?} : optional route parameter (�����)
                // [����!] : controller�� action ������ �������� ��. ( ��Ī or �⺻���� ����)

                // ����� ������ �Ʒ��� �ڵ� ������� ��Ī��. ����������. ��Ī �Ǹ� �Ʒ� ����.

                // Constraint ���� (�����Ȳ)
                // "{controller=Home}/{action=Index}/{id?}"
                // id�� �������ϴٴ� ������ ���� ���ڿ��� �� ���� ������
                // {cc:int} : ������
                // {cc:min(18)} : 18�̻� ������
                // {cc:length(5)} : 5���� string

                // Default Value�� Constraint�� �����ϴ� 2��° ��� (Anonymous Object)

                // Match-All (������ ��Ŀī�� )
                // {*joker} *�� ���̸� ��� ���ڿ� �� ��Ī

                // Redirection : �ٸ� URL�� �佺!
                // Redirect(url) << URL ���� ���� ���
                // - Url.Action ���
                // - Url.RouteUrl ���
                // RedirectToAction() ���
                // RedirectToRoute() ���


                endpoints.MapControllerRoute(
                    name: "test",
                    //pattern: "api/{controller=Home}/{action=Privacy}/{test?}");
                    //pattern: "api/{controller=Home}/{action=Privacy}/{test:int}",
                    pattern: "api/{test}",
                    defaults: new { controller = "Home", action = "Privacy" },
                    constraints: new { test = new IntRouteConstraint() });

                // ����� ���� ����
                endpoints.MapControllerRoute(
                        name: "default",
                        // = : �Ӽ� �⺻��, ? : �ɼ� �Ӽ�
                        pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "joker",
                    pattern: "{*joker}", // � �����̵� ��Ī -> ���Ʈ ������ �������� �ؾ߰���?
                    defaults: new { controller = "Home", action = "Error" });
            });
        }
    }
}
