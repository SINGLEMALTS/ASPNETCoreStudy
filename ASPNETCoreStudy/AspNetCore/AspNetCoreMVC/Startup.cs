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

            app.UseEndpoints(endpoints =>
            {
                // ����� ���� ����
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
