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

    /* [View]
     * Razor View Page(.cshtml)
     * 기본적으로 HTML과 유사한 느낌.
     * HTML은 동적 처리가 애매하다.
     * - 동적이라 함은 if else과 분기문 처리라거나,
     * - 특정 리스트 개수에 따라서 <ul><li> 갯수를 변화시키기가 어려움
     * 따라서 C#을 이용해서 생명을 불어넣겠다!
     * 
     *HTML : 고정 부분 담당 (실제 클라에 응답을 줄 HTML)
     *C# : 동적으로 변화하는 부분을 담당
     *Razor Template을 우리가 만들어주고,
     *이를 Razor 템플릿 엔진이 템플릿을 분석해서 최종 결과물을 동적을 생성
     *
     *일반적으로 View에 데이터를 건내주는 역할은 Action으로.
     *데이터를 전달하는 방법
     *1) ViewModel (Best)
     *-클래스로 만들어서 넘겨주는 방식
     *2) ViewData
     *-Dictionary<string,object> key/value 넘기는 방식
     *3) ViewBag
     *-Dictionary가 아니라 dynamic 문법 사용
     *
     *ViewModel 그냥 클래스일뿐.
     *간단한 데이터를 넘긴다면 ViewData도 괜찮.
     *실제로 Error 페이지를 살펴보자
     *
     *Layout, PartialView, _ViewStart
     *보통 웹사이트에서는 공통적으로 등장하는 UI가많다.(ex. Header, footer...)
     *동일한 CSS, Javascript 사용할 가능성도 높음
     *공통적인 부분만 따로 빼서 관리하는 layout
     *
     *Layout도 그냥 Razor 템플릿과 크게 다르지 않고
     *다만 무조건 @RenderBody()를 포함해야 한다
     *ChildView가 어디에 위치하는지를 지정하는 것
     *실제 ChildView의 HTML이 복붙 된다고 보면 됨.
     *
     *그런데 1개의 위치가 아니라, 이리저리 Child를 뿌려주고 싶다면?
     *RenderSection을 이용해 다른 이름으로 넣어준다.
     *
     *_ViewStart, _ViewImports 라는 정체는? -> 그냥 공통적인 부분을 넣어주는 곳
     *모든 View 마다 어떤 Layout을 적용할지 일일히 코드를 넣어주긴 귀찮다..
     *@using같은건 매번쓰기 귀찮으니까
     *_ViewStart, _ViewImports만들어주면 해당 폴더내 모든 View에 일괄 적용
     *
     *참고) PartialView라고 반복적으로 등장하는 View
     *재사용할 수 있게 만들 수 있는데, 이름 앞에 _를 붙여야함.
     *_이 붙으면, _ViewStart가 적용되지않음
     */

    /* [TagHelper] 일종의 HTML Helper
     * 웹페이지에서 거꾸로 유저가 Submit을 한다거나 하면 받아서 로직이 이어서 실행이 되어야함
     * 클라에서 어떤 Controller/ Action / 데이터를 보낼 것인가?
     * 
     * HTML로 순수 다 작성해도 되지만
     * Tag-Helper를 이용하면 쉽게 처리가능
     */

    /*[WebAPI]
     * MVC의 View가 HTML을 반환하지 않고, JSON/XML 데이터를 반환하면
     * 나머지 Routing, Model Binding, Validation, Response 등 동일함
     * 
     * IActionResult 대신 그냥 List<string> 데이터를 반환하면 그게 WebAPI
     * 이렇게 바로 Data를 반환하면 , ApiModel이 적용되어
     * Asp.NET Core에서 default: JSON으로 만들어서 보냄.
     * 
     * ASP.NET 이전 버전에서는 MVC와 WebAPI가 분리.
     * ASP.NET Core이후는 동일한 프레임워크 사용
     * 몇몇 설정과 반환값만 달라짐.
     * 
     * 1) request
     * 2) routing
     * 3) Model Binding + validation
     * 4) Action (<-> Service ApplicationModel)
     * 5) ViewModel vs ApiModel // 여기만 다름. ViewModel 대신 ApiModel
     * - View(Razor Template) vs Formatter ( 어떤 포맷으로? json)
      * 6) responseㅕ
      * 
      * WebAPI 프로젝트를 기본적으로 만들면, 기본 설정값들이 다르긴한데
      * 어디까지나 옵션. MVC 방식 설정 WebAPI를 운영해도 무방.
      * 일반적으로 WebAPI에서는 Convention방식의 Routing(Controller/Action)를 사용하지 않음
      * Why? REST 서버를 생각해보면
      * URL요청 자체가어떤 기능을 하는지 , 이름에서 보다 명확하게 드러나야 좋다.(api/ranking/find)
      * Attribute Routing
      * 
      * 특정 HTTP verb (POST,GET등)에 대해서만 요청을 받고 싶다면?
      * [HttpGet] [HttpPost] 사용
      * [HttpPost("주소")] = [HttpPost] + [Route("주소")]
     */

    /*[Dependency Injection(DI 종속성 주입)]
     * 디자인 패턴에서 코드간 종속성을 줄이는 것을 중요하게 생각(loosely coupled)
     * HomeController 안에 FileLogger와 FileLoggerSettings가 들어가게 되므로 FileLogger와 FileLoggerSettings를 수정하게되면
     * 다른 것에도 영향을 미침. -> 종속성이 생김.->DI필요성
     * 
     * 객체 인스턴스를 어디선가 관리를하다가 필요하면 꽂아주는 방식이라고 볼 수 있음(주로 생성자에 꽂아줌)
     * 생성자에서 new를해서 직접만들어줘야하나? -> No
     * 인터페이스 A에 대해서 ~ B라는 구현을 사용해. 그러면 생성자에 이를 연결해주는 것은 알아서 처리됨.
     * 
     * 1) Request
     * 2) Routing
     * 3) Controller Activator라는 애가 DI Container 한테 Controller 생성 + 알맞는 Dependency 연결 위탁
     * 4) DI Container 임무실시
     * 5) Controller가 생성 끝
     * 
     * 만약 3번에서 요청한 Dependency를 못찾으면 -> Error! (=ConfigureServices에 서비스 등록이 없으면 찾지못해서 에러)
     * 서비스 등록 방법 3가지
     * - Service Type(인터페이스 or 클래스 가능)
     * - Implementation Type(클래스)
     * - LifeTime 생명주기 설정 (Transient, Scoped, Singleton 중 하나)
     * AddTransient, AddScoped, AddSingleton 라이프타임 설정에 맞게 해줘야됨)
     * 
     * 원한다면 동일한 인터페이스에 대해 다수의 서비스 등록 가능
     * IEnumerable<IBaseLogger> 
     * 
     * 보통은 생성자에서 DI하지만, Action에도 DI를 [FromServices]를 사용해서 붙일수 있음
     * 
     * Razor View Template에서도 서비스가 필요하다면?
     * 이경우 생성자를 사용할 수 없으니, @inject 문법을 사용.
     * 
     * !중요
     * LifeTime
     * DI Container에 특정 서비스를 달라고 요청하면 
     * 1) 만들어서 반환하거나
     * 2) 있는 걸 반환하거나
     * 즉, 서비스 instance를 재사용할지 말지를 결정
     * 
     *Transient (항상 DI필요할때마다 새로운 서비스 instnace를 만든다. 매번 new)
     *Scoped (동일한 요청 내에서 같음. DbContext, Authentication) << 가장 일반적
     *Singleton (항상 동일한 인스턴스를 사용)
     *- 웹에서의 싱글톤은 Thread-safe 해야 함.
     * 
     * 어떤 서비스에서 DI 부품을 사용한다면, 부품들의 수명주기는 최소한 서비스의 수명주기보다는 같거나 길어야함.
     * ex) logger는 Singleton인데 logger에 사용되는 settings가 인스턴스면;
     * 개발환경에서는 이를 검사하도록 체크 가능
     * 
     */

    #region
    public interface IBaseLogger
    {
        public void Log(string log);
    }

    public class DbLogger : IBaseLogger
    {
        public DbLogger()
        {
        }
        public void Log(string log)
        {
            Console.WriteLine("log");
        }

    }

    public class FileLogSettings
    {
        string _filename;
        public FileLogSettings(string filename)
        {
            _filename = filename;
        }
    }

    public class FileLogger : IBaseLogger
    {
        FileLogSettings _settings;
        public FileLogger(FileLogSettings settings)
        {
            _settings = settings;
        }

        public void Log(string log)
        {
            Console.WriteLine("log");
        }
    }
    #endregion


    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        //public IActionResult BuyItem()
        //{
        //    return View();
        //}

        ////// 기본적으로 Views/Controller/Action.csthml을 템플릿으로 사용
        ////public IActionResult Test()
        ////    {
        ////        //return View(); // View-> new ViewResult (확장메서드) 이름으로 cshtml파일 매칭
        ////        //return View("Privacy"); // 상대경로
        ////        //return View("Views/Shared/Error.cshtml"); // 절대경로

        ////        TestViewModel testViewModel = new TestViewModel()
        ////        {
        ////            //Names = new List<string>()
        ////            //{
        ////            //    "1","2","3"
        ////            //}
        ////            Id = 1005,
        ////            Count=2
        ////        };
        ////        return View(testViewModel);
        ////    }

        //public IEnumerable<string> Test()
        //{
        //    List<string> names = new List<string>()
        //    {
        //        "Faker","Deft","Dopa"
        //    };

        //    return names;
        //}

        ///*
        //        public IActionResult Test2(TestModels testModels)
        //        {
        //            if (!ModelState.IsValid)
        //                return RedirectToAction("Error");
        //            return null;
        //        }

        //        // 1) names[0]=Faker&names[1]=Deft
        //        // 2) [0]=Faker&[1]=Deft
        //        // 3) names=Faker&names=Deft
        //        public IActionResult Test3(List<string> names)
        //        {

        //            return null;
        //        }


        //        public IActionResult Test4(int id, [FromHeader] string value)
        //        {
        //            return null;
        //        }
        //*/


        IBaseLogger _logger;
        //// 로거 여러개를 쓰고자 한다면, 서비스에서 여려개를 등록하고 여기서도 IEnumerable을 사용
        //IEnumerable<IBaseLogger> _logger;

        public HomeController(IBaseLogger logger)
        {
            _logger = logger;
        }

        [Route("Test")]
        public IActionResult Test([FromServices] IBaseLogger logger)
        {
            return Ok();
        }

        public IActionResult Index()
        {
            //// var url = Url.Action("Privacy", "Home");
            //var url = Url.RouteUrl("test", new { test = 1234 });
            //// return View();
            //// return Redirect(url);
            //return RedirectToAction("Privacy");

            FileLogger logger = new FileLogger(new FileLogSettings("log.txt"));
            logger.Log("Log Test");

            return Ok();
        }

        public IActionResult Privacy()
        {
            // 데이터 넘기는 테스트
            // ViewData는 Controller에서 상속받은 속성
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
