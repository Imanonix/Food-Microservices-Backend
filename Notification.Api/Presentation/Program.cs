using Application.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// تنظیمات MassTransit را به اینجا منتقل کردیم
builder.Services.AddMassTransit(x =>
{
    // ۱. حتماً کلاس را اینجا ثبت کن (این مرحله همان 'تعریف' است)
    x.AddConsumer<UserRegisterConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        // ۲. حالا به مس‌تنزیت بگو که این Consumer را به کدام صف وصل کند
        cfg.ReceiveEndpoint("notification-final-test", e =>
        {
            // ۳. اینجا مس‌تنزیت کلاس را از لیست بالا برمی‌دارد
            e.ConfigureConsumer<UserRegisterConsumer>(context);
        });
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
