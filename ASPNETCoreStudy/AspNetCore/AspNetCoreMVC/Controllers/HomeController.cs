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
    /* [Routing]
    MVC(Model-View-Controller)
     Model(메모리, 파일, DB 등 정보 추출) 재료
     Controller(데이터 가공, 필터링, 유효성 체크, 서비스호출)
     + 각종 서비스 -> 요리하는 역할
     View(최종 결과물을 어떻게 보여줄지) 최종 서빙

     역할이 분담되어 유동적으로 기능을 변경할 수 있음
      ex) SPA(Json) Web(HTML) 결과물이 다르면 View Layer만 바꾸고, Controller는 재사용가능

     Action은 요청에 대한 실제 처리 함수(Handler)
     Controller 는 Action을 포함하고 있는 그룹
     Controller 상속이 무조건 필요한 것은 아님
     View()처럼 이미 정의된 Helpler 기능 사용하고 싶으면 필요
     UI(View)와 관련된 기능들을 뺀 ControllerBase 가 바로 -> WebApi

    MVC에서 Controller 각종 데이터 가공을 담당, UI 랑 무관
    넘길 때 IActionResult를 넘김(return을)
     자주사용되는 IActionResult
     1) ViewResult : HTML View 생성 -> View() 가 ViewResult를 반환하는 메서드
     2) RedirectResult : 요청을 다른 곳으로 토스(다른 페이지로 연결해줄 때)
     3) FileResult : 파일을 반환
     4) ContentResult : 특정 string을 반환
     5) StartupCodeResult : HTTP status code 반환
     6) NotFoundResult : 404 HTTP status code 반환

     결론 : MVC를 사용하면 역할 분담이 확실해진다.
     서로 종속된 코드를 만들면, 하나를 수정할 때 다른 쪽에서 문제가 터지는 일이 빈번
     MVC는 역할 분담으로 인해 코드 종속성을 제거
     MVC에서 V?[MC] 만 사용하면 결국 WebAPI가 되는것
     결과적으로 MVC나 WebAPI나 설계 철학 자체는 큰 차이없음
    */

    /* [Model Binding]
     * M : model
     * 데이터 모델, 데이터 종류가 다양하다
     * - binding Model 
     * 클라에서 보낸 데이터를 파싱하기 위한 바인딩 모델 << 유효성 검증 필수
     * - Application Model
     * 서버의 각종 서비스들이 사용하는 데이터 모델 (ex. ranking 서비스라면 ranking 모델)
     * - View Model
     * Response UI를 만들기 위한 데이터 모델
     * - API Model
     * WebAPI Controller에서 Json / XMl 으로 포맷으로 응답할 때 필요한 데이터 모델
     * 
     * 일반적인 MVC 순서를 생각해보면 
     * 1. HTTP Request가 온다
     * 2. Routing에 의해 Controller와 Action 정해짐
     * 3. Model binding으로 Request에 있는 데이터를 파싱 (Validation)
     * 4. 담당 서비스로 전달 (Application Model)
     * 5. 담당 서비스가 결과물을  Action 돌려주면
     * 6. Action에서 ViewModel을 이용해서 View로 전달
     * 7. View에서 HTML 생성
     * 8. Response로 HTML 결과물 전송
     * 
     * Model Binding 방식 3가지 , 요청이 왔을때 우선처리는 1,2,3의 순서대로 발생
     * 1. Form Values
     * request의 body 에서 보낸값(HTTP Post방식의 요청)
     * 2. Routes Values
     * URL 매칭이나 Default Value에 의한 매칭
     * 3. Query String Values
     * URL 끝에 붙이는 방법. ex ?name=Rookiss(HTTP Get방식의 요청)
     * 
     * Complex Types
     * 넘겨받을 인자가 많으면 부담스러우니 별도의 모델링 클래스를 만듬
     * 
     * Collections
     * 더 나아가서 List나 Dictionary로도 매핑할 수 있음
     * 
     * Binding String 지정
     * 기본적으로 Binding Model은 Form , Route, QueryString
     * 위의 삼총사 중 하나를 명시적으로 지정해서 파싱하거나, 다른 애로 지정할 수 있음
     * ex> 대표적으로 body에서 JSON 형태로 데이터를 보내주고 싶을 때
     * 
     * [FromHeader] HeadersValue 에서 찾아라
     * [FromQuery] QueryString 에서 찾아라
     * [FromRoute] Route Parameter 에서 찾아라
     * [FromForm] Post Body에서 찾아라
     * [FromBody] 그냥 Body 에서 찾아라( 디폴트 JSON-> 다른 형태로도 세팅가능)
     * 
     * 
     * Validation 유효성 검사
     * 클라-서버 구조는 신용할 수 없음
     * 전화번호에 abcd를 보낸다거나..
     * 
     * DataAnnotation
     * 공용으로사용되는 모델의 장점은 -> 기본 검증 모델을 하나 만들고 여러곳에서 사용, but세부적인 검사는 힘듬
     * [Required] : 필수로 있어야함 없으면 에러발생
     * [CreditCard] : 올바른 결제카드 번호인가 
     * [EmailAddress] : 올바른 이메일 형식인가
     * [StringLength(max)] : String 길이가 최대 max인지
     * [MinLength(min)] : Collection의 크기가 최소 min인지
     * [Phone] 올바른 전화번호인지
     * [Range(min,max)] Min-Max 사이인지
     * [Url] : 올바른 Url인지
     * [Compare] : 2개의 프로퍼티 비교
     * 
     * [!] Validation은 Attribute만 적용하면 알아서 적용되지만, 결과에 대해서 어떻게 처리할지는 Action에서 정해야함
     * Validation의 결과는 ControllerBase의 ModelState에 결과를 저장한다. Action에서 읽어와서 처리할 수 있음.
     * 
     */

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Test(int id, string value)
        {
            return null;
        }

        public IActionResult Test2(TestModels testModels)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Error");
            return null;
        }

        // 1) names[0]=Faker&names[1]=Deft
        // 2) [0]=Faker&[1]=Deft
        // 3) names=Faker&names=Deft
        public IActionResult Test3(List<string> names)
        {

            return null;
        }


        public IActionResult Test4(int id, [FromHeader] string value)
        {
            return null;
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
