# =========================
# 1️⃣ Base Runtime Image
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# ساخت فولدر upload و دادن دسترسی کامل برای ذخیره فایل‌ها (عکس و غیره)
RUN mkdir -p /app/wwwroot/upload && chmod -R 777 /app/wwwroot

# =========================
# 2️⃣ Build & Publish Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# فقط فایل‌های csproj را برای restore کپی می‌کنیم تا cache بهتر کار کند
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infra/Infra.csproj", "Infra/"]

RUN dotnet restore "Presentation/Presentation.csproj"

# حالا کل سورس را کپی می‌کنیم
COPY . .

WORKDIR "/src/Presentation"
RUN dotnet build "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# =========================
# 3️⃣ Publish Stage
# =========================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# =========================
# 4️⃣ Final Runtime Image
# =========================
FROM base AS final
WORKDIR /app

# فایل‌های publish‌شده را از مرحله‌ی قبلی کپی می‌کنیم
COPY --from=publish /app/publish .

# اطمینان از وجود فولدرهای ضروری (در صورتی که موقع اجرا ساخته نشدن)
RUN mkdir -p /app/wwwroot/upload && chmod -R 777 /app/wwwroot

# ENTRYPOINT برای اجرای برنامه
ENTRYPOINT ["dotnet", "Presentation.dll"]
