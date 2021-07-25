using AspNetCoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMVC.Controllers
{
    // MVC (Model-View-Controller)
    // Model (메모리,파일, DB 등 정보 추출) 재료
    // Controller (데이터 가공, 필터링, 유효성 체크, 서비스호출)
    // + 각종 서비스 -> 요리하는 역할
    // View (최종 결과물을 어떻게 보여줄지) 최종 서빙

    // 역할이 분담되어 유동적으로 기능을 변경할 수 있음
    //  ex) SPA(Json) Web(HTML) 결과물이 다르면 View Layer만 바꾸고, Controller는 재사용가능

    // Action은 요청에 대한 실제 처리 함수(Handler)
    // Controller 는 Action을 포함하고 있는 그룹
    // Controller 상속이 무조건 필요한 것은 아님
    // View()처럼 이미 정의된 Helpler 기능 사용하고 싶으면 필요
    // UI (View)와 관련된 기능들을 뺀 ControllerBase 가 바로 -> WebApi

    // MVC에서 Controller 각종 데이터 가공을 담당, UI 랑 무관
    // 넘길 때 IActionResult를 넘김 (return을)
    // 자주사용되는 IActionResult 
    // 1) ViewResult : HTML View 생성 -> View() 가 ViewResult를 반환하는 메서드
    // 2) RedirectResult : 요청을 다른 곳으로 토스 ( 다른 페이지로 연결해줄 때)
    // 3) FileResult : 파일을 반환
    // 4) ContentResult : 특정 string을 반환
    // 5) StartupCodeResult : HTTP status code 반환
    // 6) NotFoundResult : 404 HTTP status code 반환

    // 결론 : MVC를 사용하면 역할 분담이 확실해진다.
    // 서로 종속된 코드를 만들면, 하나를 수정할 때 다른 쪽에서 문제가 터지는 일이 빈번
    // MVC는 역할 분담으로 인해 코드 종속성을 제거
    // MVC에서 V? [MC] 만 사용하면 결국 WebAPI가 되는것
    // 결과적으로 MVC나 WebAPI나 설계 철학 자체는 큰 차이없음

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // var url = Url.Action("Privacy", "Home");
            var url = Url.RouteUrl("test", new { test = 1234 });
            // return View();
            // return Redirect(url);
            return RedirectToAction("Privacy");
        }

        public IActionResult Privacy()
        {
            // 데이터 넘기는 테스트
            ViewData["Message"] = "DataFromPrivacy";
            return View();
        }

        // [Route("Hello")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
