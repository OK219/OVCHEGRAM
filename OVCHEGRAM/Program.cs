using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OVCHEGRAM;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;
using OVCHEGRAM.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        // Этот OutputFormatter позволяет возвращать данные в XML, если требуется.
        options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
        // Эта настройка позволяет отвечать кодом 406 Not Acceptable на запросы неизвестных форматов.
        options.ReturnHttpNotAcceptable = true;
        // Эта настройка приводит к игнорированию заголовка Accept, когда он содержит */*
        // Здесь она нужна, чтобы в этом случае ответ возвращался в формате JSON
        options.RespectBrowserAcceptHeader = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
    });

// Add services to the container.
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<FileManager>();
builder.Services.AddScoped<FileRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<ConversationService>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<OvchegramDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(OvchegramDbContext))));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthCoockie";
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/ZUEV";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<UserEntity, UserDto>();
    cfg.CreateMap<UserCreateDto, UserEntity>();
    cfg.CreateMap<UserEntity, UserCreateDto>();
    cfg.CreateMap<UserEntity, UserUpdateDto>();
    cfg.CreateMap<UserUpdateDto, UserEntity>();
    cfg.CreateMap<UserEntity, UserPartialDto>()
        .ForMember(x => x.FilePath,
            y => y.MapFrom(x => FileRepository.GetFilePathByName(x.ProfilePic == null ? null : x.ProfilePic.FileName)));
    cfg.CreateMap<MessageEntity, MessageModel>()
        .ForMember(x => x.SenderName, y => y.MapFrom(x => x.User.FirstName + " " + x.User.SecondName))
        .ForMember(x => x.FilePath, y => y.MapFrom(x => FileRepository.GetFilePathByName(x.File.FileName)))
        .ForMember(x => x.ProfilePicPath,
            y => y.MapFrom(x => FileRepository.GetFilePathByName(x.User.ProfilePic.FileName)))
        .ForMember(x => x.isImage, y => y.MapFrom(x => x.File.isImage));
    cfg.CreateMap<UsersConversationEntity, ConversationPartialDto>()
        .ForMember(x => x.PicturePath, y => y.MapFrom(x => FileRepository.GetFilePathByName(x.File.FileName)));
    cfg.CreateMap<RegistrationViewModel, UserCreateDto>()
        .ForMember(x => x.ProfilePicture, y => y.MapFrom(x => x.File));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "ZUEV",
    pattern: "{ZUEV}",
    defaults: new { controller = "Home", action = "ZUEV" });

app.Run();