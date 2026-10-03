using LostAndFound.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// เปลี่ยนมาใช้ SQLite เพื่อสร้างไฟล์ฐานข้อมูลอัตโนมัติบน Render
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=lostandfound.db"));

var app = builder.Build();

// สั่งสร้างตารางและ Database อัตโนมัติตอนรันแอป
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Creation Error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ตั้งค่า default route ให้เปิดมาเจอหน้าระบบ LostAndFound (Item)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Item}/{action=Index}/{id?}");

app.Run();