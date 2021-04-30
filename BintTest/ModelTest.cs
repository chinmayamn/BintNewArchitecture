namespace BintTest
{
   
    public class ModelTest
    {
        //[Fact]
        //public async Task AdminController_Test()
        //{
        //    // Arrange
        //    var mockContext = new Mock<IHttpContextAccessor>();
        //    var mockRoleManager = new Mock<RoleManager<IdentityRole>>();
        //   var mockUserManager = new Mock<UserManager<ApplicationUser>>();
    
        //    var mockHosting = new Mock<IHostingEnvironment>();
        //  //  var mockOwinManager = new Mock<ow<ApplicationUser, ApplicationRole>>();

        //    var context = new DefaultHttpContext();
        //   // mockContext.Setup(_ => _.HttpContext).Returns(context);
        //    mockContext.Setup(o=>o.HttpContext.User.Identity.Name).Returns(It.IsAny<string>());
        //    mockHosting
        //        .Setup(m => m.EnvironmentName)
        //        .Returns("Hosting:UnitTestEnvironment");

        //    ApplicationUser a = new  ApplicationUser() {  UserId ="12222", UserName = "chinmaya@gmail.com"};
        //    List<ApplicationUser> b = new List<ApplicationUser>();
        //    b.Add(a);

        //    mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(() => null);
        //    //  mockOwinManager.Setup(x => x.UserManager).Returns(() => yourMockOfUserManager.Object);
        //    //var userStoreMock = new Mock<IUserStore<User>>();

        //    //mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object,
        //    //    null, null, null, null, null, null, null, null);


        //    /*
        //    IdentityRole r = new IdentityRole("Admin");
        //    List<IdentityRole> s = new List<IdentityRole>();


        //    s.Add(r);
        //    //  mockRoleManager.Setup(o => o.Roles).Returns(s.AsQueryable());
            
           
        //    */
        //    var roleStore = new Mock<IRoleStore<IdentityRole>>();

        //  //  await mockRoleManager.Setup(o => o.FindByIdAsync(It.IsAny<string>())).Returns(null);

        //    var admcontroller = new AdminController(mockContext.Object,mockRoleManager.Object,mockUserManager.Object,mockHosting.Object);

        //    // Act
        //    var result = admcontroller.Dashboard();

        //    // Assert
        //    var viewResult = Assert.IsType<IActionResult>(result);
            
        //}
    }
}
